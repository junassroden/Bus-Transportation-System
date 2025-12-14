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
    public partial class Customers : Form
    {
        public Customers()
        {
            InitializeComponent();
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


        private void Customers_Load(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
            }
        }

        public void LoadData()
        {
            Application.EnableVisualStyles();
            dgvCustomers.CellPainting += dgvCustomers_CellPainting;
            try
            {
                Database db = new Database();
                dgvCustomers.AllowUserToAddRows = false;
                dgvCustomers.DataSource = db.selectCMD("Select * from dbCustomer");
                if (dgvCustomers.Columns["Edit"] == null)
                {
                    DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn
                    {
                        Name = "Edit",
                        HeaderText = "Edit",
                        Text = "Edit",
                        UseColumnTextForButtonValue = true
                    };
                    dgvCustomers.Columns.Add(editBtn);
                }

                if (dgvCustomers.Columns["Delete"] == null)
                {
                    DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn
                    {
                        Name = "Delete",
                        HeaderText = "Delete",
                        Text = "Delete",
                        UseColumnTextForButtonValue = true
                    };
                    dgvCustomers.Columns.Add(deleteBtn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
        private void dgvCustomers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == dgvCustomers.Columns["Edit"].Index || e.ColumnIndex == dgvCustomers.Columns["Delete"].Index))
            {
                e.PaintBackground(e.CellBounds, true);
                Color backColor = (e.ColumnIndex == dgvCustomers.Columns["Edit"].Index)
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

            CustomerAdd addCustomer = new CustomerAdd();
            addCustomer.StartPosition = FormStartPosition.Manual;

            int targetX = (Screen.PrimaryScreen.WorkingArea.Width - addCustomer.Width) / 2;
            int targetY = Screen.PrimaryScreen.WorkingArea.Height / 6;
            int startY = targetY - 100;

            addCustomer.Location = new Point(targetX, startY);
            addCustomer.Opacity = 0;
            addCustomer.TopMost = true;

            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 10;

            animationTimer.Tick += (s, ev) =>
            {
                if (addCustomer.Top < targetY)
                    addCustomer.Top += 5;

                if (addCustomer.Opacity < 1)
                    addCustomer.Opacity += 0.05;

                if (addCustomer.Top >= targetY && addCustomer.Opacity >= 1)
                    animationTimer.Stop();
            };

            addCustomer.FormClosed += (s, ev) =>
            {
                animationTimer.Stop();
                overlay.Close();
            };

            addCustomer.DataSaved += (s, ev) =>
            {
                LoadData();
            };
            addCustomer.Show();
            animationTimer.Start();
        }

        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
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

        private async void btnBookings_Click(object sender, EventArgs e)
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

        private void dgvCustomers_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string customerID = dgvCustomers.Rows[e.RowIndex].Cells["CustomerID"].Value.ToString();

                if (dgvCustomers.Columns[e.ColumnIndex].Name == "Edit")
                {
                    Form overlay = new Form();
                    overlay.BackColor = Color.Black;
                    overlay.Opacity = 0.5;
                    overlay.FormBorderStyle = FormBorderStyle.None;
                    overlay.WindowState = FormWindowState.Maximized;
                    overlay.ShowInTaskbar = false;
                    overlay.StartPosition = FormStartPosition.Manual;
                    overlay.Show();

                    CustomerEdit editForm = new CustomerEdit(customerID);
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

                else if (dgvCustomers.Columns[e.ColumnIndex].Name == "Delete")
                {
                    DialogResult confirm = MessageBox.Show(
                        "Are you sure you want to delete this customer?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            Database db = new Database();
                            string query = $"DELETE FROM dbCustomer WHERE CustomerID = '{customerID}'";
                            db.cudCMD(query);

                            dgvCustomers.Rows.RemoveAt(e.RowIndex);

                            MessageBox.Show("Customer deleted successfully!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting bus: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {

        }
        public void SearchBar(string search = "")
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD("SELECT * FROM dbCustomer");

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string filter = string.Format(
                        "Convert(CustomerID, 'System.String') LIKE '%{0}%' OR " +
                        "Name LIKE '%{0}%' OR " +
                        "Convert(Number, 'System.String') LIKE '%{0}%'",
                        search.Replace("'", "''")
                    );

                    dt.DefaultView.RowFilter = filter;
                }

                dgvCustomers.DataSource = dt;
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
