using System;
using GeoSit.Data.BusinessEntities.Interfaces;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    [Serializable]
    public class LiquidacionExterna : IEntity
    {

        public string Padron { get; set; }
        public string Expediente { get; set; }

        public double Importe { get; set; }

        public DateTime Fecha { get; set; }
             
    }
}
