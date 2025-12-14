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
    public partial class Bookings : Form
    {
        public Bookings()
        {
            InitializeComponent();
        }

       
        private void Bookings_Load(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();
                LoadData();
            }
            catch(Exception ex){

            }
        }

        private void SortDataTableByPNR(DataTable dt)
        {
            if (!dt.Columns.Contains("PNR"))
                return;

            DataRow[] rows = dt.Select();
            int n = rows.Length;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    string pnr1 = rows[j]["PNR"].ToString();
                    string pnr2 = rows[j + 1]["PNR"].ToString();

                    if (string.Compare(pnr1, pnr2, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        DataRow temp = rows[j];
                        rows[j] = rows[j + 1];
                        rows[j + 1] = temp;
                    }
                }
            }
            DataTable sortedTable = dt.Clone();
            foreach (DataRow row in rows)
                sortedTable.ImportRow(row);

            dt.Clear();
            foreach (DataRow row in sortedTable.Rows)
                dt.ImportRow(row);
        }
        public void LoadData()
        {
            dgvBookings.CellPainting += dgvBookings_CellPainting;
            try
            {
                Database db = new Database();
                dgvBookings.AllowUserToAddRows = false;
                DataTable dt = db.selectCMD("SELECT * FROM dbBooking");
                SortDataTableByPNR(dt);

                dgvBookings.DataSource = dt;
                dgvBookings.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dgvBookings.Columns["Edit"] == null)
                {
                    DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn
                    {
                        Name = "Edit",
                        HeaderText = "Edit",
                        Text = "Edit",
                        UseColumnTextForButtonValue = true,
                        FlatStyle = FlatStyle.Flat
                    };
                    dgvBookings.Columns.Add(editBtn);
                }
                if (dgvBookings.Columns["Delete"] == null)
                {
                    DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn
                    {
                        Name = "Delete",
                        HeaderText = "Delete",
                        Text = "Delete",
                        UseColumnTextForButtonValue = true,
                        FlatStyle = FlatStyle.Flat
                    };
                    dgvBookings.Columns.Add(deleteBtn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void dgvBookings_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == dgvBookings.Columns["Edit"].Index || e.ColumnIndex == dgvBookings.Columns["Delete"].Index))
            {
                e.PaintBackground(e.CellBounds, true);

                Color backColor = (e.ColumnIndex == dgvBookings.Columns["Edit"].Index)
                    ? Color.FromArgb(23, 26, 30)  
                    : Color.FromArgb(150, 30, 30); 

                Color foreColor = (e.ColumnIndex == dgvBookings.Columns["Edit"].Index) ? Color.White : Color.Black;
                Color borderColor = Color.White;

                Rectangle rect = new Rectangle(e.CellBounds.X + 2, e.CellBounds.Y + 2,
                                               e.CellBounds.Width - 5, e.CellBounds.Height - 5);

                int borderRadius = 8;

                using (System.Drawing.Drawing2D.GraphicsPath path = RoundedRect(rect, borderRadius))
                {
                    using (SolidBrush backBrush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillPath(backBrush, path);
                    }

                    using (Pen pen = new Pen(borderColor, 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    e.FormattedValue?.ToString() ?? "",
                    e.CellStyle.Font,
                    rect,
                    foreColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddLine(bounds.X + radius, bounds.Y, bounds.Right - radius, bounds.Y);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddLine(bounds.Right, bounds.Y + radius, bounds.Right, bounds.Bottom - radius);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.X + radius, bounds.Bottom);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.AddLine(bounds.X, bounds.Bottom - radius, bounds.X, bounds.Y + radius);

            path.CloseFigure();
            return path;
        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            Form overlay = new Form();
            overlay.BackColor = Color.Black;
            overlay.Opacity = 0.5;
            overlay.FormBorderStyle = FormBorderStyle.None;
            overlay.WindowState = FormWindowState.Maximized;
            overlay.ShowInTaskbar = false;
            overlay.StartPosition = FormStartPosition.Manual;
            overlay.Show();

            BookingAdd bookingsAdd = new BookingAdd();
            bookingsAdd.StartPosition = FormStartPosition.Manual;

            int targetX = (Screen.PrimaryScreen.WorkingArea.Width - bookingsAdd.Width) / 2;
            int targetY = Screen.PrimaryScreen.WorkingArea.Height / 10;
            int startY = targetY - 100;

            bookingsAdd.Location = new Point(targetX, startY);
            bookingsAdd.Opacity = 0;
            bookingsAdd.TopMost = true;

            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 10;

            animationTimer.Tick += (s, ev) =>
            {
                if (bookingsAdd.Top < targetY)
                    bookingsAdd.Top += 5;

                if (bookingsAdd.Opacity < 1)
                    bookingsAdd.Opacity += 0.05;

                if (bookingsAdd.Top >= targetY && bookingsAdd.Opacity >= 1)
                    animationTimer.Stop();
            };

            bookingsAdd.FormClosed += (s, ev) =>
            {
                animationTimer.Stop();
                overlay.Close();
            };

            bookingsAdd.DataSaved += (s, ev) =>
            {
                LoadData();
            };
            bookingsAdd.Show();
            animationTimer.Start();
        }

        private void dgvBookings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private async void btnDash_Click_1(object sender, EventArgs e)
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

        private void dgvBookings_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string PNR = dgvBookings.Rows[e.RowIndex].Cells["PNR"].Value.ToString();

                if (dgvBookings.Columns[e.ColumnIndex].Name == "Edit")
                {
                    Form overlay = new Form();
                    overlay.BackColor = Color.Black;
                    overlay.Opacity = 0.5;
                    overlay.FormBorderStyle = FormBorderStyle.None;
                    overlay.WindowState = FormWindowState.Maximized;
                    overlay.ShowInTaskbar = false;
                    overlay.StartPosition = FormStartPosition.Manual;
                    overlay.Show();

                    BookingEdit bookingEdit = new BookingEdit(PNR);
                    bookingEdit.StartPosition = FormStartPosition.Manual;

                    int targetX = (Screen.PrimaryScreen.WorkingArea.Width - bookingEdit.Width) / 2;
                    int targetY = Screen.PrimaryScreen.WorkingArea.Height / 10;
                    int startY = targetY - 100;

                    bookingEdit.Location = new Point(targetX, startY);
                    bookingEdit.Opacity = 0;
                    bookingEdit.TopMost = true;

                    System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
                    animationTimer.Interval = 10;

                    animationTimer.Tick += (s, ev) =>
                    {
                        if (bookingEdit.Top < targetY)
                            bookingEdit.Top += 5;

                        if (bookingEdit.Opacity < 1)
                            bookingEdit.Opacity += 0.05;

                        if (bookingEdit.Top >= targetY && bookingEdit.Opacity >= 1)
                            animationTimer.Stop();
                    };

                    bookingEdit.FormClosed += (s, ev) =>
                    {
                        animationTimer.Stop();
                        overlay.Close();
                    };

                    bookingEdit.DataSaved += (s, ev) =>
                    {
                        LoadData();
                    };

                    bookingEdit.Show();
                    animationTimer.Start();
                }

                else if (dgvBookings.Columns[e.ColumnIndex].Name == "Delete")
                {
                    DialogResult confirm = MessageBox.Show(
                        "Are you sure you want to delete this Booking?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            Database db = new Database();
                            string query = $"DELETE FROM dbBooking WHERE PNR = '{PNR}'";
                            db.cudCMD(query);

                            dgvBookings.Rows.RemoveAt(e.RowIndex);

                            MessageBox.Show("Bookings deleted successfully!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting bus: " + ex.Message);
                        }
                    }
                }
            }
        }
        public void SearchBar(string search = "")
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD("SELECT * FROM dbBooking");

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string filter = string.Format(
                        "PNR LIKE '%{0}%' OR " +
                        "Name LIKE '%{0}%' OR " +
                        "Contact LIKE '%{0}%' OR " +
                        "Bus LIKE '%{0}%' OR " +
                        "Route LIKE '%{0}%' OR " +
                        "Destination LIKE '%{0}%' OR " +
                        "Convert(Amount, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(Departure, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(Booked, 'System.String') LIKE '%{0}%'",
                        search.Replace("'", "''")
                    );

                    dt.DefaultView.RowFilter = filter;
                }

                dgvBookings.DataSource = dt;  // Correct DataGridView for Bookings
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
            }
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchBar(txtSearch.Text);
        }
    }
}
