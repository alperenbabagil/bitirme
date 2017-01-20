using BitirmeParsing.DBConnection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeParsing
{
    public abstract class ParserBase<T>
    {
        protected List<T> bufferList;

        public BlockingCollection<List<T>> dataItems;

        public int addCounter = 0;

        public void Parse()
        {
            dataItems = new BlockingCollection<List<T>>();

            DBHelper.Instance.openConnection();

            bufferList = new List<T>();

            

            Task.Run(() =>
            {
                parseLogic(dataItems);
            });

            

            while (!dataItems.IsCompleted)
            {

                List<T> data = null;
                try
                {
                    data = dataItems.Take();
                }
                catch (InvalidOperationException) { }

                if (data != null)
                {
                    writeLogic(data);
                }
            }
            Console.WriteLine("\r\nNo more items to take.");
            DBHelper.Instance.closeConnection();
        }

        public abstract void parseLogic(BlockingCollection<List<T>> dataItems);

        public abstract void writeLogic(List<T> writeList);
    }
}
