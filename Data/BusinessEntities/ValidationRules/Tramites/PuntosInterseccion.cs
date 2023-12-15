using System;
using System.Collections.Generic;
using System.Linq;
using System.Spatial;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ValidationRules.Tramites
{
    public class PuntosInterseccion
    {
        public string puntoOriginal { get; set; }
        public bool interOriginal { get; set; }
        public bool existeNomOriginal { get; set; }
        public string puntoNorth { get; set; }
        public bool interNorth { get; set; }
        public bool existeNomNorth { get; set; }
        public string puntoEast { get; set; }
        public bool interEast { get; set; }
        public bool existeNomEast { get; set; }
        public string puntoSouth { get; set; }
        public bool interSouth { get; set; }
        public bool existeNomSouth { get; set; }
        public string puntoWest { get; set; }
        public bool interWest { get; set; }
        public bool existeNomWest { get; set; }

    }
}
