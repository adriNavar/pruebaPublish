using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models.ObrasPublicas
{
    public class ZonaAtributoModel
    {
        public List<Zona> Zonas { get; set; }
        public class Zona
        {
            public long Id { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
        }

        public class GetZonasModel
        {
            public int Observaciones { get; set; }
            public List<Zona> Zonas { get; set; }
        }
    }
}