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
    public partial class RouteEdit : Form
    {
        internal string _routeID;
        public event EventHandler DataSaved;
        public RouteEdit(string routeID)
        {
            InitializeComponent();
            _routeID = routeID;
        }

        private void RouteEdit_Load(object sender, EventArgs e)
        {
            try
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "hh:mm tt";
                dateTimePicker1.ShowUpDown = true;
                dateTimePicker1.Value = DateTime.Now;
                Database db = new Database();
                DataTable buses = db.selectCMD("SELECT BusID FROM dbBus");
                cboBuses.DataSource = buses;
                cboBuses.DisplayMember = "BusID";
                cboBuses.ValueMember = "BusID";

                DataTable dt = db.selectCMD($"SELECT * FROM dbRoutes WHERE RouteID = '{_routeID}'");

                if (dt.Rows.Count > 0)
                {
                    txtRoutesID.Text = dt.Rows[0]["RouteID"].ToString();
                    txtViaCities.Text = dt.Rows[0]["ViaCities"].ToString();
                    cboBuses.SelectedValue = dt.Rows[0]["Bus"].ToString();
                    dateTimePick.Value = Convert.ToDateTime(dt.Rows[0]["DepartureDate"]);
                    dateTimePicker1.Value = DateTime.Parse(dt.Rows[0]["DepartureTime"].ToString());
                    txtCost.Text = dt.Rows[0]["Cost"].ToString();
                }
                else
                {
                    MessageBox.Show("Route not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading route data: " + ex.Message);
            }
        }


        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();

                string query = $"UPDATE dbRoutes SET " +
                $"RouteID = '{txtRoutesID.Text}', " + 
                $"ViaCities = '{txtViaCities.Text}', " +
               $"Bus = '{cboBuses.SelectedValue}', " +
               $"DepartureDate = '{dateTimePick.Value:yyyy-MM-dd}', " +
               $"DepartureTime = '{dateTimePicker1.Value:hh:mm tt}', " +
               $"Cost = '{txtCost.Text}' " +
               $"WHERE RouteID = '{_routeID}'";

                db.cudCMD(query);

                MessageBox.Show("Route updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DataSaved?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating route: " + ex.Message);
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
   