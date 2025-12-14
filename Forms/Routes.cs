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
    public partial class Routes : Form
    {
        public Routes()
        {
            InitializeComponent();
        }
        private void Routes_Load(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();
                LoadData();
            }
            catch (Exception ex)
            {

            }
        }
        public void LoadData()
        {
            Application.EnableVisualStyles();
            dgvRoutes.CellPainting += dgvRoutes_CellPainting;
            try
            {
                Database db = new Database();
                dgvRoutes.AllowUserToAddRows = false;
                dgvRoutes.DataSource = db.selectCMD("Select * from dbRoutes");
                dgvRoutes.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dgvRoutes.Columns["Edit"] == null)
                {
                    DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn
                    {
                        Name = "Edit",
                        HeaderText = "Edit",
                        Text = "Edit",
                        UseColumnTextForButtonValue = true
                    };
                    dgvRoutes.Columns.Add(editBtn);
                }

                if (dgvRoutes.Columns["Delete"] == null)
                {
                    DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn
                    {
                        Name = "Delete",
                        HeaderText = "Delete",
                        Text = "Delete",
                        UseColumnTextForButtonValue = true
                    };
                    dgvRoutes.Columns.Add(deleteBtn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void dgvRoutes_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == dgvRoutes.Columns["Edit"].Index || e.ColumnIndex == dgvRoutes.Columns["Delete"].Index))
            {
                e.PaintBackground(e.CellBounds, true);

                Color backColor = (e.ColumnIndex == dgvRoutes.Columns["Edit"].Index)
                    ? Color.FromArgb(23, 26, 30)    
                    : Color.FromArgb(150, 30, 30); 

                Color foreColor = Color.White;
                Color borderColor = Color.White; 

                Rectangle rect = new Rectangle(e.CellBounds.X + 2, e.CellBounds.Y + 2,
                                               e.CellBounds.Width - 5, e.CellBounds.Height - 5);

                int borderRadius = 8;

                using (System.Drawing.Drawing2D.GraphicsPath path = RoundedRect(rect, borderRadius))
                {
                    using (SolidBrush brush = new SolidBrush(backColor))
                    {
                        e.Graphics.FillPath(brush, path);
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

            RouteAdd routes = new RouteAdd();
            routes.StartPosition = FormStartPosition.Manual;

            int targetX = (Screen.PrimaryScreen.WorkingArea.Width - routes.Width) / 2;
            int targetY = Screen.PrimaryScreen.WorkingArea.Height / 6;
            int startY = targetY - 100;

            routes.Location = new Point(targetX, startY);
            routes.Opacity = 0;
            routes.TopMost = true;

            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 10;

            animationTimer.Tick += (s, ev) =>
            {
                if (routes.Top < targetY)
                    routes.Top += 5;

                if (routes.Opacity < 1)
                    routes.Opacity += 0.05;

                if (routes.Top >= targetY && routes.Opacity >= 1)
                    animationTimer.Stop();
            };

            routes.FormClosed += (s, ev) =>
            {
                animationTimer.Stop();
                overlay.Close();
            };

            routes.DataSaved += (s, ev) =>
            {
                LoadData();
            };
            routes.Show();
            animationTimer.Start();
        }

        private void dgvRoutes_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private async void btnBus_Click_1(object sender, EventArgs e)
        {
            Buses buses = new Buses();
            buses.StartPosition = FormStartPosition.Manual;
            buses.Location = this.Location;
            buses.Show();

            await Task.Delay(200);
            this.Close();
        }

        private async void btnCustomers_Click_1(object sender, EventArgs e)
        {
            Customers customers = new Customers();
            customers.StartPosition = FormStartPosition.Manual;
            customers.Location = this.Location;
            customers.Show();

            await Task.Delay(200);
            this.Close();
        }

        private async void btnBookings_Click_1(object sender, EventArgs e)
        {
            Bookings bookings = new Bookings();
            bookings.StartPosition = FormStartPosition.Manual;
            bookings.Location = this.Location;
            bookings.Show();

            await Task.Delay(200);
            this.Close();
        }

        private void btnExit_Click_1(object sender, EventArgs e)
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

        private void dgvRoutes_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string RouteID = dgvRoutes.Rows[e.RowIndex].Cells["RouteID"].Value.ToString();

                if (dgvRoutes.Columns[e.ColumnIndex].Name == "Edit")
                {
                    Form overlay = new Form();
                    overlay.BackColor = Color.Black;
                    overlay.Opacity = 0.5;
                    overlay.FormBorderStyle = FormBorderStyle.None;
                    overlay.WindowState = FormWindowState.Maximized;
                    overlay.ShowInTaskbar = false;
                    overlay.StartPosition = FormStartPosition.Manual;
                    overlay.Show();

                    RouteEdit editForm = new RouteEdit(RouteID);
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

                else if (dgvRoutes.Columns[e.ColumnIndex].Name == "Delete")
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
                            string query = $"DELETE FROM dbRoutes WHERE RouteID = '{RouteID}'";
                            db.cudCMD(query);

                            dgvRoutes.Rows.RemoveAt(e.RowIndex);

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
        public void SearchBar(string search = "")
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD("SELECT * FROM dbRoutes");

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string filter = string.Format(
                        "Convert(RouteID, 'System.String') LIKE '%{0}%' OR " +
                        "ViaCities LIKE '%{0}%' OR " +
                        "Bus LIKE '%{0}%' OR " +
                        "Convert(DepartureDate, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(DepartureTime, 'System.String') LIKE '%{0}%' OR " +
                        "Convert(Cost, 'System.String') LIKE '%{0}%'",
                        search.Replace("'", "''")
                    );

                    dt.DefaultView.RowFilter = filter;
                }

                dgvRoutes.DataSource = dt;
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