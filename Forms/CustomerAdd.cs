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
    public partial class CustomerAdd : Form
    {
        public event EventHandler DataSaved;
        public CustomerAdd()
        {
            InitializeComponent();
        }

        private void CustomerAdd_Load(object sender, EventArgs e)
        {

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Database db = new Database();

            string query = "INSERT INTO dbCustomer (CustomerID, Name, Number) VALUES (" +
                           "'" + txtCustomerID.Text + "', " +
                           "'" + txtName.Text + "', " +
                           "'" + txtContact.Text + "')";

            if (db.cudCMD(query) > 0)
            {
                MessageBox.Show("Success!");
                DataSaved?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }
    }
}
