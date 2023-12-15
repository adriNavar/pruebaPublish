using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class LayerGraf
    {
        public long FeatId { get; set; }

        public string Nombre { get; set; }

        public DbGeometry Geom { get; set; }

        public int? Rotation { get; set; }

        public string Descripcion { get; set; }
    }
}
