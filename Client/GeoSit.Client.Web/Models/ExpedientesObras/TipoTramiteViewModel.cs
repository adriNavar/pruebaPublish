using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class TipoTramiteViewModel
    {
        public SelectList TipoList { get; set; }

        public long IdTipo { get; set; }
    }
}