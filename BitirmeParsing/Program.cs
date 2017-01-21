using BitirmeParsing.DBConnection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace BitirmeParsing
{
    class Program
    {
        public Program()
        {

            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            //Console.WriteLine(ConfigurationSettings.AppSettings["mysqlConnectionString"]);
            //Console.WriteLine(ConfigurationManager.AppSettings["countoffiles"]);
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
            //new DirectorParser.DirectorParser().Parse();
            ////Console.WriteLine("movie counter db: "+DBHelper.Instance.addMovieCounter);

            //var vc = DBConnection.DBHelper.Instance.getMovieByProperty("name", "Way Out",true);

            //new RatingParser.RatingParser().Parse();
            //new SoundTrackParser.SoundtrackParser().Parse();
            //new SoundTrackParser.SoundtrackMovieParser().Parse();

            //new ActorParser.ActorParser().Parse();
            //new ActorParser.ActorMovieParser().Parse();

            //bool isConnectionAvaliable=MySQLConnectionTest.testConnection();
            //new ColorParser.ColorParser().Parse();
            //Console.WriteLine("color counter db " + DBHelper.Instance.addColorCounter);
            //new GenreParser.GenreParser().Parse();
            //Console.WriteLine("genre counter db " + DBHelper.Instance.addGenreCounter);

            new MovieParser.MovieParser().Parse();
            //Console.WriteLine("movie counter db: " + DBHelper.Instance.addMovieCounter);





            //new DirectorParser.DirectorParser().Parse();

            Console.ReadLine();



        }

        void programOrdered()
        {

            new MovieParser.MovieParser().Parse();
            new ActorParser.ActorMovieParser().Parse();
            new SoundTrackParser.SoundtrackMovie();
        }
        static void Main(string[] args)
        {
            new Program();
        }
    }
}
