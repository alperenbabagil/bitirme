﻿ using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.MovieParser
{
    class MovieParser
    {
        List<Movie> bufferList;

        BlockingCollection<List<Movie>> dataItems;

        public void Parse()
        {

            dataItems = new BlockingCollection<List<Movie>>();

            DBHelper.Instance.openConnection();

            bufferList = new List<Movie>();

            long addCounter = 0;

            Task.Run(() =>
            {
                //using (FileStream fs = File.Open(@"C:\Users\alperen\Desktop\bitirme\parsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (FileStream fs = File.Open(@"C:\Users\alperen\Desktop\bitirme\temiz list\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
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

                            string nameString = fields[0].Substring(0, fields[0].IndexOf('('));

                            nameString = nameString.Replace("\"", "");

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

                            if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                            {
                                dataItems.Add(blockMovies);
                                blockMovies = new List<Movie>();
                            }

                            //addMovieToDb(new Movie() { Name = nameString, Year = year });
                            blockMovies.Add(new Movie() { Name = nameString, Year = year });
                            //yearString.Remove('"')
                        }

                        
                        if (addCounter % 10000 == 0) Console.WriteLine(addCounter + " " + line);
                        //Console.WriteLine(counter + " " + line);

                    }
                    dataItems.CompleteAdding();
                }

                
            });


            //int consumerCounter = 0;

            while (!dataItems.IsCompleted )
            {

                List<Movie> data = null;
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
                    addMovieToDb(data);
                }
            }
            Console.WriteLine("\r\nNo more items to take.");
            DBHelper.Instance.closeConnection();


        }

        void addMovieToDb(List<Movie> movies)
        {
            DBHelper.Instance.addMovie(movies);

        }
    }
}
