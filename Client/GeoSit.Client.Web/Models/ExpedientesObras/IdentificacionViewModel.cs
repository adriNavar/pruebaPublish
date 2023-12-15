using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class IdentificacionViewModel
    {
        public string NumeroLegajo { get; set; }

        public string FechaLegajo { get; set; }

        public string NumeroExpediente { get; set; }

        public string FechaExpediente { get; set; }

        public bool EnPosesion { get; set; }

        public string Chapa { get; set; }

        public SelectList PlanList { get; set; }

        public int PlanId { get; set; }
    }
}