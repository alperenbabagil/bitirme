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
    public class SoundtrackParser : ParserBase<Soundtrack>
    {

        bool addSoundtracksAlone = true;



        Type dataItemsType = typeof(List<Soundtrack>);
        Type bufferListType = typeof(Soundtrack);




        //Type dataItemsType = typeof(List<SoundtrackMovie>);
        //Type bufferListType = typeof(SoundtrackMovie);

        //BlockingCollection<List<Soundtrack>> dataItems;
        //BlockingCollection <List<SoundtrackMovie>> dataItems;

        //List<Soundtrack> bufferList;
        //List<SoundtrackMovie> bufferList;


        public override void parseLogic(BlockingCollection<List<Soundtrack>> dataItems)
        {
            using (FileStream fs = File.Open(ConfigurationSettings.AppSettings["soundtracksListLocation"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {
                string line;
                string currentPerformer = null;
                //Movie currentMovie = null;
                string currentSoundtrack = null;
                int addCtr = 0;

                while ((line = sr.ReadLine()) != null)
                {

                    if (line.Length == 0) continue;

                    else if (line.StartsWith("-"))
                    {
                        try
                        {
                            line = line.Replace("\"", "");
                            line = line.Replace("'", "");
                            var reg = new Regex("\".*?\"");
                            var matches = reg.Matches(line);
                            //var strckName = matches[0].ToString();
                            var strckName = line;

                            

                            if (currentSoundtrack == null || currentSoundtrack != strckName)
                            {
                                
                                if (currentSoundtrack != null)
                                {
                                    if (addCtr % GlobalVariables.writeToDbBulkSize == 0)
                                    {
                                        Console.WriteLine("soundtrack  " + addCtr);
                                        dataItems.Add(bufferList);
                                        bufferList = new List<Soundtrack>();
                                    }
                                    addCtr++;
                                    var soundtck = new Soundtrack() { id = addCtr, name = currentSoundtrack, performer = currentPerformer };
                                    bufferList.Add(soundtck);
                                }
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
                                currentPerformer = line;

                            }
                            catch (Exception df)
                            {
                                continue;
                            }



                        }
                        else
                            currentPerformer = null;
                    }



                }
            }
        }

        public override void writeLogic(List<Soundtrack> writeList)
        {
            if(writeList.Count>0) DBHelper.Instance.addSoundtrack(writeList);
        }
    }
}
