using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing
{
    class MySQLConnectionTest
    {
        public static bool testConnection()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = "server=127.0.0.1;uid=root;" +
            "pwd=asede;database=bitirme;";
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                conn.Close();
                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }
        
    }
}
