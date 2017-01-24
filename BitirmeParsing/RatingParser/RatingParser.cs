using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.RatingParser
{
    public class RatingParser : ParserBase<Movie>
    {

        string newTableName;
        string readFromtableName;

        public RatingParser(string readFromtableName, string newTableName_)
        {
            newTableName = newTableName_;
            this.readFromtableName = readFromtableName;
        }

        //public void Parse()
        //{
        //    dataItems = new BlockingCollection<List<Movie>>();

        //    DBHelper.Instance.openConnection();

        //    long addCounter = 0;

        //    bufferList = new List<Movie>();

        //    Task.Run(() =>
        //    {
        //        using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["ratingsListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //        using (BufferedStream bs = new BufferedStream(fs))
        //        using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
        //        {
        //            string line;
        //            string currentMovieName = null;
        //            int addCtr = 0;

        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                if (line.Length == 0) continue;
        //                line = line.TrimStart();

        //                var components = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

        //                string rating = "";
        //                string movieName = "";

        //                try
        //                {
        //                    rating = components[2];
        //                    for (int i = 3; i < components.Length ; i++)
        //                    {
        //                        movieName += components[i]+" ";
        //                    }
        //                }
        //                catch(Exception e)
        //                {

        //                }

        //                var s = MovieParser.MovieParser.extractDbText(movieName); // her movie name extract işleminde bu kullanılacak
        //                if (currentMovieName == null) currentMovieName = s;
        //                if (currentMovieName != s)
        //                {
        //                    //DBHelper.Instance.updateMovieDirectorColumn(s,currentDirector.id);
        //                    //int id = DBHelper.Instance.findMovieId(s);
        //                    //currentDirector.addMovie(new Movie { Name = s });



        //                    currentMovieName = s;

        //                    Movie movie = DBHelper.Instance.getMovieByProperty(readFromtableName, "name", s, true);

        //                    if (movie.Name == null)
        //                    {

        //                    }
        //                    try
        //                    {
        //                        movie.rating = float.Parse(rating, CultureInfo.InvariantCulture);
        //                    }
        //                    catch (Exception ee)
        //                    {

        //                    }
                            

        //                    addCtr++;
        //                    //Console.WriteLine("addCtr: " + addCtr + "    " + movie.id);
        //                    if (addCtr % 1000 == 0)
        //                    {
        //                        Console.WriteLine("Movie get: " + addCtr + "    " + movie.id);

        //                    }

                                

        //                    if (addCtr % GlobalVariables.writeToDbBulkSize == 0)
        //                    {
        //                        dataItems.Add(bufferList);
        //                        bufferList = new List<Movie>();
        //                    }

        //                    bufferList.Add(movie);

        //                }


        //            }
        //        }
        //    });

        //    while (!dataItems.IsCompleted)
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
        //            DBHelper.Instance.addMovie(data, newTableName);
        //        }
        //    }
        //    Console.WriteLine("\r\nNo more items to take.");
        //    DBHelper.Instance.closeConnection();
        //}

        public override void parseLogic(BlockingCollection<List<Movie>> dataItems)
        {
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["ratingsListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {
                string line;
                string currentMovieName = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;
                    line = line.TrimStart();

                    var components = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                    string rating = "";
                    string movieName = "";

                    try
                    {
                        rating = components[2];
                        for (int i = 3; i < components.Length; i++)
                        {
                            movieName += components[i] + " ";
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    var s = MovieParser.MovieParser.extractDbText(movieName); // her movie name extract işleminde bu kullanılacak
                    if (currentMovieName == null) currentMovieName = s;
                    if (currentMovieName != s)
                    {
                        //DBHelper.Instance.updateMovieDirectorColumn(s,currentDirector.id);
                        //int id = DBHelper.Instance.findMovieId(s);
                        //currentDirector.addMovie(new Movie { Name = s });



                        currentMovieName = s;

                        Movie movie = DBHelper.Instance.getMovieByProperty(readFromtableName, "name", s, true);

                        try
                        {
                             movie.rating = float.Parse(rating, CultureInfo.InvariantCulture);
                        }
                        catch (Exception ee)
                        {

                        }


                        addCounter++;


                        if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                        {
                            Console.WriteLine("Rating Parser : " + addCounter );
                            dataItems.Add(bufferList);
                            bufferList = new List<Movie>();
                            if (limitWithOneWrite) break;
                        }

                         bufferList.Add(movie);

                    }


                }
            }
        }

        public override void writeLogic(List<Movie> writeList)
        {
            DBHelper.Instance.addMovie(writeList, newTableName);
        }
    }
}
