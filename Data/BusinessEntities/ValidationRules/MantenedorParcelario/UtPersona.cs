using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario
{
    public class UtPersona
    {
        public long UnidadTributariaId { get; set; }

        public long PersonaId { get; set; }

        public long SavedPersonaId { get; set; }

        public Operaciones<UnidadTributariaPersona> OperacionesUnidadTributariaPersona { get; set; }

        public Operation Operacion { get; set; }
    }
}
