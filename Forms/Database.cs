using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;


namespace PublicTransportationSystem.Forms
{
    internal class Database
    {
        private SqlConnection con;
        SqlDataAdapter DA;
        DataTable dTable;
        SqlCommand cmd;
        string conStr;
        public Database()
        {
            conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\HP\Desktop\Pang GitHub\PublicTransportationSystem\PublicTransportationSystem\Database\dbTranspo.mdf"";Integrated Security=True";
            con = new SqlConnection(conStr);
            con.Open();
        }

        public bool CheckLogin(string username, string password)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                return count > 0;
            }

        }
        public void executeCMD(string sql)
        {
            cudCMD(sql);
        }
        public int cudCMD(string sql)
        {
            cmd = new SqlCommand(sql, con);
            return cmd.ExecuteNonQuery();
        }

        public DataTable selectCMD(string sql)
        {
            dTable = new DataTable();
            DA = new SqlDataAdapter(sql, conStr);
            DA.Fill(dTable);
            DA.Dispose();
            return dTable;
        }
    }
}
