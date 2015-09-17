using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ConfigConector
{
    public partial class frmBanco : Form
    {

        public string Item { get; set; }

        public frmBanco()
        {
            InitializeComponent();
        }

        private void btnBanco_Click(object sender, EventArgs e)
        {
            if (ValidarServidor())
                ProcurarBancos();
        }

        public bool ValidarServidor()
        {
            StringBuilder erro = new StringBuilder();

            if (String.IsNullOrWhiteSpace(txtServer.Text))
                erro.AppendLine("- Especifique um servidor sql para conexão;");

            if (String.IsNullOrWhiteSpace(txtUser.Text))
                erro.AppendLine("- Especifique o usuário para conectar ao banco;");

            if (String.IsNullOrWhiteSpace(txtPassword.Text))
                erro.AppendLine("- Especifique a senha do usuário para conectar ao banco;");

            if (String.IsNullOrWhiteSpace(erro.ToString()))
                return true;
            else
            {
                MessageBox.Show(String.Format("Verifique os erros abaixo:\n\r{0}", erro.ToString()));
                return false;
            }
        }

        public void ProcurarBancos()
        {
            //Connection String
            string connection = String.Format("Data Source={0};Persist Security Info=True;User ID={1};Password={2};",
                                txtServer.Text, txtUser.Text, txtPassword.Text);

            try
            {
                //Abrir a conexão com o banco de dados
                SqlConnection con = new SqlConnection(connection);
                con.Open();

                DataTable bancos = con.GetSchema("Databases");

                List<String> databases = new List<String>();
                foreach (DataRow dr in bancos.Rows)
                    databases.Add(dr["database_name"].ToString());

                cboDatabases.DataSource = databases;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Ocorreu um erro:\n\r{0}", ex.ToString()));
            }
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            Item = "<add key=\"ConnectionString\" value=\"Data Source=" + txtServer.Text + ";Initial Catalog=" + cboDatabases.SelectedItem.ToString() + ";Persist Security Info=True;User ID=" + txtUser.Text + ";Password=" + txtPassword.Text + "\"/>";
            Close();
        }

    }
}
