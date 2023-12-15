using GeoSit.Client.Web.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Seguridad;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;
using System.Linq;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Client.Web.Helpers;

namespace GeoSit.Client.Web.Controllers
{
    public class UnidadTributariaController : Controller
    {
        private readonly HttpClient _client = new HttpClient();

        public UnidadTributariaController()
        {
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }

        // GET: /UnidadTributaria/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUnidadTributaria(UnidadTributaria ut)
        {
            var parcela = Session["Parcela"] as Parcela;
            var uts = parcela.UnidadesTributarias.OrderBy(x => x.TipoUnidadTributariaID);
            var utActual = uts.SingleOrDefault(x => x.UnidadTributariaId == ut.UnidadTributariaId);
            if (utActual == null)
            {
                ut.JurisdiccionID = uts.FirstOrDefault()?.JurisdiccionID ?? 0;
                ut.FechaAlta = ut.FechaAlta ?? DateTime.Now;
                ut.TipoUnidadTributariaID = uts.Any(x => x.TipoUnidadTributariaID == 2) ? 3 : 1;
                utActual = ut;
            }

            utActual.ParcelaID = parcela.ParcelaID;

            ViewData["TiposUnidadTributaria"] = GetTiposUnidadTributaria();

            ViewData["Jurisdicciones"] = GetJurisdicciones();

            ViewData["TipoParcelaId"] = parcela.TipoParcelaID;

            return PartialView("UnidadTributaria", utActual);
        }

        private List<SelectListItem> GetTiposUnidadTributaria()
        {
            var parcela = Session["Parcela"] as Parcela;
            using (var response = _client.GetAsync("api/TipoUnidadTributaria/Get").Result)
            {
                response.EnsureSuccessStatusCode();
                var tiposParcelas = response.Content.ReadAsAsync<ICollection<TipoUnidadTributaria>>().Result;
                if(parcela.ClaseParcelaID == 6)
                {
                    tiposParcelas.First(a => a.TipoUnidadTributariaID == 3).Descripcion = "UP de CI";
                    tiposParcelas.First(a => a.TipoUnidadTributariaID == 2).Descripcion = "CI";
                }
                return tiposParcelas.Select(tut => new SelectListItem { Text = tut.Descripcion, Value = tut.TipoUnidadTributariaID.ToString() }).ToList();
            }
        }
        private List<SelectListItem> GetJurisdicciones()
        {
            using (var resp = _client.GetAsync($"api/ObjetoAdministrativoService/GetObjetoByTipo/{Convert.ToInt64(TiposObjetoAdministrativo.JURISDICCION)}").Result)
            {
                resp.EnsureSuccessStatusCode();
                var jurisdicciones = resp.Content.ReadAsAsync<ICollection<ObjetoAdministrativoModel>>().Result;
                return jurisdicciones
                        .OrderBy(jur => jur.Codigo)
                        .Select(jur => new SelectListItem { 
                            Text = $"{jur.Codigo} - {jur.Nombre}", 
                            Value = jur.FeatId.ToString() 
                        })
                        .ToList();
            }
        }

        public JsonResult GetPorcentajeCopropiedadTotal(long idParcela)
        {
            var result = _client.GetAsync("api/unidadtributaria/getporcentajecopropiedadbyparcela?idParcela=" + idParcela).Result;
            result.EnsureSuccessStatusCode();
            var porcentajeCopropiedadTotal = result.Content.ReadAsAsync<decimal>().Result;
            return new JsonResult { Data = porcentajeCopropiedadTotal };
        }

        public string GetRegularExpression()
        {
            var result = _client.GetAsync("api/unidadtributaria/getregularexpression").Result;
            result.EnsureSuccessStatusCode();
            var listadoExp = result.Content.ReadAsAsync<IEnumerable<ParametrosGenerales>>().Result;

            return /*"{\"data\":" +*/ JsonConvert.SerializeObject(listadoExp) /*+ "}"*/;
        }

        public string ValidarPartida(string CodigoProvincial, long IdUnidadTributaria)
        {
            CodigoProvincial = (CodigoProvincial ?? string.Empty).ToUpper();
            var CodigoReceptoria = CodigoProvincial.Substring(0, 2);
            var utsParcela = (Session["Parcela"] as Parcela).UnidadesTributarias.Where(ut => ut.CodigoProvincial == CodigoProvincial);
            var unidadTributaria = (Session["Parcela"] as Parcela).UnidadesTributarias.FirstOrDefault(x => x.UnidadTributariaId == IdUnidadTributaria);
            var utParcelaPHCodigoReceptoria = string.Empty;
            if (unidadTributaria != null)
            {
                if (unidadTributaria.TipoUnidadTributariaID != 1)
                {
                    utParcelaPHCodigoReceptoria = (Session["Parcela"] as Parcela).UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 2).FirstOrDefault().CodigoProvincial.Substring(0, 2);
                }
            }

            bool valida = false;
            string mensaje = "";
            if (utsParcela.Any())
            {
                valida = utsParcela.All(ut => ut.UnidadTributariaId == IdUnidadTributaria);
                mensaje = "El Código Provincial ya existe.";

            }
            else if (unidadTributaria != null)
            {
                if (CodigoReceptoria != utParcelaPHCodigoReceptoria && unidadTributaria.TipoUnidadTributariaID == 3)
                {
                    valida = false;
                    mensaje = "El Código Provincial posee un código de localidad y receptoría diferente al de la unidad tributaria PH.";
                }
                else if (CodigoReceptoria == utParcelaPHCodigoReceptoria && unidadTributaria.TipoUnidadTributariaID == 3)
                {
                    valida = true;
                }
            }
            else
            {
                using (_client)
                using (var resp = _client.GetAsync($"api/UnidadTributaria/GetPartidaDisponible?idUnidadTributaria={IdUnidadTributaria}&partida={CodigoProvincial}").Result)
                {
                    valida = resp.IsSuccessStatusCode;
                }
            }
            return JsonConvert.SerializeObject(new { valid = valida, message = mensaje });// $"{{\"valid\":{()}}}";
        }

        public ActionResult GenerarReportePropiedad(long idUnidadTributaria)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            using (var resp = apiReportes.GetAsync($"api/InformePropiedad/Get?idUnidadTributaria={idUnidadTributaria}&usuario={usuario}").Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformePropiedad);
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeCatastral_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GenerarReporteHistoricoTitulares(long idUnidadTributaria)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            using (var resp = apiReportes.GetAsync($"api/InformeTitulares/Get?idUnidadTributaria={idUnidadTributaria}&usuario={usuario}").Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                AuditoriaHelper.Register(((UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeHistoricoTitulares);
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result), $"InformeHistoricoTitulares{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GenerarPartida(long jurisdiccion, int tipo, bool stripped = false)
        {
            try
            {
                using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
                using (var resp = cliente.GetAsync($"api/unidadtributaria/partida/jurisdiccion/{jurisdiccion}/tipo/{tipo}").Result.EnsureSuccessStatusCode())
                {
                    string partida = resp.Content.ReadAsAsync<string>().Result;
                    if (stripped)
                    {
                        partida = partida.Substring(2, partida.Length - 3);
                    }
                    return Content(partida);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

        }

        public FileResult AbrirReporte()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoDescarga.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoDescarga.Contenido, ArchivoDescarga.MimeType);
        }
    }
}