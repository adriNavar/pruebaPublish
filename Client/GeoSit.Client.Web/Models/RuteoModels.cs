using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class RuteoModel
    {
        public string Modo { get; set; }
        public Objeto Inicio { get; set; }
        public Objeto Fin { get; set; }
        public IList<Objeto> Waypoints { get; set; }
        public string OverviewPolyline { get; set; }

        public RuteoModel()
        {
            this.Waypoints = new List<Objeto>();
        }
    }
}