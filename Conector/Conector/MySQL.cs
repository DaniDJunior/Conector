using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conector
{
    public class MySQL
    {

        /// <summary>
        /// String de conexão com o Banco de Dados
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ConnectionString"].ToString();
            }
        }

        /// <summary>
        /// Parametros para o comando sql a ser usado
        /// </summary>
        public List<MySqlParameter> Parametros { get; set; }

        /// <summary>
        /// Dados de retorno do comando sql
        /// </summary>
        public DataSet DADOS
        {
            get
            {
                return Dados;
            }
        }

        private DataSet Dados;

        /// <summary>
        /// Tabela padrao de retorno de dados
        /// </summary>
        public DataTable TabelaPadrao
        {
            get
            {
                if (Dados != null)
                {
                    return Dados.Tables[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Quantidade de registro da tabela padrao de dados
        /// </summary>
        public int TamanhoTabelaPadrao
        {
            get
            {
                if (Dados != null)
                {
                    return Dados.Tables[0].Rows.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        private MySqlConnection CN;

        private MySqlTransaction TRAN;

        private bool Tran_Flag;

        /// <summary>
        /// Construtor
        /// </summary>
        public MySQL()
        {
            Parametros = new List<MySqlParameter>();
            Tran_Flag = false;
            Dados = null;
        }

        /// <summary>
        /// Abre Transação no banco
        /// </summary>
        public void OPEN()
        {
            Tran_Flag = true;
            CN = Connection();
            TRAN = Transaction(CN);
        }

        /// <summary>
        /// Consolida as informações no banco
        /// </summary>
        public void COMMIT()
        {
            Tran_Flag = false;
            TRAN.Commit();
        }

        /// <summary>
        /// Cansela as informações no banco
        /// </summary>
        public void ROLLBACK()
        {
            Tran_Flag = false;
            TRAN.Rollback();
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <returns>Dados da Procedure</returns>
        public DataSet execProcedureComand(string StoredProcedure)
        {
            if (Tran_Flag)
                Dados = ProcedureComand(StoredProcedure, Parametros, CN, TRAN);
            else
                Dados = ProcedureComand(StoredProcedure, Parametros);
            return Dados;
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <returns>Dados de retorno do select</returns>
        public DataSet execSelectComand(string Select)
        {
            if (Tran_Flag)
                Dados = SelectComand(Select, Parametros, CN, TRAN);
            else
                Dados = SelectComand(Select, Parametros);
            return Dados;
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        public void execComando(string Comand)
        {
            if (Tran_Flag)
                Comando(Comand, Parametros, CN, TRAN);
            else
                Comando(Comand, Parametros);
        }

        /// <summary>
        /// Adiciona um parametro ao comando a ser executado
        /// </summary>
        /// <param name="nome">Nome do parametro</param>
        /// <param name="tipo">Tipo do parametro</param>
        /// <param name="valor">Valor do parametro</param>
        public void AddParametros(string nome, MySqlDbType tipo, object valor)
        {
            Parametros.Add(CriaParametro(nome, tipo, valor));
        }

        /// <summary>
        /// Adiciona um parametro ao comando a ser executado
        /// </summary>
        /// <param name="nome">Nome do parametro</param>
        /// <param name="tamanho">Tamanho do parametro</param>
        /// <param name="tipo">Tipo do parametro</param>
        /// <param name="valor">Valor do parametro</param>
        public void AddParametros(string nome, int tamanho, MySqlDbType tipo, object valor)
        {
            Parametros.Add(CriaParametro(nome, tamanho, tipo, valor));
        }

        /// <summary>
        /// Limpa os parametros do comando a ser executado
        /// </summary>
        public void LimpaParametros()
        {
            Parametros = new List<MySqlParameter>();
        }

        #region Staticos

        /// <summary>
        /// Cria um parametro de SQL
        /// </summary>
        /// <param name="nome">Nome do parametro</param>
        /// <param name="tipo">Tipo do parametro</param>
        /// <param name="valor">Valor do parametro</param>
        /// <returns>Parametro gerado</returns>
        public static MySqlParameter CriaParametro(string nome, MySqlDbType tipo, object valor)
        {
            return CriaParametro(nome, 0, tipo, valor);
        }

        /// <summary>
        /// Cria um parametro de SQL
        /// </summary>
        /// <param name="nome">Nome do parametro</param>
        /// <param name="tamanho"></param>
        /// <param name="tipo">Tipo do parametro</param>
        /// <param name="valor">Valor do parametro</param>
        /// <returns>Parametro gerado</returns>
        public static MySqlParameter CriaParametro(string nome, int tamanho, MySqlDbType tipo, object valor)
        {
            MySqlParameter aux;
            if (tipo == MySqlDbType.VarChar)
                aux = new MySqlParameter(nome, tipo, tamanho);
            else
                aux = new MySqlParameter(nome, tipo);
            if (valor != null)
            {
                aux.Value = valor;
            }
            else
            {
                aux.Value = DBNull.Value;
            }
            return aux;
        }

        /// <summary>
        /// Cria uma conecção com o banco de dados
        /// </summary>
        /// <returns>Conecção com o banco</returns>
        public static MySqlConnection Connection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Cria um transação com o banco de dados
        /// </summary>
        /// <param name="connection">Conecção que a transação ira usar</param>
        /// <returns>Transação do banco</returns>
        public static MySqlTransaction Transaction(MySqlConnection connection)
        {
            return connection.BeginTransaction();
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <returns>Dados da Procedure</returns>
        public static DataSet ProcedureComand(string StoredProcedure)
        {
            return ProcedureComand(StoredProcedure, new List<MySqlParameter>());
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <returns>Dados da Procedure</returns>
        public static DataSet ProcedureComand(string StoredProcedure, List<MySqlParameter> Parametros)
        {
            MySqlConnection Conect = new MySqlConnection(ConnectionString);
            MySqlCommand COmando = new MySqlCommand(StoredProcedure, Conect);
            COmando.CommandType = CommandType.StoredProcedure;
            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (MySqlParameter i in Parametros)
                COmando.Parameters.Add(i);
            MySqlDataAdapter Adapter = new MySqlDataAdapter(COmando);
            DataSet Retorno = new DataSet();
            Adapter.Fill(Retorno);
            return Retorno;
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        /// <returns>Dados da Procedure</returns>
        public static DataSet ProcedureComand(string StoredProcedure, MySqlConnection cn, MySqlTransaction trans)
        {
            return ProcedureComand(StoredProcedure, new List<MySqlParameter>(), cn, trans);
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        /// <returns>Dados da Procedure</returns>
        public static DataSet ProcedureComand(string StoredProcedure, List<MySqlParameter> Parametros, MySqlConnection cn, MySqlTransaction trans)
        {
            MySqlCommand COmando = new MySqlCommand(StoredProcedure, cn, trans);
            COmando.CommandType = CommandType.StoredProcedure;
            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (MySqlParameter i in Parametros)
                COmando.Parameters.Add(i);
            MySqlDataAdapter Adapter = new MySqlDataAdapter(COmando);
            DataSet Retorno = new DataSet();
            Adapter.Fill(Retorno);
            return Retorno;
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <returns>Dados de retorno do select</returns>
        public static DataSet SelectComand(string Select)
        {
            return SelectComand(Select, new List<MySqlParameter>());
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <returns>Dados de retorno do select</returns>
        public static DataSet SelectComand(string Select, List<MySqlParameter> Parametros)
        {
            MySqlConnection Conect = new MySqlConnection(ConnectionString);
            MySqlDataAdapter Adapt = new MySqlDataAdapter(Select, Conect);

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                Adapt.SelectCommand.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

            foreach (MySqlParameter i in Parametros)
                Adapt.SelectCommand.Parameters.Add(i);

            Adapt.SelectCommand.Connection.Open();
            Adapt.SelectCommand.ExecuteNonQuery();
            Adapt.SelectCommand.Connection.Close();

            DataSet Retorno = new DataSet();

            Adapt.Fill(Retorno);

            return Retorno;
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        /// <returns>Dados de retorno do select</returns>
        public static DataSet SelectComand(string Select, MySqlConnection cn, MySqlTransaction trans)
        {
            return SelectComand(Select, new List<MySqlParameter>(), cn, trans);
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        /// <returns>Dados de retorno do select</returns>
        public static DataSet SelectComand(string Select, List<MySqlParameter> Parametros, MySqlConnection cn, MySqlTransaction trans)
        {
            MySqlDataAdapter Adapt = new MySqlDataAdapter(Select, cn);

            Adapt.SelectCommand.Transaction = trans;

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                Adapt.SelectCommand.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

            foreach (MySqlParameter i in Parametros)
                Adapt.SelectCommand.Parameters.Add(i);

            Adapt.SelectCommand.Connection.Open();
            Adapt.SelectCommand.ExecuteNonQuery();
            Adapt.SelectCommand.Connection.Close();

            DataSet Retorno = new DataSet();

            Adapt.Fill(Retorno);

            return Retorno;
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        public static void Comando(string Comand)
        {
            Comando(Comand, new List<MySqlParameter>());
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        public static void Comando(string Comand, List<MySqlParameter> Parametros)
        {
            MySqlConnection Conect = new MySqlConnection(ConnectionString);
            MySqlCommand COmando = new MySqlCommand(Comand, Conect);

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (MySqlParameter i in Parametros)
                COmando.Parameters.Add(i);

            COmando.Connection.Open();
            COmando.ExecuteNonQuery();
            COmando.Connection.Close();
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        public static void Comando(string Comand, MySqlConnection cn, MySqlTransaction trans)
        {
            Comando(Comand, new List<MySqlParameter>(), cn, trans);
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        public static void Comando(string Comand, List<MySqlParameter> Parametros, MySqlConnection cn, MySqlTransaction trans)
        {
            MySqlCommand COmando = new MySqlCommand(Comand, cn, trans);

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (MySqlParameter i in Parametros)
                COmando.Parameters.Add(i);

            COmando.Connection.Open();
            COmando.ExecuteNonQuery();
            COmando.Connection.Close();
        }

        public static DataSet DirectSelectComand(string tableName, List<MySqlParameter> campos, List<MySqlParameter> parametros)
        {
            string comando = "SELECT ";
            foreach(MySqlParameter i in campos)
            {
                comando += i.ParameterName + ",";
            }
            comando = comando.Remove(comando.Length - 1, 1);
            comando += " FROM " + tableName;
            if(parametros.Count > 0)
            {
                comando += " where ";
                foreach (MySqlParameter i in parametros)
                {
                    comando += i.ParameterName + " = @" + i.ParameterName + " AND ";
                }
                comando = comando.Remove(comando.Length - 5, 5);
            }
            return SelectComand(comando, parametros);
        }

        public static DataSet DirectSelectComand(string tableName, List<MySqlParameter> campos, List<MySqlParameter> parametros, string whereCondition)
        {
            string comando = "SELECT ";
            foreach (MySqlParameter i in campos)
            {
                comando += i.ParameterName + ",";
            }
            comando = comando.Remove(comando.Length - 1, 1);
            comando += " FROM " + tableName;
            if (parametros.Count > 0)
            {
                comando += " where " + whereCondition;
            }
            return SelectComand(comando, parametros);
        }

        public static DataSet DirectInsertComand(string tableName, string idName, List<MySqlParameter> parametros)
        {
            string comandoInsert = "INSERT INTO " + tableName;
            string comandoInsertCampos = "(";
            string comandoInsertValues = "(";
            foreach (MySqlParameter i in parametros)
            {
                if (i.ParameterName != idName)
                {
                    comandoInsertCampos += i.ParameterName + ",";
                    comandoInsertValues += "@" + i.ParameterName + ",";
                }
            }
            comandoInsertCampos = comandoInsertCampos.Remove(comandoInsertCampos.Length - 1, 1) + ")";
            comandoInsertValues = comandoInsertValues.Remove(comandoInsertValues.Length - 1, 1) + ")";
            comandoInsert += comandoInsertCampos + " VALUES " + comandoInsertValues;
            string comandoSelect = "SELECT ";
            foreach (MySqlParameter i in parametros)
            {
                comandoSelect += i.ParameterName + ",";
            }
            comandoSelect = comandoSelect.Remove(comandoSelect.Length - 1, 1);
            comandoSelect += " FROM " + tableName;
            if (parametros.Count > 0)
            {
                comandoSelect += " where " + idName + " = LAST_INSERT_ID()";
            }
            Comando(comandoInsert, parametros);
            return SelectComand(comandoSelect);
        }

        public static DataSet DirectUpdateComand(string tableName, string idName, List<MySqlParameter> parametros)
        {
            string comandoUpdate = "UPDATE " + tableName + " SET ";
            foreach (MySqlParameter i in parametros)
            {
                if (i.ParameterName != idName)
                {
                    comandoUpdate += i.ParameterName + " = @" + i.ParameterName + ",";
                }
            }
            comandoUpdate = comandoUpdate.Remove(comandoUpdate.Length - 1, 1);
            comandoUpdate += "  WHERE " + idName + " = @" + idName;
            string comandoSelect = "SELECT ";
            foreach (MySqlParameter i in parametros)
            {
                comandoSelect += i.ParameterName + ",";
            }
            comandoSelect = comandoSelect.Remove(comandoSelect.Length - 1, 1);
            comandoSelect += " FROM " + tableName;
            if (parametros.Count > 0)
            {
                comandoSelect += " where " + idName + " = @" + idName;
            }
            Comando(comandoUpdate, parametros);
            return SelectComand(comandoSelect);
        }

        #endregion

    }
}
