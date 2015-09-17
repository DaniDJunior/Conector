﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Conector
{
    /// <summary>
    /// Classe de conecção com banco de dados SQL
    /// </summary>
    public class SQL
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
        public List<SqlParameter> Parametros { get; set; }

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
                if(Dados != null)
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

        private SqlConnection CN;

        private SqlTransaction TRAN;

        private bool Tran_Flag;

        /// <summary>
        /// Construtor
        /// </summary>
        public SQL()
        {
            Parametros = new List<SqlParameter>();
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
        public void AddParametros(string nome, SqlDbType tipo, object valor)
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
        public void AddParametros(string nome, int tamanho, SqlDbType tipo, object valor)
        {
            Parametros.Add(CriaParametro(nome, tamanho, tipo, valor));
        }

        /// <summary>
        /// Limpa os parametros do comando a ser executado
        /// </summary>
        public void LimpaParametros()
        {
            Parametros = new List<SqlParameter>();
        }

        #region Staticos

        /// <summary>
        /// Cria um parametro de SQL
        /// </summary>
        /// <param name="nome">Nome do parametro</param>
        /// <param name="tipo">Tipo do parametro</param>
        /// <param name="valor">Valor do parametro</param>
        /// <returns>Parametro gerado</returns>
        public static SqlParameter CriaParametro(string nome, SqlDbType tipo, object valor)
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
        public static SqlParameter CriaParametro(string nome, int tamanho, SqlDbType tipo, object valor)
        {
            SqlParameter aux;
            if (tipo == SqlDbType.VarChar)
                aux = new SqlParameter(nome, tipo, tamanho);
            else
                aux = new SqlParameter(nome, tipo);
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
        public static SqlConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Cria um transação com o banco de dados
        /// </summary>
        /// <param name="connection">Conecção que a transação ira usar</param>
        /// <returns>Transação do banco</returns>
        public static SqlTransaction Transaction(SqlConnection connection)
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
            return ProcedureComand(StoredProcedure, new List<SqlParameter>());
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <returns>Dados da Procedure</returns>
        public static DataSet ProcedureComand(string StoredProcedure, List<SqlParameter> Parametros)
        {
            SqlConnection Conect = new SqlConnection(ConnectionString);
            SqlCommand COmando = new SqlCommand(StoredProcedure, Conect);
            COmando.CommandType = CommandType.StoredProcedure;
            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (SqlParameter i in Parametros)
                COmando.Parameters.Add(i);
            SqlDataAdapter Adapter = new SqlDataAdapter(COmando);
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
        public static DataSet ProcedureComand(string StoredProcedure, SqlConnection cn, SqlTransaction trans)
        {
            return ProcedureComand(StoredProcedure, new List<SqlParameter>(), cn, trans);
        }

        /// <summary>
        /// Executa uma procedure no banco de dados
        /// </summary>
        /// <param name="StoredProcedure">Nome da Procedure a ser executada</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        /// <returns>Dados da Procedure</returns>
        public static DataSet ProcedureComand(string StoredProcedure, List<SqlParameter> Parametros, SqlConnection cn, SqlTransaction trans)
        {
            SqlCommand COmando = new SqlCommand(StoredProcedure, cn, trans);
            COmando.CommandType = CommandType.StoredProcedure;
            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (SqlParameter i in Parametros)
                COmando.Parameters.Add(i);
            SqlDataAdapter Adapter = new SqlDataAdapter(COmando);
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
            return SelectComand(Select, new List<SqlParameter>());
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <returns>Dados de retorno do select</returns>
        public static DataSet SelectComand(string Select, List<SqlParameter> Parametros)
        {
            SqlConnection Conect = new SqlConnection(ConnectionString);
            SqlDataAdapter Adapt = new SqlDataAdapter(Select, Conect);

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                Adapt.SelectCommand.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

            foreach (SqlParameter i in Parametros)
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
        public static DataSet SelectComand(string Select, SqlConnection cn, SqlTransaction trans)
        {
            return SelectComand(Select, new List<SqlParameter>(), cn, trans);
        }

        /// <summary>
        /// Executa um comando de select no banco
        /// </summary>
        /// <param name="Select">Comando de select</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        /// <returns>Dados de retorno do select</returns>
        public static DataSet SelectComand(string Select, List<SqlParameter> Parametros, SqlConnection cn, SqlTransaction trans)
        {
            SqlDataAdapter Adapt = new SqlDataAdapter(Select, cn);

            Adapt.SelectCommand.Transaction = trans;

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                Adapt.SelectCommand.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);

            foreach (SqlParameter i in Parametros)
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
            Comando(Comand, new List<SqlParameter>());
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        public static void Comando(string Comand, List<SqlParameter> Parametros)
        {
            SqlConnection Conect = new SqlConnection(ConnectionString);
            SqlCommand COmando = new SqlCommand(Comand, Conect);

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (SqlParameter i in Parametros)
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
        public static void Comando(string Comand, SqlConnection cn, SqlTransaction trans)
        {
            Comando(Comand, new List<SqlParameter>(), cn, trans);
        }

        /// <summary>
        /// Executa um comando no banco de dados
        /// </summary>
        /// <param name="Comand">Comando a ser executado</param>
        /// <param name="Parametros">Parametros de SQL a serem usados</param>
        /// <param name="cn">Conecção com o banco</param>
        /// <param name="trans">Transação do banco</param>
        public static void Comando(string Comand, List<SqlParameter> Parametros, SqlConnection cn, SqlTransaction trans)
        {
            SqlCommand COmando = new SqlCommand(Comand, cn, trans);

            if (ConfigurationManager.AppSettings["SQLCommandTimeout"] != null)
                COmando.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["SQLCommandTimeout"]);
            foreach (SqlParameter i in Parametros)
                COmando.Parameters.Add(i);

            COmando.Connection.Open();
            COmando.ExecuteNonQuery();
            COmando.Connection.Close();
        }

        /// <summary>
        /// Abre um campo de uma coluna do banco de dados
        /// </summary>
        /// <typeparam name="T">Tipo do valor e ser aberto</typeparam>
        /// <param name="coluna">Coluna que contem o valor</param>
        /// <param name="nomecampo">Nome do campo a ser aberto</param>
        /// <param name="valorPadrao">Valor padrão caso o campo seja NULL</param>
        /// <returns>Valor do campo na coluna</returns>
        public static T AbreCampo<T>(DataRow coluna, string nomecampo, T valorPadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorPadrao : (T)coluna[nomecampo];
        }

        //public static int AbreCampo_INT(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? 0 : int.Parse(coluna[nomecampo].ToString());
        //}

        //public static int AbreCampo_INT(DataRow coluna, string nomecampo, int valorpadrao)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : int.Parse(coluna[nomecampo].ToString());
        //}

        //public static int? AbreCampo_INT_NULL(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? null : (int?)coluna[nomecampo];
        //}

        //public static float AbreCampo_FLOAT(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? 0.0f : (float)coluna[nomecampo];
        //}

        //public static float AbreCampo_FLOAT(DataRow coluna, string nomecampo, float valorpadrao)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : (float)coluna[nomecampo];
        //}

        //public static float? AbreCampo_FLOAT_NULL(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? null : (float?)coluna[nomecampo];
        //}

        //public static bool AbreCampo_BIT(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? false : (bool)coluna[nomecampo];
        //}

        //public static bool AbreCampo_BIT(DataRow coluna, string nomecampo, bool valorpadrao)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : (bool)coluna[nomecampo];
        //}

        //public static bool? AbreCampo_BIT_NULL(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? null : (bool?)coluna[nomecampo];
        //}

        //public static string AbreCampo_VARCHAR(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
        //}

        //public static string AbreCampo_VARCHAR(DataRow coluna, string nomecampo, string valorpadrao)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : coluna[nomecampo].ToString();
        //}

        //public static string AbreCampo_VARCHAR(DataRow coluna, string nomecampo, bool nulo)
        //{
        //    if (nulo)
        //    {
        //        return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
        //    }
        //    else
        //    {
        //        return coluna[nomecampo].Equals(DBNull.Value) ? null : coluna[nomecampo].ToString();
        //    }
        //}

        //public static string AbreCampo_TEXTO(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
        //}

        //public static string AbreCampo_TEXTO(DataRow coluna, string nomecampo, string valorpadrao)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : coluna[nomecampo].ToString();
        //}

        //public static string AbreCampo_TEXTO(DataRow coluna, string nomecampo, bool nulo)
        //{
        //    if (nulo)
        //    {
        //        return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
        //    }
        //    else
        //    {
        //        return coluna[nomecampo].Equals(DBNull.Value) ? null : coluna[nomecampo].ToString();
        //    }
        //}

        //public static DateTime AbreCampo_DATETIME(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? new DateTime(0) : (DateTime)coluna[nomecampo];
        //}

        //public static DateTime AbreCampo_DATETIME(DataRow coluna, string nomecampo, DateTime valorpadrao)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : (DateTime)coluna[nomecampo];
        //}

        //public static DateTime? AbreCampo_DATETIME_NULL(DataRow coluna, string nomecampo)
        //{
        //    return coluna[nomecampo].Equals(DBNull.Value) ? null : (DateTime?)coluna[nomecampo];
        //}

        #endregion

    }
}
