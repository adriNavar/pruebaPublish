using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ActaPersonas : IEntity
    {
        public long ActaId { get; set; }
        public long PersonaId { get; set; }
        public long PersonaRolId { get; set; }
        public Persona Persona { get; set; }
        public Acta Acta { get; set; }
    }
}
