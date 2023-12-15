using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models.Search
{
    public class SearchResultModel
    {
        public string Pattern { get; set; }
        public List<SearchResultGroup> Groups { get; set; }
        public int MaxItemsPerSection { get; set; }
    }
}