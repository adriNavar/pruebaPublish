using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Client.Web.Helpers;
using GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Models.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using Resources;
using SearchViewModel = GeoSit.Client.Web.Models.ExpedientesObras.SearchViewModel;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Net.Mime;
using GeoSit.Data.BusinessEntities.Certificados;

namespace GeoSit.Client.Web.Controllers
{
    public class ExpedienteObraController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private readonly HttpClient _clienteInformes = new HttpClient();
        private const string ApiUri = "api/expedienteobra/";

        private UnidadExpedienteObra UnidadExpedienteObra
        {
            get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; }
            set { Session["UnidadExpedienteObra"] = value; }
        }

        private UsuariosModel Usuario
        {
            get { return Session["usuarioPortal"] as UsuariosModel; }
        }

        public ExpedienteObraController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            _clienteInformes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        public ActionResult Index(long id = 0)
        {
            var searchViewModel = new SearchViewModel { LoadExpediente = id };
            UnidadExpedienteObra = new UnidadExpedienteObra();

            var result = _cliente.GetAsync("api/estadoexpediente/get").Result;
            result.EnsureSuccessStatusCode();
            var estados = (List<EstadoExpediente>)result.Content.ReadAsAsync<IEnumerable<EstadoExpediente>>().Result;

            estados.Insert(0, new EstadoExpediente { EstadoExpedienteId = 0, Descripcion = "(Todos)" });
            searchViewModel.EstadoList = new SelectList(estados, "EstadoExpedienteId", "Descripcion");

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularExpediente).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularExpediente = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularLegajo).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularLegajo = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularChapa).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularChapa = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularExpedienteVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularExpedienteVisible = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularLegajoVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularLegajoVisible = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularChapaVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularChapaVisible = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ActivarAutonumeracionLegajo).Result;
            result.EnsureSuccessStatusCode();
            ViewBag.ActivarAutonumeracionLegajo = result.Content.ReadAsStringAsync().Result;

            ParametrosGenerales pg = GetParametrosGeneralesByNombre("LiquidacionesExternas");
            if (pg != null && pg.Valor == "1")
            {
                ViewBag.liquidacionExterna = true;
            }
            else
            {
                ViewBag.liquidacionExterna = false;
            }
            return PartialView("~/Views/ExpedientesObras/Index.cshtml", searchViewModel);
        }

        public string LoadExpediente(long id)
        {
            var result = _cliente.GetAsync("api/expedienteobra/getloadexpediente/" + id).Result;
            result.EnsureSuccessStatusCode();
            var expedientes = result.Content.ReadAsAsync<IEnumerable<ExpedienteObra>>().Result;

            return "{\"data\":" + JsonConvert.SerializeObject(expedientes) + "}";
        }

        public string SearchAll(long unidadTributariaId, string numeroLegajoIni, string numeroLegajoFin,
            string numeroExpedienteIni, string numeroExpedienteFin, string fechaLegajoIni, string fechaLegajoFin,
            string fechaExpedienteIni, string fechaExpedienteFin, long personaId, long estadoId)
        {
            var parameters = string.Format("?unidadTributariaId={0}&numeroLegajoIni={1}&numeroLegajoFin={2}&" +
                "numeroExpedienteIni={3}&numeroExpedienteFin={4}&fechaLegajoIni={5}&fechaLegajoFin={6}&" +
                "fechaExpedienteIni={7}&fechaExpedienteFin={8}&personaId={9}&estadoId={10}",
                unidadTributariaId, numeroLegajoIni, numeroLegajoFin, numeroExpedienteIni, numeroExpedienteFin,
                fechaLegajoIni, fechaLegajoFin, fechaExpedienteIni, fechaExpedienteFin, personaId, estadoId);

            var result = _cliente.GetAsync(ApiUri + "getsearch" + parameters).Result;
            result.EnsureSuccessStatusCode();
            var expedientes = result.Content.ReadAsAsync<IEnumerable<ExpedienteObra>>().Result;

            var a = "{\"data\":" + JsonConvert.SerializeObject(expedientes) + "}";
            return a;
        }

        public JsonResult GetNumeroLegajoSiguiente()
        {
            var result = _cliente.GetAsync("api/expedienteobra/getnumerolegajosiguiente").Result;
            result.EnsureSuccessStatusCode();
            return new JsonResult { Data = result.Content.ReadAsStringAsync().Result };
        }

        public JsonResult SetNumeroLegajoSiguiente()
        {
            var result = _cliente.GetAsync("api/expedienteobra/setnumerolegajosiguiente").Result;
            result.EnsureSuccessStatusCode();
            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Save(ExpedienteObraViewModel expedienteObraViewModel)
        {
            var operacion = Operation.Add;

            var expedienteObra = new ExpedienteObra
            {
                ExpedienteObraId = expedienteObraViewModel.ExpedienteObraId,
                NumeroExpediente = expedienteObraViewModel.NumeroExpediente,
                FechaExpediente = expedienteObraViewModel.FechaExpediente != null ?
                DateTime.Parse(expedienteObraViewModel.FechaExpediente) : (DateTime?)null,
                NumeroLegajo = expedienteObraViewModel.NumeroLegajo,
                FechaLegajo = expedienteObraViewModel.FechaLegajo != null ?
                DateTime.Parse(expedienteObraViewModel.FechaLegajo) : (DateTime?)null,
                PlanId = expedienteObraViewModel.PlanId,
                UsuarioAltaId = Usuario.Id_Usuario,
                FechaAlta = DateTime.Now,
                UsuarioModificacionId = Usuario.Id_Usuario,
                FechaModificacion = DateTime.Now
            };

            expedienteObra.AttributosCreate(expedienteObraViewModel.EnPosesion, expedienteObraViewModel.Chapa, expedienteObraViewModel.Ph, expedienteObraViewModel.PermisosProvisorios);

            ExpedienteObra expedienteGuardado = null;

            if (expedienteObra.ExpedienteObraId > 0)
            {
                operacion = Operation.Update;
                expedienteGuardado = GetExpedienteObra(expedienteObra.ExpedienteObraId);
            }

            UnidadExpedienteObra.OperacionExpedienteObra = new OperationItem<ExpedienteObra> { Item = expedienteObra, Operation = operacion };

            using (var result = _cliente.PostAsync(ApiUri + "/validate", new ObjectContent<UnidadExpedienteObra>(UnidadExpedienteObra, new JsonMediaTypeFormatter())).Result)
            {
                result.EnsureSuccessStatusCode();
                string response = result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(response))
                {
                    return new JsonResult { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }

            using (var result = _cliente.PostAsync(ApiUri + "/saveall", new ObjectContent<UnidadExpedienteObra>(UnidadExpedienteObra, new JsonMediaTypeFormatter())).Result)
            {
                try
                {
                    result.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    return new JsonResult { Data = result.Content.ReadAsAsync<string>().Result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }


                var aux = result.Content.ReadAsAsync<ExpedienteObraInspecciones>().Result;

                if (!string.IsNullOrEmpty(aux.Inspecciones))
                {
                    string numero;
                    int tipo;

                    if (string.IsNullOrEmpty(expedienteObra.NumeroExpediente))
                    {
                        numero = expedienteGuardado.NumeroLegajo;
                        tipo = 1;
                    }
                    else
                    {
                        numero = expedienteObra.NumeroExpediente;
                        tipo = 2;
                    }
                    var updateRelacion = _cliente.GetAsync("api/Inspeccion/UpdateRelacion?inspecciones=" + aux.Inspecciones + "&numero=" + numero + "&tipo=" + tipo).Result;
                    updateRelacion.EnsureSuccessStatusCode();
                }

                UnidadExpedienteObra = new UnidadExpedienteObra();

                AuditoriaHelper.Register(Usuario.Id_Usuario,
                operacion == Operation.Add ? "Alta Expedientes Obra" : "Modificacion Expedientes Obra",
                Request,
                operacion == Operation.Add ? TiposOperacion.Alta : TiposOperacion.Modificacion,
                Autorizado.Si,
                operacion == Operation.Add ? Eventos.AltaExpedientesObra : Eventos.ModificacionExpedientesObra, 
                "Expediente de Obra",
                operacion == Operation.Add ? JsonConvert.SerializeObject(expedienteObra) : JsonConvert.SerializeObject(expedienteGuardado),
                operacion == Operation.Add ? string.Empty : JsonConvert.SerializeObject(expedienteObra));

                return new JsonResult { Data = new { resultado = "Ok", expId = aux.idExpediente }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        private ExpedienteObra GetExpedienteObra(long id)
        {
            var result = _cliente.GetAsync(ApiUri + "/getexpediente/" + id).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<ExpedienteObra>().Result;
        }

        public JsonResult CancelAll()
        {
            UnidadExpedienteObra = new UnidadExpedienteObra();
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long id)
        {
            var uri = "api/expedienteobra/delete?idExpedienteObra=" + id +
                      "&idUsuarioBaja=" + Usuario.Id_Usuario;
            var response = _cliente.DeleteAsync(uri).Result;
            response.EnsureSuccessStatusCode();

            AuditoriaHelper.Register(Usuario.Id_Usuario, "Delete Expedientes Obra", Request, TiposOperacion.Baja, Autorizado.Si, Eventos.BajaExpedientedeObra, "Expediente de Obra");

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult InformeExpedienteObra()
        {
            var searchViewModel = new SearchViewModel { LoadExpediente = 0 };
            UnidadExpedienteObra = new UnidadExpedienteObra();

            var result = _cliente.GetAsync("api/estadoexpediente/get").Result;
            result.EnsureSuccessStatusCode();

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularExpediente).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularExpediente = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularLegajo).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularLegajo = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularExpedienteVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularExpedienteVisible = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularLegajoVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularLegajoVisible = result.Content.ReadAsStringAsync().Result;

            return PartialView(searchViewModel);
        }

        public ActionResult InformeExpedienteObraDetallado()
        {
            var searchViewModel = new SearchViewModel { LoadExpediente = 0 };
            UnidadExpedienteObra = new UnidadExpedienteObra();

            var result = _cliente.GetAsync("api/estadoexpediente/get").Result;
            result.EnsureSuccessStatusCode();

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularExpediente).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularExpediente = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularLegajo).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularLegajo = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularExpedienteVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularExpedienteVisible = result.Content.ReadAsStringAsync().Result;

            result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ExpresionRegularLegajoVisible).Result;
            result.EnsureSuccessStatusCode();
            searchViewModel.ExpresionRegularLegajoVisible = result.Content.ReadAsStringAsync().Result;

            return PartialView(searchViewModel);
        }

        public ActionResult GenerateInformeExpedienteObra(string numero, string tipoBusqueda)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var resp = _clienteInformes.GetAsync("api/InformeExpedienteObra/GetExpediente?id=" + numero + "&tipo=" + tipoBusqueda + "&usuario=" + usuario).Result;

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msg = resp.Content.ReadAsStringAsync().Result;
                return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
            }

            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            Session["SeguimientodeExpedientedeObra.pdf"] = bytes;

            return Json(new { success = true, file = "SeguimientodeExpedientedeObra.pdf" }, JsonRequestBehavior.AllowGet);
        }

        //informe expediente obra detallado
        public ActionResult GenerateInformeExpedienteObraDetallado(string numero, string tipoBusqueda)
        {
            string usuario = $"{((GeoSit.Client.Web.Models.UsuariosModel)Session["usuarioPortal"]).Nombre} {((GeoSit.Client.Web.Models.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            var resp = _clienteInformes.GetAsync("api/InformeExpedienteObraDetallado/GetInformeDetallado?id=" + numero + "&tipo=" + tipoBusqueda + "&usuario=" + usuario).Result;
            if (!resp.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);
            }
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            Session["InformeExpedientedeObraDetallado.pdf"] = bytes;

            return Json(new { success = true, file = "InformeExpedientedeObraDetallado.pdf" }, JsonRequestBehavior.AllowGet);

            /*var cd = new ContentDisposition
            {
                FileName = "InformeExpedienteObraDetallado.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "Application/pdf");*/
        }

        //informe certificado catastral
        //funcion sin el insert del registro - borrar funcion
        public ActionResult GenerateInformeCertificadoCatastral(long id)
        {
            string usuario = $"{((GeoSit.Client.Web.Models.UsuariosModel)Session["usuarioPortal"]).Nombre} {((GeoSit.Client.Web.Models.UsuariosModel)Session["usuarioPortal"]).Apellido}";
            //var resp = _clienteInformes.GetAsync("api/InformeCertificadoCatastral/GetInforme?id=" + id).Result;
            long? nTramite = null;
            var resp = _clienteInformes.GetAsync("api/InformeCertificadoCatastral/GetInforme?id=" + id + "&usuario=" + usuario + "&tramite=" + nTramite).Result;

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msg = resp.Content.ReadAsStringAsync().Result;
                return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
            }
            /*if (!resp.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);
            }*/
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            Session["InformeCertificadoCatastral.pdf"] = bytes;

            var cd = new ContentDisposition
            {
                FileName = "CertificadoCatastral.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());

            //return Json(new { success = true, file = "InformeCertificadoCatastral.pdf" }, JsonRequestBehavior.AllowGet);
            return File(bytes, "Application/pdf");
        }

        public ActionResult GenerateInformeCertificadoCatastral2(/*string numero,*/ string fechaemision, string motivo, string vigencia, string descripcion, string observaciones, string planoMensura, /*string planoVep,*/ string solicitante, string partida)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            long idUsuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;
            long? tramite = null;

            /*
            hacer el insert
            */
            var certificadoCatastral = new INMCertificadoCatastral()
            {
                //Numero = numero,
                FechaEmision = Convert.ToDateTime(fechaemision),
                Motivo = motivo,
                Vigencia = Convert.ToInt32(vigencia),
                Descripcion = descripcion,
                Observaciones = observaciones,
                FechaAlta = DateTime.Now,
                FechaModif = DateTime.Now,
                UsuarioAltaId = idUsuario,
                UsuarioModifId = idUsuario,

                MensuraId = Convert.ToInt64(planoMensura),
                //MensuraVepId = string.IsNullOrEmpty(planoVep) ? null : (long?)Convert.ToInt64(planoVep),
                SolicitanteId = Convert.ToInt64(solicitante),
                UnidadTributariaId = Convert.ToInt64(partida)
            };

            string id;
            using (var result = _cliente.PostAsync(ApiUri + "/InsertCertificadoCatastral", new ObjectContent<INMCertificadoCatastral>(certificadoCatastral, new JsonMediaTypeFormatter())).Result)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return Json(new { error = true, mensaje = result.Content.ReadAsStringAsync().Result }, JsonRequestBehavior.AllowGet);
                }
                id = result.Content.ReadAsStringAsync().Result;
            }
            ////// OBTIENE EL INFORME
            var resp = _clienteInformes.GetAsync("api/InformeCertificadoCatastral/GetInforme?id=" + id + "&usuario=" + usuario + "&tramite=" + tramite).Result;

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return Json(new { error = true, mensaje = resp.Content.ReadAsStringAsync().Result }, JsonRequestBehavior.AllowGet);
            }

            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            Session["InformeCertificadoCatastral.pdf"] = bytes;
            return Json(new { file = "InformeCertificadoCatastral.pdf" });
        }

        public ActionResult CertificadoCatastral()
        {
            //agregar en viewbags todos los combobox
            //ejemplo
            //var result = _cliente.GetAsync("api/Parametro/GetValor?id=" + @Recursos.ActivarAutonumeracionLegajo).Result;
            //result.EnsureSuccessStatusCode();
            //ViewBag.ActivarAutonumeracionLegajo = result.Content.ReadAsStringAsync().Result;
            var planomenusra = new List<string>()
                    {
                        "ejemplo1",
                        "ejemplo2"
                    };
            var planovep = new List<string>()
                    {
                        "ejemplo1",
                        "ejemplo2"
                    };
            var solicitante = new List<string>()
                    {
                        "ejemplo1",
                        "ejemplo2"
                    };
            var partida = new List<string>()
                    {
                        "ejemplo1",
                        "ejemplo2"
                    };


            ViewBag.PlanoMensura = planomenusra;
            ViewBag.PanoVep = planovep;
            ViewBag.Solicitante = solicitante;
            ViewBag.Partida = partida;

            return PartialView(new INMCertificadoCatastral());
        }

        public ActionResult GetFileCertificadoCatastral(string file)
        {
            byte[] bytes = Session[file] as byte[];
            if (bytes == null)
                return new EmptyResult();
            Session[file] = null;

            var cd = new ContentDisposition
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


        public ActionResult GetFileInformeExpedienteObra(string file)
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
            //return File(bytes, "application/pdf", file);
        }

        public ParametrosGenerales GetParametrosGeneralesByNombre(string nombre)
        {
            HttpResponseMessage resp = _cliente.GetAsync("api/Liquidacion/GetParametrosGeneralesByNombre/" + nombre).Result;
            resp.EnsureSuccessStatusCode();
            return (ParametrosGenerales)resp.Content.ReadAsAsync<ParametrosGenerales>().Result;
        }

        public string ShowActas()
        {
            var result = _cliente.GetAsync("api/parametro/getvalor/" + Recursos.ExpedientesObraMostrarActas).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsStringAsync().Result;
        }
    }
}