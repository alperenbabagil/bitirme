using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.GenreParser
{
    public class GenreMovieParser : ParserBase<GenreMovie>
    {

        //        Action Adventure       Adult Animation
        //    Comedy Crime       Documentary Drama
        //    Fantasy Family      Film-Noir Horror
        //    Musical Mystery     Romance Sci-Fi
        //Short       Thriller War     Western

        //Dictionary<string, int> genreIDDict;

        //public GenreMovieParser()
        //{
        //    genreIDDict = new Dictionary<string, int>();
        //    genreIDDict["Action"] = 1;
        //    genreIDDict["Adventure"] = 2;
        //    genreIDDict["Adult"] = 3;
        //    genreIDDict["Animation"] = 4;
        //    genreIDDict["Comedy"] = 5;
        //    genreIDDict["Crime"] = 6;
        //    genreIDDict["Documentary"] = 7;
        //    genreIDDict["Drama"] = 8;
        //    genreIDDict["Fantasy"] = 9;
        //    genreIDDict["Family"] = 10;
        //    genreIDDict["Film-Noir"] = 11;
        //    genreIDDict["Horror"] = 12;
        //    genreIDDict["Musical"] = 13;
        //    genreIDDict["Mystery"] = 14;
        //    genreIDDict["Romance"] = 15;
        //    genreIDDict["Sci-Fi"] = 16;
        //    genreIDDict["Short"] = 17;
        //    genreIDDict["Thriller"] = 18;
        //    genreIDDict["War"] = 19;
        //    genreIDDict["Western"] = 20;
        //}

        public override void parseLogic(BlockingCollection<List<GenreMovie>> dataItems)
        {
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["genresListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {
                string line;
                string[] fields;
                string currentMovieName = null;
                int currentMovieId = -1;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;

                    fields = line.Split('\t');

                    if (fields[0].Length == 0) continue;

                    var tempName = MovieParser.MovieParser.extractDbText(fields[0]);

                    if (tempName != null && currentMovieName != tempName) // movielerin director idlerini güncellerken kullanılan blok
                    {
                        currentMovieName = tempName;
                        Movie movie = DBHelper.Instance.getMovieByProperty("movie", "name", currentMovieName, true);
                        currentMovieId = movie.id;
                    }

                        int index = fields.Length - 1;      // color field'in en sonunda bulunuyor
                        var genreName = fields[index];
                    
                        addCounter++;

                    GenreMovie genMov = new GenreMovie() { genre = genreName, movieId = currentMovieId };


                        if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                        {
                            Console.WriteLine("genre get: " + addCounter );
                            dataItems.Add(bufferList);
                            bufferList = new List<GenreMovie>();
                        }
                        bufferList.Add(genMov);

                    }

                    Console.WriteLine(line);
                }

            }

        public override void writeLogic(List<GenreMovie> writeList)
        {
            DBHelper.Instance.addGenreMovie(writeList);
        }
    }
}
