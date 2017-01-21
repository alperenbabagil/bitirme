using BitirmeParsing.DBConnection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;


namespace BitirmeParsing.ActorParser
{
    public class ActorParser : ParserBase<Actor>
    {

        string fileLoc = "";
        string tableName = "";
        public ActorParser(string type)
        {
            if (type == "actor")
            {
                fileLoc = ConfigurationSettings.AppSettings["actorsListLocation"];
                tableName = "actor";
            }
            else
            {
                fileLoc = ConfigurationSettings.AppSettings["actressListLocation"];
                tableName = "actress";
            }
        }

        public override void parseLogic(System.Collections.Concurrent.BlockingCollection<List<Actor>> dataItems)
        {
            using (FileStream fs = File.Open(fileLoc, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (FileStream fs = File.Open(@"C:\Users\bgulsen\Documents\Visual Studio 2015\Projects\bitirme\BitirmeParsing\movies.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
            {

                string line;
                string currentActorName = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line == null || line.Length == 0) continue;
                    var parts = line.Split('\t');
                    if (!line.StartsWith("\t"))
                    {
                        var imdex = line.IndexOf('\t');
                        

                        if (parts[0] != null && parts[0].Length > 0)
                        {
                            currentActorName = parts[0];
                        }
                           
                    }
                    addCounter++;

                    if (addCounter % 1000 == 0) Console.WriteLine("Actor: " + addCounter );

                    if (addCounter % GlobalVariables.writeToDbBulkSize == 0)
                    {
                        dataItems.Add(bufferList);
                        bufferList = new List<Actor>();
                    }

                    bufferList.Add(new Actor() { id = addCounter, Name = currentActorName });
                    
                }
                Console.WriteLine("end");
            }
        }

        public override void writeLogic(List<Actor> writeList)
        {
            DBHelper.Instance.addActors(writeList,tableName);
        }
    }
}
