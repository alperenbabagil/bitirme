using BitirmeParsing.DBConnection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing
{
    class Program
    {
        public Program()
        {
            // test bilal
            //Stopwatch stopwatch = new Stopwatch();
            //Console.WriteLine("start "+DateTime.Now.Second);

            //stopwatch.Start();
            //DBHelper.Instance.searchMovie(null);
            //stopwatch.Stop();

            //Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            //Console.WriteLine("end " + DateTime.Now.Second);

            //MySQLConnectionTest.testConnection();
            //new MovieParser.MovieParser().Parse();
            new DirectorParser.DirectorParser().Parse();
            Console.WriteLine("movie counter db: "+DBHelper.Instance.addMovieCounter);
            Console.ReadLine();



        }

        

        static void Main(string[] args)
        {
            new Program();
        }


    }
}
