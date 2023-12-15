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
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Xml;
using System.Text;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Linq;

namespace GeoSit.Client.Web.Controllers
{
    public class DomicilioController : Controller
    {
        private HttpClient cliente = new HttpClient();

        public DomicilioController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }


        // GET: /Domicilio/Index
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /Domicilio/DatosDomicilio
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult DatosDomicilio(string id, DomicilioModel domicilio, string id_elemento)
        {
            ViewBag.IdentificadorConfigurado = id_elemento;
            string idPais;
            string idLocalidad;
            string idPartido;
            string idProvincia;

            if (string.IsNullOrEmpty(id))
            {
                ViewBag.DatosDomicilio = GetDatosDomicilioById("-1");
                ViewBag.IdentificadorConfigurado = "0";

                //Configuración por default.
                idLocalidad = ConfigurationManager.AppSettings["LocalidadDefault"];
                idPartido = GetObjetoById(idLocalidad).ObjetoPadreId.GetValueOrDefault().ToString();
                idProvincia = GetObjetoById(idPartido).ObjetoPadreId.GetValueOrDefault().ToString();
                idPais = GetObjetoById(idProvincia).ObjetoPadreId.GetValueOrDefault().ToString();

            }
            else
            {
                ViewBag.IdentificadorConfigurado = id;
                ViewBag.DatosDomicilio = domicilio ?? GetDatosDomicilioById(id);

                idPais = Convert.ToString(GetIdObjetoByNombreTipo(ViewBag.DatosDomicilio.pais, TiposObjetoAdministrativo.PAIS));
                idProvincia = Convert.ToString(GetIdObjetoByNombreTipo(ViewBag.DatosDomicilio.provincia, TiposObjetoAdministrativo.PROVINCIA));
                idPartido = Convert.ToString(GetIdObjetoByNombreTipo(ViewBag.DatosDomicilio.municipio, TiposObjetoAdministrativo.DEPARTAMENTO));
                idLocalidad = Convert.ToString(GetIdObjetoByNombreTipo(ViewBag.DatosDomicilio.localidad, TiposObjetoAdministrativo.LOCALIDAD));
            }

            ViewBag.ItemPais = idPais;
            ViewBag.ItemProvincia = idProvincia;
            ViewBag.ItemPartido = idPartido;
            ViewBag.ItemLocalidad = idLocalidad;

            ViewData["paises"] = new SelectList(GetPaises(), "Value", "Text");
            ViewData["provincias"] = new SelectList(GetProvincias(idPais), "Value", "Text");
            ViewData["partidos"] = new SelectList(GetDepartamentos(idProvincia), "Value", "Text");
            ViewData["localidades"] = new SelectList(GetLocalidades(idPartido), "Value", "Text");

            ViewData["tiposdomicilio"] = new SelectList(GetTiposDomicilio(), "Value", "Text");

            ViewData["viaslista"] = new SelectList(GetViasDefault(), "Value", "Text");
            return PartialView(new DomicilioModels() { DatosDomicilio = domicilio });
        }

        [ValidateInput(false)]
        public ActionResult DatosDomicilio(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.DatosDomicilio = GetDatosDomicilioById("-1");
            }
            else
            {
                ViewBag.DatosDomicilio = GetDatosDomicilioById(id);
            }


            bool confDefecto = false;
            var idPais = ViewBag.DatosDomicilio.pais;
            var idLocalidad = "";
            var idPartido = "";
            var idProvincia = "";
            if (string.IsNullOrEmpty(id))
            {
                confDefecto = true;
                //Configuración por default.
                idLocalidad = ConfigurationManager.AppSettings["LocalidadDefault"];

                idPartido = (GetObjetoById(idLocalidad)?.ObjetoPadreId ?? 0).ToString();
                idProvincia = (GetObjetoById(idPartido)?.ObjetoPadreId ?? 0).ToString();
                idPais = (GetObjetoById(idProvincia)?.ObjetoPadreId ?? 0).ToString();
            }


            if (confDefecto != true)
            {
                idPais = ViewBag.DatosDomicilio.pais;
                idPais = Convert.ToString(GetIdObjetoByNombreTipo(idPais, TiposObjetoAdministrativo.PAIS));

                idProvincia = ViewBag.DatosDomicilio.provincia;
                idProvincia = Convert.ToString(GetIdObjetoByNombreTipo(idProvincia, TiposObjetoAdministrativo.PROVINCIA));

                idPartido = ViewBag.DatosDomicilio.municipio;
                idPartido = Convert.ToString(GetIdObjetoByNombreTipo(idPartido, TiposObjetoAdministrativo.DEPARTAMENTO));

                idLocalidad = ViewBag.DatosDomicilio.localidad;
                idLocalidad = Convert.ToString(GetIdObjetoByNombreTipo(idLocalidad, TiposObjetoAdministrativo.LOCALIDAD));
            }
            ViewBag.ItemPais = idPais;
            ViewBag.ItemProvincia = idProvincia;
            ViewBag.ItemPartido = idPartido;
            ViewBag.ItemLocalidad = idLocalidad;

            ViewData["paises"] = new SelectList(GetPaises(), "Value", "Text");
            ViewData["provincias"] = new SelectList(GetProvincias(idPais), "Value", "Text");
            ViewData["partidos"] = new SelectList(GetDepartamentos(idProvincia), "Value", "Text");
            ViewData["localidades"] = new SelectList(GetLocalidades(idPartido), "Value", "Text");

            ViewData["tiposdomicilio"] = new SelectList(GetTiposDomicilio(), "Value", "Text");

            ViewBag.DescripcionPartido = "Departamento";

            //var ver = GetViasPorNombre("SAN");
            ViewData["viaslista"] = new SelectList(GetViasDefault(), "Value", "Text");
            return PartialView();
        }
        [HttpGet, ValidateInput(false)]
        public ActionResult DatosDomicilioXml(string xml)
        {
            DomicilioModel domi = new DomicilioModel();
            PersonaDomicilioModel personaDomi = new PersonaDomicilioModel();

            byte[] data = Convert.FromBase64String(xml);
            var domicilio = Encoding.UTF8.GetString(data);
            //var domicilio = Encoding.UTF8.GetString(Convert.FromBase64String(xml[0]));

            #region Complex
            domicilio = domicilio.Replace("#?", "<");
            domicilio = domicilio.Replace("?#", ">");
            domicilio = domicilio.Replace("#?", "<");
            domicilio = domicilio.Replace("?#", ">");
            int iStar = domicilio.IndexOf("<ViaNombre>") + "<ViaNombre>".Length;
            int iEnd = domicilio.IndexOf("</ViaNombre>");
            domi.ViaNombre = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<DomicilioId>") + "<DomicilioId>".Length;
            iEnd = domicilio.IndexOf("</DomicilioId>");

            if (!string.IsNullOrEmpty(domicilio.Substring(iStar, iEnd - iStar)))
            {
                domi.DomicilioId = -1;
            }
            else
            {
                personaDomi.DomicilioId = (domi.DomicilioId = 0);
            }

            iStar = domicilio.IndexOf("<numero_puerta>") + "<numero_puerta>".Length;
            iEnd = domicilio.IndexOf("</numero_puerta>");
            domi.numero_puerta = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<piso>") + "<piso>".Length;
            iEnd = domicilio.IndexOf("</piso>");
            domi.piso = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<unidad>") + "<unidad>".Length;
            iEnd = domicilio.IndexOf("</unidad>");
            domi.unidad = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<barrio>") + "<barrio>".Length;
            iEnd = domicilio.IndexOf("</barrio>");
            domi.barrio = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<localidad>") + "<localidad>".Length;
            iEnd = domicilio.IndexOf("</localidad>");
            domi.localidad = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<municipio>") + "<municipio>".Length;
            iEnd = domicilio.IndexOf("</municipio>");
            domi.municipio = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<provincia>") + "<provincia>".Length;
            iEnd = domicilio.IndexOf("</provincia>");
            domi.provincia = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<pais>") + "<pais>".Length;
            iEnd = domicilio.IndexOf("</pais>");
            domi.pais = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<ubicacion>") + "<ubicacion>".Length;
            iEnd = domicilio.IndexOf("</ubicacion>");
            domi.ubicacion = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<codigo_postal>") + "<codigo_postal>".Length;
            iEnd = domicilio.IndexOf("</codigo_postal>");
            domi.codigo_postal = domicilio.Substring(iStar, iEnd - iStar);

            iStar = domicilio.IndexOf("<UsuarioAltaId>") + "<UsuarioAltaId>".Length;
            iEnd = domicilio.IndexOf("</UsuarioAltaId>");
            string dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.UsuarioAltaId = Convert.ToInt32(dato);
            else domi.UsuarioAltaId = 0;


            iStar = domicilio.IndexOf("<FechaAlta>") + "<FechaAlta>".Length;
            iEnd = domicilio.IndexOf("</FechaAlta>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.FechaAlta = Convert.ToDateTime(dato);

            iStar = domicilio.IndexOf("<ViaId>") + "<ViaId>".Length;
            iEnd = domicilio.IndexOf("</ViaId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.ViaId = Convert.ToInt32(dato);

            iStar = domicilio.IndexOf("<IdLocalidad>") + "<IdLocalidad>".Length;
            iEnd = domicilio.IndexOf("</IdLocalidad>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.IdLocalidad = Convert.ToInt32(dato);

            iStar = domicilio.IndexOf("<ProvinciaId>") + "<ProvinciaId>".Length;
            iEnd = domicilio.IndexOf("</ProvinciaId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.ProvinciaId = Convert.ToInt32(dato);

            iStar = domicilio.IndexOf("<TipoDomicilioId>") + "<TipoDomicilioId>".Length;
            iEnd = domicilio.IndexOf("</TipoDomicilioId>");
            domi.TipoDomicilioId = Convert.ToInt32(domicilio.Substring(iStar, iEnd - iStar));
            personaDomi.TipoDomicilioId = domi.TipoDomicilioId;

            #endregion
            Domicilio DatosDomicilio = new Domicilio
            {
                DomicilioId = domi.DomicilioId,
                ViaId = domi.ViaId,
                ViaNombre = domi.ViaNombre,
                numero_puerta = domi.numero_puerta,
                piso = domi.piso,
                unidad = domi.unidad,
                barrio = domi.barrio,
                provincia = domi.provincia,
                ProvinciaId = domi.ProvinciaId,
                localidad = domi.localidad,
                IdLocalidad = domi.IdLocalidad,
                municipio = domi.municipio,
                pais = domi.pais,
                codigo_postal = domi.codigo_postal,
                UsuarioAltaId = domi.UsuarioAltaId,
                FechaAlta = domi.FechaAlta,
                UsuarioModifId = domi.UsuarioModifId,
                FechaModif = domi.FechaModif,
                UsuarioBajaId = domi.UsuarioBajaId,
                FechaBaja = domi.FechaBaja,
                TipoDomicilioId = domi.TipoDomicilioId,
                ubicacion = domi.ubicacion
            };
            ViewBag.DatosDomicilio = DatosDomicilio;

            string idPais = Convert.ToString(GetIdObjetoByNombreTipo(domi.pais, TiposObjetoAdministrativo.PAIS));
            string idProvincia = Convert.ToString(GetIdObjetoByNombreTipo(domi.provincia, TiposObjetoAdministrativo.PROVINCIA));
            string idPartido = Convert.ToString(GetIdObjetoByNombreTipo(domi.municipio, TiposObjetoAdministrativo.DEPARTAMENTO));
            string idLocalidad = Convert.ToString(GetIdObjetoByNombreTipo(domi.localidad, TiposObjetoAdministrativo.LOCALIDAD));

            ViewBag.ItemPais = idPais;
            ViewBag.ItemProvincia = idProvincia;
            ViewBag.ItemPartido = idPartido;
            ViewBag.ItemLocalidad = idLocalidad;

            ViewData["paises"] = new SelectList(GetPaises(), "Value", "Text");
            ViewData["provincias"] = new SelectList(GetProvincias(idPais), "Value", "Text");
            ViewData["partidos"] = new SelectList(GetDepartamentos(idProvincia), "Value", "Text");
            ViewData["localidades"] = new SelectList(GetLocalidades(idPartido), "Value", "Text");

            ViewData["tiposdomicilio"] = new SelectList(GetTiposDomicilio(), "Value", "Text");

            ViewBag.DescripcionPartido = "Departamento";

            ViewData["viaslista"] = new SelectList(GetViasDefault(), "Value", "Text");
            return PartialView("~/Views/Domicilio/DatosDomicilio.cshtml");
        }

        public List<SelectListItem> GetTiposDomicilio()
        {
            List<SelectListItem> itemsTipos = new List<SelectListItem>();
            foreach (var tipo in GetTiposDomicilios())
            {
                itemsTipos.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoDomicilioId) });
            }
            return itemsTipos;
        }
        public List<TiposDomicilioModel> GetTiposDomicilios()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoDomicilioService/GetTiposDomicilio").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposDomicilioModel>)resp.Content.ReadAsAsync<IEnumerable<TiposDomicilioModel>>().Result;
        }
        public List<DomicilioModel> GetDatosDomicilioByViaNombre(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DomicilioService/GetDatosDomicilioByViaNombre/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<DomicilioModel>)resp.Content.ReadAsAsync<IEnumerable<DomicilioModel>>().Result;
        }

        public DomicilioModel GetDatosDomicilioById(string id)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/DomicilioService/GetDomicilioById/" + id).Result;
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DomicilioModel>().Result;
            }
            catch
            {
                return new DomicilioModel();
            }
        }

        public ActionResult GetDatosDomicilioByIdJson(string id)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/DomicilioService/GetDomicilioById/" + id).Result;
                resp.EnsureSuccessStatusCode();
                return Json(resp.Content.ReadAsAsync<DomicilioModel>().Result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                DomicilioModel dom = new DomicilioModel();
                return Json(dom, JsonRequestBehavior.AllowGet);
            }
        }

        public List<SelectListItem> GetPaises()
        {
            List<SelectListItem> itemsPaises = new List<SelectListItem>();
            foreach (var item in GetObjetosByTipo(TiposObjetoAdministrativo.PAIS))
            {
                itemsPaises.Add(new SelectListItem { Text = item.Nombre, Value = Convert.ToString(item.FeatId) });
            }
            return itemsPaises;
        }

        public List<ObjetoAdministrativoModel> GetObjetosByTipo(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObjetoAdministrativoService/GetObjetoByTipo/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ObjetoAdministrativoModel>)resp.Content.ReadAsAsync<IEnumerable<ObjetoAdministrativoModel>>().Result;
        }

        // Recupera las vias
        public JsonResult GetViasPorNombreJson(string id, string LocaId)
        {
            return Json(GetViasPorNombre(id, LocaId));
        }
        public List<ViaModel> GetViasPorNombre(string id, string LocaId)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync($"api/ViaService/GetViaByNombreYLocalidad?nombre={id}&idLocalidad={LocaId}").Result;
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<IEnumerable<ViaModel>>().Result.ToList();
            }
            catch
            {
                return new List<ViaModel>();
            }
        }


        public JsonResult GetProvinciaPorPadreJson(string id)
        {
            return Json(GetObjetoByIdPadreYTipo(id, TiposObjetoAdministrativo.PROVINCIA));
        }
        public JsonResult GetPartidoPorPadreJson(string id)
        {
            return Json(GetObjetoByIdPadreYTipo(id, TiposObjetoAdministrativo.DEPARTAMENTO));
        }
        public JsonResult GetLocalidadPorPadreJson(string id)
        {
            return Json(GetObjetoByIdPadreYTipo(id, TiposObjetoAdministrativo.LOCALIDAD));
        }
        public List<ObjetoAdministrativoModel> GetObjetoByIdPadreYTipo(string id, string tipo)
        {
            HttpResponseMessage resp = cliente.GetAsync(string.Format("api/ObjetoAdministrativoService/GetObjetoByIdPadreTipo?id={0}&tipo={1}", id, tipo)).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<IEnumerable<ObjetoAdministrativoModel>>().Result.ToList();
        }

        // Recupera los objetos de acuerdo a su padre (objetos provincia, departamento, localidad, etc.)
        public JsonResult GetObjetoPorPadreJson(string id)
        {
            return Json(GetObjetoByIdPadre(id));
        }

        public List<ObjetoAdministrativoModel> GetObjetoByIdPadre(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObjetoAdministrativoService/GetObjetoByIdPadre/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ObjetoAdministrativoModel>)resp.Content.ReadAsAsync<IEnumerable<ObjetoAdministrativoModel>>().Result;
        }

        public JsonResult GetObjetoPorIdJson(string id)
        {
            return Json(GetObjetoById(id));
        }

        public ObjetoAdministrativoModel GetObjetoById(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ObjetoAdministrativoService/GetObjetoById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ObjetoAdministrativoModel)resp.Content.ReadAsAsync<ObjetoAdministrativoModel>().Result;
        }

        public long GetIdObjetoByNombreTipo(string nombre, string tipo)
        {
            ObjetoAdministrativoModel obj = GetObjetoByNombreTipo(nombre, tipo);
            if (obj == null)
                return 0;
            else
                return obj.FeatId;
        }
        public ObjetoAdministrativoModel GetObjetoByNombreTipo(string nombre, string tipo)
        {
            HttpResponseMessage resp = cliente.GetAsync(string.Format("api/ObjetoAdministrativoService/GetObjetoByNombreTipo?nombre={0}&tipo={1}", nombre, tipo)).Result;
            resp.EnsureSuccessStatusCode();
            return (ObjetoAdministrativoModel)resp.Content.ReadAsAsync<ObjetoAdministrativoModel>().Result;
        }


        public List<SelectListItem> GetProvincias(string id)
        {
            var TipoObjetoProvincia = "2";
            List<SelectListItem> itemsProvincias = new List<SelectListItem>();
            foreach (var item in GetObjetoByIdPadreYTipo(id, TipoObjetoProvincia)) //GetObjetosByTipo("2")
            {
                itemsProvincias.Add(new SelectListItem { Text = item.Nombre, Value = Convert.ToString(item.FeatId) });
            }
            return itemsProvincias;
        }

        public List<SelectListItem> GetDepartamentos(string id)
        {
            List<SelectListItem> itemsDepartamentos = new List<SelectListItem>();
            var TipoObjetoPartido = ConfigurationManager.AppSettings["DepartamentoTipoObjeto"];
            foreach (var item in GetObjetoByIdPadreYTipo(id, TipoObjetoPartido)) // GetObjetosByTipo(TipoObjetoPartido)
            {
                itemsDepartamentos.Add(new SelectListItem { Text = item.Nombre, Value = Convert.ToString(item.FeatId) });
            }
            return itemsDepartamentos;
        }

        public List<SelectListItem> GetLocalidades(string id)
        {
            List<SelectListItem> itemsLocalidades = new List<SelectListItem>();
            var TipoObjetoLocalidad = ConfigurationManager.AppSettings["LocalidadTipoObjeto"];
            foreach (var item in GetObjetoByIdPadreYTipo(id, TipoObjetoLocalidad)) //GetObjetosByTipo(TipoObjetoLocalidad)
            {
                itemsLocalidades.Add(new SelectListItem { Text = item.Nombre, Value = Convert.ToString(item.FeatId) });
            }
            return itemsLocalidades;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_DatosDomicilio(DomicilioModels Save_Datos)
        {
            var domicilio = Save_Datos.DatosDomicilio.DomicilioId;
            Save_Datos.DatosDomicilio.DomicilioId = domicilio;
            var esAlta = Save_Datos.DatosDomicilio.DomicilioId > 0;

            //HttpResponseMessage resp = cliente.PostAsJsonAsync("api/DomicilioService/SetDomicilio_Save", Save_Datos.DatosDomicilio).Result;
            //resp.EnsureSuccessStatusCode();
            //var grabado = resp.Content.ReadAsAsync<DomicilioModels>().Result;
            //ViewBag.MensajeSalida = resp.StatusCode.ToString();

            //if (ViewBag.MensajeSalida == "OK")
            //    if (Save_Datos.DatosDomicilio.DomicilioId == 0)
            //    {
            //        ViewBag.MensajeSalida = "AltaOK";
            //    }
            //    else
            //        ViewBag.MensajeSalida = "ModificacionOK";
            //else
            //    ViewBag.MensajeSalida = "Error";

            //ViewBag.DatosDomicilio = GetDatosDomicilioByViaNombre("-1");
            //ViewData["paises"] = new SelectList(GetPaises(), "Value", "Text");
            //ViewData["provincias"] = new SelectList(GetProvincias("1"), "Value", "Text");
            //ViewData["partidos"] = new SelectList(GetDepartamentos("2"), "Value", "Text");
            //ViewData["localidades"] = new SelectList(GetLocalidades("-1"), "Value", "Text");
            //ViewData["tiposdomicilio"] = new SelectList(GetTiposDomicilio(), "Value", "Text");
            return new JsonResult { Data = new { nuevo = esAlta, domicilio = Save_Datos } };
        }

        public List<SelectListItem> GetViasDefault()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "None", Value = "0" });
            return items;
        }

        public DomicilioModel ParseDomicilio(string domiciliopersona)
        {
            DomicilioModel domi = new DomicilioModel();
            var domicilio = domiciliopersona;

            var iStar = domicilio.IndexOf("<ViaNombre>") + "<ViaNombre>".Length;
            var iEnd = domicilio.IndexOf("</ViaNombre>");
            var dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.ViaNombre = dato;

            iStar = domicilio.IndexOf("<DomicilioId>") + "<DomicilioId>".Length;
            iEnd = domicilio.IndexOf("</DomicilioId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "")
            {
                domi.DomicilioId = Convert.ToInt32(dato);
            }
            else
            {
                domi.DomicilioId = 0;
            }

            iStar = domicilio.IndexOf("<numero_puerta>") + "<numero_puerta>".Length;
            iEnd = domicilio.IndexOf("</numero_puerta>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.numero_puerta = dato;

            iStar = domicilio.IndexOf("<piso>") + "<piso>".Length;
            iEnd = domicilio.IndexOf("</piso>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.piso = dato;

            iStar = domicilio.IndexOf("<unidad>") + "<unidad>".Length;
            iEnd = domicilio.IndexOf("</unidad>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.unidad = dato;

            iStar = domicilio.IndexOf("<barrio>") + "<barrio>".Length;
            iEnd = domicilio.IndexOf("</barrio>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.barrio = dato;

            iStar = domicilio.IndexOf("<localidad>") + "<localidad>".Length;
            iEnd = domicilio.IndexOf("</localidad>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.localidad = dato;

            iStar = domicilio.IndexOf("<municipio>") + "<municipio>".Length;
            iEnd = domicilio.IndexOf("</municipio>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.municipio = dato;

            iStar = domicilio.IndexOf("<provincia>") + "<provincia>".Length;
            iEnd = domicilio.IndexOf("</provincia>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.provincia = dato;

            iStar = domicilio.IndexOf("<pais>") + "<pais>".Length;
            iEnd = domicilio.IndexOf("</pais>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.pais = dato;

            iStar = domicilio.IndexOf("<ubicacion>") + "<ubicacion>".Length;
            iEnd = domicilio.IndexOf("</ubicacion>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.ubicacion = dato;

            iStar = domicilio.IndexOf("<codigo_postal>") + "<codigo_postal>".Length;
            iEnd = domicilio.IndexOf("</codigo_postal>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.codigo_postal = dato;

            iStar = domicilio.IndexOf("<UsuarioAltaId>") + "<UsuarioAltaId>".Length;
            iEnd = domicilio.IndexOf("</UsuarioAltaId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.UsuarioAltaId = Convert.ToInt32(dato);
            else domi.UsuarioAltaId = 0;


            iStar = domicilio.IndexOf("<FechaAlta>") + "<FechaAlta>".Length;
            iEnd = domicilio.IndexOf("</FechaAlta>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.FechaAlta = Convert.ToDateTime(dato);

            iStar = domicilio.IndexOf("<UsuarioModifId>") + "<UsuarioModifId>".Length;
            iEnd = domicilio.IndexOf("</UsuarioModifId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.UsuarioModifId = Convert.ToInt32(dato);
            else domi.UsuarioModifId = 0;

            iStar = domicilio.IndexOf("<FechaModif>") + "<FechaModif>".Length;
            iEnd = domicilio.IndexOf("</FechaModif>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.FechaModif = Convert.ToDateTime(dato);

            iStar = domicilio.IndexOf("<UsuarioBajaId>") + "<UsuarioBajaId>".Length;
            iEnd = domicilio.IndexOf("</UsuarioBajaId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.UsuarioBajaId = Convert.ToInt32(dato);
            else domi.UsuarioBajaId = 0;

            iStar = domicilio.IndexOf("<FechaBaja>") + "<FechaBaja>".Length;
            iEnd = domicilio.IndexOf("</FechaBaja>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.FechaBaja = Convert.ToDateTime(dato);

            iStar = domicilio.IndexOf("<ViaId>") + "<ViaId>".Length;
            iEnd = domicilio.IndexOf("</ViaId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.ViaId = Convert.ToInt32(dato);

            /*iStar = domicilio.IndexOf("<IdLocalidad>") + "<IdLocalidad>".Length;
            iEnd = domicilio.IndexOf("</IdLocalidad>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.IdLocalidad = Convert.ToInt32(dato);*/

            /*iStar = domicilio.IndexOf("<ProvinciaId>") + "<ProvinciaId>".Length;
            iEnd = domicilio.IndexOf("</ProvinciaId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            if (dato != "") domi.ProvinciaId = Convert.ToInt32(dato);*/

            iStar = domicilio.IndexOf("<TipoDomicilioId>") + "<TipoDomicilioId>".Length;
            iEnd = domicilio.IndexOf("</TipoDomicilioId>");
            dato = domicilio.Substring(iStar, iEnd - iStar);
            domi.TipoDomicilioId = Convert.ToInt32(dato);

            return domi;
        }


        public JsonResult GetExisteTramoJson(long ViaId, long altura)
        {
            return Json(GetExisteTramo(ViaId, altura));
        }
        public string GetExisteTramo(long ViaId, long altura)
        {
            var codigo = "";
            try
            {
                TramoViaModel tramo;
                HttpResponseMessage resp = cliente.GetAsync(string.Format("api/TramoViaService/GetByViaIdAndAltura?id={0}&altura={1}", ViaId, altura)).Result;
                resp.EnsureSuccessStatusCode();
                tramo = resp.Content.ReadAsAsync<TramoViaModel>().Result;
                if (tramo == null)
                    codigo = "El tramo correspondiente a la Via y altura no existe.";
            }
            catch
            {
                codigo = "El tramo correspondiente a la Via y altura no existe.";
            }
            return codigo;
        }

        public JsonResult GetCpaTramoViaJson(long ViaId, long altura)
        {
            return Json(GetCpaTramoVia(ViaId, altura));
        }

        public string GetCpaTramoVia(long ViaId, long altura)
        {
            var codigo = "";
            try
            {
                TramoViaModel tramo;
                HttpResponseMessage resp = cliente.GetAsync(string.Format("api/TramoViaService/GetByViaIdAndAltura?id={0}&altura={1}", ViaId, altura)).Result;
                resp.EnsureSuccessStatusCode();
                tramo = resp.Content.ReadAsAsync<TramoViaModel>().Result;
                if (tramo != null)
                    codigo = tramo.Cpa;
                else
                {
                    codigo = "";
                }
            }
            catch
            {
                codigo = "";
            }
            return codigo;
        }
    }
}