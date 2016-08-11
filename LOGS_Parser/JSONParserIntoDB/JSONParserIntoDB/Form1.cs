using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Data.ConnectionUI;

namespace JSONParserIntoDB
{
    public partial class Form1 : Form
    {
        private static string connection;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fold = new FolderBrowserDialog();
            if (fold.ShowDialog() == DialogResult.OK)
            {
                var js = new ReadJson(fold.SelectedPath, this, connection);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DataConnectionDialog dcd = new DataConnectionDialog();
            DataSource.AddStandardDataSources(dcd);
            if (DataConnectionDialog.Show(dcd) == DialogResult.OK)
            {
                connection = dcd.ConnectionString;
                label5Connection.Text = connection ?? ConfigurationManager.ConnectionStrings["LogsDB"].ConnectionString;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label5Connection.Text = ConfigurationManager.ConnectionStrings["LogsDB"].ConnectionString;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
