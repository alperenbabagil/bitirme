using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            List<string> names = new List<string>();
            List<string> nameyear = new List<string>();
            //List<string> years = new List<string>();
            List<int> years = new List<int>();
            foreach (Movie movie in movies)
            {
                nameyear.Add(movie.Name + "," + movie.Year.ToString());
                //names.Add(movie.Name);
                //years.Add(movie.Year.ToString());
                //years.Add(movie.Year);
                //command.Parameters.AddRange
            }
            //string query = "SET autocommit = 0; ";


            if (connection!=null)
            {
                //create command and assign the query and connection from the constructor

                MySqlCommand command = connection.CreateCommand();

                //command.CommandText = "SET autocommit = 0; INSERT INTO movie_deneme (name, year) VALUES (?name,?year); COMMIT;";
                

                //var param1 = command.Parameters.Add("?nameyear", MySqlDbType.VarChar);
                //var param2 = command.Parameters.Add("?year", MySqlDbType.Int32);

                //param1.Value = string.Join(",", nameyear);
                //param2.Value = string.Join(",", years);

                //command.CommandText = @"SET autocommit = 0; INSERT INTO movie_deneme (name, year) VALUES (?name,?year); COMMIT;";
                //command.CommandText = @"SET autocommit = 0; INSERT INTO movie_deneme (name,year) VALUES (?name,?year); COMMIT;";
                //command.CommandText = @"SET autocommit = 0; INSERT INTO movie_deneme (name,year) VALUES (?nameyear); COMMIT;";

                //foreach (Movie movie in movies)
                //{

                //    //command.Parameters.AddWithValue("?name", movie.Name);
                //    var param1 = command.Parameters.Add("?name", MySqlDbType.VarChar);
                //    var param2 = command.Parameters.Add("?year", MySqlDbType.Int32);

                //    param1.Value = string.Join(",", names);
                //    param2.Value = string.Join(",", years);

                //    //command.Parameters.AddWithValue("?year", movie.Year);
                //    //command.Parameters.AddRange
                //}

                //Execute command

                //const string refcmdText = " INSERT INTO Entries (name, year) VALUES (@name,@year);";
                //int count = 0;
                //string query = string.Empty;
                ////build a large query
                //foreach (Movie movie in movies)
                //{
                //    query += string.Format(refcmdText, count);
                //    command.Parameters.AddWithValue("@name",  movie.Name);
                //    command.Parameters.AddWithValue("@year",  movie.Year);
                //    count++;
                //}

                ////query +=" COMMIT;";
                //command.CommandText = query;

                string cmd = "SET autocommit = 0; INSERT INTO movie_deneme VALUES ";
                //string cmd = " INSERT INTO movie_deneme VALUES ";
                int counter = 0;

                string sql = string.Empty;

                foreach (Movie movie in movies)
                {
                    sql += "(NULL,@name" + counter + ", @year" + counter + "),";
                    command.Parameters.Add(new MySqlParameter("name" + counter, movie.Name));
                    command.Parameters.Add(new MySqlParameter("year" + counter, movie.Year.ToString()));
                    counter++;
                }

                command.CommandText = cmd+sql.Substring(0, sql.Length - 1)+ "; COMMIT;"; //Remove ',' at the end
                //command.CommandText = cmd+sql.Substring(0, sql.Length - 1); //Remove ',' at the end

                try
                {
                    command.ExecuteNonQuery();
                    addMovieCounter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("addMovie error " + e.Message);
                    Console.WriteLine("addMovie error Movie:" + movies[movies.Count-1].ToString());
                }
                
            }
        }

        //private SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, IEnumerable<T> values, string paramNameRoot, int start = 1, string separator = ", ")
        //{
        //    /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
        //     * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
        //     * IN statement in the CommandText.
        //     */
        //    var parameters = new List<SqlParameter>();
        //    var parameterNames = new List<string>();
        //    var paramNbr = start;
        //    foreach (var value in values)
        //    {
        //        var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
        //        parameterNames.Add(paramName);
        //        parameters.Add(cmd.Parameters.AddWithValue(paramName, value));
        //    }

        //    cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(separator, parameterNames));

        //    return parameters.ToArray();
        //}

    }
}
