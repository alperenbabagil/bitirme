using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing.GenreParser
{
    public class GenreParserAlperen : ParserBase<Movie>
    {

        //        Action Adventure       Adult Animation
        //    Comedy Crime       Documentary Drama
        //    Fantasy Family      Film-Noir Horror
        //    Musical Mystery     Romance Sci-Fi
        //Short       Thriller War     Western

        Dictionary<string, int> genreIDDict;

        public GenreParserAlperen()
        {
            genreIDDict = new Dictionary<string, int>();
            genreIDDict["Action"] = 1;
            genreIDDict["Adventure"] = 2;
            genreIDDict["Adult"] = 3;
            genreIDDict["Animation"] = 4;
            genreIDDict["Comedy"] = 5;
            genreIDDict["Crime"] = 6;
            genreIDDict["Documentary"] = 7;
            genreIDDict["Drama"] = 8;
            genreIDDict["Fantasy"] = 9;
            genreIDDict["Family"] = 10;
            genreIDDict["Film-Noir"] = 11;
            genreIDDict["Horror"] = 12;
            genreIDDict["Musical"] = 13;
            genreIDDict["Mystery"] = 14;
            genreIDDict["Romance"] = 15;
            genreIDDict["Sci-Fi"] = 16;
            genreIDDict["Short"] = 17;
            genreIDDict["Thriller"] = 18;
            genreIDDict["War"] = 19;
            genreIDDict["Western"] = 20;
        }

        public override void parseLogic(BlockingCollection<List<Movie>> dataItems)
        {
            throw new NotImplementedException();
        }

        public override void writeLogic(List<Movie> writeList)
        {
            throw new NotImplementedException();
        }
    }
}
