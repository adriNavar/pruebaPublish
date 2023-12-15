using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.Documentos;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ActaDocumento : IEntity
    {
        public long ActaID { get; set; }
        public long id_documento { get; set; }
        public Documento documento { get; set; }
        public Acta Acta { get; set; }
    }
}
