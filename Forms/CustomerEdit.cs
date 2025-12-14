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
    public partial class CustomerEdit : Form
    {
        private string _customerID;
        public event EventHandler DataSaved;
        public CustomerEdit(string customerID)
        {
            InitializeComponent();
            _customerID = customerID;
        }

        private void CustomerEdit_Load(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD($"SELECT * FROM dbCustomer WHERE CustomerID = '{_customerID}'");

                if (dt.Rows.Count > 0)
                {
                    txtCustomerID.Text = dt.Rows[0]["CustomerID"].ToString();
                    txtName.Text = dt.Rows[0]["Name"].ToString();
                    txtContact.Text = dt.Rows[0]["Number"].ToString();
                }
                else
                {
                    MessageBox.Show("Customer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer data: " + ex.Message);
            }
        }
        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();

                string query = $"UPDATE dbCustomer SET " +
                               $"CustomerID = '{txtCustomerID.Text}', " +
                               $"Name = '{txtName.Text}', " +
                               $"Number = '{txtContact.Text}' " +
                               $"WHERE CustomerID = '{_customerID}'";

                int result = db.cudCMD(query);

                if (result > 0)
                {
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DataSaved?.Invoke(this, EventArgs.Empty);
                    this.Close();
                }   
                else
                {
                    MessageBox.Show("No changes were made.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer: " + ex.Message);
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
