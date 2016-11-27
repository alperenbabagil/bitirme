using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.DirectorParser
{
    class DirectorParser
    {

        public DirectorParser()
        {

        }

        public void Parse()
        {
            using (FileStream fs = File.Open(@"C:\Users\alperen\Desktop\bitirme\parsing\directors.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                string[] fields;
                Director currentDirector = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;
                    fields = line.Split('\t');
                    if (fields[0].Length != 0)
                    {
                        if (currentDirector != null) writeDirectorToDb(currentDirector);
                        currentDirector = new Director();
                        currentDirector.Name = fields[0];
                    }
                    if (currentDirector != null)
                    {

                        currentDirector.addMovie(new Movie { Name = fields[fields.Length - 1] });
                    }

                    Console.WriteLine(line);
                }
            }
        }

        void writeDirectorToDb(Director director)
        {

        }
    }
}
