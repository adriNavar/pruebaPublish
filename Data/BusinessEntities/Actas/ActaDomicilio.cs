using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ActaDomicilio : IEntity
    {
        public long ActaID { get; set; }
        public long id_domicilio { get; set; }
        public Domicilio domicilio { get; set; }
        public Acta Acta { get; set; }
    }
}
