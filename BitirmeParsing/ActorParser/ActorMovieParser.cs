using BitirmeParsing.DBConnection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.ActorParser
{
    public class ActorMovieParser : ParserBase<ActorMovie>
    {
        string fileLoc = "";
        string tableName = "";
        string readFromtableName;



        bool limitWithOneWriteMovie = false;

        public ActorMovieParser(string readFromtableName, string type)
        {
            this.readFromtableName = readFromtableName;
            if (type == "actor")
            {
                fileLoc = ConfigurationSettings.AppSettings["actorsListLocation"];
                tableName = "actormovie";
            }
            else
            {
                fileLoc = ConfigurationSettings.AppSettings["actressListLocation"];
                tableName = "actressmovie";
            }
        }

        public override void parseLogic(System.Collections.Concurrent.BlockingCollection<List<ActorMovie>> dataItems)
        {
            using (FileStream fs = File.Open(fileLoc, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {

                string line;
                string currentMovieName = " ";
                string currentActorName = null;
                int addActorId = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == null || line.Length == 0) continue;

                    if (limitWithOneWriteMovie)  break;

                    var parts = line.Split('\t');
                    if (!line.StartsWith("\t"))
                    {
                        var imdex = line.IndexOf('\t');


                        if (parts[0] != null && parts[0].Length > 0)
                        {
                            currentActorName = parts[0];
                            addActorId++;
                        }


                        for (int i = 1; i < parts.Count(); i++)
                        {
                            if (parts[i] != null && parts[i].Length > 0)
                            {
                                var movname = MovieParser.MovieParser.extractDbText(parts[i]);
                                //if (currentActorName == null) currentActorName = movname;

                                if (!currentMovieName.Equals(movname))
                                {
                                    currentMovieName = movname;
                                    addToList(addActorId, currentMovieName);
                                }
                                break;
                            }
                        }
                        //if (imdex == 0) Console.WriteLine("indx0");
                        //if (parts.Count() == 0) Console.WriteLine("parts0");
                    }

                    else
                    {
                        for (int i = 0; i < parts.Count(); i++)
                        {
                            if (parts[i] != null && parts[i].Length > 0)
                            {
                                var movname = MovieParser.MovieParser.extractDbText(parts[i]);
                                //if (currentActorName == null) currentActorName = movname;

                                if (!currentMovieName.Equals(movname))
                                {
                                    currentMovieName = movname;
                                    addToList(addActorId, currentMovieName);
                                }
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine("end");
            }
        }

        public override void writeLogic(List<ActorMovie> writeList)
        {
            DBHelper.Instance.addActorMovie(writeList, tableName);
        }

        void addToList(int addActorId, string currentMovieName)
        {
            addCounter++;



            Movie movie = DBHelper.Instance.getMovieByProperty(readFromtableName, "name", currentMovieName, true);

            var movAct = new ActorMovie() { actorID = addActorId, movieID = movie.id };

            //Console.WriteLine("MovieActor get: " + addCounter + "    " + movie.id);
            if (addCounter % 1000 == 0) Console.WriteLine("MovieActor get: " + addCounter + "    " + movie.id);

            if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
            {
                dataItems.Add(bufferList);
                bufferList = new List<ActorMovie>();
                if (limitWithOneWrite) limitWithOneWriteMovie=true;
            }

            bufferList.Add(movAct);
        }
    }
}
