using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Reportes.Api.Models
{
    public class ExpedienteObraModel
    {
        public ICollection<Expediente> Expedientes { get; set; }
        public ICollection<ControlTecnico> Observaciones { get; set; }
        public ICollection<UnidadTributariaExpedienteObra> UnidadTributariaExpedienteObras { get; set; }
    }

    public class Expediente
    {
        public string NumeroLegajo { get; set; }
        public string NumeroExpediente { get; set; }
        public DateTime? FechaExpediente { get; set; }
        public string TipoExpediente { get; set; }
        public string EstadoExpediente { get; set; }
    }
}