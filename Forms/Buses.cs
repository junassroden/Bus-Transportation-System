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
    public partial class Buses : Form
    {
        public Buses()
        {
            InitializeComponent();
        }
            public void LoadData()
            {
                Application.EnableVisualStyles();
                dgvBus.CellPainting += dgvBus_CellPainting;
                try
                {
                    Database db = new Database();
                    dgvBus.AllowUserToAddRows = false;
                    dgvBus.DataSource = db.selectCMD("SELECT BusNumber, BusID FROM dbBus");
                    dgvBus.Columns["BusNumber"].HeaderText = "Bus Number";
                    dgvBus.Columns["BusID"].HeaderText = "Bus ID";
                    if (dgvBus.Columns["Edit"] == null)
                    {

                        DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn
                        {
                            Name = "Edit",
                            HeaderText = "Edit",
                            Text = "Edit",
                            UseColumnTextForButtonValue = true
                        };
                        dgvBus.Columns.Add(editBtn);
                    }
              

                    if (dgvBus.Columns["Delete"] == null)
                    {
                        DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn
                        {
                            Name = "Delete",
                            HeaderText = "Delete",
                            Text = "Delete",
                            UseColumnTextForButtonValue = true
                        };
                        dgvBus.Columns.Add(deleteBtn);
                    
                    }
               
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message);
                }
            }

            private void dgvBus_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
                if (e.RowIndex >= 0 && (e.ColumnIndex == dgvBus.Columns["Edit"].Index || e.ColumnIndex == dgvBus.Columns["Delete"].Index))
                {
                    e.PaintBackground(e.CellBounds, true);
                    Color backColor = (e.ColumnIndex == dgvBus.Columns["Edit"].Index)
                        ? Color.FromArgb(23, 26, 30)    
                        : Color.FromArgb(150, 30, 30); 

                    Color foreColor = Color.White;
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


        private void Buses_Load(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();
                LoadData();
            }
            catch(Exception ex)
            {

            }
        }
        private void dgvBus_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string busID = dgvBus.Rows[e.RowIndex].Cells["BusID"].Value.ToString();

                if (dgvBus.Columns[e.ColumnIndex].Name == "Edit")
                {
                    dgvBus.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(23, 26, 30);
                    ;
                    Form overlay = new Form();
                    overlay.BackColor = Color.Black;
                    overlay.Opacity = 0.5;
                    overlay.FormBorderStyle = FormBorderStyle.None;
                    overlay.WindowState = FormWindowState.Maximized;
                    overlay.ShowInTaskbar = false;
                    overlay.StartPosition = FormStartPosition.Manual;
                    overlay.Show();

                    busEdit editForm = new busEdit(busID);
                    editForm.StartPosition = FormStartPosition.Manual;

                    int targetX = (Screen.PrimaryScreen.WorkingArea.Width - editForm.Width) / 2;
                    int targetY = Screen.PrimaryScreen.WorkingArea.Height / 6;
                    int startY = targetY - 100;

                    editForm.Location = new Point(targetX, startY);
                    editForm.Opacity = 0;
                    editForm.TopMost = true;

                    System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
                    animationTimer.Interval = 10;

                    animationTimer.Tick += (s, ev) =>
                    {
                        if (editForm.Top < targetY)
                            editForm.Top += 5;

                        if (editForm.Opacity < 1)
                            editForm.Opacity += 0.05;

                        if (editForm.Top >= targetY && editForm.Opacity >= 1)
                            animationTimer.Stop();
                    };

                    editForm.FormClosed += (s, ev) =>
                    {
                        animationTimer.Stop();
                        overlay.Close();
                    };

                    editForm.DataSaved += (s, ev) =>
                    {
                        LoadData();
                    };

                    editForm.Show();
                    animationTimer.Start();
                }

                else if (dgvBus.Columns[e.ColumnIndex].Name == "Delete")
                {
                    DialogResult confirm = MessageBox.Show(
                        "Are you sure you want to delete this bus?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            Database db = new Database();
                            string query = $"DELETE FROM dbBus WHERE BusID = '{busID}'";
                            db.cudCMD(query);

                            dgvBus.Rows.RemoveAt(e.RowIndex);

                            MessageBox.Show("Bus deleted successfully!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting bus: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            Form overlay = new Form();
            overlay.BackColor = Color.Black;
            overlay.Opacity = 0.5;
            overlay.FormBorderStyle = FormBorderStyle.None;
            overlay.WindowState = FormWindowState.Maximized;
            overlay.ShowInTaskbar = false;
            overlay.StartPosition = FormStartPosition.Manual;
            overlay.Show();

            BusDetails busDetails = new BusDetails();
            busDetails.StartPosition = FormStartPosition.Manual;

            int targetX = (Screen.PrimaryScreen.WorkingArea.Width - busDetails.Width) / 2;
            int targetY = Screen.PrimaryScreen.WorkingArea.Height / 6;
            int startY = targetY - 100;

            busDetails.Location = new Point(targetX, startY);
            busDetails.Opacity = 0;
            busDetails.TopMost = true;

            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 10;

            animationTimer.Tick += (s, ev) =>
            {
                if (busDetails.Top < targetY)
                    busDetails.Top += 5;

                if (busDetails.Opacity < 1)
                    busDetails.Opacity += 0.05;

                if (busDetails.Top >= targetY && busDetails.Opacity >= 1)
                    animationTimer.Stop();
            };

            busDetails.FormClosed += (s, ev) =>
            {
                animationTimer.Stop();
                overlay.Close();
            };

            busDetails.DataSaved += (s, ev) =>
            {
                LoadData();
            };
            busDetails.Show();
            animationTimer.Start();
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
        public void SearchBar(string search = "")
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD("SELECT * FROM dbBus");

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string filter = string.Format(
                        "Convert(BusNumber, 'System.String') LIKE '%{0}%' OR " +
                        "BusID LIKE '%{0}%'",
                        search.Replace("'", "''")
                    );

                    dt.DefaultView.RowFilter = filter;
                }

                dgvBus.DataSource = dt; // correct grid for Buses
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
