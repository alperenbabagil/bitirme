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
            new MovieParser.MovieParser().Parse();
            Console.WriteLine("movie counter db: "+DBHelper.Instance.addMovieCounter);
            Console.ReadLine();

        }

        

        static void Main(string[] args)
        {
            new Program();
        }


    }
}
