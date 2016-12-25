using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.ColorParser
{
    public class Color
    {
        public List<Movie> movies;

        public int id { get; set; }

        public string Name { get; set; }

        public Color()
        {
            movies = new List<Movie>();
        }

        public void addMovie(Movie movie)
        {
            movies.Add(movie);
        }
    }
}
