using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublicTransportationSystem.Forms
{
    public partial class BusDetails : Form
    {
        public event EventHandler DataSaved;
        public BusDetails()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Database db = new Database();
            string query = "INSERT INTO dbBus (BusID) VALUES ('" + txtNumber.Text + "')";

            if (db.cudCMD(query) > 0)
            {
                MessageBox.Show("Adding Success!");
                DataSaved?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("Adding Failed");
            }
        }

        private void BusDetails_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
