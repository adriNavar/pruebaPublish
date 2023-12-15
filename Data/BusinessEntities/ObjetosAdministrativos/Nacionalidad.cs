using System;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class Nacionalidad : IEntity
    {
        public long NacionalidadId { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
