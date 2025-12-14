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
    public partial class BookingEdit : Form
    {
        private readonly string connectionString =
             @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";

        private Dictionary<string, Dictionary<string, int>> distanceGraph = new Dictionary<string, Dictionary<string, int>>();
        private decimal ratePerKm = 5m;
        private bool isUpdatingFare = false;
        private string originCity = "";
        private decimal basePrice = 0m;
        private string _pnr;
        public event EventHandler DataSaved;
        public BookingEdit(string pnr )
        {
            InitializeComponent();
            _pnr = pnr;
        }

        private void BookingEdit_Load(object sender, EventArgs e)
        {
            LoadCustomerIDs();
            LoadRoutes();
            LoadBuses();
            InitializeGraph();
            LoadDestinations();

            cboDestination.SelectedIndexChanged += cboDestination_SelectedIndexChanged_1;
            cboRoutes.SelectedIndexChanged += cboRoutes_SelectedIndexChanged;

            LoadBookingData();
        }

        private void LoadBookingData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbBooking WHERE PNR = @pnr", conn))
                    {
                        cmd.Parameters.AddWithValue("@pnr", _pnr);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            txtName.Text = reader["Name"].ToString();
                            txtContactNumber.Text = reader["Contact"].ToString();
                            cboBus.Text = reader["Bus"].ToString();
                            cboRoutes.Text = reader["Route"].ToString();
                            cboDestination.Text = reader["Destination"].ToString();
                            txtTotalAmount.Text = Convert.ToDecimal(reader["Amount"]).ToString("F2");

                            originCity = cboRoutes.Text.Split('-')[0].Trim();
                        }
                        reader.Close();
                    }

                    using (SqlCommand baseCmd = new SqlCommand("SELECT Cost FROM dbRoutes WHERE ViaCities = @route", conn))
                    {
                        baseCmd.Parameters.AddWithValue("@route", cboRoutes.Text);
                        object result = baseCmd.ExecuteScalar();
                        if (result != null)
                            basePrice = Convert.ToDecimal(result);
                    }
                }

                UpdateTotalFare();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading booking: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadCustomerIDs()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CustomerID FROM dbCustomer", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cboCustomerID.Items.Add(reader["CustomerID"].ToString());
                }
            }
        }

        public void LoadRoutes()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ViaCities FROM dbRoutes", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cboRoutes.Items.Add(reader["ViaCities"].ToString());
                }
            }
        }

        public void LoadBuses()
        {
           using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT BusID FROM dbBus", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cboBus.Items.Add(reader["BusID"].ToString());
                }
            }
        }

        public void LoadDestinations()
        {
            string[] destinations = {
                "Calapan", "Baco", "Bansud", "Bongabong", "Bulalacao",
                "Gloria", "Mansalay", "Naujan", "Pinamalayan",
                "Roxas", "Socorro", "Victoria", "Pola"
            };
            cboDestination.Items.AddRange(destinations);
        }
        private void cboDestination_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) ||
               string.IsNullOrEmpty(txtContactNumber.Text) ||
               string.IsNullOrEmpty(cboBus.Text) ||
               string.IsNullOrEmpty(cboRoutes.Text) ||
               string.IsNullOrEmpty(cboDestination.Text))
            {
                MessageBox.Show("Please fill out all fields before saving changes.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE dbBooking 
                        SET Name = @name, 
                            Contact = @contact, 
                            Bus = @bus, 
                            Route = @route, 
                            Amount = @amount, 
                            Destination = @destination
                        WHERE PNR = @pnr", conn);

                    cmd.Parameters.AddWithValue("@pnr", _pnr);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@contact", txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@bus", cboBus.Text);
                    cmd.Parameters.AddWithValue("@route", cboRoutes.Text);
                    cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txtTotalAmount.Text));
                    cmd.Parameters.AddWithValue("@destination", cboDestination.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Booking updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DataSaved?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating booking: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboRoutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cboRoutes.Text))
                return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ViaCities, Cost FROM dbRoutes WHERE ViaCities=@route", conn);
                cmd.Parameters.AddWithValue("@route", cboRoutes.Text);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string viaCities = reader["ViaCities"].ToString();
                    basePrice = Convert.ToDecimal(reader["Cost"]);
                    originCity = viaCities.Split('-')[0].Trim();
                    txtTotalAmount.Text = basePrice.ToString("F2");
                }
                reader.Close();
            }

            if (!string.IsNullOrEmpty(cboDestination.Text))
                UpdateTotalFare();
        }
        private void UpdateTotalFare()
        {
            if (isUpdatingFare) return;
            isUpdatingFare = true;

            try
            {
                if (string.IsNullOrEmpty(originCity) || string.IsNullOrEmpty(cboDestination.Text))
                    return;

                string destinationCity = cboDestination.Text.Trim();

                if (originCity.Equals(destinationCity, StringComparison.OrdinalIgnoreCase))
                {
                    if (cboDestination.Focused)
                    {
                        MessageBox.Show("Please choose a different destination.",
                                        "Invalid Destination", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    txtTotalAmount.Text = basePrice.ToString("F2");
                    cboDestination.SelectedIndex = -1;
                    return;
                }

                decimal totalFare = basePrice;

                if (distanceGraph.ContainsKey(originCity) && distanceGraph[originCity].ContainsKey(destinationCity))
                {
                    int distance = distanceGraph[originCity][destinationCity];
                    decimal addedFare = distance * ratePerKm;
                    totalFare += addedFare;
                }

                txtTotalAmount.Text = totalFare.ToString("F2");
            }
            finally
            {
                isUpdatingFare = false;
            }
        }
        private void InitializeGraph()
        {
            string[] distanceData = {
        "Calapan,Baco,10.7", "Calapan,Bansud,68.2", "Calapan,Bongabong,81.1", "Calapan,Bulalacao,122.1",
        "Calapan,Gloria,56.8", "Calapan,Mansalay,103.1", "Calapan,Naujan,16.4", "Calapan,Pinamalayan,52.7",
        "Calapan,Pola,40.9", "Calapan,Puerto Galera,27.2", "Calapan,Roxas,98.4", "Calapan,San Teodoro,21.4",
        "Calapan,Socorro,46.6", "Calapan,Victoria,29.2",

        "Baco,Calapan,10.7", "Baco,Bansud,67.5", "Baco,Bongabong,79.9", "Baco,Bulalacao,117.8",
        "Baco,Gloria,57.6", "Baco,Mansalay,100.2", "Baco,Naujan,22.4", "Baco,Pinamalayan,54.4",
        "Baco,Pola,43.9", "Baco,Puerto Galera,23.1", "Baco,Roxas,96.6", "Baco,San Teodoro,11",
        "Baco,Socorro,47.4", "Baco,Victoria,27.1",

        "Bansud,Calapan,68.2", "Bansud,Baco,67.5", "Bansud,Bongabong,13.1", "Bansud,Bulalacao,60.7",
        "Bansud,Gloria,13.7", "Bansud,Mansalay,37.9", "Bansud,Naujan,54.2", "Bansud,Pinamalayan,20",
        "Bansud,Pola,31.6", "Bansud,Puerto Galera,90.4", "Bansud,Roxas,30.8", "Bansud,San Teodoro,72.2",
        "Bansud,Socorro,22.3", "Bansud,Victoria,40.4",

        "Bongabong,Calapan,81.1", "Bongabong,Baco,79.9", "Bongabong,Bansud,13.1", "Bongabong,Bulalacao,49.4",
        "Bongabong,Gloria,26.4", "Bongabong,Mansalay,25.8", "Bongabong,Naujan,67.3", "Bongabong,Pinamalayan,32.4",
        "Bongabong,Pola,44.4", "Bongabong,Puerto Galera,102.6", "Bongabong,Roxas,17.8", "Bongabong,San Teodoro,83.9",
        "Bongabong,Socorro,35.4", "Bongabong,Victoria,52.8",

        "Bulalacao,Calapan,122.1", "Bulalacao,Baco,117.8", "Bulalacao,Bansud,60.7", "Bulalacao,Bongabong,49.4",
        "Bulalacao,Gloria,74.4", "Bulalacao,Mansalay,24", "Bulalacao,Naujan,111.2", "Bulalacao,Pinamalayan,80.7",
        "Bulalacao,Pola,91.6", "Bulalacao,Puerto Galera,138", "Bulalacao,Roxas,34.9", "Bulalacao,San Teodoro,118.1",
        "Bulalacao,Socorro,81.4", "Bulalacao,Victoria,93",

        "Gloria,Calapan,56.8", "Gloria,Baco,57.6", "Gloria,Bansud,13.7", "Gloria,Bongabong,26.4",
        "Gloria,Bulalacao,74.4", "Gloria,Mansalay,51.6", "Gloria,Naujan,41.9", "Gloria,Pinamalayan,6.3",
        "Gloria,Pola,18.1", "Gloria,Puerto Galera,80.7", "Gloria,Roxas,44.1", "Gloria,San Teodoro,63.8",
        "Gloria,Socorro,10.3", "Gloria,Victoria,31.2",

        "Mansalay,Calapan,103.1", "Mansalay,Baco,100.2", "Mansalay,Bansud,37.9", "Mansalay,Bongabong,25.8",
        "Mansalay,Bulalacao,24", "Mansalay,Gloria,51.6", "Mansalay,Naujan,90.7", "Mansalay,Pinamalayan,57.9",
        "Mansalay,Pola,69.4", "Mansalay,Puerto Galera,121.8", "Mansalay,Roxas,11.5", "Mansalay,San Teodoro,102.2",
        "Mansalay,Socorro,59.6", "Mansalay,Victoria,74",

        "Naujan,Calapan,16.4", "Naujan,Baco,22.4", "Naujan,Bansud,54.2", "Naujan,Bongabong,67.3",
        "Naujan,Bulalacao,111.2", "Naujan,Gloria,41.9", "Naujan,Mansalay,90.7", "Naujan,Pinamalayan,37.2",
        "Naujan,Pola,25", "Naujan,Puerto Galera,43.3", "Naujan,Roxas,84.9", "Naujan,San Teodoro,32.7",
        "Naujan,Socorro,32", "Naujan,Victoria,19.9",

        "Pinamalayan,Calapan,52.7", "Pinamalayan,Baco,54.4", "Pinamalayan,Bansud,20", "Pinamalayan,Bongabong,32.4",
        "Pinamalayan,Bulalacao,80.7", "Pinamalayan,Gloria,6.3", "Pinamalayan,Mansalay,57.9", "Pinamalayan,Naujan,37.2",
        "Pinamalayan,Pola,12.5", "Pinamalayan,Puerto Galera,77.5", "Pinamalayan,Roxas,50.1", "Pinamalayan,San Teodoro,61.4",
        "Pinamalayan,Socorro,8.1", "Pinamalayan,Victoria,29.1",

        "Pola,Calapan,40.9", "Pola,Baco,43.9", "Pola,Bansud,31.6", "Pola,Bongabong,44.4",
        "Pola,Bulalacao,91.6", "Pola,Gloria,18.1", "Pola,Mansalay,69.4", "Pola,Naujan,25",
        "Pola,Pinamalayan,12.5", "Pola,Puerto Galera,66.7", "Pola,Roxas,62.2", "Pola,San Teodoro,52.1",
        "Pola,Socorro,10.5", "Pola,Victoria,21.7",

        "Puerto Galera,Calapan,27.2", "Puerto Galera,Baco,23.1", "Puerto Galera,Bansud,90.4", "Puerto Galera,Bongabong,102.6",
        "Puerto Galera,Bulalacao,138", "Puerto Galera,Gloria,80.7", "Puerto Galera,Mansalay,121.8", "Puerto Galera,Naujan,43.3",
        "Puerto Galera,Pinamalayan,77.5", "Puerto Galera,Pola,66.7", "Puerto Galera,Roxas,119", "Puerto Galera,San Teodoro,19.9",
        "Puerto Galera,Socorro,70.5", "Puerto Galera,Victoria,50",

        "Roxas,Calapan,98.4", "Roxas,Baco,96.6", "Roxas,Bansud,30.8", "Roxas,Bongabong,17.8",
        "Roxas,Bulalacao,34.9", "Roxas,Gloria,44.1", "Roxas,Mansalay,11.5", "Roxas,Naujan,84.9",
        "Roxas,Pinamalayan,50.1", "Roxas,Pola,62.2", "Roxas,Puerto Galera,119", "Roxas,San Teodoro,99.8",
        "Roxas,Socorro,53.1", "Roxas,Victoria,69.7",

        "San Teodoro,Calapan,21.4", "San Teodoro,Baco,11", "San Teodoro,Bansud,72.2", "San Teodoro,Bongabong,83.9",
        "San Teodoro,Bulalacao,118.1", "San Teodoro,Gloria,63.8", "San Teodoro,Mansalay,102.2", "San Teodoro,Naujan,32.7",
        "San Teodoro,Pinamalayan,61.4", "San Teodoro,Pola,52.1", "San Teodoro,Puerto Galera,19.9", "San Teodoro,Roxas,99.8",
        "San Teodoro,Socorro,53.8", "San Teodoro,Victoria,32.6",

        "Socorro,Calapan,46.6", "Socorro,Baco,47.4", "Socorro,Bansud,22.3", "Socorro,Bongabong,35.4",
        "Socorro,Bulalacao,81.4", "Socorro,Gloria,10.3", "Socorro,Mansalay,59.6", "Socorro,Naujan,32",
        "Socorro,Pinamalayan,8.1", "Socorro,Pola,10.5", "Socorro,Puerto Galera,70.5", "Socorro,Roxas,53.1",
        "Socorro,San Teodoro,53.8", "Socorro,Victoria,21.3",

        "Victoria,Calapan,29.2", "Victoria,Baco,27.1", "Victoria,Bansud,40.4", "Victoria,Bongabong,52.8",
        "Victoria,Bulalacao,93", "Victoria,Gloria,31.2", "Victoria,Mansalay,74", "Victoria,Naujan,19.9",
        "Victoria,Pinamalayan,29.1", "Victoria,Pola,21.7", "Victoria,Puerto Galera,50", "Victoria,Roxas,69.7",
        "Victoria,San Teodoro,32.6", "Victoria,Socorro,21.3"
    };

            foreach (string entry in distanceData)
            {
                string[] parts = entry.Split(',');
                string from = parts[0].Trim();
                string to = parts[1].Trim();
                int distance = (int)Math.Round(Convert.ToDecimal(parts[2]));

                if (!distanceGraph.ContainsKey(from))
                    distanceGraph[from] = new Dictionary<string, int>();

                distanceGraph[from][to] = distance;
            }
        }
        private void cboDestination_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateTotalFare();
        }
    }
}
