using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.ValidationRules;
using GeoSit.Data.BusinessEntities.ValidationRules.Parcela;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Controllers
{
    public class NomenclaturaController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private static string _modelParcela = "Parcela";
        public NomenclaturaController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }
        public ActionResult Index()
        {
            ViewData["tiposNomenclaturas"] = GetTiposNomenclaturas();
            return PartialView("Nomenclatura", new Nomenclatura() { TipoNomenclaturaID = 3 });
        }

        [HttpGet]
        public ActionResult Generar(long tipo)
        {
            using (cliente)
            {
                var resp = cliente.GetAsync($"api/nomenclatura/generar?idParcela={(Session[_modelParcela] as Parcela).ParcelaID}&tipo={tipo}").Result;
                resp.EnsureSuccessStatusCode();
                return Content(resp.Content.ReadAsAsync<string>().Result);
            }
        }

        //[HttpPost]
        public ActionResult GetNomenclatura(Nomenclatura nomenclatura)
        {
            var tiposNomenclatura = GetTiposNomenclaturas();
            nomenclatura.Tipo = nomenclatura.Tipo ?? tiposNomenclatura.Single(x => x.TipoNomenclaturaID == 3);
            nomenclatura.TipoNomenclaturaID = nomenclatura.Tipo.TipoNomenclaturaID;
            nomenclatura.FechaModificacion = nomenclatura.FechaModificacion ?? DateTime.Today;

            ViewData["tiposNomenclaturas"] = tiposNomenclatura;

            return PartialView("Nomenclatura", nomenclatura);
        }

        [HttpPost]
        public ActionResult ValidarNomenclatura(Nomenclatura nomenclatura)
        {
            try
            {
                var parcela = (Parcela)Session[_modelParcela];
                var unidadMantenimientoParcelario = (UnidadMantenimientoParcelario)Session["UnidadMantenimientoParcelario"];

                nomenclatura.Tipo = GetNomenclaturaById(nomenclatura.TipoNomenclaturaID);
                unidadMantenimientoParcelario.OperacionesNomenclatura.AnalyzeOperations("NomenclaturaID");

                //Nombre en bd
                using (var resp = cliente.GetAsync("api/nomenclatura/Get?nomenclatura=" + nomenclatura.Nombre).Result)
                {
                    var nomenBD = resp.Content.ReadAsAsync<Nomenclatura>().Result;
                    if (nomenBD != null)
                    {
                        //Nombre en memoria
                        var nomenclaturaOperacion = unidadMantenimientoParcelario.OperacionesNomenclatura
                                                                                 .FirstOrDefault(x => x.Item.Nombre == nomenclatura.Nombre);
                        if (nomenclaturaOperacion != null && nomenclaturaOperacion.Operation != Operation.Remove)
                        {
                            var nomenLst = nomenclaturaOperacion.Item;
                            var errors = FluentValidator<Nomenclatura>.Validate(new NomenclaturaValidator(nomenLst), nomenclatura);
                            if (errors.Any())
                            {
                                return new HttpStatusCodeResult(System.Net.HttpStatusCode.ExpectationFailed, string.Join("<br />", errors));
                            }
                        }
                        else
                        {
                            var errors = FluentValidator<Nomenclatura>.Validate(new NomenclaturaValidator(nomenBD), nomenclatura);
                            if (errors.Any()) return Json(new { OK = false, msg = errors });
                        }
                    }
                    else
                    {
                        //Tipo en BD
                        using (var resp2 = cliente.GetAsync("api/nomenclatura/GetByIdParcelaIdTipoNomenclatura?idParcela=" + parcela.ParcelaID + "&idTipoNomenclatura=" + nomenclatura.TipoNomenclaturaID).Result)
                        {
                            nomenBD = resp2.Content.ReadAsAsync<Nomenclatura>().Result;
                            if (nomenBD != null)
                            {
                                //Nombre en memoria
                                var nomenclaturaOperacion = unidadMantenimientoParcelario.OperacionesNomenclatura
                                                                                         .FirstOrDefault(x => x.Item.Nombre == nomenclatura.Nombre);
                                if (nomenclaturaOperacion != null && nomenclaturaOperacion.Operation != Operation.Remove)
                                {
                                    var nomenLst = nomenclaturaOperacion.Item;
                                    var errors = FluentValidator<Nomenclatura>.Validate(new NomenclaturaValidator(nomenLst), nomenclatura);
                                    if (errors.Any()) return Json(new { OK = false, msg = errors });
                                }
                                else
                                {
                                    var errors = FluentValidator<Nomenclatura>.Validate(new NomenclaturaValidator(nomenBD), nomenclatura);
                                    if (errors.Any()) return Json(new { OK = false, msg = errors });
                                }

                            }
                        }
                    }
                    nomenclatura.FechaModificacion = DateTime.Today;
                    return Json(new { OK = true, nomenclatura });
                }
            }
            catch (Exception ex)
            {
                return Json(new { OK = false, msg = ex.Message });
            }
        }

        private List<TipoNomenclatura> GetTiposNomenclaturas()
        {
            using (var resp = cliente.GetAsync("api/TipoNomenclatura/Get").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<List<TipoNomenclatura>>().Result;
            }
        }
        private TipoNomenclatura GetNomenclaturaById(long tipoNomenclaturaID)
        {
            var resp = cliente.GetAsync("api/TipoNomenclatura/GetById?Id=" + tipoNomenclaturaID).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<TipoNomenclatura>().Result;
        }

        public JsonResult ValidarTipoNomenclatura(long idTipoNomenclatura, string value)
        {
            var tipoNomenclatura = GetNomenclaturaById(idTipoNomenclatura);
            var regex = new Regex(tipoNomenclatura.ExpresionRegular);
            return new JsonResult { Data = regex.IsMatch(value) };
        }
    }
}