using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.BusinessEntities.Actas
{
    public class ActaUnidadTributaria : IEntity
    {
        public long ActaId { get; set; }
        public long UnidadTributariaID { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }
        public Acta Acta { get; set; }
    }
}
