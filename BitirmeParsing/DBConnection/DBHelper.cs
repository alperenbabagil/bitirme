<<<<<<< HEAD
﻿
using BitirmeParsing.ActorParser;
using BitirmeParsing.GenreParser;
using BitirmeParsing.SoundTrackParser;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// director id'nin aynı anda oluşmasına dikkat
//
namespace BitirmeParsing.DBConnection
{
    public class DBHelper
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

        Stopwatch stopwatch = new Stopwatch();

        private DBHelper() { }

        private MySqlConnection connection;

        public int updateCtr = 0;

        //string myConnectionString = "server=localhost; uid=root;" + "pwd=1234; database=bitirme;";
        string myConnectionString = ConfigurationSettings.AppSettings["mysqlConnectionString"];


        public long addMovieCounter = 0;
        public long addGenreCounter = 0;
        public int addColorCounter = 0;
        public int updateMovieCounter = 0;
        public int nullNameCounter = 0;

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

        public void addMovie(List<Movie> movies, string tableName)
        {

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();
                    var exampleMov = movies[0];


                    //string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (name,year,directorId,color,country,rating,certificate,runningTime) VALUES ", tableName);
                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (name,year", tableName);
                    if (exampleMov.color != null) cmd += ",color";
                    if (exampleMov.certificate != null) cmd += ",certificate";
                    if (exampleMov.directorId != 0) cmd += ",directorId";
                    if (exampleMov.country != null) cmd += ",country";
                    if (exampleMov.rating != 0) cmd += ",rating";
                    if (exampleMov.runningTime != 0) cmd += ",runningTime";
                    cmd += ") VALUES ";

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (Movie movie in movies)
                    {
                        if (movie.Name == null)
                        {
                            nullNameCounter++;
                            Console.WriteLine("nullname ctr " + nullNameCounter);
                        }
                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);


                        sql += string.Format("(@name{0},@year{0}", counter);

                        if (exampleMov.color != null) cmd += ",@color{0}";
                        if (exampleMov.certificate != null) cmd += ",@certificate{0}";
                        if (exampleMov.directorId != 0) cmd += ",@directorId{0}";
                        if (exampleMov.country != null) cmd += ",@country{0}";
                        if (exampleMov.rating != 0) cmd += ",@rating{0}";
                        if (exampleMov.runningTime != 0) cmd += ",@runningTime{0}";

                        sql += "),";

                        command.Parameters.Add(new MySqlParameter("name" + counter, movie.Name));
                        command.Parameters.Add(new MySqlParameter("year" + counter, movie.Year.ToString()));
                        if (exampleMov.directorId != 0) command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        if (exampleMov.color != null) command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        if (exampleMov.country != null) command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        if (exampleMov.rating != 0) command.Parameters.Add(new MySqlParameter("rating" + counter, movie.rating));
                        if (exampleMov.certificate != null) command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        if (exampleMov.runningTime != 0) command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("addMovie error " + e.Message);
                        Console.WriteLine("addMovie error Movie:" + movies[movies.Count - 1].ToString());
                    }

                }
            }



        }

        public void addActors(List<Actor> actors,string tableName) // actor-actress
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();


                    //string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (name,year,directorId,color,country,rating,certificate,runningTime) VALUES ", tableName);
                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (id,name) VALUES ", tableName);

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (Actor actor in actors)
                    {
                        if (actor.Name == null)
                        {
                            nullNameCounter++;
                            Console.WriteLine("nullname ctr " + nullNameCounter);
                        }
                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@id{0},@name{0}),", counter);

                        command.Parameters.Add(new MySqlParameter("name" + counter, actor.Name));
                        command.Parameters.Add(new MySqlParameter("id" + counter, actor.id.ToString()));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        //command.Parameters.Add(new MySqlParameter("rating" + counter, movie.rating));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("add actor actress error " + e.Message);
                        //Console.WriteLine("addMovie error Movie:" + movies[movies.Count - 1].ToString());
                    }

                }
            }
        }

        public void addSoundtrack(List<Soundtrack> soundtracks)
        {
            if (connection == null) openConnection();

            MySqlCommand command = connection.CreateCommand();

            string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (id,name,performer) VALUES ", "soundtrack");

            int counter = 0;

            string sql = string.Empty;

            foreach (Soundtrack soundtrack in soundtracks)
            {
                if (soundtrack.name == null)
                {
                    nullNameCounter++;
                    Console.WriteLine("nullname ctr " + nullNameCounter);
                }
                //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                sql += string.Format("(@id{0},@name{0},@performer{0}),", counter);

                command.Parameters.Add(new MySqlParameter("name" + counter, soundtrack.name));
                command.Parameters.Add(new MySqlParameter("performer" + counter, soundtrack.performer));
                //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                command.Parameters.Add(new MySqlParameter("id" + counter, soundtrack.id));
                //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                counter++;
            }
            command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


            try
            {
                command.ExecuteNonQuery();
                //addMovieCounter++;
            }
            catch (Exception e)
            {
                Console.WriteLine("addsoundtrack error " + e.Message);
                //Console.WriteLine("addsoundtrack error Movie:" + movies[movies.Count - 1].ToString());
            }
        }

        public int searchMovie(string name)
        {
            openConnection();
            MySqlCommand command = connection.CreateCommand();

            for (int i = 0; i < 100000; i++)
            {
                string cmd = "SELECT name FROM movie_deneme WHERE name = 'Nenasytnye';  ";
                command.CommandText = cmd;
                command.ExecuteNonQuery();
            }

            return 0;
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

        public void addGenres(List<Genre> genres,string tableName)
        {
            List<string> nameGenre = new List<string>();
            foreach (Genre genre in genres)
            {
                nameGenre.Add(genre.movieId + "," + genre.genreName);
            }

            if (connection != null)
            {

                MySqlCommand command = connection.CreateCommand();

                string cmd = "SET autocommit = 0; INSERT INTO "+tableName+" VALUES ";
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

        public void addDirectors(List<Director> directors)
        {
            openConnection();
            if (connection != null)
            {
                //create command and assign the query and connection from the constructor

                MySqlCommand command = connection.CreateCommand();

                string cmd = "SET autocommit = 0; INSERT INTO director (id,name) VALUES ";
                //string cmd = " INSERT INTO movie_deneme VALUES ";
                int counter = 0;

                string sql = string.Empty;

                foreach (Director director in directors)
                {
                    //sql += "(null,@name" + counter + ", @year" + counter + ",null,null,null,null,null,null)";
                    //command.Parameters.Add(new MySqlParameter("name" + counter, movie.Name));
                    //command.Parameters.Add(new MySqlParameter("year" + counter, movie.Year.ToString()));
                    ////command.Parameters.Add(new MySqlParameter("genreId" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("directorId" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("color" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("moviecol" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("country" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("rating" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("certificateId" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("runningTime" + counter, null));

                    sql += "(@id" + counter + ", @name" + counter + "),";
                    command.Parameters.Add(new MySqlParameter("id" + counter, director.id.ToString()));
                    command.Parameters.Add(new MySqlParameter("name" + counter, director.Name));
                    counter++;
                }

                command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end
                //command.CommandText = cmd+sql.Substring(0, sql.Length - 1); //Remove ',' at the end

                try
                {
                    command.ExecuteNonQuery();
                    //addMovieCounter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("addMovie error " + e.Message);
                    //Console.WriteLine("addMovie error Movie:" + movies[movies.Count - 1].ToString());
                }
            }
        }

        public void addSoundtrackMovie(List<SoundtrackMovie> soundtrackMovies)
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();

                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (movieId,soundtrackId) VALUES ", "soundtrackmovie");

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (SoundtrackMovie soundtrackMovie in soundtrackMovies)
                    {

                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@movieId{0},@soundtrackId{0}),", counter);

                        //command.Parameters.Add(new MySqlParameter("id" + counter, soundtrackMovie.id));
                        command.Parameters.Add(new MySqlParameter("movieId" + counter, soundtrackMovie.movieId));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        command.Parameters.Add(new MySqlParameter("soundtrackId" + counter, soundtrackMovie.SoundtrackId));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        //addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("addsoundtrack error " + e.Message);
                        //Console.WriteLine("addsoundtrack error Movie:" + movies[movies.Count - 1].ToString());
                    }
                }
            }

        }

        public void addActorMovie(List<ActorMovie> actorMovies)
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();

                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (actorid,movieid) VALUES ", "actormovie");

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (ActorMovie actorMovie in actorMovies)
                    {

                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@actorid{0},@movieid{0}),", counter);

                        //command.Parameters.Add(new MySqlParameter("id" + counter, soundtrackMovie.id));
                        command.Parameters.Add(new MySqlParameter("actorid" + counter, actorMovie.actorID));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        command.Parameters.Add(new MySqlParameter("movieid" + counter, actorMovie.movieID));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        //addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("actorMovie error " + e.Message);
                        //Console.WriteLine("addsoundtrack error Movie:" + movies[movies.Count - 1].ToString());
                    }
                }
            }

        }

        public int findMovieId(string movieName)
        {
            int id = 0;
            if (connection == null) openConnection();


            //var sqlQuery = "Select id from movie where name = '" + movieName + "';";
            var sqlQuery = "Select id from movie_dir_id where name = '" + movieName + "';";

            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
            //cmd.Parameters.AddWithValue("@pname", movieName);
            using (MySqlDataReader Reader = cmd.ExecuteReader())
            {
                while (Reader.Read())
                {
                    id = Int32.Parse(Reader.GetString("id"));

                }
            }
            return id;
        }

        public Movie getMovieByProperty(string tableName, string propertyName, string property, bool putQuotes)
        {
            if (connection == null) openConnection();
            Movie newMovie = new Movie();
            string sqlQuery = null;
            if (putQuotes) sqlQuery = string.Format("Select * from movie where {0} = '{1}'", propertyName, property);
            else sqlQuery = string.Format("Select * from movie where {0} = {1}", propertyName, property);

            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
            //cmd.Parameters.AddWithValue("@pname", movieName);
            using (MySqlDataReader Reader = cmd.ExecuteReader())
            {
                while (Reader.Read())
                {
                    try
                    {
                        newMovie.Name = (Reader.IsDBNull(Reader.GetOrdinal("name"))) ? null : Reader.GetString("name");
                        newMovie.color = (Reader.IsDBNull(Reader.GetOrdinal("color"))) ? null : Reader.GetString("color");
                        newMovie.country = (Reader.IsDBNull(Reader.GetOrdinal("country"))) ? null : Reader.GetString("country");
                        newMovie.certificate = (Reader.IsDBNull(Reader.GetOrdinal("certificate"))) ? null : Reader.GetString("certificate");
                        var Year = (Reader.IsDBNull(Reader.GetOrdinal("year"))) ? null : Reader.GetString("year");
                        var directorId = (Reader.IsDBNull(Reader.GetOrdinal("directorId"))) ? null : Reader.GetString("directorId");
                        var id = (Reader.IsDBNull(Reader.GetOrdinal("id"))) ? null : Reader.GetString("id");
                        var rating = (Reader.IsDBNull(Reader.GetOrdinal("rating"))) ? null : Reader.GetString("rating");
                        var runningTime = (Reader.IsDBNull(Reader.GetOrdinal("runningTime"))) ? null : Reader.GetString("runningTime");

                        newMovie.id = (id != null) ? Int32.Parse(id) : -1;
                        newMovie.Year = (Year != null) ? Int32.Parse(Year) : -1;
                        newMovie.directorId = (directorId != null) ? Int32.Parse(directorId) : -1;
                        newMovie.runningTime = (runningTime != null) ? Int32.Parse(runningTime) : -1;
                        newMovie.rating = (rating != null) ? float.Parse(runningTime) : -1;
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            return newMovie;
        }

        public void updateMovieDirectorColumn(string movieName, int directorId)
        {
            if (connection == null) openConnection();

            //MySqlCommand command = connection.CreateCommand();
            //var sqlQuery = "Select id from movie where name = '" + movieName + "';";
            var sqlQuery = string.Format(" SET autocommit = 0; UPDATE movie SET directorId={0} WHERE name='{1}'", directorId, movieName);

            if (updateCtr % 1000 == 0) sqlQuery = string.Format("UPDATE movie SET directorId={0} WHERE name='{1}'; COMMIT;", directorId, movieName);
            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
            //cmd.Parameters.AddWithValue("@pname", movieName);
            try
            {
                cmd.ExecuteNonQuery();
                updateCtr++;
                if (updateCtr % 1000 == 0)
                {
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine(updateCtr);
                }


            }
            catch (Exception e)
            {

            }

        }

        public void updateMovieDirectorColumn2(List<Director> directors)
        {
            if (connection == null) openConnection();

            MySqlCommand command = connection.CreateCommand();

            string cmd = "SET autocommit = 0; UPDATE bitirme.movie SET directorId= CASE ";

            string sql = "";

            string whereSet = "";

            foreach (Director director in directors)
            {

                foreach (Movie movie in director.movies)
                {
                    sql += string.Format("WHEN name='{0}' THEN {1} ", movie.Name, director.id);
                    updateMovieCounter++;
                    if (updateMovieCounter % 1000 == 0) Console.WriteLine(updateMovieCounter);
                    whereSet += ("'" + movie.Name + "',");
                }

            }

            whereSet = whereSet.Substring(0, whereSet.Length - 1);

            sql += " ELSE name END";

            cmd += (sql + " WHERE name IN (" + whereSet + ")");

            command.CommandText = cmd + "; COMMIT;";
            try
            {
                //stopwatch.Start();
                // if (!stopwatch.IsRunning) stopwatch.Start();
                //Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

                command.ExecuteNonQuery();
                updateCtr++;
                if (updateCtr % 1000 == 0)
                {
                    //stopwatch.Stop();
                    // if (!stopwatch.IsRunning) stopwatch.Start();
                    Console.WriteLine(DateTime.Now);
                    // stopwatch.Start();
                }
                //stopwatch.Stop();
                //stopwatch.Start();
                //Console.WriteLine(updateCtr);

                //Console.WriteLine(updateCtr);
            }
            catch (Exception e)
            {

            }

        }


    }
}

//
//Console.WriteLine("start "+DateTime.Now.Second);

//stopwatch.Start();
//DBHelper.Instance.searchMovie(null);
=======
﻿
using BitirmeParsing.ActorParser;
using BitirmeParsing.GenreParser;
using BitirmeParsing.SoundTrackParser;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// director id'nin aynı anda oluşmasına dikkat
//
namespace BitirmeParsing.DBConnection
{
    public class DBHelper
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

        Stopwatch stopwatch = new Stopwatch();

        private DBHelper() { }

        private MySqlConnection connection;

        public int updateCtr = 0;

        //string myConnectionString = "server=localhost; uid=root;" + "pwd=1234; database=bitirme;";
        string myConnectionString = ConfigurationSettings.AppSettings["mysqlConnectionString"];


        public long addMovieCounter = 0;
        public long addGenreCounter = 0;
        public int addColorCounter = 0;
        public int updateMovieCounter = 0;
        public int nullNameCounter = 0;

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

        public void addMovie(List<Movie> movies,string tableName)
        {

            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();

                    
                    //string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (name,year,directorId,color,country,rating,certificate,runningTime) VALUES ", tableName);
                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (name,year,rating) VALUES ", tableName);

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (Movie movie in movies)
                    {
                        if (movie.Name == null)
                        {
                            nullNameCounter++;
                            Console.WriteLine("nullname ctr " + nullNameCounter);
                        }
                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@name{0},@year{0},@rating{0}),", counter);
                        
                        command.Parameters.Add(new MySqlParameter("name" + counter, movie.Name));
                        command.Parameters.Add(new MySqlParameter("year" + counter, movie.Year.ToString()));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        command.Parameters.Add(new MySqlParameter("rating" + counter, movie.rating));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end
                                                                                                

                    try
                    {
                        command.ExecuteNonQuery();
                        addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("addMovie error " + e.Message);
                        Console.WriteLine("addMovie error Movie:" + movies[movies.Count - 1].ToString());
                    }

                }
            }



        }

        public void addActors(List<Actor> actors,string tableName) // actor-actress
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();


                    //string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (name,year,directorId,color,country,rating,certificate,runningTime) VALUES ", tableName);
                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (id,name) VALUES ", tableName);

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (Actor actor in actors)
                    {
                        if (actor.Name == null)
                        {
                            nullNameCounter++;
                            Console.WriteLine("nullname ctr " + nullNameCounter);
                        }
                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";
             
                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@id{0},@name{0}),", counter);

                        command.Parameters.Add(new MySqlParameter("name" + counter, actor.Name));
                        command.Parameters.Add(new MySqlParameter("id" + counter, actor.id.ToString()));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        //command.Parameters.Add(new MySqlParameter("rating" + counter, movie.rating));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
        }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("add actor actress error " + e.Message);
                        //Console.WriteLine("addMovie error Movie:" + movies[movies.Count - 1].ToString());
                    }

                }
            }
        }

        public void addSoundtrack(List<Soundtrack> soundtracks)
        {
            if (connection == null) openConnection();

            MySqlCommand command = connection.CreateCommand();

            string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (id,name,performer) VALUES ", "soundtrack");

            int counter = 0;

            string sql = string.Empty;

            foreach (Soundtrack soundtrack in soundtracks)
            {
                if (soundtrack.name == null)
                {
                    nullNameCounter++;
                    Console.WriteLine("nullname ctr " + nullNameCounter);
                }
                //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                sql += string.Format("(@id{0},@name{0},@performer{0}),", counter);

                command.Parameters.Add(new MySqlParameter("name" + counter, soundtrack.name));
                command.Parameters.Add(new MySqlParameter("performer" + counter, soundtrack.performer));
                //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                command.Parameters.Add(new MySqlParameter("id" + counter, soundtrack.id));
                //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                counter++;
            }
            command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


            try
            {
                command.ExecuteNonQuery();
                //addMovieCounter++;
            }
            catch (Exception e)
            {
                Console.WriteLine("addsoundtrack error " + e.Message);
                //Console.WriteLine("addsoundtrack error Movie:" + movies[movies.Count - 1].ToString());
            }
        }

        public int searchMovie(string name)
        {
            openConnection();
            MySqlCommand command = connection.CreateCommand();

        //    for (int i = 0; i < 100000; i++)
        //        {                   
        //            string cmd = "SELECT name FROM movie_deneme WHERE name = 'Nenasytnye';  ";
        //            command.CommandText = cmd;
        //            command.ExecuteNonQuery();
        //        }
                
        //        return 0;
        //    } 

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

        public void addGenres(List<Genre> genres,string tableName)
        {
            List<string> nameGenre = new List<string>();
            foreach (Genre genre in genres)
            {
                nameGenre.Add(genre.movieId + "," + genre.genreName);
            }

            if (connection != null)
            {

                MySqlCommand command = connection.CreateCommand();

                string cmd = "SET autocommit = 0; INSERT INTO "+tableName+" VALUES ";
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

        public void addDirectors(List<Director> directors)
        {
            openConnection();
            if (connection != null)
            {
                //create command and assign the query and connection from the constructor

                MySqlCommand command = connection.CreateCommand();

                string cmd = "SET autocommit = 0; INSERT INTO director (id,name) VALUES ";
                //string cmd = " INSERT INTO movie_deneme VALUES ";
                int counter = 0;

                string sql = string.Empty;

                foreach (Director director in directors)
                {
                    //sql += "(null,@name" + counter + ", @year" + counter + ",null,null,null,null,null,null)";
                    //command.Parameters.Add(new MySqlParameter("name" + counter, movie.Name));
                    //command.Parameters.Add(new MySqlParameter("year" + counter, movie.Year.ToString()));
                    ////command.Parameters.Add(new MySqlParameter("genreId" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("directorId" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("color" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("moviecol" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("country" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("rating" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("certificateId" + counter, null));
                    //command.Parameters.Add(new MySqlParameter("runningTime" + counter, null));

                    sql += "(@id" + counter + ", @name" + counter + "),";
                    command.Parameters.Add(new MySqlParameter("id" + counter, director.id.ToString()));
                    command.Parameters.Add(new MySqlParameter("name" + counter, director.Name));
                    counter++;
                }

                command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end
                //command.CommandText = cmd+sql.Substring(0, sql.Length - 1); //Remove ',' at the end

                try
                {
                    command.ExecuteNonQuery();
                    //addMovieCounter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine("addMovie error " + e.Message);
                    //Console.WriteLine("addMovie error Movie:" + movies[movies.Count - 1].ToString());
                }
            }
        }

        public void addSoundtrackMovie(List<SoundtrackMovie> soundtrackMovies)
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();

                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (movieId,soundtrackId) VALUES ", "soundtrackmovie");

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (SoundtrackMovie soundtrackMovie in soundtrackMovies)
                    {

                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@movieId{0},@soundtrackId{0}),", counter);

                        //command.Parameters.Add(new MySqlParameter("id" + counter, soundtrackMovie.id));
                        command.Parameters.Add(new MySqlParameter("movieId" + counter, soundtrackMovie.movieId));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        command.Parameters.Add(new MySqlParameter("soundtrackId" + counter, soundtrackMovie.SoundtrackId));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        //addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("addsoundtrack error " + e.Message);
                        //Console.WriteLine("addsoundtrack error Movie:" + movies[movies.Count - 1].ToString());
                    }
                }
            }

        }

        public void addActorMovie(List<ActorMovie> actorMovies)
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection())
            {
                connection.ConnectionString = myConnectionString;
                connection.Open();

                if (connection != null)
                {
                    MySqlCommand command = connection.CreateCommand();

                    string cmd = string.Format("SET autocommit = 0; INSERT INTO {0} (actorid,movieid) VALUES ", "actormovie");

                    int counter = 0;

                    string sql = string.Empty;

                    foreach (ActorMovie actorMovie in actorMovies)
                    {

                        //sql += "(@name" + counter + ", @year" + counter + ", @directorId" + counter + "),";

                        //sql += string.Format("(@name{0},@year{0},@directorId{0},@color{0},@country{0},@rating{0},@certificate{0},@runningTime{0}),", counter);
                        sql += string.Format("(@actorid{0},@movieid{0}),", counter);

                        //command.Parameters.Add(new MySqlParameter("id" + counter, soundtrackMovie.id));
                        command.Parameters.Add(new MySqlParameter("actorid" + counter, actorMovie.actorID));
                        //command.Parameters.Add(new MySqlParameter("directorId" + counter, movie.directorId.ToString()));
                        //command.Parameters.Add(new MySqlParameter("color" + counter, movie.color));
                        //command.Parameters.Add(new MySqlParameter("country" + counter, movie.country));
                        command.Parameters.Add(new MySqlParameter("movieid" + counter, actorMovie.movieID));
                        //command.Parameters.Add(new MySqlParameter("certificate" + counter, movie.certificate));
                        //command.Parameters.Add(new MySqlParameter("runningTime" + counter, movie.runningTime.ToString()));
                        counter++;
                    }
                    command.CommandText = cmd + sql.Substring(0, sql.Length - 1) + "; COMMIT;"; //Remove ',' at the end


                    try
                    {
                        command.ExecuteNonQuery();
                        //addMovieCounter++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("actorMovie error " + e.Message);
                        //Console.WriteLine("addsoundtrack error Movie:" + movies[movies.Count - 1].ToString());
                    }
                }
            }

        }

        public int findMovieId(string movieName)
        {
            int id = 0;
            if (connection == null) openConnection();
            
                
                //var sqlQuery = "Select id from movie where name = '" + movieName + "';";
                var sqlQuery = "Select id from movie_dir_id where name = '" + movieName + "';";

                MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                //cmd.Parameters.AddWithValue("@pname", movieName);
                using (MySqlDataReader Reader = cmd.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        id = Int32.Parse(Reader.GetString("id"));

                    }
                }
            return id;
        }

        public Movie getMovieByProperty(string tableName, string propertyName, string property, bool putQuotes)
        {
            if (connection == null) openConnection();
            Movie newMovie = new Movie();
            string sqlQuery = null;
            if (putQuotes) sqlQuery = string.Format("Select * from movie where {0} = '{1}'", propertyName, property);
            else sqlQuery = string.Format("Select * from movie where {0} = {1}", propertyName, property);

            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
            //cmd.Parameters.AddWithValue("@pname", movieName);
            using (MySqlDataReader Reader = cmd.ExecuteReader())
            {
                while (Reader.Read())
                {
                    try
                    {
                        newMovie.Name = (Reader.IsDBNull(Reader.GetOrdinal("name"))) ? null : Reader.GetString("name");
                        newMovie.color = (Reader.IsDBNull(Reader.GetOrdinal("color"))) ? null : Reader.GetString("color");
                        newMovie.country = (Reader.IsDBNull(Reader.GetOrdinal("country"))) ? null : Reader.GetString("country");
                        newMovie.certificate = (Reader.IsDBNull(Reader.GetOrdinal("certificate"))) ? null : Reader.GetString("certificate");
                        var Year = (Reader.IsDBNull(Reader.GetOrdinal("year"))) ? null : Reader.GetString("year");
                        var directorId = (Reader.IsDBNull(Reader.GetOrdinal("directorId"))) ? null : Reader.GetString("directorId");
                        var id = (Reader.IsDBNull(Reader.GetOrdinal("id"))) ? null : Reader.GetString("id");
                        var rating = (Reader.IsDBNull(Reader.GetOrdinal("rating"))) ? null : Reader.GetString("rating");
                        var runningTime = (Reader.IsDBNull(Reader.GetOrdinal("runningTime"))) ? null : Reader.GetString("runningTime");

                        newMovie.id = (id != null) ? Int32.Parse(id) : -1;
                        newMovie.Year = (Year != null) ? Int32.Parse(Year) : -1;
                        newMovie.directorId = (directorId != null) ? Int32.Parse(directorId) : -1;
                        newMovie.runningTime = (runningTime != null) ? Int32.Parse(runningTime) : -1;
                        newMovie.rating = (rating != null) ? float.Parse(runningTime) : -1;
                    }
                    catch (Exception e)
                    {

                    }
                    
                }
            }
            return newMovie;
        }

        public void updateMovieDirectorColumn(string movieName, int directorId)
        {
            if (connection == null) openConnection();

            //MySqlCommand command = connection.CreateCommand();
            //var sqlQuery = "Select id from movie where name = '" + movieName + "';";
            var sqlQuery = string.Format(" SET autocommit = 0; UPDATE movie SET directorId={0} WHERE name='{1}'", directorId, movieName);

            if (updateCtr % 1000 == 0) sqlQuery = string.Format("UPDATE movie SET directorId={0} WHERE name='{1}'; COMMIT;", directorId, movieName);
            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
            //cmd.Parameters.AddWithValue("@pname", movieName);
            try
            {
                cmd.ExecuteNonQuery();
                updateCtr++;
                if (updateCtr % 1000 == 0)
                {
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine(updateCtr);
                }

                    
            }
            catch (Exception e)
            {

            }
            
        }

        public void updateMovieDirectorColumn2(List<Director> directors)
        {
            if (connection == null) openConnection();

            MySqlCommand command = connection.CreateCommand();

            string cmd = "SET autocommit = 0; UPDATE bitirme.movie SET directorId= CASE ";

            string sql = "";

            string whereSet = "";

            foreach (Director director in directors)
            {

                foreach (Movie movie in director.movies)
                {
                    sql += string.Format("WHEN name='{0}' THEN {1} ", movie.Name, director.id);
                    updateMovieCounter++;
                    if (updateMovieCounter % 1000 == 0) Console.WriteLine(updateMovieCounter);
                    whereSet += ("'" + movie.Name + "',");
                }

            }

            whereSet = whereSet.Substring(0, whereSet.Length - 1);

            sql += " ELSE name END";

            cmd += (sql + " WHERE name IN (" + whereSet + ")");

            command.CommandText = cmd + "; COMMIT;";
            try
            {
                //stopwatch.Start();
               // if (!stopwatch.IsRunning) stopwatch.Start();
                //Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
                
                command.ExecuteNonQuery();
                updateCtr++;
                if (updateCtr % 1000 == 0)
                {
                    //stopwatch.Stop();
                    // if (!stopwatch.IsRunning) stopwatch.Start();
                    Console.WriteLine(DateTime.Now);                   
                   // stopwatch.Start();
                }
                //stopwatch.Stop();
                //stopwatch.Start();
                //Console.WriteLine(updateCtr);

                //Console.WriteLine(updateCtr);
            }
            catch (Exception e)
            {
                // ignored
            }
        }


    }
}

//
//Console.WriteLine("start "+DateTime.Now.Second);

//stopwatch.Start();
//DBHelper.Instance.searchMovie(null);
>>>>>>> master
//stopwatch.Stop();