using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Client.Web.Models.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{    
    public class PersonaExpedienteObraController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/personaexpedienteobra/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }

        public PersonaExpedienteObraController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public void ClearId()
        {
        }

        public string SearchPersona(string nombre)
        {
            if (nombre.Trim().Length == 0) return "{\"data\":[]}";

            var result = _cliente.GetAsync("api/persona/get?nombre=" + nombre).Result;
            result.EnsureSuccessStatusCode();
            var personas = result.Content.ReadAsAsync<IEnumerable<PersonaExpedienteRolDomicilio>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(personas) + "}";
        }

        public JsonResult GetPersona(long id)
        {
            using (var result = _cliente.GetAsync("api/persona/get/" + id).Result)
            {
                result.EnsureSuccessStatusCode();
                var persona = result.Content.ReadAsAsync<PersonaExpedienteRolDomicilio>().Result;
                return Json(persona);
            }
        }

        public JsonResult GetPersonaDatos(long id)
        {
            var result = _cliente.GetAsync("api/persona/getdatos/" + id).Result;
            result.EnsureSuccessStatusCode();
            var persona = result.Content.ReadAsAsync<Persona>().Result;

            return Json(persona);
        }

        public string List(long id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync(ApiUri + "get/" +  id).Result;
            result.EnsureSuccessStatusCode();
            var personaExpedienteRolDomicilio = result.Content.ReadAsAsync<IEnumerable<PersonaExpedienteRolDomicilio>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(personaExpedienteRolDomicilio) + "}";
        }

        public ActionResult FormContent()
        {
            var persona = new PersonaViewModel();

            var result = _cliente.GetAsync("api/rol/get").Result;
            result.EnsureSuccessStatusCode();
            var roles = result.Content.ReadAsAsync<IEnumerable<Rol>>().Result;

            persona.RolList = new SelectList(roles, "RolId", "Descripcion");

            return PartialView("~/Views/ExpedientesObras/Partial/_Persona.cshtml", persona);
        }

        public JsonResult Save(PersonaViewModel personaViewModel)
        {
            var personaExpedienteObra = new PersonaExpedienteObra
            {
                ExpedienteObraId = ExpedienteObraId,
                PersonaInmuebleId = personaViewModel.IdPersona,
                RolId = personaViewModel.IdRol,
                UsuarioAltaId = 1,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = 1,
                FechaModificacion = DateTime.Now
            };
            #region Validaciones
            var result = _cliente.PostAsync(ApiUri + string.Format("validate?idExpedienteObra={0}&idPersona={1}&idRol={2}",
                                                                    personaExpedienteObra.ExpedienteObraId,
                                                                    personaExpedienteObra.PersonaInmuebleId,
                                                                    personaExpedienteObra.RolId),
                                            new ObjectContent<Operaciones<PersonaExpedienteObra>>(UnidadExpedienteObra.OperacionesPersonas, new JsonMediaTypeFormatter())).Result;
            result.EnsureSuccessStatusCode();
            var response = result.Content.ReadAsStringAsync().Result.ToStringOrDefault();
            #endregion
            if (string.IsNullOrEmpty(response))
            {
                UnidadExpedienteObra.OperacionesPersonas.Add(new OperationItem<PersonaExpedienteObra> { Operation = Operation.Add, Item = personaExpedienteObra });
                return new JsonResult { Data = "Ok" };
            }
            else
            {
                return new JsonResult { Data = response };
            }
        }

        public JsonResult Delete(long idPersona, int idRol)
        {
            var personaDefault = new PersonaExpedienteObra { ExpedienteObraId = ExpedienteObraId, PersonaInmuebleId = idPersona, RolId = idRol };
            var uri = string.Format(ApiUri + "getpersonabyrolexpedienteobra?idExpedienteObra={0}&idPersona={1}&idRol={2}", ExpedienteObraId, idPersona, idRol);
            var result = _cliente.GetAsync(uri).Result;
            result.EnsureSuccessStatusCode();
            var existente = result.Content.ReadAsAsync<PersonaExpedienteObra>().Result;
            UnidadExpedienteObra.OperacionesPersonas.Add(new OperationItem<PersonaExpedienteObra> { Operation = Operation.Remove, Item = existente ?? personaDefault });
            return new JsonResult { Data = "Ok" };
        }
    }
}