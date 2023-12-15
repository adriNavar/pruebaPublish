using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web.Http;

namespace GeoSit.WebDCGDummy.Api.Controllers
{
    public class InterfaseDCGDummyController : ApiController
    {
        [Route("DomPorPartida")]
        [HttpGet]
        public IHttpActionResult DomPorPartida(string idUsuario, string partida)
        {
            string result = "{\"inscripcion\": {\"matricula\": \"12345\", \"libro\": null, \"tomo\": null, \"folio\": null, \"finca\": null, \"anio\": null, \"uf\": null, \"fecha\": \"2015-11-17\", \"titulares\":[ {\"titular\": { \"tipo_documento\": \"CUIT\", \"numero_documento\": \"20123456789\", \"nombre_completo\": \"José Manuel Araujo\", \"porcentaje\": 35.25 }}, {\"titular\": { \"tipo_documento\": \"DNI\", \"numero_documento\": \"12345678\", \"nombre_completo\": \"Luis Angel Araujo\", \"porcentaje\": 34.00 }}, {\"titular\": { \"tipo_documento\": \"CUIT\", \"numero_documento\": \"23123456788\", \"nombre_completo\": \"Luisa María Araujo\", \"porcentaje\": 30.25 }}], \"descripcion\": \"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ut tellus venenatis, volutpat nibh eu, posuere eros. Nam quis magna tempor, feugiat diam vitae, semper nulla. Mauris eu scelerisque tortor. Sed ornare enim eu ornare faucibus. Phasellus id suscipit nisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Praesent metus nisi, luctus et suscipit id, varius dapibus sapien. Proin a orci cursus, pharetra arcu ut, commodo tellus. Pellentesque tristique fringilla massa quis hendrerit. Cras imperdiet ipsum consectetur lobortis luctus. Cras in lacus quam. Pellentesque sed justo id felis tempus accumsan. Ut eu justo sed elit porttitor porta. Donec sed rutrum nibh. Sed quis nunc magna. Nunc dictum quam sed nisl laoreet lobortis. Vestibulum maximus ligula sem, sed commodo mauris mollis sed. Suspendisse nisi lorem, gravida vestibulum fermentum sed, hendrerit eu sem. Aliquam elementum, dolor a pellentesque sagittis, erat purus viverra risus, eget efficitur erat metus eu arcu. Sed lorem sapien, dignissim eget ultricies sed, porttitor vel tellus. Proin mollis nibh sed diam placerat, nec efficitur quam tempor. Nulla ornare augue vel dolor aliquet aliquet. In ipsum orci, semper non dapibus id, condimentum accumsan libero. Maecenas ante lacus, aliquet non rutrum in, faucibus vel ligula. Sed sit amet dui egestas, dignissim leo vitae, fringilla metus. Aliquam placerat, enim eget vulputate auctor, nisl orci lobortis massa, et tincidunt ex nulla viverra sem. Ut tincidunt arcu vitae neque efficitur, non dignissim ipsum consectetur.\", \"antecedentes\": null, \"provisorio\": \"no\", \"restricciones\": [{ \"restriccion\": \"No Innovar\"}, {\"restriccion\": \"Embargo\" }] }}";
            return Json(JObject.Parse(result));
        }

        [Route("DomPorNomenclatura")]
        [HttpGet]
        public IHttpActionResult DomPorNomenclatura(string idUsuario, string nomenclatura)
        {
            string result = "{\"inscripcion\": {\"matricula\": \"12345\", \"libro\": null, \"tomo\": null, \"folio\": null, \"finca\": null, \"anio\": null, \"uf\": null, \"fecha\": \"2015-11-17\", \"titulares\":[ {\"titular\": { \"tipo_documento\": \"CUIT\", \"numero_documento\": \"20123456789\", \"nombre_completo\": \"José Manuel Araujo\", \"porcentaje\": 35.25 }}, {\"titular\": { \"tipo_documento\": \"DNI\", \"numero_documento\": \"12345678\", \"nombre_completo\": \"Luis Angel Araujo\", \"porcentaje\": 34.00 }}, {\"titular\": { \"tipo_documento\": \"CUIT\", \"numero_documento\": \"23123456788\", \"nombre_completo\": \"Luisa María Araujo\", \"porcentaje\": 30.25 }}], \"descripcion\": \"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ut tellus venenatis, volutpat nibh eu, posuere eros. Nam quis magna tempor, feugiat diam vitae, semper nulla. Mauris eu scelerisque tortor. Sed ornare enim eu ornare faucibus. Phasellus id suscipit nisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Praesent metus nisi, luctus et suscipit id, varius dapibus sapien. Proin a orci cursus, pharetra arcu ut, commodo tellus. Pellentesque tristique fringilla massa quis hendrerit. Cras imperdiet ipsum consectetur lobortis luctus. Cras in lacus quam. Pellentesque sed justo id felis tempus accumsan. Ut eu justo sed elit porttitor porta. Donec sed rutrum nibh. Sed quis nunc magna. Nunc dictum quam sed nisl laoreet lobortis. Vestibulum maximus ligula sem, sed commodo mauris mollis sed. Suspendisse nisi lorem, gravida vestibulum fermentum sed, hendrerit eu sem. Aliquam elementum, dolor a pellentesque sagittis, erat purus viverra risus, eget efficitur erat metus eu arcu. Sed lorem sapien, dignissim eget ultricies sed, porttitor vel tellus. Proin mollis nibh sed diam placerat, nec efficitur quam tempor. Nulla ornare augue vel dolor aliquet aliquet. In ipsum orci, semper non dapibus id, condimentum accumsan libero. Maecenas ante lacus, aliquet non rutrum in, faucibus vel ligula. Sed sit amet dui egestas, dignissim leo vitae, fringilla metus. Aliquam placerat, enim eget vulputate auctor, nisl orci lobortis massa, et tincidunt ex nulla viverra sem. Ut tincidunt arcu vitae neque efficitur, non dignissim ipsum consectetur.\", \"antecedentes\": null, \"provisorio\": \"no\", \"restricciones\": [{ \"restriccion\": \"No Innovar\"}, {\"restriccion\": \"Embargo\" }] }}";
            return Json(JObject.Parse(result));
        }

        [Route("DomPorInscripcion")]
        [HttpGet]
        public IHttpActionResult DomPorInscripcion(string idUsuario, int? matricula, int? libro, int? tomo, int? folio, int? finca, int? anio, int? uf)
        {
            string result = "{\"inscripcion\": {\"matricula\": \"" + matricula + "\", \"libro\":  \"" + libro + "\", \"tomo\": \"" + tomo + "\", \"folio\": \"" + folio + "\", \"finca\": \"" + finca + "\", \"anio\": \"" + anio + "\", \"uf\": \"" + uf + "\", \"fecha\": \"2015-11-17\", \"titulares\":[ {\"titular\": { \"tipo_documento\": \"CUIT\", \"numero_documento\": \"20123456789\", \"nombre_completo\": \"José Manuel Araujo\", \"porcentaje\": 35.25 }}, {\"titular\": { \"tipo_documento\": \"DNI\", \"numero_documento\": \"12345678\", \"nombre_completo\": \"Luis Angel Araujo\", \"porcentaje\": 34.00 }}, {\"titular\": { \"tipo_documento\": \"CUIT\", \"numero_documento\": \"23123456788\", \"nombre_completo\": \"Luisa María Araujo\", \"porcentaje\": 30.25 }}], \"descripcion\": \"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ut tellus venenatis, volutpat nibh eu, posuere eros. Nam quis magna tempor, feugiat diam vitae, semper nulla. Mauris eu scelerisque tortor. Sed ornare enim eu ornare faucibus. Phasellus id suscipit nisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Praesent metus nisi, luctus et suscipit id, varius dapibus sapien. Proin a orci cursus, pharetra arcu ut, commodo tellus. Pellentesque tristique fringilla massa quis hendrerit. Cras imperdiet ipsum consectetur lobortis luctus. Cras in lacus quam. Pellentesque sed justo id felis tempus accumsan. Ut eu justo sed elit porttitor porta. Donec sed rutrum nibh. Sed quis nunc magna. Nunc dictum quam sed nisl laoreet lobortis. Vestibulum maximus ligula sem, sed commodo mauris mollis sed. Suspendisse nisi lorem, gravida vestibulum fermentum sed, hendrerit eu sem. Aliquam elementum, dolor a pellentesque sagittis, erat purus viverra risus, eget efficitur erat metus eu arcu. Sed lorem sapien, dignissim eget ultricies sed, porttitor vel tellus. Proin mollis nibh sed diam placerat, nec efficitur quam tempor. Nulla ornare augue vel dolor aliquet aliquet. In ipsum orci, semper non dapibus id, condimentum accumsan libero. Maecenas ante lacus, aliquet non rutrum in, faucibus vel ligula. Sed sit amet dui egestas, dignissim leo vitae, fringilla metus. Aliquam placerat, enim eget vulputate auctor, nisl orci lobortis massa, et tincidunt ex nulla viverra sem. Ut tincidunt arcu vitae neque efficitur, non dignissim ipsum consectetur.\", \"antecedentes\": null, \"provisorio\": \"no\", \"restricciones\": [{ \"restriccion\": \"No Innovar\"}, {\"restriccion\": \"Embargo\" }] }}";
            return Json(JObject.Parse(result));
        }

        [Route("VerificarPermisosAcceso")]
        [HttpGet]
        public IHttpActionResult VerificarPermisosAcceso(string idUsuario)
        {
            if (new string[] { "12345", "2345", "345", "45" }.Contains(idUsuario))
            {
                return Ok();
            }
            else if (new string[] { "54321", "5432", "543", "54" }.Contains(idUsuario))
            {
                return Unauthorized();
            }
            else
            {
                return StatusCode(System.Net.HttpStatusCode.Forbidden);
            }
        }
    }
}