using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ConfigConector
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            frmBanco tela = new frmBanco();
            tela.ShowDialog();
            txtConfig.Text = tela.Item;
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            if (fbdDLL.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(Application.StartupPath + "\\Conector.dll", fbdDLL.SelectedPath + "\\Conector.dll");
            }
        }
    }
}
