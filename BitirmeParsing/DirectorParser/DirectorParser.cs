using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            int counter = 1;
            List<Director> directors = new List<Director>();
            using (FileStream fs = File.Open(@"C:\Users\alperen\Desktop\bitirme\parsing\directors.list", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs, System.Text.Encoding.Default))
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
                        currentDirector.id = counter;
                        counter++;
                        directors.Add(currentDirector);
                    }
                    if (currentDirector != null)
                    {

                        int index = fields.Length - 1;

                        string currentMovieName = fields[index];

                        int year = 0;



                        //string yearString = currentMovieName.Substring(currentMovieName.IndexOf('(') + 1, 4);

                        string yearString = null;

                        string nameString = null;

                        if (currentMovieName.Contains("(????)")) { nameString = currentMovieName.Substring(0, currentMovieName.IndexOf("(????)") - 1); }



                        //string nameString = currentMovieName.Substring(0, currentMovieName.IndexOf('(')-1);

                        else
                        {
                            try
                            {
                                //string parantez = Regex.Matches(currentMovieName, @"\d+");
                                //List<string> yearStr = Regex.Matches(currentMovieName, @"\d+").OfType<Match>().Select(m => m.Value).Where(m => Int32.Parse(m) > 1800 && Int32.Parse(m) < 2017));
                                //string yearStr = Regex.Match(currentMovieName, @"\(\d+\)").Groups[0].Value;

                                if (!Regex.Match(currentMovieName, @"\(\d+\)").Success)
                                {

                                }

                                int grpCount=Regex.Match(currentMovieName, @"\(\d+\)").Groups.Count;

                                
                                
                                
                                for (int i=0;i< grpCount; i++)
                                {
                                    string grp = Regex.Match(currentMovieName, @"\(\d+\)").Groups[i].Value;
                                    yearString = grp.Trim(new Char[] { '(', ')' });
                                    if (yearString.Length > 4) yearString = yearString.Substring(0, 4);
                                    try
                                    {
                                        year = Int32.Parse(yearString);
                                        if(year > 1800 && year < 2017)
                                        {
                                            nameString = currentMovieName.Substring(0, currentMovieName.IndexOf(year.ToString()) - 1);
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        nameString = currentMovieName;
                                        continue;
                                    }
                                    
                                }
                                //foreach(var str in Regex.Match(currentMovieName, @"\(\d+\)").Groups)
                                //{
                                //    year = Int32.Parse(str.Value);
                                //}
                                
                                //nameString = currentMovieName.Substring(0, currentMovieName.IndexOf(yearStr) - 2); //parantez ve boşluk

                            }
                            catch (Exception e)
                            {
                                year = 0;
                            }
                        }

                        if (nameString != null) nameString.Replace("\"", "");
                        if (nameString.EndsWith(" ")) nameString.Remove(nameString.Length - 2, 1);

                        ////int year = 0;

                        //if (yearString != null && yearString.Length > 0)
                        //{
                        //    try
                        //    {
                        //        year = Int32.Parse(yearString);
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        year = 0;
                        //    }

                        //}

                        currentDirector.addMovie(new Movie { Name = nameString , Year=year });
                    }

                    //Console.WriteLine(line);
                }
                Console.WriteLine(line);
            }
        }

        void writeDirectorToDb(Director director)
        {

        }
    }
}
