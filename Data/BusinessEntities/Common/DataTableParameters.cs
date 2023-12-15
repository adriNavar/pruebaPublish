using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Common
{
    public class DataTableParameters
    {
        public int draw { get; set; }
        public DataTableColumn[] columns { get; set; }
        public DataTableSort[] order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public SearchValue search { get; set; }
    }
    public class DataTableColumn
    {
        public int data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public SearchValue search { get; set; }
    }

    public class DataTableSort
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
    public class SearchValue
    {
        public string value { get; set; }
        public bool regex { get; set; }
    }
}
