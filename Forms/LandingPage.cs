using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublicTransportationSystem.Forms
{
    public partial class LandingPage : Form
    {
        private SqlConnection conn = new SqlConnection(
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\HP\\Desktop\\Pang GitHub\\PublicTransportationSystem\\PublicTransportationSystem\\Database\\dbTranspo.mdf\";Integrated Security=True"
        );
        public LandingPage()
        {
            InitializeComponent();
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            DialogResult confirmLogout = MessageBox.Show(
     "Are you sure you want to log out?",
     "Confirm Logout",
     MessageBoxButtons.YesNo,
     MessageBoxIcon.Question
 );

            if (confirmLogout == DialogResult.Yes)
            {
                Login login = new Login();
                login.Show();
                this.Close();
            }
        }

        private void btnDash_Click(object sender, EventArgs e)
        {
                
        }

        private async void btnBus_Click(object sender, EventArgs e)
        {
            Buses buses = new Buses();
            buses.StartPosition = FormStartPosition.Manual;
            buses.Location = this.Location;
            buses.Show();

            await Task.Delay(200);
            this.Close();
        }

        private async void btnRoute_Click(object sender, EventArgs e)
        {
            Routes routes = new Routes();
            routes.StartPosition = FormStartPosition.Manual;
            routes.Location = this.Location;
            routes.Show();

            await Task.Delay(200);
            this.Close();
        }

        private async void btnCustomers_Click(object sender, EventArgs e)
        {
            Customers customer = new Customers();
            customer.StartPosition = FormStartPosition.Manual;
            customer.Location = this.Location;
            customer.Show();

            await Task.Delay(200);
            this.Close();
        }

        private async void btnBookings_Click(object sender, EventArgs e)
        {
            Bookings bookings = new Bookings();
            bookings.StartPosition = FormStartPosition.Manual;
            bookings.Location = this.Location;
            bookings.Show();

            await Task.Delay(200);
            this.Close();
        }

        private void LandingPage_Load(object sender, EventArgs e)
        {
            LoadDashboardCounts();
        }
        private void LoadDashboardCounts()
        {
            lblBuses.Text = GetRowCount("dbBus").ToString();
            lblRoutes.Text = GetRowCount("dbRoutes").ToString();
            lblCustomer.Text = GetRowCount("dbCustomer").ToString();
            lblBookings.Text = GetRowCount("dbBooking").ToString();
        }

        private int GetRowCount(string tableName)
        {
            int count = 0;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", conn);
                count = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading count for {tableName}: {ex.Message}");
            }
            finally
            {   
                conn.Close();
            }
            return count;
        }

        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
