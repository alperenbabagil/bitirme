using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.DBConnection
{
    class DBHelper
    {
        private static DBHelper _instance;

        public static DBHelper Instance
        {
            get
            {
                if (_instance == null) _instance = new DBHelper();
                return _instance;
            }
        }

        private DBHelper() { }

        private MySqlConnection connection;

        string myConnectionString = "server=127.0.0.1;uid=root;" +
            "pwd=asede;database=bitirme;";

        public long addMovieCounter = 0;

        public bool openConnection()
        {
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection();
                connection.ConnectionString = myConnectionString;
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public void addMovie(List<Movie> movies)
        {


            //string query = "SET autocommit = 0; ";


            if (connection!=null)
            {
                //create command and assign the query and connection from the constructor

                MySqlCommand command = connection.CreateCommand();

                foreach (Movie movie in movies)
                {              
                    command.CommandText = "SET autocommit = 0; INSERT INTO movies (name, year) VALUES (?name,?year); COMMIT;";
                    command.Parameters.AddWithValue("?name", movie.Name);
                    command.Parameters.AddWithValue("?year", movie.Year);
                }

                //Execute command
                try
                {
                    command.ExecuteNonQuery();
                    addMovieCounter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("addMovie error " + e.Message);
                    Console.WriteLine("addMovie error Movie:" + movies[movies.Count].ToString());
                }
                
            }
        }



    }
}
