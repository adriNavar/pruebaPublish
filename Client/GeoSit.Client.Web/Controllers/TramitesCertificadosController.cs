using GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Models.ExpedientesObras;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers
{

    public class TramitesCertificadosController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private readonly HttpClient _clienteReportes = new HttpClient();
        // GET: TramitesCertificados
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConsultaTramites()
        {
            //InformeInspeccionModel model = new InformeInspeccionModel();

            //TramiteModels model = new TramiteModels();
            //// obtener los tipos dede tramites
            //  HttpResponseMessage resp = _cliente.GetAsync("api/ObrasParticularesService/GetInspectores").Result;

            //List<InspectorModel> inspectores = (List<InspectorModel>)resp.Content.ReadAsAsync<IEnumerable<InspectorModel>>().Result;
            ////model.Inspectores = inspectores.GroupBy(s => s.Usuario.Id_Usuario).Select(p => p.First()).ToList();
            //return PartialView(model);

            TramiteModels model = new TramiteModels();
            TipoDeTramite TT = new TipoDeTramite();
            TT.Descripcion = "Certificado Liso de sueldo";
            TT.Id = 12233444;
            model.TramitesList.Add(TT);
            TT = new TipoDeTramite();
            TT.Descripcion = "El otro Tramite";
            TT.Id = 154546678;
            model.TramitesList.Add(TT);
            TT = new TipoDeTramite();
            TT.Descripcion = "Otro tramite de mierda";
            TT.Id = 35544465;
            model.TramitesList.Add(TT);


            return PartialView(model);
        }
        public ActionResult GenerarInformeEstadoTramite(TramiteModels model)
        {
            //preguntar si es null identificador o nro de tramite para poder optar por cual hacer la consulta
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
            var resp = _cliente.GetAsync("api/ReporteEstadoTramite/GetInformeEstadoTramite?Id=" + model.Identificador + "&Numero=" + model.Numero + "&Operacion=" + model.Operacion).Result;        
           
            

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msg = resp.Content.ReadAsStringAsync().Result;
                return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
            }

            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);

            Session["InformeEstadoTramite.pdf"] = bytes;
            return Json(new { success = true, file = "InformeEstadoTramite.pdf" }, JsonRequestBehavior.AllowGet);
        }
         public ActionResult GetFileInformeEstadoTramite(string file)
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
