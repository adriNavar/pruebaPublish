using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models.Search
{
    public class SearchResultGroup
    {
        public string Nombre { get; set; }
        public List<object> Items { get; set; }
        public int Cantidad { get; set; }
    }
}