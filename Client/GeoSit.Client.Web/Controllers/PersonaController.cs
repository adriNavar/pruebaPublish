using System;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;
using System.Linq;

namespace GeoSit.Client.Web.Controllers
{
    public class PersonaController : Controller
    {
        private HttpClient cliente = new HttpClient();

        public PersonaController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }

        // GET: /Persona/Index
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult BuscadorPersona()
        {
            return RedirectToAction("DatosPersona", new { altaBuscador = true });
        }

        // GET: /Persona/DatosPersona
        [ValidateInput(false)]
        public ActionResult DatosPersona(bool altaBuscador = false)
        {
            var model = new PersonaModels();
            ViewBag.DatosPersona = new PersonaModel();
            ViewData["tipospersonas"] = new SelectList(GetTipoPersonas(), "Value", "Text");
            ViewData["tiposdocumentos"] = new SelectList(GetTipoDocumentos(), "Value", "Text");
            ViewData["tipossexo"] = new SelectList(GetTipoSexo(), "Value", "Text");
            ViewData["tiposestadocivil"] = new SelectList(GetTipoEstadoCivil(), "Value", "Text");
            ViewData["tiposnacionalidad"] = new SelectList(GetTipoNacionalidad(), "Value", "Text");
            ViewData["tiposprof"] = new SelectList(GetTipoProfesiones(), "Value", "Text");

            ViewData["esAltaBuscador"] = altaBuscador;

            ViewBag.MensajeSalida = model.Mensaje;
            ViewBag.ListaProfesiones = new List<ProfesionModel>();
            ViewData["tiposdomicilio"] = new SelectList(GetTiposDomicilio(), "Value", "Text");

            return PartialView(model);
        }

        public ActionResult LoadDatosPersona(long id)
        {
            var persona = GetDatosPersonaById(id);
            ViewBag.DatosPersona = persona;

            ViewData["tipospersonas"] = new SelectList(GetTipoPersonas(), "Value", "Text");

            ViewData["tiposdocumentos"] = new SelectList(GetTipoDocumentos(), "Value", "Text");

            ViewData["tipossexo"] = new SelectList(GetTipoSexo(), "Value", "Text");

            ViewData["tiposestadocivil"] = new SelectList(GetTipoEstadoCivil(), "Value", "Text");

            ViewData["tiposnacionalidad"] = new SelectList(GetTipoNacionalidad(), "Value", "Text");

            ViewData["tiposprof"] = new SelectList(GetTipoProfesiones(), "Value", "Text");

            var model = new PersonaModels();
            ViewBag.MensajeSalida = model.Mensaje;
            ViewBag.ListaProfesiones = new List<ProfesionModel>();
            ViewData["tiposdomicilio"] = new SelectList(GetTiposDomicilio(), "Value", "Text");

            return PartialView("~/Views/Persona/DatosPersona.cshtml", model);
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


        public List<PersonaModel> GetDatosPersona()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaService/GetPersonas").Result;
            resp.EnsureSuccessStatusCode();
            return (List<PersonaModel>)resp.Content.ReadAsAsync<IEnumerable<PersonaModel>>().Result;
        }

        public JsonResult GetPersonaByDocumentoJson(string id)
        {
            return Json(GetDatosPersonaByDocumento(id));
        }

        public JsonResult GetPersonasJson(string nombre_completo)
        {
            return Json(GetDatosPersonaByAll(nombre_completo));
        }
        public List<PersonaModel> GetDatosPersonaByNombre(string nombre_completo)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaService/GetDatosPersonaByNombre/" + nombre_completo).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PersonaModel>)resp.Content.ReadAsAsync<IEnumerable<PersonaModel>>().Result;
        }

        public List<PersonaModel> GetDatosPersonaByDocumento(string documento)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaService/GetDatosPersonaByDocumento/" + documento).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PersonaModel>)resp.Content.ReadAsAsync<IEnumerable<PersonaModel>>().Result;
        }
        public List<PersonaModel> GetDatosPersonaByAll(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaService/GetDatosPersonaByAll/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PersonaModel>)resp.Content.ReadAsAsync<IEnumerable<PersonaModel>>().Result;
        }

        public JsonResult GetDatosPersonaJson(long id)
        {
            return Json(GetDatosPersonaById(id));
        }

        public PersonaModel GetDatosPersonaById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaService/GetPersonaById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<PersonaModel>().Result;
        }

        public JsonResult GetDatosProfesionByPersonaJson(long id)
        {
            return Json(GetDatosProfesionByPersona(id));
        }
        public List<ProfesionModel> GetDatosProfesionByPersona(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ProfesionService/GetProfesionByPersona/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ProfesionModel>)resp.Content.ReadAsAsync<IEnumerable<ProfesionModel>>().Result;
        }

        public JsonResult GetDatosDomicilioByPersonaJson(long id)
        {
            var listB = new List<PersonaDomicilioModel>();
            listB = GetDatosDomicilioByPersona(id);
            var domicilios = new List<DomicilioModel>();
            foreach (PersonaDomicilioModel element in listB)
            {
                var dom = GetDatosDomicilioById(element.DomicilioId);
                if (dom != null)
                    domicilios.Add(dom);
            };
            return Json(domicilios);
        }

        public DomicilioModel GetDatosDomicilioById(long id)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/DomicilioService/GetDomicilioById/" + id).Result;
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<DomicilioModel>().Result;
            }
            catch
            {
                return null;
            }
        }
        public List<PersonaDomicilioModel> GetDatosDomicilioByPersona(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaDomicilioService/GetPersonaDomiciliosByPersona/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PersonaDomicilioModel>)resp.Content.ReadAsAsync<IEnumerable<PersonaDomicilioModel>>().Result;
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save_DatosPersona(PersonaModels Save_Datos, List<long> TipoProfesionId, List<string> MatriculaNumero, List<string> DomicilioPersona)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            string machineName;
            try
            {
                machineName = Dns.GetHostEntry(Request.UserHostAddress)?.HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                machineName = Request.UserHostName;
            }
            Save_Datos.DatosPersona.UsuarioModifId = usuario.Id_Usuario;

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/PersonaService/SetPersona_Save", Save_Datos.DatosPersona).Result;
            resp.EnsureSuccessStatusCode();
            var guardado = resp.Content.ReadAsAsync<PersonaModel>().Result;
            ViewBag.MensajeSalida = resp.StatusCode.ToString();

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                if (Save_Datos.DatosPersona.PersonaId == 0)
                {
                    ViewBag.MensajeSalida = "AltaOK";
                }
                else
                {
                    ViewBag.MensajeSalida = "ModificacionOK";
                }
            }
            else
            {
                ViewBag.MensajeSalida = "Error";
            }


            // Graba la información de las profesiones.
            if (TipoProfesionId != null)
            {
                var profesion = new Data.BusinessEntities.Personas.Profesion()
                {
                    PersonaId = guardado.PersonaId,
                    _Id_Usuario = guardado.UsuarioModifId,
                    _Ip = Request.UserHostAddress,
                    _Machine_Name = machineName
                };
                for (int i = 0; i < TipoProfesionId.Count; i++)
                {
                    profesion.TipoProfesionId = TipoProfesionId[i];
                    profesion.Matricula = MatriculaNumero[i];

                    resp = cliente.PostAsJsonAsync("api/ProfesionService/SetProfesion_Save", profesion).Result;
                    resp.EnsureSuccessStatusCode();
                }
            }

            // Determina todos los domicilios.
            var domiciliosGuardados = new HashSet<long>();
            if (DomicilioPersona != null)
            {
                var domi = new Data.BusinessEntities.ObjetosAdministrativos.Domicilio()
                {
                    UsuarioModifId = guardado.UsuarioModifId,
                    _Id_Usuario = guardado.UsuarioModifId,
                    _Ip = Request.UserHostAddress,
                    _Machine_Name = machineName
                };
                var personaDomi = new Data.BusinessEntities.Personas.PersonaDomicilio()
                {
                    PersonaId = guardado.PersonaId,
                    UsuarioModifId = guardado.UsuarioModifId,
                    _Id_Usuario = guardado.UsuarioModifId,
                    _Ip = Request.UserHostAddress,
                    _Machine_Name = machineName
                };
                for (int i = 0; i < DomicilioPersona.Count; i++)
                {
                    var domicilio = DomicilioPersona[i];
                    domicilio = domicilio.Replace("#?", "<");
                    domicilio = domicilio.Replace("?#", ">");
                    domicilio = domicilio.Replace("#?", "<");
                    domicilio = domicilio.Replace("?#", ">");
                    var iStar = domicilio.IndexOf("<ViaNombre>") + "<ViaNombre>".Length;
                    var iEnd = domicilio.IndexOf("</ViaNombre>");
                    var dato = domicilio.Substring(iStar, iEnd - iStar);
                    domi.ViaNombre = dato;

                    iStar = domicilio.IndexOf("<DomicilioId>") + "<DomicilioId>".Length;
                    iEnd = domicilio.IndexOf("</DomicilioId>");
                    dato = domicilio.Substring(iStar, iEnd - iStar);
                    if (dato != "")
                    {
                        personaDomi.DomicilioId = domi.DomicilioId = Convert.ToInt32(dato);
                    }
                    else
                    {
                        personaDomi.DomicilioId = domi.DomicilioId = 0;
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
                    dato = domicilio.Substring(iStar, iEnd - iStar);

                    personaDomi.TipoDomicilioId = domi.TipoDomicilioId = Convert.ToInt32(dato);

                    resp = cliente.PostAsJsonAsync("api/DomicilioService/SetDomicilio_Save", domi).Result;
                    resp.EnsureSuccessStatusCode();
                    personaDomi.DomicilioId = resp.Content.ReadAsAsync<Data.BusinessEntities.ObjetosAdministrativos.Domicilio>().Result.DomicilioId;

                    resp = cliente.PostAsJsonAsync("api/PersonaDomicilioService/SetPersonaDomicilio_Save", personaDomi).Result;
                    resp.EnsureSuccessStatusCode();
                    domiciliosGuardados.Add(personaDomi.DomicilioId);
                }
            }

            // Controla si algún domicilio fue eliminado de la lista para darlo de baja.
            foreach (var element in GetDatosDomicilioByPersona(guardado.PersonaId))
            {
                if (!domiciliosGuardados.Contains(element.DomicilioId))
                {
                    // Elimina relación.
                    var httpMSG = new HttpRequestMessage(HttpMethod.Delete, "api/PersonaDomicilioService/DeletePersonaDomicilio_Save")
                    {
                        Content = new ObjectContent<Data.BusinessEntities.Personas.PersonaDomicilio>(new Data.BusinessEntities.Personas.PersonaDomicilio()
                        {
                            DomicilioId = element.DomicilioId,
                            PersonaId = element.PersonaId,
                            TipoDomicilioId = element.TipoDomicilioId,
                            UsuarioBajaId = guardado.UsuarioModifId,
                            _Id_Usuario = guardado.UsuarioModifId,
                            _Ip = Request.UserHostAddress,
                            _Machine_Name = machineName
                        }, new JsonMediaTypeFormatter())
                    };
                    cliente.SendAsync(httpMSG).Result.EnsureSuccessStatusCode();
                }
            }
            return Json(guardado);
        }

        public JsonResult QuitarPersonaViewJson(string nombre)
        {
            ViewBag.DatosPersona = GetDatosPersonaByNombre(nombre).Concat(GetDatosPersonaByDocumento(nombre));
            return Json("Ok");
        }

        public JsonResult DeletePersonaJson(long id)
        {
            return Json(DeletePersonaById(id));
        }

        public JsonResult DeleteDomicilioJson(long id)
        {
            return Json(DeleteDomicilioById(id));
        }

        public JsonResult DeletePersonaDomicilioJson(long id, long persona, long tipodomicilio)
        {
            return Json(DeletePersonaDomicilioById(id, persona, tipodomicilio));
        }

        public string DeletePersonaById(long id)
        {
            string machineName;
            try
            {
                machineName = Dns.GetHostEntry(Request.UserHostAddress)?.HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                machineName = Request.UserHostName;
            }
            var per = new Data.BusinessEntities.Personas.Persona()
            {
                PersonaId = id,
                _Id_Usuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario,
                _Ip = Request.UserHostAddress,
                _Machine_Name = machineName
            };
            return cliente.PostAsJsonAsync("api/PersonaService/DeletePersona_Save", per).Result.EnsureSuccessStatusCode().StatusCode.ToString();
        }

        public string DeleteDomicilioById(long id)
        {
            string machineName;
            try
            {
                machineName = Dns.GetHostEntry(Request.UserHostAddress)?.HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                machineName = Request.UserHostName;
            }
            var dom = new Data.BusinessEntities.ObjetosAdministrativos.Domicilio()
            {
                DomicilioId = id,
                _Id_Usuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario,
                _Ip = Request.UserHostAddress,
                _Machine_Name = machineName
            };
            var resp = cliente.PostAsJsonAsync("api/DomicilioService/DeleteDomicilio_Save", dom).Result.EnsureSuccessStatusCode();
            return resp.StatusCode.ToString();
        }

        public string DeletePersonaDomicilioById(long id, long persona, long tipodomicilio)
        {
            string machineName;
            try
            {
                machineName = Dns.GetHostEntry(Request.UserHostAddress)?.HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                // Error al recuperar el nombre de la maquina
                machineName = Request.UserHostName;
            }
            var httpMSG = new HttpRequestMessage(HttpMethod.Delete, "api/PersonaDomicilioService/DeletePersonaDomicilio_Save")
            {
                Content = new ObjectContent<Data.BusinessEntities.Personas.PersonaDomicilio>(new Data.BusinessEntities.Personas.PersonaDomicilio()
                {
                    DomicilioId = id,
                    PersonaId = persona,
                    TipoDomicilioId = tipodomicilio,
                    _Id_Usuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario,
                    _Ip = Request.UserHostAddress,
                    _Machine_Name = machineName
                }, new JsonMediaTypeFormatter())
            };
            return cliente.SendAsync(httpMSG).Result.EnsureSuccessStatusCode().StatusCode.ToString();
        }

        public List<TiposProfesionesModel> GetDatosTiposProfesiones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoProfesionService/GetTiposProfesion").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposProfesionesModel>)resp.Content.ReadAsAsync<IEnumerable<TiposProfesionesModel>>().Result;
        }

        public List<NacionalidadModel> GetNacionalidades()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/NacionalidadService/GetNacionalidades").Result;
            resp.EnsureSuccessStatusCode();
            return (List<NacionalidadModel>)resp.Content.ReadAsAsync<IEnumerable<NacionalidadModel>>().Result;
        }

        public JsonResult GetProfesionesJson(long id)
        {
            return Json(GetDatosProfesionById(id));
        }
        public List<ProfesionModel> GetDatosProfesionById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ProfesionService/GetProfesionByPersona/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ProfesionModel>)resp.Content.ReadAsAsync<IEnumerable<ProfesionModel>>().Result;
        }


        //Tipos
        public List<SelectListItem> GetTipoPersonas()
        {
            //ESTO YA ESTA EN LA BASE DE DATOS OJO
            List<SelectListItem> itemsTipos = new List<SelectListItem>();
            itemsTipos.Add(new SelectListItem { Text = "Física", Value = "1", Selected = true });
            itemsTipos.Add(new SelectListItem { Text = "Jurídica", Value = "2" });
            return itemsTipos;
        }
        public List<SelectListItem> GetTipoDocumentos()
        {
            SeguridadController s = new SeguridadController();
            List<TipoDocModel> model = s.GetTipoDoc();

            List<SelectListItem> itemsTiposDoc = new List<SelectListItem>();

            itemsTiposDoc.Add(new SelectListItem { Text = "", Value = "1" });

            foreach (var a in model)
            {
                itemsTiposDoc.Add(new SelectListItem { Text = a.Descripcion, Value = a.Id_Tipo_Doc_Ident.ToString() });
            }

            return itemsTiposDoc;
        }

        public List<SelectListItem> GetTipoSexo()
        {
            List<SelectListItem> itemsTiposSexo = new List<SelectListItem>();
            itemsTiposSexo.Add(new SelectListItem { Text = "Femenino", Value = "1" });
            itemsTiposSexo.Add(new SelectListItem { Text = "Masculino", Value = "2" });
            itemsTiposSexo.Add(new SelectListItem { Text = "Sin Identificar", Value = "3" });
            return itemsTiposSexo;
        }

        public List<SelectListItem> GetTipoEstadoCivil()
        {
            List<SelectListItem> itemsTiposEstadoCivil = new List<SelectListItem>();
            itemsTiposEstadoCivil.Add(new SelectListItem { Text = "Casado/a", Value = "1" });
            itemsTiposEstadoCivil.Add(new SelectListItem { Text = "Separado/a", Value = "2" });
            itemsTiposEstadoCivil.Add(new SelectListItem { Text = "Divorciado/a", Value = "3" });
            itemsTiposEstadoCivil.Add(new SelectListItem { Text = "Viudo/a", Value = "4" });
            itemsTiposEstadoCivil.Add(new SelectListItem { Text = "Soltero/a", Value = "5" });
            itemsTiposEstadoCivil.Add(new SelectListItem { Text = "Sin Identificar", Value = "6" });
            return itemsTiposEstadoCivil;
        }

        public List<SelectListItem> GetTipoNacionalidad()
        {
            List<SelectListItem> itemsTiposNacionalidad = new List<SelectListItem>();
            foreach (var tipoNac in GetNacionalidades())
            {
                if (tipoNac.NacionalidadId == 1)
                {
                    itemsTiposNacionalidad.Add(new SelectListItem { Text = tipoNac.Descripcion, Value = Convert.ToString(tipoNac.NacionalidadId), Selected = true });
                }
                else
                {
                    itemsTiposNacionalidad.Add(new SelectListItem { Text = tipoNac.Descripcion, Value = Convert.ToString(tipoNac.NacionalidadId) });
                }
            }
            return itemsTiposNacionalidad;
        }

        public List<SelectListItem> GetTipoProfesiones()
        {
            List<SelectListItem> itemsTiposProf = new List<SelectListItem>();
            foreach (var tipo in GetDatosTiposProfesiones())
            {
                if (tipo.TipoProfesionId == 1)
                {
                    itemsTiposProf.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId), Selected = true });
                }
                else
                {
                    itemsTiposProf.Add(new SelectListItem { Text = tipo.Descripcion, Value = Convert.ToString(tipo.TipoProfesionId) });
                }
            }
            return itemsTiposProf;
        }

        public ActionResult GenerarReportePersona(long id)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            {
                HttpResponseMessage resp = apiReportes.GetAsync($"api/InformePersona/Get?idPersona={id}&usuario={usuario}").Result;
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;

                var fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformePersona_{fecha}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
                //return AbrirReporte();
            }
        }

        public ActionResult GenerarReporteBienesRegistrados(long id)
        {
            long? idTramite = null;
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            {
                HttpResponseMessage resp = apiReportes.GetAsync($"api/InformeBienesRegistrados/GetInformeBienesRegistrados?idPersona={id}&usuario={usuario}&idTramite={idTramite}").Result;
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;

                var fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeBienesRegistrados{fecha}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
                //return AbrirReporte();
            }
        }

        public FileResult AbrirReporte()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoDescarga.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoDescarga.Contenido, ArchivoDescarga.MimeType);
        }
    }
}
