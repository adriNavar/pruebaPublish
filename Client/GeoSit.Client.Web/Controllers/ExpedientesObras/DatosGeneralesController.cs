using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class DatosGeneralesController : Controller
    {
        public ActionResult FormContent()
        {
            return PartialView("~/Views/ExpedientesObras/Partial/_DatosGenerales.cshtml");
        }          
    }
}