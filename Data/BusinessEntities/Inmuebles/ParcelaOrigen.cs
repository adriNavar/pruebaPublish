using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaOrigen
    {
        public long IdOperacion { get; set; }
        public string TipoParcela { get; set; }
        public string Nomenclatura { get; set; }
        public string TipoOperacion { get; set; }
        public string CodigoProvincial { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdParcela { get; set; }
        public long IdTipoOperacion { get; set; }
    }
}
