using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Models.ResponsableFiscal
{
    public class ResponsableFiscalViewModel
    {
        public short UnidadTributariaPersonaId { get; set; }

        public long TipoPersonaId { get; set; }

        public string TipoPersona { get; set; }

        public long PersonaId { get; set; }

        public string NombreCompleto { get; set; }

        public long UnidadTributariaId { get; set; }

        public long SavedPersonaId { get; set; }

        public string DomicilioFisico { get; set; }

        public Operation Operacion { get; set; }

        public string CodSistemaTributario { get; set; }
    }
}