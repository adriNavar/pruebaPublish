using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class ServicioViewModel
    {
        public SelectList ServicioList { get; set; }

        public long IdServicio { get; set; }
    }
}