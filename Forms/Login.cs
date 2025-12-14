using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PublicTransportationSystem.Forms;
using System.Windows.Forms;

namespace PublicTransportationSystem
{
    public partial class Login : Form
    {
        private Queue<string> bookingQueue = new Queue<string>();
        public Login()
        {
            InitializeComponent();
        }
        private void btnLog_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT * FROM dbUser WHERE [User] = @User AND [Password] = @Password";
                    using (SqlCommand sqlCMD = new SqlCommand(sql, conn))
                    {
                        sqlCMD.Parameters.AddWithValue("@User", txtUser.Text.Trim());
                        sqlCMD.Parameters.AddWithValue("@Password", txtPass.Text.Trim());

                        using (SqlDataReader sqlDR = sqlCMD.ExecuteReader())
                        {
                            if (sqlDR.HasRows)
                            {
                                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LandingPage landingPage = new LandingPage();
                                landingPage.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnLog_Click_1(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT * FROM dbUser WHERE [User] = @User AND [Password] = @Password";
                    using (SqlCommand sqlCMD = new SqlCommand(sql, conn))
                    {
                        sqlCMD.Parameters.AddWithValue("@User", txtUser.Text.Trim());
                        sqlCMD.Parameters.AddWithValue("@Password", txtPass.Text.Trim());

                        using (SqlDataReader sqlDR = sqlCMD.ExecuteReader())
                        {
                            if (sqlDR.HasRows)
                            {
                                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LandingPage landingPage = new LandingPage();
                                landingPage.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Login_Load(object sender, EventArgs e)
        {
            LoadPNRs();
            LoadQueueFromDatabase();
        }
        private void LoadPNRs()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";

            cboPNR.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT PNR FROM dbBooking ORDER BY PNR ASC";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cboPNR.Items.Add(reader["PNR"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading PNRs: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void LoadQueueFromDatabase()
        {
            bookingQueue.Clear();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT PNR FROM dbBooking ORDER BY Booked ASC"; // FIFO order
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string pnr = reader["PNR"].ToString();
                                bookingQueue.Enqueue(pnr);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading bookings into queue: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (bookingQueue.Count == 0)
            {
                MessageBox.Show("No bookings in the queue to process.", "Queue Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string pnr = bookingQueue.Dequeue();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string deleteSql = "DELETE FROM dbBooking WHERE PNR = @pnr";
                    using (SqlCommand cmdDelete = new SqlCommand(deleteSql, conn))
                    {
                        cmdDelete.Parameters.AddWithValue("@pnr", pnr);
                        cmdDelete.ExecuteNonQuery();
                    }
                    cboPNR.Items.Remove(pnr);

                    MessageBox.Show($"Booking with PNR {pnr} has been processed and removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error processing booking: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtPass_TabIndexChanged(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT * FROM dbUser WHERE [User] = @User AND [Password] = @Password";
                    using (SqlCommand sqlCMD = new SqlCommand(sql, conn))
                    {
                        sqlCMD.Parameters.AddWithValue("@User", txtUser.Text.Trim());
                        sqlCMD.Parameters.AddWithValue("@Password", txtPass.Text.Trim());

                        using (SqlDataReader sqlDR = sqlCMD.ExecuteReader())
                        {
                            if (sqlDR.HasRows)
                            {
                                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LandingPage landingPage = new LandingPage();
                                landingPage.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtPass_AcceptsTabChanged(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT * FROM dbUser WHERE [User] = @User AND [Password] = @Password";
                    using (SqlCommand sqlCMD = new SqlCommand(sql, conn))
                    {
                        sqlCMD.Parameters.AddWithValue("@User", txtUser.Text.Trim());
                        sqlCMD.Parameters.AddWithValue("@Password", txtPass.Text.Trim());

                        using (SqlDataReader sqlDR = sqlCMD.ExecuteReader())
                        {
                            if (sqlDR.HasRows)
                            {
                                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LandingPage landingPage = new LandingPage();
                                landingPage.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
