using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Client.Web.Models.ExpedientesObras;
using GeoSit.Data.BusinessEntities.Actas;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class ActaController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private readonly HttpClient _clienteReportes = new HttpClient();

        public ActaController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            _clienteReportes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        public void ClearId()
        {
        }

        public ActionResult List(long id)
        {
            var result = _cliente.GetAsync("api/acta/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var actas = result.Content.ReadAsAsync<IEnumerable<ActaExpedienteObra>>().Result;

            return PartialView("~/Views/ExpedientesObras/Partial/_ListaActas.cshtml", actas);
        }


        public ActionResult InformeActasVencidas()
        {
            return PartialView();
        }

        public ActionResult GenerarInformeActasVencidas(Acta model)
        {
            var resp = _clienteReportes.GetAsync("api/Inspecciones/GetInformeActaVencidas?fecha=" + model.Fecha).Result;

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msg = resp.Content.ReadAsStringAsync().Result;
                return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
            }

            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);

            Session["InformeActasVencida.pdf"] = bytes;
            return Json(new { success = true, file = "InformeActasVencida.pdf" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFileInformeActasVencida(string file)
        {
            byte[] bytes = Session[file] as byte[];
            if (bytes == null)
                return new EmptyResult();
            Session[file] = null;

            var cd = new System.Net.Mime.ContentDisposition
            {
                Size = bytes.Length,
                FileName = file,
                Inline = true,
            };
            Response.Clear();
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.ContentType = "application/pdf";
            Response.Buffer = true;
            Response.BinaryWrite(bytes);

            return null;
        }
    }
}