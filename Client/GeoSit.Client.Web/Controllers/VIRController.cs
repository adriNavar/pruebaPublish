using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using System;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Collections.Generic;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using System.Linq;

namespace GeoSit.Client.Web.Controllers
{
    public class VIRController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private UnidadMantenimientoParcelario UnidadMantenimientoParcelario { get { return (UnidadMantenimientoParcelario)Session["UnidadMantenimientoParcelario"]; } }
        public VIRController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        #region VIR
        public ActionResult GetVIR(string idInmueble)
        {
            try
            {
                using (var result = cliente.GetAsync($"api/vir/get?idinmueble={idInmueble}").Result)
                {
                    result.EnsureSuccessStatusCode();
                    var res = result.Content.ReadAsAsync<List<VIRValuacion>>().Result;
                    return Json(new { data = res }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult VIREdit(long idInmueble)
        {
            VIRInmueble model = UnidadMantenimientoParcelario.OperacionesVIR.SingleOrDefault()?.Item ?? GetVirDetalle(idInmueble);

            var estados = GetEstados();
            string mejoraEstado = string.IsNullOrEmpty(model.MejoraEstado) ? "SIN IDENTIFICAR" : model.MejoraEstado;
            List<SelectListItem> lstEstados = estados.ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Text = x.Descripcion,
                    Value = x.Descripcion,
                    Selected = string.Compare(x.Descripcion, mejoraEstado, true) == 0
                };
            });
            ViewBag.Estados = lstEstados;

            var usos = GetUsos();
            int tipoUso = usos.Find(x => x.Uso == model.MejoraUsoVIR).IdUso;
            List<SelectListItem> lstUsos = usos.ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Text = x.Descripcion,
                    Value = x.Uso.ToString(),
                    Selected = x.IdUso == tipoUso
                };
            });
            ViewBag.Usos = lstUsos;


            var tipoEdif = GetTipoEdif(tipoUso);
            int.TryParse(model.MejoraTipoVIR, out int idTipoEdif);
            List<SelectListItem> lstTipoEdif = tipoEdif.ConvertAll(x =>
            {
                return new SelectListItem()
                {
                    Text = x.TipoDescripcion,
                    Value = x.IdTipo.ToString(),
                    Selected = idTipoEdif == x.IdTipo
                };
            });


            List<SelectListItem> lsttipoEdifEquiv = new List<SelectListItem>();
            if ((model.MejoraUsoVIR == null) || (model.MejoraTipoVIR == null))
            {
                var tipoEdifEquiv = GetEquivInm();

                if (model.MejoraUsoVIR != null)
                {
                    List<VIREquivInmDestinosMejoras> TipoEdifUso = new List<VIREquivInmDestinosMejoras>();
                    TipoEdifUso = tipoEdifEquiv.FindAll(x => x.VIRUso.Equals(model.MejoraUsoVIR));

                    lsttipoEdifEquiv = TipoEdifUso.ConvertAll(x =>
                    {
                        return new SelectListItem()
                        {
                            Text = x.DCGDescripcion,
                            Value = x.Id.ToString(),
                            Selected = false
                        };
                    });
                }
                else
                {
                    lsttipoEdifEquiv = tipoEdifEquiv.ConvertAll(x =>
                    {
                        return new SelectListItem()
                        {
                            Text = x.DCGDescripcion,
                            Value = x.Id.ToString(),
                            Selected = false
                        };
                    });
                }
            }

            if (lsttipoEdifEquiv.Count != 0)
            {
                ViewBag.TipoEdif = lsttipoEdifEquiv;
            }
            else
            {
                ViewBag.TipoEdif = lstTipoEdif;
            }


            return PartialView("VIREdit", model);
        }
        #endregion

        private VIRInmueble GetVirDetalle(long idInmueble)
        {
            using (var result = cliente.GetAsync("api/vir/getVirDetalle?idinmueble=" + idInmueble).Result)
            {
                result.EnsureSuccessStatusCode();
                var res = result.Content.ReadAsAsync<VIRInmueble>().Result;
                return res;
            }
        }

        private List<VIRVbEuCoefEstado> GetEstados()
        {
            using (var result = cliente.GetAsync("api/vir/getVirEstados").Result)
            {
                result.EnsureSuccessStatusCode();
                var res = result.Content.ReadAsAsync<List<VIRVbEuCoefEstado>>().Result;
                return res;
            }
        }

        private List<VIRVbEuUsos> GetUsos()
        {
            using (var result = cliente.GetAsync("api/vir/GetVirUsos").Result)
            {
                result.EnsureSuccessStatusCode();
                var res = result.Content.ReadAsAsync<List<VIRVbEuUsos>>().Result;
                return res;
            }
        }

        private VIRVbEuUsos GetUsoByUso(string uso)
        {
            using (var result = cliente.GetAsync("api/vir/GetVirUsoByUso?uso=" + uso).Result)
            {
                result.EnsureSuccessStatusCode();
                var res = result.Content.ReadAsAsync<VIRVbEuUsos>().Result;
                return res;
            }
        }

        private List<VIRVbEuTipoEdif> GetTipoEdif(int tipo)
        {
            using (var result = cliente.GetAsync("api/vir/GetTipoEdif?tipo=" + tipo).Result)
            {
                result.EnsureSuccessStatusCode();
                var res = result.Content.ReadAsAsync<List<VIRVbEuTipoEdif>>().Result;
                return res;
            }
        }

        public JsonResult GetTipoEdifJson(string uso)
        {
            var usoId = GetUsoByUso(uso);
            var id = usoId.IdUso;
            var tipoEdif = GetTipoEdif(id);

            return Json(tipoEdif);
        }

        private List<VIREquivInmDestinosMejoras> GetEquivInm()
        {
            using (var result = cliente.GetAsync("api/vir/GetVIREquivInmDestinosMejoras").Result)
            {
                result.EnsureSuccessStatusCode();
                var res = result.Content.ReadAsAsync<List<VIREquivInmDestinosMejoras>>().Result;
                return res;
            }
        }

        public ActionResult GrabaVIR(long IdInmueble, string Partida, double SupCub, double SupSemiCub, string Usos, int TipoEdif, string FechaConst, string Estados)
        {
            if (!DateTime.TryParse(FechaConst, out DateTime dateTime))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            if (dateTime.Date > DateTime.Today)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Conflict);
            }

            UnidadMantenimientoParcelario.OperacionesVIR.Clear();
            UnidadMantenimientoParcelario.OperacionesVIR.Add(new OperationItem<VIRInmueble>()
            {
                Operation = Operation.Update,
                Item = new VIRInmueble()
                {
                    InmuebleId = IdInmueble,
                    Partida = Partida,
                    MejoraSupCubierta = SupCub,
                    MejoraSupSemicubierta = SupSemiCub,
                    MejoraUsoVIR = Usos,
                    MejoraTipoVIR = TipoEdif.ToString(),
                    FechaVigenciaDesde = Convert.ToDateTime(FechaConst),
                    MejoraEstado = Estados
                }
            });

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
    }
}
