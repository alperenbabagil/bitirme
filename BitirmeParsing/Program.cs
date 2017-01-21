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

            //new MovieParser.MovieParser().Parse();
            //new ColorParser.ColorParser("movieUpdColor").Parse();
            //new GenreParser.GenreMovieParser().Parse();
            //new CountryParser.CountryParser("ww").Parse();
            //Console.WriteLine("movie counter db: " + DBHelper.Instance.addMovieCounter);




            //if(DBHelper.Instance.resetDb()) Console.WriteLine("Reset Done");
            programOrdered();
            Console.ReadLine();



        }

        //DROP TABLE IF EXISTS movie,movie_onlyMovie,movie_directorId,movie_color,movie_country,
        //   movie_rating,movie_runningTime,
        //   actor,actormovie,
        //   director,
        //   genremovie,
        //   soundtrack,soundtrackmovie,
        //   actress,actressmovie ;


        void programOrdered()
        {
            new MovieParser.MovieParser().Parse();
            new ActorParser.ActorMovieParser("actormovie").Parse();
            new ActorParser.ActorMovieParser("actressmovie").Parse();
            //new SoundTrackParser.SoundtrackMovieParser().Parse();
            new GenreParser.GenreMovieParser().Parse();

            new DirectorParser.MovieDirectorUpdateParser("movie_directorId");
            new RunningTimesParser.RunningTimesParser("movie_runningTime");
            new RatingParser.RatingParser("movie_rating");
            new CountryParser.CountryParser("movie_country");
            new ColorParser.ColorParser("movie_country");

            new DirectorParser.DirectorParser().Parse();
            //new SoundTrackParser.SoundtrackParser().Parse();
            new ActorParser.ActorParser("actor").Parse();
            new ActorParser.ActorParser("actress").Parse();


            //new SoundTrackParser.SoundtrackMovie();
        }
        static void Main(string[] args)
        {
            new Program();
        }
    }
}
