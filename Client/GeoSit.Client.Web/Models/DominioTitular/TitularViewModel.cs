using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Models.DominioTitular
{
    public class TitularViewModel
    {
        public short DominioPersonaId { get; set; }

        public long DominioId { get; set; }

        public short? TipoTitularidadId { get; set; }

        public long PersonaId { get; set; }

        public string NombreCompleto { get; set; }

        public string TipoNoDocumento { get; set; }

        public decimal PorcientoCopropiedad { get; set; }

        public string DomicilioFisico { get; set; }

        public Operation Operacion { get; set; }

        public decimal PorcientoCopropiedadTotal { get; set; }
    }
}