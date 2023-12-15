using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class CuadraPlot
    {
        public long FeatId { get; set; }
        public long IdCuadra { get; set; }
        public DbGeometry Geom { get; set; }

        public long IdManzana { get; set; }

        public long IdCalle { get; set; }

        public int AlturaMin { get; set; }
        public int AlturaMax { get; set; }

    }
}
