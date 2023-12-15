using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Configuration;

namespace GeoSit.Client.Web.Controllers
{
    public class ProfesionesController : Controller
    {
        private HttpClient cliente = new HttpClient();
        
        private string UploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public ProfesionesController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        // GET: /Profesiones/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Profesiones/DatosProfesiones
        public ActionResult DatosProfesiones()
        {
            ViewBag.listaDatosProfesiones = GetDatosProfesiones();    
            return View();
		}
		
        public List<ProfesionModel> GetDatosProfesiones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ProfesionService/GetProfesiones").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ProfesionModel>)resp.Content.ReadAsAsync<IEnumerable<ProfesionModel>>().Result;
        }	
    }
}
