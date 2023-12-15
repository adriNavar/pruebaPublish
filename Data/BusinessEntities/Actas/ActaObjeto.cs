using GeoSit.Data.BusinessEntities.Actas;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ActaObjeto : IEntity
    {
        public long ActaID { get; set; }
        public long id_tipo_objeto { get; set; }
        public long id_objeto { get; set; }
        public Acta Acta { get; set; }
    }
}
