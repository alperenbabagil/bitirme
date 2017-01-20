using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitirmeParsing.DBConnection;

/*
Important!!!
Color can be ->
Color
HD Black and White
color and black & white archive footage


    For this reason color -> charchar(75)
*/
namespace BitirmeParsing.ColorParser
{
    public class ColorParser
    {
        BlockingCollection<List<Movie>> _dataItems;

        public void Parse()
        {
            int counter = 1;
            List<Color> color = new List<Color>();

            _dataItems = new BlockingCollection<List<Movie>>();

            DBHelper.Instance.openConnection();

            Task.Run(() =>
            {
                using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["colorsListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
                {
                    string line;
                    string[] fields;
                    Color movieName = null;
                    string currentColorName = null;
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


                            movieName = new Color();
                            movieName.movies = new List<Movie>();
                            movieName.Name = fields[0];
                            movieName.id = counter;
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

                        if (movieName != null) // movielerin director idlerini güncellerken kullanılan blok
                        {

                            int index = fields.Length - 1;      // color field'in en sonunda bulunuyor
                            var colorRawString = fields[index];

                            var s = MovieParser.MovieParser.extractDbText(movieName.Name); // her movie name extract işleminde bu kullanılacak
                            if (movieName.Name == null) movieName.Name = s;
                            if (movieName.Name != s)
                            {
                                //DBHelper.Instance.updateMovieDirectorColumn(s,currentDirector.id);
                                //currentDirector.addMovie(new Movie { Name = s });
                                movieName.Name = s;

                                Movie movie = DBHelper.Instance.getMovieByProperty("name", s, true);
                                movie.color = colorRawString;

                                addCtr++;
                                if (addCtr % 1000 == 0) Console.WriteLine("Movie get: " + addCtr + "    " + movie.id);

                                if (addCtr % GlobalVariables.writeToDbBulkSize == 0)
                                {
                                    _dataItems.Add(blockMovies);
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

            while (!_dataItems.IsCompleted)
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
                    data = _dataItems.Take();
                }
                catch (InvalidOperationException) { }

                if (data != null)
                {
                    DBHelper.Instance.addMovie(data, "moviev3");
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
