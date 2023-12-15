using System;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class EstadoViewModel
    {
        public long ExpedienteObraId { get; set; }

        public SelectList EstadoList { get; set; }

        public int EstadoExpedienteId { get; set; }

        public DateTime Fecha { get; set; }

        public string Observaciones { get; set; }
    }
}