using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers
{
    public class PlantillaEscalaController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });
        private const string ApiUri = "api/plantillaescala/";

        public PlantillaEscalaController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult List(int id)
        {
            var result = _cliente.GetAsync(ApiUri + "listbyplantillaid/" + id).Result;
            result.EnsureSuccessStatusCode();
            var escalas = (List<PlantillaEscala>)result.Content.ReadAsAsync<IEnumerable<PlantillaEscala>>().Result;

            return PartialView("~/Views/Plantilla/Partial/_ListaEscalas.cshtml", escalas);
        }

        public ActionResult Save(ViewPlantillaEscala viewPlantillaEscala)
        {
            //var plantilla = Session["Plantilla"] as Plantilla;
            //viewPlantillaEscala.IdPlantilla = plantilla.IdPlantilla;
            //viewPlantillaEscala.IdUsuarioAlta = 1;
            //viewPlantillaEscala.FechaAlta = DateTime.Now;
            //viewPlantillaEscala.IdUsuarioModificacion = 1;
            //viewPlantillaEscala.FechaModificacion = DateTime.Now;
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            var exists = viewPlantillaEscala.IdPlantillaEscala > 0 || admin.OperacionesEscalas.Exists(o => o.Item.IdPlantillaEscala == viewPlantillaEscala.IdPlantillaEscala);
            admin.OperacionesEscalas.Add(new OperationItem<PlantillaEscala> { Operation = (exists ? Operation.Update : Operation.Add), Item = viewPlantillaEscala.GetPlantillaEscala(viewPlantillaEscala) });

            //var response = _cliente.PostAsync(ApiUri + "post", viewPlantillaEscala, new JsonMediaTypeFormatter()).Result;
            //response.EnsureSuccessStatusCode();

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult FormContent()
        {
            return PartialView("~/Views/Plantilla/Partial/_PlantillaEscalaAjaxForm.cshtml");
        }

        public ActionResult Delete(int id)
        {
            PlantillaEscala escala = new PlantillaEscala { IdPlantillaEscala = id };
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            var isnew = id < 0;
            if (!isnew)
            {
                var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
                //var result = _cliente.PostAsync(ApiUri + "delete", id, new JsonMediaTypeFormatter()).Result;
                //var result = _cliente.PostAsync(ApiUri + "post", viewPlantillaEscala, new JsonMediaTypeFormatter()).Result;
                //var result = _cliente.PostAsync(ApiUri + "delete", escala, new JsonMediaTypeFormatter()).Result;
                result.EnsureSuccessStatusCode();
                escala = result.Content.ReadAsAsync<PlantillaEscala>().Result;
            }

            admin.OperacionesEscalas.Add(new OperationItem<PlantillaEscala> { Operation = Operation.Remove, Item = escala });

            return new JsonResult { Data = "Ok" };
        }
    }
}
