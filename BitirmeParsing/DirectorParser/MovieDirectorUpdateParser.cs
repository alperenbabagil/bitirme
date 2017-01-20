using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.DirectorParser
{
    public class MovieDirectorUpdateParser : ParserBase<Movie>
    {

        string newTableName;

        public MovieDirectorUpdateParser(string newTableName_)
        {
            newTableName = newTableName_;
        }

        public override void parseLogic(BlockingCollection<List<Movie>> dataItems)
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
                int counter = 1;

                List<Movie> blockMovies = new List<Movie>();


                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;
                    fields = line.Split('\t');
                    if (fields[0].Length != 0)
                    {
                        currentDirector = new Director();
                        currentDirector.movies = new List<Movie>();
                        currentDirector.Name = fields[0];
                        currentDirector.id = counter;
                        counter++;
                    }

                    if (currentDirector != null) // movielerin director idlerini güncellerken kullanılan blok
                    {

                        int index = fields.Length - 1;
                        var movieRawString = fields[index];

                        var s = MovieParser.MovieParser.extractDbText(movieRawString); // her movie name extract işleminde bu kullanılacak
                        if (currentMovieName == null) currentMovieName = s;
                        if (currentMovieName != s)
                        {
                            currentMovieName = s;

                            Movie movie = DBHelper.Instance.getMovieByProperty("movie", "name", s, true);
                            movie.directorId = currentDirector.id;

                            addCtr++;
                            if (addCtr % 1000 == 0) Console.WriteLine("Movie get: " + addCtr + "    " + movie.id);

                            if (addCtr % GlobalVariables.writeToDbBulkSize == 0)
                            {
                                dataItems.Add(blockMovies);
                                blockMovies = new List<Movie>();
                            }
                            blockMovies.Add(movie);
                        }
                    }

                    if (addCtr % 10000 == 0) Console.WriteLine(addCtr + " " + line);
                }

                Console.WriteLine(line);
            }
        }

        public override void writeLogic(List<Movie> writeList)
        {
            DBHelper.Instance.addMovie(writeList, newTableName);
        }
    }
}
