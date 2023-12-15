using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class Rango : IEntity
    {
        public double? Desde { get; set; }
        public double? Hasta { get; set; }
        public string Valor { get; set; }
        public int Casos { get; set; }
        public string Leyenda { get; set; }
        public string Color { get; set; }
        public string ColorBorde { get; set; }
        public int AnchoBorde { get; set; }
        public bool PonerLabel { get; set; }
        public byte[] Icono { get; set; }
        public string sIcono { get; set; }
    }
}
