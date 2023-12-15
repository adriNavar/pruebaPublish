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
using System.Net.Http.Formatting;

namespace GeoSit.Client.Web.Controllers
{
    public class ProfesionController : Controller
    {

        private HttpClient cliente = new HttpClient();

        public ProfesionController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }


        // GET: /Profesion/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Profesion/DatosProfesion
        public ActionResult DatosProfesion()
        {
            ViewBag.DatosProfesion = GetDatosProfesion();
            ViewBag.DatosTiposProfesiones = GetDatosTiposProfesiones();
            List<SelectListItem> itemsTipos = new List<SelectListItem>();
            foreach (var tipo in ViewBag.DatosTiposProfesiones)
            {
                if (tipo.TipoProfesionId == 1)
                {
                    itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId), Selected = true });
                }
                else
                {
                    itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId) });
                }
            }
            ViewData["tiposprof"] = new SelectList(itemsTipos, "Value", "Text"); ;
            return View();
        }

        public List<ProfesionModel> GetDatosProfesion()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ProfesionService/GetProfesiones").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ProfesionModel>)resp.Content.ReadAsAsync<IEnumerable<ProfesionModel>>().Result;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Set_DatosProfesionPersona(long personaId)
        {
            ProfesionModel resul = new ProfesionModel();
            resul = GrabarProfesion(personaId);

            ViewBag.DatosProfesion = GetDatosProfesion();
            ViewBag.DatosTiposProfesiones = GetDatosTiposProfesiones();
            List<SelectListItem> itemsTipos = new List<SelectListItem>();
            foreach (var tipo in ViewBag.DatosTiposProfesiones)
            {
                if (tipo.TipoProfesionId == 1)
                {
                    itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId), Selected = true });
                }
                else
                {
                    itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId) });
                }
            }
            ViewData["tiposprof"] = new SelectList(itemsTipos, "Value", "Text"); 
            return View("DatosProfesion");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_DatosProfesionPersona(ProfesionModels  Save_Datos)
        {
            //ProfesionModel resul = new ProfesionModel();
            //resul = GrabarProfesion(Save_Datos.PersonaId);
            var matricula = Save_Datos.DatosProfesion.Matricula;
            var persona = Save_Datos.DatosProfesion.PersonaId;
            var tipoprofesional = Save_Datos.DatosProfesion.TipoProfesionId;
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ProfesionService/SetProfesion_Save", Save_Datos.DatosProfesion).Result;
            resp.EnsureSuccessStatusCode();

            ViewBag.DatosProfesion = GetDatosProfesion();
            ViewBag.DatosTiposProfesiones = GetDatosTiposProfesiones();
            List<SelectListItem> itemsTipos = new List<SelectListItem>();
            foreach (var tipo in ViewBag.DatosTiposProfesiones)
            {
                if (tipo.TipoProfesionId == 1)
                {
                    itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId), Selected = true });
                }
                else
                {
                    itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId) });
                }
            }
            ViewData["tiposprof"] = new SelectList(itemsTipos, "Value", "Text"); 
            return View("DatosProfesion");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatosProfesionPersona(long personaId)
        {
            ViewBag.DatosProfesion = GetDatosProfesion(personaId);
            ViewBag.DatosTiposProfesiones = GetDatosTiposProfesiones();
            return View("DatosProfesion");
        }
        
        public List<ProfesionModel> GetDatosProfesion(long personaId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ProfesionService/GetProfesiones").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ProfesionModel>)resp.Content.ReadAsAsync<IEnumerable<ProfesionModel>>().Result;
        }

        public List<TiposProfesionesModel> GetDatosTiposProfesiones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoProfesionService/GetTiposProfesion").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposProfesionesModel>)resp.Content.ReadAsAsync<IEnumerable<TiposProfesionesModel>>().Result;
        }

        /*
        public TiposProfesionesModel GrabarProfesion(TiposProfesionesModel mt)
        {
            HttpResponseMessage resp = cliente.PostAsync<TiposProfesionesModel>("api/ProfesionService/GrabaProfesion", mt, new XmlMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return (TiposProfesionesModel)resp.Content.ReadAsAsync<TiposProfesionesModel>().Result;
        }
        */

        public ProfesionModel GrabarProfesion(long personaId)
        {
            //MediaTypeFormatter formatter = new TiposProfesionesModel();
            //HttpResponseMessage resp = cliente.PostAsync<TiposProfesionesModel>("api/ProfesionService/SetProfesion_Save", mt, formatter).Result;
            ProfesionModel mt = new ProfesionModel();
            mt.PersonaId = personaId;
            mt.Matricula = ViewBag.DatosProfesion.Matricula;
            mt.TipoProfesionId = 1;

            //HttpResponseMessage resp = cliente.PostAsync<ProfesionModel>("api/ProfesionService/SetProfesion_Save", mt, new XmlMediaTypeFormatter()).Result;
            HttpResponseMessage resp = cliente.PostAsync<ProfesionModel>("api/ProfesionService/SetProfesion_Save", mt, new XmlMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return (ProfesionModel)resp.Content.ReadAsAsync<ProfesionModel>().Result;
        }
    }
}
