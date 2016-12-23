using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.GenreParser
{
    class Genre
    {
        List<Genre> genres;

        public int movieId { get; set; }
        public string genreName { get; set; }

        public Genre()
        {
            genres = new List<Genre>();
        }

        public void addGenre(Genre genre)
        {
            genres.Add(genre);
        }
    }
}
