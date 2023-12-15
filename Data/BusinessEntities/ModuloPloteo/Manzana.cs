using System.Data.Entity.Spatial;
using System;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class Manzana
    {
        public long FeatId { get; set; }
        public string Nomenclatura { get; set; }

        //[NotMapped]
        public DbGeometry Geom { get; set; }

        // [NotMapped]
        //public string WKT { get; set; }
    }
}
