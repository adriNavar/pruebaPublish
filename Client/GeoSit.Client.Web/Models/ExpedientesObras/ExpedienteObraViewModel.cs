namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class ExpedienteObraViewModel
    {
        public long ExpedienteObraId { get; set; }

        public string NumeroLegajo { get; set; }

        public string FechaLegajo { get; set; }

        public string NumeroExpediente { get; set; }

        public string FechaExpediente { get; set; }

        public bool EnPosesion { get; set; }

        public string Chapa { get; set; }

        public int PlanId { get; set; }

        public bool Ph { get; set; }

        public bool PermisosProvisorios { get; set; }
    }
}