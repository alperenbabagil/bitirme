using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BitirmeParsing.DBConnection;
using System.Configuration;

namespace BitirmeParsing.GenreParser
{
    public class GenreParser
    {
        string newTableName;

        public GenreParser(string newTableName_)
        {
            newTableName = newTableName_;
        }
        List<Genre> bufferList;

        BlockingCollection<List<Genre>> dataItems;
        private MySqlConnection connection;

        public int id;
        public void Parse()
        {
            dataItems = new BlockingCollection<List<Genre>>();
            DBHelper.Instance.openConnection();

            bufferList = new List<Genre>();
            List<Genre> blockGenres = new List<Genre>();


            string myConnectionString = "server=localhost; uid=root;" + "pwd=1234; database=bitirme;";
            connection = new MySql.Data.MySqlClient.MySqlConnection();
            connection.ConnectionString = myConnectionString;
            connection.Open();
            MySqlCommand command = connection.CreateCommand();


            long addCounter = 0;
            Task.Run(() =>
            {
 
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["genresListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line = string.Empty;
                string movieName = string.Empty;
                string genreName = string.Empty;
                string sqlQuery = string.Empty;
                int count;
                string[] fields;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue; // bos satiri atladik.
                    fields = line.Split('\t');
                    if (fields[0] != null)
                    {
                        int length = fields.Length;
                        //if (fields[0].Contains("("))
                        //{
                        //    count = fields[0].Count(c => c == '(');
                        //    //count = fields[0].Split('/').Length - 1;            // eğer ismin içinde yılın haricinde bir parantez daha varsa
                        //    if (count > 1)
                        //    {
                        //        continue;
                        //    }
                        //}

                        //movieName = fields[0].Substring(0, fields[0].IndexOf('('));
                        //if (movieName.Contains('"'))
                        //{
                        //    movieName = movieName.Replace('"',' ');
                        //}

                        movieName = MovieParser.MovieParser.extractDbText(fields[0]); //genel isim extract fonksiyonu. Daha sonra düzenlenecek

                        genreName = fields[length - 1];
                        if (genreName.Contains('"'))
                        {
                            genreName = genreName.Replace('"',' ');
                        }
                        
                        //movieName = movieName.Remove(0,1);              // Baştaki ve sondaki boşluğu silmek için yazıldı
                        //movieName = movieName.Remove(movieName.Length - 1);

                       
                        string nameMovieTable = string.Empty;
                        
                        string genreListname = string.Empty;
                        if (connection!=null)
                        {
                            if (movieName.Contains('\''))
                            {
                                movieName = movieName.Replace("'", "");         // single quote silindi.
                            }
                            sqlQuery = "Select id from movie where name = '" + movieName + "';";
                            
                            MySqlCommand cmd = new MySqlCommand(sqlQuery, connection);
                            //cmd.Parameters.AddWithValue("@pname", movieName);
                            using (MySqlDataReader Reader = cmd.ExecuteReader())
                            {
                                while (Reader.Read())
                                {
                                    id = Int32.Parse(Reader.GetString("id"));

                                }
                            }
                        }
                        addCounter++;

                        if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                        {
                            dataItems.Add(blockGenres);
                            blockGenres = new List<Genre>();
                        }
                        

                        blockGenres.Add(new Genre() { movieId = id, genreName = genreName});
                    }
                    if (addCounter % 10000 == 0) Console.WriteLine(addCounter + " " + line);
                }
                dataItems.CompleteAdding();
            }
            });
            while (!dataItems.IsCompleted)
            {

                List<Genre> data = null;
                // Blocks if number.Count == 0
                // IOE means that Take() was called on a completed collection.
                // Some other thread can call CompleteAdding after we pass the
                // IsCompleted check but before we call Take. 
                // In this example, we can simply catch the exception since the 
                // loop will break on the next iteration.
                

                try
                {
                    data = dataItems.Take();
                }
                catch (InvalidOperationException) { }

                if (data != null)
                {
                    addGenretoDb(data);
                }
            }
            Console.WriteLine("\r\nNo more items to take.");
            DBHelper.Instance.closeConnection();
        }
        void addGenretoDb(List<Genre> genres)
        {
            DBHelper.Instance.addGenres(genres,newTableName);
        }
    }
}
