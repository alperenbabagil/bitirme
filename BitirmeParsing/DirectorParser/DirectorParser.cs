using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BitirmeParsing.DirectorParser
{
    class DirectorParser
    {
        // 2 iş yapar. Directorları okuyup director tablosuna yazma ve directorların filmlerine göre movie okuyup yeni movie oluşturarak 
        //db ye yazma.
        //aynı mantık actor ve actress için de geçerli olacak


        List<Movie> bufferList;

        BlockingCollection<List<Movie>> dataItems;

        public DirectorParser()
        {

        }

        private string parseMovieName(string rawName)
        {
            //if (!fields[0].Contains("(") || currentMovieName == fields[0].Substring(0, fields[0].IndexOf('('))) continue;

            //currentMovieName = fields[0].Substring(0, fields[0].IndexOf('('));

            string yearString = rawName.Substring(rawName.IndexOf('(') + 1, 4);

            string nameString = rawName.Substring(0, rawName.IndexOf('('));

            nameString = nameString.Replace("\"", "");
            nameString = nameString.Replace("'", "");

            return nameString;
        }

        public void Parse()
        {
            int counter = 1;
            List<Director> directors = new List<Director>();

            dataItems = new BlockingCollection<List<Movie>>();

            DBHelper.Instance.openConnection();

            bufferList = new List<Movie>();

            Task.Run(() =>
            {
                using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["directorsListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
                {
                    string line;
                    string[] fields;
                    Director currentDirector = null;
                    string currentMovieName = null;
                    int addCtr = 0;

                    List<Movie> blockMovies = new List<Movie>();


                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length == 0) continue;
                        fields = line.Split('\t');
                        if (fields[0].Length != 0)
                        {

                            //if (addCtr == 10)
                            //{
                            //addCtr = 0;
                            //DBHelper.Instance.updateMovieDirectorColumn2(directors);
                            //directors = new List<Director>();
                            //}


                            currentDirector = new Director();
                            currentDirector.movies = new List<Movie>();
                            currentDirector.Name = fields[0];
                            currentDirector.id = counter;
                            counter++;


                            //addCtr++;                    
                            //directors.Add(currentDirector);


                            //addCtr++;
                            //if (addCtr == 2000)
                            //{
                            //    writeDirectorToDb(directors);
                            //    directors = new List<Director>();
                            //    addCtr = 0;
                            //}
                        }

                        if (currentDirector != null) // movielerin director idlerini güncellerken kullanılan blok
                        {

                            int index = fields.Length - 1;
                            var movieRawString = fields[index];

                            var s = MovieParser.MovieParser.extractDbText(movieRawString); // her movie name extract işleminde bu kullanılacak
                            if (currentMovieName == null) currentMovieName = s;
                            if (currentMovieName != s)
                            {
                                //DBHelper.Instance.updateMovieDirectorColumn(s,currentDirector.id);
                                int id = DBHelper.Instance.findMovieId(s);
                                //currentDirector.addMovie(new Movie { Name = s });
                                currentMovieName = s;

                                Movie movie = DBHelper.Instance.getMovieByProperty("name",s,true);
                                movie.directorId = currentDirector.id;

                                addCtr++;
                                if (addCtr % 1000 == 0) Console.WriteLine("Movie get: "+  addCtr + "    " + id);

                                if (addCtr % GlobalVariables.writeToDbBulkSize == 0)
                                {
                                    dataItems.Add(blockMovies);
                                    blockMovies = new List<Movie>();
                                }

                                blockMovies.Add(movie);

                            }
                        }

                        if (addCtr % 10000 == 0) Console.WriteLine(addCtr + " " + line);





                        //if (currentDirector != null)
                        //{

                        //    int index = fields.Length - 1;

                        //    string currentMovieName = fields[index];

                        //    int year = 0;

                        //    string yearString = null;

                        //    string nameString = null;

                        //    if (currentMovieName.Contains("(????)")) { nameString = currentMovieName.Substring(0, currentMovieName.IndexOf("(????)") - 1); }

                        //    else
                        //    {
                        //        try
                        //        {

                        //            if (!Regex.Match(currentMovieName, @"\(\d+\)").Success)
                        //            {

                        //            }

                        //            int grpCount=Regex.Match(currentMovieName, @"\(\d+\)").Groups.Count;




                        //            for (int i=0;i< grpCount; i++)
                        //            {
                        //                string grp = Regex.Match(currentMovieName, @"\(\d+\)").Groups[i].Value;
                        //                yearString = grp.Trim(new Char[] { '(', ')' });
                        //                if (yearString.Length > 4) yearString = yearString.Substring(0, 4);
                        //                try
                        //                {
                        //                    year = Int32.Parse(yearString);
                        //                    if(year > 1800 && year < 2017)
                        //                    {
                        //                        nameString = currentMovieName.Substring(0, currentMovieName.IndexOf(year.ToString()) - 1);
                        //                    }
                        //                }
                        //                catch(Exception ex)
                        //                {
                        //                    nameString = currentMovieName;
                        //                    continue;
                        //                }

                        //            }
                        //        }
                        //        catch (Exception e)
                        //        {
                        //            year = 0;
                        //        }
                        //    }

                        //    if (nameString != null) nameString.Replace("\"", "");
                        //    if (nameString.EndsWith(" ")) nameString.Remove(nameString.Length - 2, 1);
                        //    currentDirector.addMovie(new Movie { Name = nameString , Year=year });
                        //}

                        //Console.WriteLine(line);
                    }

                    Console.WriteLine(line);
                }
            }
            );

            while (!dataItems.IsCompleted)
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
                    DBHelper.Instance.addMovie(data,"movie");
                }
            }
            Console.WriteLine("\r\nNo more items to take.");
            DBHelper.Instance.closeConnection();



        }

        void writeDirectorToDb(List<Director> directors)
        {
            DBHelper.Instance.addDirectors(directors);
        }
    }
}
