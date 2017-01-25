using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.MovieParser
{
    public class MovieParser : ParserBase<Movie>
    {
        //List<Movie> bufferList;

        //BlockingCollection<List<Movie>> dataItems;

        //public void Parse()
        //{

        //    dataItems = new BlockingCollection<List<Movie>>();

        //    DBHelper.Instance.openConnection();

        //    bufferList = new List<Movie>();

        //    //long addCounter = 0;

        //    Task.Run(() =>
        //    {
        //        using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["moviesListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        using (BufferedStream bs = new BufferedStream(fs))
        //        using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))        //!!!!!!!!!
        //        {
        //            string line;
        //            string[] fields;
        //            string currentMovieName = null;

        //            List<Movie> blockMovies = new List<Movie>();

        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                if (line.Length == 0) continue;
        //                fields = line.Split('\t');
        //                if (fields[0].Length != 0)
        //                {
        //                    if (!fields[0].Contains("(") || currentMovieName == fields[0].Substring(0, fields[0].IndexOf('('))) continue;

        //                    currentMovieName = fields[0].Substring(0, fields[0].IndexOf('('));

        //                    string yearString = fields[0].Substring(fields[0].IndexOf('(') + 1, 4);

        //                    string nameString = extractDbText(fields[0]);

        //                    int year = 0;

        //                    if (yearString != null && yearString.Length > 0)
        //                    {
        //                        try
        //                        {
        //                            year = Int32.Parse(yearString);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            year = 0;
        //                        }

        //                    }

        //                    addCounter++;

        //                    if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
        //                    {
        //                        dataItems.Add(blockMovies);
        //                        blockMovies = new List<Movie>();
        //                    }

        //                    //addMovieToDb(new Movie() { Name = nameString, Year = year });
        //                    blockMovies.Add(new Movie() { Name = nameString, Year = year });
        //                    //yearString.Remove('"')
        //                }

                        
        //                if (addCounter % 10000 == 0) Console.WriteLine(addCounter + " " + line);
        //                //Console.WriteLine(counter + " " + line);

        //            }
        //            dataItems.CompleteAdding();
        //        }

                
        //    });


        //    //int consumerCounter = 0;

        //    while (!dataItems.IsCompleted )
        //    {

        //        List<Movie> data = null;
        //        // Blocks if number.Count == 0
        //        // IOE means that Take() was called on a completed collection.
        //        // Some other thread can call CompleteAdding after we pass the
        //        // IsCompleted check but before we call Take. 
        //        // In this example, we can simply catch the exception since the 
        //        // loop will break on the next iteration.


                

        //        try
        //        {
        //            data = dataItems.Take();
        //        }
        //        catch (InvalidOperationException) { }

        //        if (data != null)
        //        {
        //            addMovieToDb(data);
        //        }
        //    }
        //    Console.WriteLine("\r\nNo more items to take.");
        //    DBHelper.Instance.closeConnection();


        //}

        public static string extractDbText(string movieName)
        {
            //string yearString = fields[0].Substring(fields[0].IndexOf('(') + 1, 4);
            try
            {
                string nameString = movieName.Substring(0, movieName.IndexOf('('));

                nameString = nameString.Replace("\"", "");
                nameString = nameString.Replace("'", "");
                nameString = nameString.Substring(0,nameString.Length-1);
                return nameString;
            }
            catch (Exception e)
            {
                return " ";
            }
            

            
        }

        void addMovieToDb(List<Movie> movies)
        {
            //DBHelper.Instance.addMovie(movies,"movie_onlyMovie");
            DBHelper.Instance.addMovie(movies, "movie");
        }

        public override void parseLogic(BlockingCollection<List<Movie>> dataItems)
        {
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["moviesListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))        //!!!!!!!!!
            {
                string line;
                string[] fields;
                string currentMovieName = null;

                List<Movie> blockMovies = new List<Movie>();

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;
                    fields = line.Split('\t');
                    if (fields[0].Length != 0)
                    {
                        if (!fields[0].Contains("(") || currentMovieName == fields[0].Substring(0, fields[0].IndexOf('('))) continue;

                        currentMovieName = fields[0].Substring(0, fields[0].IndexOf('('));

                        string yearString = fields[0].Substring(fields[0].IndexOf('(') + 1, 4);

                        string nameString = extractDbText(fields[0]);

                        int year = 0;

                        if (yearString != null && yearString.Length > 0)
                        {
                            try
                            {
                                year = Int32.Parse(yearString);
                            }
                            catch (Exception e)
                            {
                                year = 0;
                            }

                        }

                        addCounter++;
                        if (addCounter % 10000 == 0 ){
                            Console.WriteLine("Movie add" + " " + line + " ctr " + addCounter);
                        }
                       
                        if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                        {
                            
                            dataItems.Add(blockMovies);
                            blockMovies = new List<Movie>();
                            if (limitWithOneWrite) break;
                        }

                        //addMovieToDb(new Movie() { Name = nameString, Year = year });
                        blockMovies.Add(new Movie() { Name = nameString, Year = year });
                        //yearString.Remove('"')
                    }
                }
            }


        }

        public override void writeLogic(List<Movie> writeList)
        {
            DBHelper.Instance.addMovie(writeList, "movie_onlymovie");
        }
    }
}
