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
    public partial class RouteAdd : Form
    {
        public event EventHandler DataSaved;
        public RouteAdd()
        {
            InitializeComponent();
            txtCost.KeyPress += txtCost_KeyPress;
        }
        
        public void LoadBuses()
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD("SELECT BusID FROM dbBus");

                cboBuses.DataSource = dt;
                cboBuses.DisplayMember = "BusID"; 
                cboBuses.ValueMember = "BusID";   
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading buses: " + ex.Message);
            }
        }
        private void RouteAdd_Load(object sender, EventArgs e)
        {
            LoadBuses();
            try
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "hh:mm tt";
                dateTimePicker1.ShowUpDown = true;
                dateTimePicker1.Value = DateTime.Now;

                dateTimePick.Format = DateTimePickerFormat.Short;
                dateTimePick.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing date/time: " + ex.Message);
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuDatePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoutesID.Text) ||
                string.IsNullOrWhiteSpace(txtViaCities.Text) ||
                string.IsNullOrWhiteSpace(txtCost.Text))
            {
                MessageBox.Show("Please fill all required fields.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                Database db = new Database();

                string query = "INSERT INTO dbRoutes (RouteID, ViaCities, Bus, DepartureDate, DepartureTime, Cost) VALUES (" +
                               "'" + txtRoutesID.Text + "', " +
                               "'" + txtViaCities.Text + "', " +
                               "'" + cboBuses.SelectedValue + "', " +
                               "'" + dateTimePick.Value.ToString("yyyy-MM-dd") + "', " +
                               "'" + dateTimePicker1.Value.ToString("hh:mm tt") + "', " +
                               "'" + txtCost.Text + "')";

                if (db.cudCMD(query) > 0)
                {
                    MessageBox.Show("Route added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DataSaved?.Invoke(this, EventArgs.Empty);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to add route. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding route: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboBuses_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Please enter numbers only.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
