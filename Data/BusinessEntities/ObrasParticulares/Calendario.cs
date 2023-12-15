using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class Calendario
    {
        public string success { get; set; }
        public List<Evento> result { get; set; }
    }
    public class Evento
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string cssclass { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }
}
