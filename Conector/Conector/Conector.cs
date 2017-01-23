using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;

namespace Conector
{
    public class Conector
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

        public static int AbreCampo_INT(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? 0 : int.Parse(coluna[nomecampo].ToString());
        }

        public static int AbreCampo_INT(DataRow coluna, string nomecampo, int valorpadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : int.Parse(coluna[nomecampo].ToString());
        }

        public static int? AbreCampo_INT_NULL(DataRow coluna, string nomecampo)
        {
            if (coluna[nomecampo].Equals(DBNull.Value))
            {
                return null;
            }
            else
            {
                return int.Parse(coluna[nomecampo].ToString());
            }
        }

        public static float AbreCampo_FLOAT(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? 0.0f : float.Parse(coluna[nomecampo].ToString());
        }

        public static float AbreCampo_FLOAT(DataRow coluna, string nomecampo, float valorpadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : float.Parse(coluna[nomecampo].ToString());
        }

        public static float? AbreCampo_FLOAT_NULL(DataRow coluna, string nomecampo)
        {
            if (coluna[nomecampo].Equals(DBNull.Value))
            {
                return null;
            }
            else
            {
                return float.Parse(coluna[nomecampo].ToString());
            }
        }

        public static bool AbreCampo_BIT(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? false : (bool)coluna[nomecampo];
        }

        public static bool AbreCampo_BIT(DataRow coluna, string nomecampo, bool valorpadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : (bool)coluna[nomecampo];
        }

        public static bool? AbreCampo_BIT_NULL(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? null : (bool?)coluna[nomecampo];
        }

        public static string AbreCampo_VARCHAR(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
        }

        public static string AbreCampo_VARCHAR(DataRow coluna, string nomecampo, string valorpadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : coluna[nomecampo].ToString();
        }

        public static string AbreCampo_VARCHAR(DataRow coluna, string nomecampo, bool nulo)
        {
            if (nulo)
            {
                return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
            }
            else
            {
                return coluna[nomecampo].Equals(DBNull.Value) ? null : coluna[nomecampo].ToString();
            }
        }

        public static string AbreCampo_TEXTO(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
        }

        public static string AbreCampo_TEXTO(DataRow coluna, string nomecampo, string valorpadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : coluna[nomecampo].ToString();
        }

        public static string AbreCampo_TEXTO(DataRow coluna, string nomecampo, bool nulo)
        {
            if (nulo)
            {
                return coluna[nomecampo].Equals(DBNull.Value) ? string.Empty : coluna[nomecampo].ToString();
            }
            else
            {
                return coluna[nomecampo].Equals(DBNull.Value) ? null : coluna[nomecampo].ToString();
            }
        }

        public static DateTime AbreCampo_DATETIME(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? new DateTime(0) : (DateTime)coluna[nomecampo];
        }

        public static DateTime AbreCampo_DATETIME(DataRow coluna, string nomecampo, DateTime valorpadrao)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? valorpadrao : (DateTime)coluna[nomecampo];
        }

        public static DateTime? AbreCampo_DATETIME_NULL(DataRow coluna, string nomecampo)
        {
            return coluna[nomecampo].Equals(DBNull.Value) ? null : (DateTime?)coluna[nomecampo];
        }
    }
}
