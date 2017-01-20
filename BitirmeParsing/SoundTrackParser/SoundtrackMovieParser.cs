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

namespace BitirmeParsing.SoundTrackParser
{
    public class SoundtrackMovieParser : ParserBase<SoundtrackMovie>
    {
        public override void parseLogic(BlockingCollection<List<SoundtrackMovie>> dataItems)
        {
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["soundtracksListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {
                string line;
                string currentMovieName = null;
                Movie currentMovie = null;
                string currentSoundtrack = null;
                int addCtr = 0;
                int soundtrackCount = 0;

                while ((line = sr.ReadLine()) != null)
                {

                    if (line.Length == 0) continue;

                    if (line.StartsWith("#") && line.Count(f => f == '"') > 1)
                    {
                        try
                        {
                            line = line.Replace("'", "");
                            var reg = new Regex("\".*?\"");
                            var matches = reg.Matches(line);
                            string movName = null;
                            if (matches[0] != null)
                            {
                                movName = matches[0].ToString();

                                if (currentMovieName == null || currentMovieName != movName)
                                {
                                    if (currentMovie != null)  // adding new moviesoundtrack id item
                                    {
                                        Movie movie = null;
                                        try
                                        {
                                            movie = DBHelper.Instance.getMovieByProperty("movie", "name", currentMovie.Name, true);
                                        }
                                        catch (Exception we)
                                        {
                                            //currentMovie = null;
                                            continue;
                                        }


                                        addCtr++;

                                        //Console.WriteLine("Movie get: " + addCtr + "    " + movName);

                                        ////Console.WriteLine("Movie get: " + addCtr + "    " + movie.id);



                                        if (addCtr % 500 == 0)
                                        {
                                            //Console.WriteLine("Movie get: " + addCtr + "    " + movie.id);
                                            dataItems.Add(bufferList);
                                            bufferList = new List<SoundtrackMovie>();
                                        }

                                        foreach (Soundtrack str in currentMovie.soundtracks)
                                        {
                                            bufferList.Add(new SoundtrackMovie() { movieId = movie.id, SoundtrackId = str.id });
                                        }

                                    }

                                    currentMovieName = movName;
                                    currentMovie = new Movie();
                                    currentMovie.Name = movName;

                                    currentMovie.soundtracks = new List<Soundtrack>();
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            continue;
                        }


                    }


                    else if (line.StartsWith("-"))
                    {
                        try
                        {
                            line = line.Replace("\"", "");
                            line = line.Replace("'", "");
                            var reg = new Regex("\".*?\"");
                            var matches = reg.Matches(line);
                            var strckName = matches[0].ToString();

                            if (currentSoundtrack == null || currentSoundtrack != strckName)
                            {
                                currentSoundtrack = strckName;
                            }
                        }
                        catch (Exception e)
                        {
                            continue;
                        }

                    }

                    else
                    {
                        if (line.Contains("Performed by"))
                        {
                            try
                            {
                                line = line.Replace("'", "");
                                line = line.Replace("\"", "");
                                line = line.Substring(line.IndexOf("Performed by") + 13);

                            }
                            catch (Exception df)
                            {
                                continue;
                            }



                            // bool hasThisSong = currentMovie.soundtracks.Find(x => x.name == currentSoundtrack) != null;

                            try
                            {
                                if (currentMovie.soundtracks.Find(x => x.name == currentSoundtrack) == null && line.Length < 500 && currentSoundtrack.Length < 250)
                                {
                                    soundtrackCount++;
                                    var strck = new Soundtrack() { id = soundtrackCount, name = currentSoundtrack, performer = line };
                                    currentMovie.soundtracks.Add(strck);


                                    //addCtr++;


                                    ////Console.WriteLine("addCtr: " + addCtr + "    " + strck.name);
                                    //if (addCtr % 1000 == 0)
                                    //{
                                    //    Console.WriteLine("Movie get: " + addCtr + "    " + strck.name);

                                    //}



                                    //if (addCtr % GlobalVariables.writeToDbBulkSize == 0)
                                    //{
                                    //    dataItems.Add(bufferList);
                                    //    bufferList = new List<Soundtrack>();
                                    //}

                                    //bufferList.Add(strck);
                                }
                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public override void writeLogic(List<SoundtrackMovie> writeList)
        {
            DBHelper.Instance.addSoundtrackMovie(writeList);
        }
    }
}
