using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class ExpansionPlot
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public DbGeometry Geom { get; set; }

        public string IdReferencia { get; set; }
    }
}