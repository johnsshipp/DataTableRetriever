using System.Collections.Generic;

namespace DataTableRetriever
{
    public class Result
    {
        public IEnumerable<dynamic> Results { get; set; }
        public int Size { get; set; }
    }
}