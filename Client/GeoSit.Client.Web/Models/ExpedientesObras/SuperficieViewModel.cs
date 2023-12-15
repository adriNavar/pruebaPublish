using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class SuperficieViewModel
    {
        public long ExpedienteObraSuperficieId { get; set; }

        public long ExpedienteObraId { get; set; }

        public string Fecha { get; set; }

        public SelectList TipoList { get; set; }

        public long TipoSuperficieId { get; set; }

        public SelectList DestinoList { get; set; }

        public long DestinoId { get; set; }

        public decimal Superficie { get; set; }

        public int CantidadPlantas { get; set; }
    }
}