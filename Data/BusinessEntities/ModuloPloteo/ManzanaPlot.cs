using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class ManzanaPlot
    {
        public long FeatId { get; set; }
        public string Nombre { get; set; }
        public string Numero { get; set; }
        public DbGeometry Geom { get; set; }
        public double Superficie { get; set; }
    }
}