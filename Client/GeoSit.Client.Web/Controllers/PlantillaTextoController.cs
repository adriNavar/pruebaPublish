using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Text;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers
{
    public class PlantillaTextoController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });
        private const string ApiUri = "api/plantillatexto/";
        
        public PlantillaTextoController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult List(int id)
        {
            var result = _cliente.GetAsync(ApiUri + "listbyplantillaid/" + id).Result;
            result.EnsureSuccessStatusCode();
            var plantillaTextos = (List<PlantillaTexto>)result.Content.ReadAsAsync<IEnumerable<PlantillaTexto>>().Result;

            return PartialView("~/Views/Plantilla/Partial/_ListaPlantillaTextos.cshtml", plantillaTextos);
        }

        public ActionResult FormContent()
        {
            var viewPlantillaTexto = new ViewPlantillaTexto
            {
                FuenteColor = "000000"
            };

            var response = _cliente.GetAsync("api/atributo/getvisiblesbycomponente/0").Result;
            response.EnsureSuccessStatusCode();
            var atributos = response.Content.ReadAsAsync<List<Atributo>>().Result;

            atributos.Insert(0, new Atributo { AtributoId = 0, Nombre = "(Ninguno)" });

            viewPlantillaTexto.AtributoList = new SelectList(atributos, "AtributoId", "Nombre");

            var installedFontCollection = new InstalledFontCollection();
            var fuentes = installedFontCollection.Families;

            viewPlantillaTexto.FuenteList = new SelectList(fuentes, "Name", "Name");

            return PartialView("~/Views/Plantilla/Partial/_PlantillaTextoAjaxForm.cshtml", viewPlantillaTexto);
        }

        public ActionResult Save(ViewPlantillaTexto viewPlantillaTexto)
        {
            var plantilla = Session["Plantilla"] as Plantilla;
            viewPlantillaTexto.IdPlantilla = plantilla.IdPlantilla;

            var estilo = viewPlantillaTexto.Negrita ? "1" : "0";
            estilo += viewPlantillaTexto.Cursiva ? ",1" : ",0";
            estilo += viewPlantillaTexto.Tachada ? ",1" : ",0";
            estilo += viewPlantillaTexto.Subrayada ? ",1" : ",0";

            viewPlantillaTexto.FuenteEstilo = estilo;
            viewPlantillaTexto.FuenteColor = "#" + viewPlantillaTexto.FuenteColor;

            viewPlantillaTexto.IdUsuarioAlta = 1;
            viewPlantillaTexto.FechaAlta = DateTime.Now;
            viewPlantillaTexto.IdUsuarioModificacion = 1;
            viewPlantillaTexto.FechaModificacion = DateTime.Now;

            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            var exists = viewPlantillaTexto.IdPlantillaTexto > 0 || admin.OperacionesTextos.Exists(o => o.Item.IdPlantillaTexto == viewPlantillaTexto.IdPlantillaTexto);
            admin.OperacionesTextos.Add(new OperationItem<PlantillaTexto>
            {
                Operation = (exists ? Operation.Update : Operation.Add),
                Item = viewPlantillaTexto.GetPlantillaTexto()
            });

            //var response = _cliente.PostAsync(ApiUri + "post", viewPlantillaTexto, new JsonMediaTypeFormatter()).Result;
            //response.EnsureSuccessStatusCode();

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Delete(int id)
        {
            PlantillaTexto texto = new PlantillaTexto { IdPlantillaTexto = id };
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            var isnew = id < 0;
            if (!isnew)
            {
                var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
                result.EnsureSuccessStatusCode();
                texto = result.Content.ReadAsAsync<PlantillaTexto>().Result;
            }

            admin.OperacionesTextos.Add(new OperationItem<PlantillaTexto> { Operation = Operation.Remove, Item = texto });

            return new JsonResult { Data = "Ok" };
        }
    }
}