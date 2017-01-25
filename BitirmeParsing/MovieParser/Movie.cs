using BitirmeParsing.SoundTrackParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing
{
    public class Movie
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public int directorId { get; set; } = -1;
        public int id { get; set; }
        public string color { get; set; }
        public string country { get; set; }
        public string certificate { get; set; }
        public float rating { get; set; } = -1;
        public int runningTime { get; set; } = -1;
        public List<Soundtrack> soundtracks { get; set; }

    }
}
