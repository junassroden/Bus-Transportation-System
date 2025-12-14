using System;
using System.Data;
using System.Windows.Forms;

namespace PublicTransportationSystem.Forms
{
    public partial class busEdit : Form
    {
        private string _busID;
        public event EventHandler DataSaved;

        public busEdit(string busID)
        {
            InitializeComponent();
            _busID = busID;
        }

        public busEdit()
        {
            InitializeComponent();
        }

        private void busEdit_Load(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();
                DataTable dt = db.selectCMD($"SELECT * FROM dbBus WHERE BusID = '{_busID}'");

                if (dt.Rows.Count > 0)
                {
                    txtNumber.Text = dt.Rows[0]["BusID"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bus data: " + ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Database db = new Database();

                string query = $"UPDATE dbBus SET BusID = '{txtNumber.Text}' WHERE BusID = '{_busID}'";
                db.cudCMD(query);

                MessageBox.Show("Bus ID updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DataSaved?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating bus: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
