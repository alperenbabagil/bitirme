using BitirmeParsing.ColorParser;
using BitirmeParsing.GenreParser;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// director id'nin aynı anda oluşmasına dikkat
//
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

        string myConnectionString = "server=localhost; uid=root;" + "pwd=1234; database=bitirme;";

        public long addMovieCounter = 0;
        public long addGenreCounter = 0;
        public int addColorCounter = 0;

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

                string cmd = "SET autocommit = 0; INSERT INTO movie VALUES ";
                //string cmd = " INSERT INTO movie_deneme VALUES ";
                int counter = 0;

                string sql = string.Empty;

                foreach (Movie movie in movies)
                {
                    sql += "(NULL,@name" + counter + ", @year" + counter + ",null,null,null,null,null,null,null),";
                    command.Parameters.Add(new MySqlParameter("name" + counter, movie.Name));
                    command.Parameters.Add(new MySqlParameter("year" + counter, movie.Year.ToString()));
                    command.Parameters.Add(new MySqlParameter("genreId" + counter, null));
                    command.Parameters.Add(new MySqlParameter("directorId" + counter, null));
                    command.Parameters.Add(new MySqlParameter("color" + counter, null));
                    command.Parameters.Add(new MySqlParameter("moviecol" + counter, null));
                    command.Parameters.Add(new MySqlParameter("country" + counter, null));
                    command.Parameters.Add(new MySqlParameter("rating" + counter, null));
                    command.Parameters.Add(new MySqlParameter("certificateId" + counter, null));
                    command.Parameters.Add(new MySqlParameter("runningTime" + counter, null));
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

        public void addGenres(List<Genre> genres)
        {
            List<string> nameGenre = new List<string>();
            foreach (Genre genre in genres)
            {
                nameGenre.Add(genre.movieId + "," + genre.genreName);
            }

            if (connection != null)
            {

                MySqlCommand command = connection.CreateCommand();

                string cmd = "SET autocommit = 0; INSERT INTO moviegenre VALUES ";
                int counter = 0;

                string sql = string.Empty;
                foreach (Genre genre in genres)
                {

                    sql += "(NULL,@genreId" + counter + ", @genrename" + counter + "),";
                    command.Parameters.Add(new MySqlParameter("movieId" + counter, genre.movieId));
                    command.Parameters.Add(new MySqlParameter("genrename" + counter, genre.genreName));
                    counter++;
                }
                command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end
                //command.CommandText = cmd+sql.Substring(0, sql.Length - 1); //Remove ',' at the end
                try
                {
                    command.ExecuteNonQuery();
                    addGenreCounter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("addGenre error " + e.Message);
                    Console.WriteLine("addGenre error Genre:" + genres[genres.Count - 1].ToString());
                }
            }
        }
        public void addColors(List<Color> colors)
        {
            List<string> nameColor = new List<string>();
            foreach (Color color in colors)
            {
                nameColor.Add(color.Name + "," + color.colorName);
            }

            if (connection != null)
            {

                MySqlCommand command = connection.CreateCommand();

                string cmd = "SET autocommit = 0; INSERT INTO colors VALUES ";
                int counter = 0;

                string sql = string.Empty;

                foreach (Color color in colors)
                {
                    sql += "(NULL,@moviename" + counter + ", @colorinfo" + counter + "),";
                    command.Parameters.Add(new MySqlParameter("moviename" + counter, color.Name));
                    command.Parameters.Add(new MySqlParameter("colorinfo" + counter, color.colorName));
                    counter++;
                }
                command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end
                try
                {
                    command.ExecuteNonQuery();
                    addColorCounter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("addColor error " + e.Message);
                    Console.WriteLine("addColor error Color:" + colors[colors.Count - 1].ToString());
                }
            }
        }
    }
}
