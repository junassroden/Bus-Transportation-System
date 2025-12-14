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
    public partial class Seats : Form
    {
        public Seats()
        {
            InitializeComponent();
        }

        private async void btnDash_Click(object sender, EventArgs e)
        {
            LandingPage landingPage = new LandingPage();
            landingPage.StartPosition = FormStartPosition.Manual;
            landingPage.Location = this.Location;
            landingPage.Show();

            await Task.Delay(200);
            this.Close();
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
            Customers customers = new Customers();
            customers.StartPosition = FormStartPosition.Manual;
            customers.Location = this.Location;
            customers.Show();

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

        private void btnExit_Click(object sender, EventArgs e)
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
    }
}
