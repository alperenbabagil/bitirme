using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing
{
    public class Director
    {
        public List<Movie> movies;

        public int id { get; set;}

        public string Name { get; set; }

        public Director()
        {
            movies = new List<Movie>();
        }

        public void addMovie(Movie movie)
        {
            movies.Add(movie);
        }
    }
}
