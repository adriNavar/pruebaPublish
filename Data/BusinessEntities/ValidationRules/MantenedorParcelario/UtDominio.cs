using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Data.BusinessEntities.ValidationRules.MantenedorParcelario
{
    public class UtDominio
    {
        public long UnidadTributariaId { get; set; }

        public long DominioId { get; set; }

        public string Inscripcion { get; set; }

        public Operaciones<Dominio> OperacionesDominio { get; set; }

    }
}
