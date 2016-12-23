using BitirmeParsing.DBConnection;
using System;
using System.Collections.Generic;
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
            MySQLConnectionTest.testConnection();
            //new ColorParser.ColorParser().Parse();
            //Console.WriteLine("color counter db " + DBHelper.Instance.addColorCounter);
            new GenreParser.GenreParser().Parse();
            Console.WriteLine("genre counter db " + DBHelper.Instance.addGenreCounter);

            //new MovieParser.MovieParser().Parse();
            //Console.WriteLine("movie counter db: " + DBHelper.Instance.addMovieCounter);
            Console.ReadLine();

        }
        static void Main(string[] args)
        {
            new Program();
        }
    }
}
