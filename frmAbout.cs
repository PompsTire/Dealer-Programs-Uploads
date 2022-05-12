using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dealer_Programs_Uploads
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            dgvData.Dock = DockStyle.Fill;
            LoadStatus();
        }

        private void LoadStatus()
        {
            DataAccess objDA = new DataAccess();
            DataTable dt = objDA.GetDataTable(ConnString, SQL_GetStatus);
            dgvData.DataSource = dt;
        }

        public string SQL_GetStatus { get => "EXEC Dealer_Programs.dbo.up_DealerPrograms_GetProgramsStatus"; }
        public string ConnString { get; set; }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
