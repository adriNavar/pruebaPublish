using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario
{
    public class DomTitular
    {
        public long DominioId { get; set; }

        public long PersonaId { get; set; }

        public Operaciones<DominioTitular> OperacionesDominioTitular { get; set; }
    }
}
