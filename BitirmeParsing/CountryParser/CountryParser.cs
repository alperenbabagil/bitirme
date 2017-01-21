using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.CountryParser
{
    public class CountryParser : ParserBase<Movie>
    {

        string newTableName;

        public CountryParser(string newTableName_)
        {
            newTableName = newTableName_;
        }

        public override void parseLogic(BlockingCollection<List<Movie>> dataItems)
        {
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["countryListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {
                string line;
                string[] fields;
                string movieName = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;

                    fields = line.Split('\t');

                    if (fields[0].Length == 0) continue;
                    var tempName = MovieParser.MovieParser.extractDbText(fields[0]);

                    if (tempName != null && movieName != tempName) // movielerin director idlerini güncellerken kullanılan blok
                    {
                        movieName = tempName;
                        int index = fields.Length - 1;      // color field'in en sonunda bulunuyor
                        var countryRawString = fields[index];

                        Movie movie = DBHelper.Instance.getMovieByProperty("movie", "name", movieName, true);
                        movie.country = countryRawString;

                        addCounter++;


                        if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                        {
                            Console.WriteLine("country update: " + addCounter + "    " + movie.id);
                            dataItems.Add(bufferList);
                            bufferList = new List<Movie>();
                        }
                        bufferList.Add(movie);

                    }

                    Console.WriteLine(line);
                }

            }
        }

        public override void writeLogic(List<Movie> writeList)
        {
            DBHelper.Instance.addMovie(writeList, newTableName);
        }
    }
}
