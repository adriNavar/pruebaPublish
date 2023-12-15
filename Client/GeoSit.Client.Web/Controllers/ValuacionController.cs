using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Valuaciones;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Via;
using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Personas;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Data;
using System.Globalization;
using GeoSit.Client.Web.Models.ResponsableFiscal;
using System.Net.Mime;
using System.Net;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Client.Web.Helpers;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Client.Web.Controllers
{
    public class ValuacionController : Controller
    {
        private UsuariosModel Usuario
        {
            get { return (UsuariosModel)Session["usuarioPortal"]; }
        }

        private HttpClient cliente = new HttpClient();

        public ValuacionController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Index()
        {
            var model = new ValuacionModel();

            return PartialView("ValuacionPadron", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MetadatoTierra()
        {

            List<ParametrosGeneralesModel> pg = GetParametrosGenerales();

            var model = new ValuacionModel();

            if (pg.Where(x => x.Clave == "FILTRO_PARCELA").Select(x => x.Valor).FirstOrDefault() == "1")
            {
                ViewBag.FiltroParcela = new SelectList(GetFiltroValuacionTierra(), "Key", "Value");
                //ViewBag.TipoParcela = new SelectList(GetTipoParcela(), "TipoParcelaID", "Descripcion");

                ViewBag.DescripcionFiltro = pg.Where(x => x.Clave == "FILTRO_PARCELA_NOMBRE").Select(x => x.Valor).FirstOrDefault();

                return PartialView("MetadatoTierra", model);
            }

            List<TipoValuacion> tiposvaluacion = GetTipoValuacion();
            // selecciono el primer tipo de valuacion de la tabla ya que a esta altura no deberia haber mas de uno , sino que deberia haber salido por el return anterior
            var tipovaluacion = tiposvaluacion.Where(x => x.Destino == "T").FirstOrDefault();
            if (tipovaluacion == null)
            {
                ViewBag.Title = "Valuación de Tierra";
                ViewBag.Description = "No posee configuración para la valuación de Tierra";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            var TipoEvaluacionTierra = tipovaluacion.MetodoValuacion.ToLower();
            model.IdFiltroParcela = tipovaluacion.IdFiltroParcela ?? "0";

            if (TipoEvaluacionTierra == "tierrapormodulo")
            {
                return RedirectToAction("TierraPorModulo", model);
            }
            else if (TipoEvaluacionTierra == "tierraporsuperficie")
            {

                return RedirectToAction("TierraPorSuperficie", model);
            }
            else if (TipoEvaluacionTierra == "tierraporsuperficievia")
            {

                return RedirectToAction("TierraPorSuperficieVia", model);
            }
            else
            {
                return RedirectToAction("TierraPorStore", model);

            };
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MetadatoFiltroTierra(ValuacionModel model)
        {


            List<TipoValuacion> tiposvaluacion = GetTipoValuacion();

            var tipovaluacion = tiposvaluacion.Where(x => x.Destino == "T" && x.IdFiltroParcela == (model.IdFiltroParcela ?? "0")).FirstOrDefault();

            if (tipovaluacion == null)
            {
                ViewBag.Title = "Valuación de las Mejoras";
                ViewBag.Description = "No posee configuración para la valuación de mejoras";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            var TipoEvaluacionTierra = tipovaluacion.MetodoValuacion.ToLower();


            if (TipoEvaluacionTierra == "tierrapormodulo")
            {
                return RedirectToAction("TierraPorModulo", model);
            }
            else if (TipoEvaluacionTierra == "tierraporsuperficie")
            {

                return RedirectToAction("TierraPorSuperficie", model);
            }
            else if (TipoEvaluacionTierra == "tierraporsuperficievia")
            {

                return RedirectToAction("TierraPorSuperficieVia", model);
            }
            else if (TipoEvaluacionTierra == "tierraporstore")
            {
                return RedirectToAction("TierraPorStore", model);

            }
            else
            {
                ViewBag.Title = "Valuación de las Tierra";
                ViewBag.Description = "La configuración para valuación de tierra es erronea.";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            };
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MetadatoMejora(ValuacionModel model)
        {

            List<ParametrosGeneralesModel> pg = GetParametrosGenerales();

            model = new ValuacionModel();

            if (pg.Where(x => x.Clave == "FILTRO_MEJORA").Select(x => x.Valor).FirstOrDefault() == "1")
            {
                ViewBag.FiltroParcela = new SelectList(GetFiltroValuacionMejora(), "Key", "Value");
                ViewBag.DescripcionFiltro = pg.Where(x => x.Clave == "FILTRO_MEJORA_NOMBRE").Select(x => x.Valor).FirstOrDefault();
                return PartialView("MetadatoMejora", model);
            }

            List<TipoValuacion> tiposvaluacion = GetTipoValuacion();

            var tipovaluacion = tiposvaluacion.Where(x => x.Destino == "M").FirstOrDefault();
            if (tipovaluacion == null)
            {
                ViewBag.Title = "Valuación de las Mejoras";
                ViewBag.Description = "No posee configuración para la valuación de mejoras";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            model.TipoValuacionId = tipovaluacion.TipoValuacionID;
            var TipoEvaluacionMejora = tipovaluacion.MetodoValuacion.ToLower();

            model.IdFiltroParcela = tipovaluacion.IdFiltroParcela ?? "0";
            if (TipoEvaluacionMejora == "mejoravaloresbasicos")
            {
                return RedirectToAction("MejoraValoresBasicos", model);
            }
            else if (TipoEvaluacionMejora == "superficiesemicubiertastore")
            {
                return RedirectToAction("SuperficieSemiCubiertaStore", model);
            }
            else if (TipoEvaluacionMejora == "superficiesemicubierta")
            {

                return PartialView("SuperficieSemiCubierta", model);
            }
            else if (TipoEvaluacionMejora == "mejoravaloresbasicosstore")
            {
                return RedirectToAction("MejoraValoresBasicosStore", model);
            }
            else if (TipoEvaluacionMejora == "sinvaluacionmejora")
            {
                ViewBag.Title = "Valuación de las Mejoras";
                ViewBag.Description = "No posee configuración para la valuación de mejoras";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            else
            {

                ViewBag.Title = "Valuación de las Mejoras";
                ViewBag.Description = "La configuración para la valuación de mejoras es erronea.";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
                //sinvaluacionmejora 

            }
            //if (TipoEvaluacionMejora == "MejoraValoresBasicos")
            //{
            //    return PartialView("MejoraValoresBasicos", model);
            //}
            //else if (TipoEvaluacionMejora == "SinValuacionMejora")
            //{
            //    MejorasValoresBasicosGrabar(new List<ValorBasicoMejora>());// borro las valuacion de mejoras
            //    return RedirectToAction("ABMPadronTemporal", "Valuacion");
            //}
            //else
            //{
            //    return PartialView("SuperficieSemiCubierta", model);
            //}

        }


        public bool ValidateCertificadoValuatorio(int idUnidadTributaria)
        {
            var resp = cliente.GetAsync($"api/DeclaracionJurada/GetDeclaracionesJuradas/Get?idUnidadTributaria={idUnidadTributaria}").Result;
            resp.EnsureSuccessStatusCode();
            bool validacion = true;
            var parcela = GetParcelaByIdUt(idUnidadTributaria);

            var ddjj = resp.Content.ReadAsAsync<List<DDJJ>>().Result;
            if (ddjj.Count() == 0 && parcela.ClaseParcelaID != 6)
            {
                validacion = false;
            }

            return validacion;
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MetadatoFiltroMejora(ValuacionModel model)
        {


            List<TipoValuacion> tiposvaluacion = GetTipoValuacion();

            var tipovaluacion = tiposvaluacion.Where(x => x.Destino == "M" && x.IdFiltroParcela == (model.IdFiltroParcela ?? "0")).FirstOrDefault();
            var TipoEvaluacionMejora = tipovaluacion.MetodoValuacion.ToLower();
            model.TipoValuacionId = tipovaluacion.TipoValuacionID;

            if (TipoEvaluacionMejora == "mejoravaloresbasicos")
            {
                return RedirectToAction("MejoraValoresBasicos", model);
            }
            else if (TipoEvaluacionMejora == "superficiesemicubiertastore")
            {
                return RedirectToAction("SuperficieSemiCubiertaStore", model);
            }
            else if (TipoEvaluacionMejora == "superficiesemicubierta")
            {

                return RedirectToAction("SuperficieSemiCubierta", model);
            }
            else if (TipoEvaluacionMejora == "mejoravaloresbasicosstore")
            {

                return RedirectToAction("MejoraValoresBasicosStore", model);
            }
            else
            {
                ViewBag.Title = "Valuación de las Mejoras";
                ViewBag.Description = "No posee método de valuación para el parámetro seleccionado.";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
                //sinvaluacionmejora 
            }
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MejoraValoresBasicosStore(ValuacionModel model, String IdFiltroParcela)
        {
            if (model.IdFiltroParcela == null || model.IdFiltroParcela == String.Empty)
            {
                model.IdFiltroParcela = IdFiltroParcela;
            }
            model.IdFiltroParcela = model.IdFiltroParcela ?? "0";
            var tiposvaluacion = GetTipoValuacion();
            var tv = tiposvaluacion.Where(x => x.Destino == "M" && x.IdFiltroParcela == model.IdFiltroParcela).FirstOrDefault();

            if (tv == null)
            {
                ViewBag.Title = "Valuación de las Mejoras";
                ViewBag.Description = "No posee configuración para la valuación de mejoras";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            tv.TipoParametro1 = tv.TipoParametro1 ?? "";
            tv.TipoParametro2 = tv.TipoParametro2 ?? "";
            model.MejoraPorStore = new MejoraPorStoreModel();
            model.MejoraPorStore.TipoParametro1 = tv.TipoParametro1.ToLower();
            model.MejoraPorStore.TipoParametro2 = tv.TipoParametro2.ToLower();
            if (tv.TipoParametro1.ToLower() == "combo")
            {
                ViewBag.listaParametro1 = new SelectList(GetAtributosParametro1(tv.TipoValuacionID), "Key", "Value");
            }
            if (tv.TipoParametro2.ToLower() == "combo")
            {
                ViewBag.listaParametro2 = new SelectList(GetAtributosParametro2(tv.TipoValuacionID), "Key", "Value");
            }

            ViewBag.NombreParametro1 = tv.NombreParametro1;
            ViewBag.NombreParametro2 = tv.NombreParametro2;


            List<ValorBasicoMejoraStore> lista = GetValorBasicoMejoraStoreXTipo(tv.TipoValuacionID, model.IdFiltroParcela);
            model.TipoValuacionId = tv.TipoValuacionID;

            foreach (var VBTS in lista)
            {
                model.MejoraPorStore.ValorComparacion1.Add("" + VBTS.Comparador1);
                model.MejoraPorStore.Parametro1.Add("" + VBTS.Parametro1Desde);
                model.MejoraPorStore.Parametro1Desde.Add("" + VBTS.Parametro1Desde);
                model.MejoraPorStore.Parametro1Hasta.Add("" + VBTS.Parametro1Hasta);
                model.MejoraPorStore.Parametro2.Add("" + VBTS.Parametro2Desde);
                model.MejoraPorStore.Parametro2Desde.Add("" + VBTS.Parametro2Desde);
                model.MejoraPorStore.Parametro2Hasta.Add("" + VBTS.Parametro2Hasta);
                model.MejoraPorStore.Cantidad.Add((VBTS.Valor ?? 0).ToString());

                model.MejoraPorStore.ValorComparacion2.Add("" + VBTS.Comparador2);
            }

            return PartialView("MejoraPorStore", model);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ValuacionDePadron()
        {
            var model = new ValuacionModel();

            return PartialView("ValuacionPadron", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ABMPadronTemporal(ValuacionModel model)
        {

            return PartialView("ABMPadronTemporal", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BuscarPadron()
        {
            var model = new ValuacionModel();

            List<ValuacionPadronCabecera> padrones = GetValuacionPadronCabecera();

            ViewBag.vigencias = new SelectList(padrones.Select(x => new { IdPadron = x.IdPadron, Vigencia = (x.VigenciaDesde ?? System.DateTime.Now).ToString("dd/MM/yyyy") + " - " + (x.VigenciaHasta ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "Vigencia");
            ViewBag.calculos = new SelectList(padrones.Where(x => x.FechaCalculo != null).Select(x => new { IdPadron = x.IdPadron, FechaCalculo = (x.FechaCalculo ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "FechaCalculo");
            ViewBag.consolidados = new SelectList(padrones.Where(x => x.FechaConsolidado != null).Select(x => new { IdPadron = x.IdPadron, FechaConsolidado = (x.FechaConsolidado ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "FechaConsolidado");

            return PartialView("BusquedaPadron", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]

        public ActionResult PadronSearch(ValuacionModel model)
        {
            if (model == null || model.BusquedaPadron == null || model.BusquedaPadron.IdPadron == 0)
            {
                return RedirectToAction("BuscarPadron");
            }
            List<ValuacionPadronCabecera> padrones = GetValuacionPadronCabecera();
            if (padrones.Where(x => x.IdPadron == model.BusquedaPadron.IdPadron).Where(a => a.FechaConsolidado != null).FirstOrDefault() != null)
            {
                model.BusquedaPadron.esConsolidado = true;
            }

            ViewBag.vigencias = new SelectList(padrones.Select(x => new { IdPadron = x.IdPadron, Vigencia = (x.VigenciaDesde ?? System.DateTime.Now).ToString("dd/MM/yyyy") + " - " + (x.VigenciaHasta ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "Vigencia", model.BusquedaPadron.IdPadron);
            ViewBag.calculos = new SelectList(padrones.Where(x => x.FechaCalculo != null).Select(x => new { IdPadron = x.IdPadron, FechaCalculo = (x.FechaCalculo ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "FechaCalculo", model.BusquedaPadron.IdPadron);
            ViewBag.consolidados = new SelectList(padrones.Where(x => x.FechaConsolidado != null).Select(x => new { IdPadron = x.IdPadron, FechaConsolidado = (x.FechaConsolidado ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "FechaConsolidado", model.BusquedaPadron.IdPadron);

            Dictionary<string, string> valores = RecuperarTotales(model.BusquedaPadron.IdPadron);
            if (valores.Keys.Contains("SumaSuperficieTierra"))
            {
                model.BusquedaPadron.SumaSuperficieTierra = float.Parse(valores["SumaSuperficieTierra"]);
                model.BusquedaPadron.SumaSuperificeCubierta = float.Parse(valores["SumaSuperificeCubierta"]);
                model.BusquedaPadron.SumaSuperficieSemiCubierta = float.Parse(valores["SumaSuperficieSemiCubierta"]);
                model.BusquedaPadron.SumaValorTierra = Decimal.Parse(valores["SumaValorTierra"]);
                model.BusquedaPadron.SumaValorMejora = Decimal.Parse(valores["SumaValorMejora"]);
                model.BusquedaPadron.SumaValorTotal = Decimal.Parse(valores["SumaValorTotal"]);
                model.BusquedaPadron.MaxSuperficieTierra = float.Parse(valores["MaxSuperficieTierra"]);
                model.BusquedaPadron.MaxSuperificeCubierta = float.Parse(valores["MaxSuperificeCubierta"]);
                model.BusquedaPadron.MaxSuperficieSemiCubierta = float.Parse(valores["MaxSuperficieSemiCubierta"]);
                model.BusquedaPadron.MaxValorTierra = Decimal.Parse(valores["MaxValorTierra"]);
                model.BusquedaPadron.MaxValorMejora = Decimal.Parse(valores["MaxValorMejora"]);
                model.BusquedaPadron.MaxValorTotal = Decimal.Parse(valores["MaxValorTotal"]);
                model.BusquedaPadron.MinSuperficieTierra = float.Parse(valores["MinSuperficieTierra"]);
                model.BusquedaPadron.MinSuperificeCubierta = float.Parse(valores["MinSuperificeCubierta"]);
                model.BusquedaPadron.MinSuperficieSemiCubierta = float.Parse(valores["MinSuperficieSemiCubierta"]);
                model.BusquedaPadron.MinValorTierra = Decimal.Parse(valores["MinValorTierra"]);
                model.BusquedaPadron.MinValorMejora = Decimal.Parse(valores["MinValorMejora"]);
                model.BusquedaPadron.MinValorTotal = Decimal.Parse(valores["MinValorTotal"]);
                model.BusquedaPadron.PromSuperficieTierra = float.Parse(valores["PromSuperficieTierra"]);
                model.BusquedaPadron.PromSuperificeCubierta = float.Parse(valores["PromSuperificeCubierta"]);
                model.BusquedaPadron.PromSuperficieSemiCubierta = float.Parse(valores["PromSuperficieSemiCubierta"]);
                model.BusquedaPadron.PromValorTierra = Decimal.Parse(valores["PromValorTierra"]);
                model.BusquedaPadron.PromValorMejora = Decimal.Parse(valores["PromValorMejora"]);
                model.BusquedaPadron.PromValorTotal = Decimal.Parse(valores["PromValorTotal"]);
                model.BusquedaPadron.NulosSuperficieTierra = float.Parse(valores["NulosSuperficieTierra"]);
                model.BusquedaPadron.NulosSuperificeCubierta = float.Parse(valores["NulosSuperificeCubierta"]);
                model.BusquedaPadron.NulosSuperficieSemiCubierta = float.Parse(valores["NulosSuperficieSemiCubierta"]);
                model.BusquedaPadron.NulosValorTierra = float.Parse(valores["NulosValorTierra"]);
                model.BusquedaPadron.NulosValorMejora = float.Parse(valores["NulosValorMejora"]);
                model.BusquedaPadron.NulosValorTotal = float.Parse(valores["NulosValorTotal"]);
                model.BusquedaPadron.CantInmuebles = long.Parse(valores["CantInmuebles"]);
            }
            return PartialView("BusquedaPadron", model);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TierraPorSuperficie(ValuacionModel model, string IdFiltroParcela)
        {
            model.TierraPorSuperficie = new TierraPorSuperficieModel();
            if (model.IdFiltroParcela == null || model.IdFiltroParcela == String.Empty)
            {
                model.IdFiltroParcela = IdFiltroParcela;
            }
            var filtro = model.IdFiltroParcela ?? "0";

            List<ValorBasicoTierraSuperficie> lista = GetValorBasicoTierraSuperficieXTipo(long.Parse(filtro));
            foreach (var VBT in lista)
            {
                model.TierraPorSuperficie.Zona.Add(VBT.IdAtributoZona);
                model.TierraPorSuperficie.Valor.Add(VBT.Valor);
                model.TierraPorSuperficie.Via.Add(VBT.Parametro1);
                model.TierraPorSuperficie.Eje_Via.Add(VBT.Parametro2);
            }

            ViewBag.listaZonas = GetAtributosZonaObjeto();
            return PartialView("TierraPorSuperficie", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TierraPorSuperficieVia(ValuacionModel model, string IdFiltroParcela)
        {
            model.TierraPorSuperficie = new TierraPorSuperficieModel();
            if (model.IdFiltroParcela == null || model.IdFiltroParcela == String.Empty)
            {
                model.IdFiltroParcela = IdFiltroParcela;
            }
            var filtro = model.IdFiltroParcela ?? "0";

            List<ValorBasicoTierraSuperficie> lista = GetValorBasicoTierraSuperficieViasXTipo(long.Parse(filtro));
            List<TramoVia> alturas = GetTramosVias();
            List<Via> vias = GetVias();
            foreach (var altura in alturas)
            {
                var VBT = lista.Where(j => long.Parse(j.Parametro1) == altura.ViaId && long.Parse(j.Parametro2) == altura.TramoViaId).FirstOrDefault();
                if (VBT != null)
                {
                    model.TierraPorSuperficie.Zona.Add(VBT.IdAtributoZona);
                    model.TierraPorSuperficie.Valor.Add(VBT.Valor);
                }
                else
                {
                    model.TierraPorSuperficie.Zona.Add(0);
                    model.TierraPorSuperficie.Valor.Add(0);
                }
                model.TierraPorSuperficie.Via.Add(altura.ViaId + "");
                model.TierraPorSuperficie.Eje_Via.Add(altura.TramoViaId + "");
                model.TierraPorSuperficie.Paridad.Add(altura.Paridad);
                var via = vias.Where(v => v.ViaId == altura.ViaId).FirstOrDefault();
                if (via != null)
                {
                    model.TierraPorSuperficie.NombreVia.Add(via.Nombre);
                }
                model.TierraPorSuperficie.Altura_Desde.Add(altura.AlturaDesde + "");
                model.TierraPorSuperficie.Altura_Hasta.Add(altura.AlturaHasta + "");
            }

            return PartialView("TierraPorSuperficieVia", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TierraPorModulo(ValuacionModel model, String IdFiltroParcela)
        {


            if (model.IdFiltroParcela == null || model.IdFiltroParcela == String.Empty)
            {
                model.IdFiltroParcela = IdFiltroParcela;
            }

            ViewBag.listaZonas = GetAtributosZonaObjeto();
            var filtro = model.IdFiltroParcela ?? "0";
            List<ValorBasicoTierraModulo> lista = GetValorBasicoTierraModulXTipo(long.Parse(filtro));
            model.TierraPorModulo = new TierraPorModuloModel();
            foreach (var VBM in lista)
            {
                model.TierraPorModulo.Superficie.Add(VBM.Desde);
                model.TierraPorModulo.SuperficieDesde.Add(VBM.Desde);
                model.TierraPorModulo.SuperficieHasta.Add(VBM.Hasta);
                model.TierraPorModulo.CantidadModulos.Add(VBM.Modulos);
                model.TierraPorModulo.Zona.Add(VBM.IdAtributoZona ?? 0);
                model.TierraPorModulo.ValorComparacion.Add(VBM.Comparador);
            }

            return PartialView("TierraPorModulo", model);

        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TierraPorStore(ValuacionModel model, String IdFiltroParcela)
        {

            if (model.IdFiltroParcela == null || model.IdFiltroParcela == String.Empty)
            {
                model.IdFiltroParcela = IdFiltroParcela;
            }
            model.IdFiltroParcela = model.IdFiltroParcela ?? "0";
            var tiposvaluacion = GetTipoValuacion();
            var tv = tiposvaluacion.Where(x => x.Destino == "T" && x.IdFiltroParcela == model.IdFiltroParcela).FirstOrDefault();
            if (tv == null)
            {
                ViewBag.Title = "Valuación de la Tierra";
                ViewBag.Description = "No posee configuración para la valuación de tierra";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            tv.TipoParametro1 = tv.TipoParametro1 ?? "";
            tv.TipoParametro2 = tv.TipoParametro2 ?? "";

            model.TierraPorStore = new TierraPorStoreModel();
            model.TierraPorStore.TipoParametro1 = tv.TipoParametro1.ToLower();
            model.TierraPorStore.TipoParametro2 = tv.TipoParametro2.ToLower();
            if (tv.TipoParametro1.ToLower() == "combo")
            {
                ViewBag.listaParametro1 = new SelectList(GetAtributosParametro1(tv.TipoValuacionID), "Key", "Value");
            }
            if (tv.TipoParametro2.ToLower() == "combo")
            {
                ViewBag.listaParametro2 = new SelectList(GetAtributosParametro2(tv.TipoValuacionID), "Key", "Value");
            }

            ViewBag.NombreParametro1 = tv.NombreParametro1;
            ViewBag.NombreParametro2 = tv.NombreParametro2;


            List<ValorBasicoTierraStore> lista = GetValorBasicoTierraStoreXTipo(tv.TipoValuacionID, IdFiltroParcela);
            model.TipoValuacionId = tv.TipoValuacionID;

            foreach (var VBTS in lista)
            {
                model.TierraPorStore.ValorComparacion1.Add("" + VBTS.Comparador1);
                model.TierraPorStore.Parametro1.Add("" + VBTS.Parametro1Desde);
                model.TierraPorStore.Parametro1Desde.Add("" + VBTS.Parametro1Desde);
                model.TierraPorStore.Parametro1Hasta.Add("" + VBTS.Parametro1Hasta);
                model.TierraPorStore.Parametro2.Add("" + VBTS.Parametro2Desde);
                model.TierraPorStore.Parametro2Desde.Add("" + VBTS.Parametro2Desde);
                model.TierraPorStore.Parametro2Hasta.Add("" + VBTS.Parametro2Hasta);
                model.TierraPorStore.Cantidad.Add((VBTS.Valor ?? 0).ToString());

                model.TierraPorStore.ValorComparacion2.Add("" + VBTS.Comparador2);
            }

            return PartialView("TierraPorStore", model);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MetadatoMejoraCoeficiente()
        {
            var model = new ValuacionModel();

            return PartialView("MetadatoMejoraCoeficiente", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult RangoCoeficienteMejora(ValuacionModel model, long? IdMejoraCoeficiente, string IdTipoCoeficienteMejora)
        {
            if (model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente == 0)
            {
                model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente = IdMejoraCoeficiente ?? 0;
                model.TipoCoeficienteMejora.IdTipoCoeficienteMejora = IdTipoCoeficienteMejora;

            }
            List<CoeficientesMejora> lista = GetCoeficientesMejoraRango(model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente);
            foreach (var VBM in lista.OrderBy(m => double.Parse(m.Desde)).ThenBy(m => double.Parse(m.Hasta)).ThenBy(m => m.Coeficiente))
            {
                model.RangoCoeficienteMejora.Rango1CoeficienteMejora.Add(double.Parse(VBM.Desde).ToString());
                model.RangoCoeficienteMejora.Rango2CoeficienteMejora.Add(double.Parse(VBM.Hasta).ToString());
                model.RangoCoeficienteMejora.Rango3CoeficienteMejora.Add(float.Parse(VBM.Coeficiente.ToString()));

            }

            return PartialView("RangoCoeficienteMejora", model);

        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ValorCoeficienteMejora(ValuacionModel model, long? IdMejoraCoeficiente, string IdTipoCoeficienteMejora)
        {
            if (model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente == 0)
            {
                model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente = IdMejoraCoeficiente ?? 0;
                model.TipoCoeficienteMejora.IdTipoCoeficienteMejora = IdTipoCoeficienteMejora;

            }
            //List<CoeficientesMejora> lista = GetCoeficientesMejoraValor(model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente);
            //foreach (var VBM in lista.OrderBy(m => double.Parse(m.Desde)).ThenBy(m => double.Parse(m.Hasta)).ThenBy(m => m.Coeficiente))
            //{
            //    model.ValorCoeficienteMejora.Valor1CoeficienteMejora.Add(double.Parse(VBM.Desde).ToString());
            //    model.ValorCoeficienteMejora.Valor2CoeficienteMejora.Add(double.Parse(VBM.Hasta).ToString());
            //    model.ValorCoeficienteMejora.Valor3CoeficienteMejora.Add(decimal.Parse(VBM.Coeficiente.ToString()));

            //}
            var tiposvaluacion = GetTipoValuacion();
            var tv = tiposvaluacion.Where(x => x.Destino == ("M" + model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente)).FirstOrDefault();
            if (tv == null)
            {
                ViewBag.Title = "Coeficiente de las Mejoras";
                ViewBag.Description = "No posee configuración para la carga de este coeficiente";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            tv.TipoParametro1 = tv.TipoParametro1 ?? "";
            tv.TipoParametro2 = tv.TipoParametro2 ?? "";

            model.CoeficienteStore = new CoeficienteStoreModel();
            model.CoeficienteStore.TipoParametro1 = tv.TipoParametro1.ToLower();
            model.CoeficienteStore.TipoParametro2 = tv.TipoParametro2.ToLower();
            if (tv.TipoParametro1.ToLower() == "combo")
            {
                ViewBag.listaParametro1 = new SelectList(GetAtributosParametro1(tv.TipoValuacionID), "Key", "Value");
            }
            if (tv.TipoParametro2.ToLower() == "combo")
            {
                ViewBag.listaParametro2 = new SelectList(GetAtributosParametro2(tv.TipoValuacionID), "Key", "Value");
            }

            ViewBag.NombreParametro1 = tv.NombreParametro1;
            ViewBag.NombreParametro2 = tv.NombreParametro2;


            List<ValorBasicoTierraStore> lista = GetCoeficientesStore(model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente, "M");
            model.TipoValuacionId = tv.TipoValuacionID;

            foreach (var VBTS in lista)
            {
                model.CoeficienteStore.ValorComparacion1.Add("" + VBTS.Comparador1);
                model.CoeficienteStore.Parametro1.Add("" + VBTS.Parametro1Desde);
                model.CoeficienteStore.Parametro1Desde.Add("" + VBTS.Parametro1Desde);
                model.CoeficienteStore.Parametro1Hasta.Add("" + VBTS.Parametro1Hasta);
                model.CoeficienteStore.Parametro2.Add("" + VBTS.Parametro2Desde);
                model.CoeficienteStore.Parametro2Desde.Add("" + VBTS.Parametro2Desde);
                model.CoeficienteStore.Parametro2Hasta.Add("" + VBTS.Parametro2Hasta);
                model.CoeficienteStore.Cantidad.Add((VBTS.Valor ?? 0).ToString());

                model.CoeficienteStore.ValorComparacion2.Add("" + VBTS.Comparador2);
            }
            return PartialView("CoeficienteMejoraPorStore", model);

        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TipoCoeficienteMejora(ValuacionModel model)
        {
            Coeficientes coeficiente = GetCoeficientes().Where(x => x.Fecha_Baja == null && x.Nro_Coeficiente == model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente && (x.Id_Tipo_Coeficiente == 3 || x.Id_Tipo_Coeficiente == 4)).FirstOrDefault();
            if (coeficiente != null)
            {
                model.TipoCoeficienteMejora.IdTipoCoeficienteMejora = coeficiente.Descripcion;
                model.TipoCoeficienteMejora.TipoCoeficiente = "";
                if (coeficiente.Id_Tipo_Coeficiente == 3)
                {
                    model.TipoCoeficienteMejora.TipoCoeficiente = "VALOR";
                }
                else if (coeficiente.Id_Tipo_Coeficiente == 4)
                {
                    model.TipoCoeficienteMejora.TipoCoeficiente = "RANGO";
                }
            }
            else
            {
                model.TipoCoeficienteMejora.TipoCoeficiente = "";
            }
            return PartialView("TipoCoeficienteMejora", model);
        }



        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MejoraValoresBasicos(ValuacionModel model)
        {
            //var model = new ValuacionModel();

            List<ValorBasicoMejora> lista = GetValorBasicoMejora();
            if (model.MejoraValoresBasicos == null)
            {
                model.MejoraValoresBasicos = new MejoraValoresBasicosModel();
            }

            foreach (var VBM in lista)
            {
                model.MejoraValoresBasicos.ValorBasicoMejora1.Add(VBM.Desde);
                model.MejoraValoresBasicos.ValorBasicoMejora2.Add(VBM.Hasta);
                model.MejoraValoresBasicos.ValorBasicoMejora3.Add(VBM.Coeficiente);

            }

            return PartialView("MejoraValoresBasicos", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SuperficieSemiCubiertaStore(ValuacionModel model)
        {
            if (model == null)
            {
                model = new ValuacionModel();
            }

            ValorBasicoMejoraStore uno = GetValorBasicoMejoraSupSemiCubiertaStore();
            if (uno != null)
            {
                model.SuperficieSemiCubiertaPorcentaje = uno.Semicubierto ?? 100m;
            }

            return PartialView("SuperficieSemiCubierta", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SuperficieSemiCubierta(ValuacionModel model)
        {
            if (model == null)
            {
                model = new ValuacionModel();
            }

            ValorBasicoMejora uno = GetValorBasicoMejoraSupSemiCubierta();
            if (uno != null)
            {
                model.SuperficieSemiCubiertaPorcentaje = uno.Semicubierto;
            }

            return PartialView("SuperficieSemiCubierta", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult SinValuacion(ValuacionModel model)
        {
            MejorasValoresBasicosGrabar(new List<ValorBasicoMejora>());// borro las valuacion de mejoras
            return RedirectToAction("ABMPadronTemporal", "Valuacion");
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult MetadatoCoeficienteTierra()
        {
            var model = new ValuacionModel();

            return PartialView("MetadatoCoeficienteTierra", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TipoCoeficienteTierra(ValuacionModel model)
        {
            Coeficientes coeficiente = GetCoeficientes().Where(x => x.Fecha_Baja == null && x.Nro_Coeficiente == model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra && (x.Id_Tipo_Coeficiente == 1 || x.Id_Tipo_Coeficiente == 2)).FirstOrDefault();
            if (coeficiente != null)
            {
                model.TipoCoeficienteTierra.IdTipoCoeficienteTierra = coeficiente.Descripcion;
                model.TipoCoeficienteTierra.TipoCoeficiente = "";
                if (coeficiente.Id_Tipo_Coeficiente == 1) // codigo obsoleto
                {
                    model.TipoCoeficienteTierra.TipoCoeficiente = "VALOR";
                }
                else if (coeficiente.Id_Tipo_Coeficiente == 2)
                {
                    model.TipoCoeficienteTierra.TipoCoeficiente = "RANGO";
                }
            }
            else
            {
                model.TipoCoeficienteMejora.TipoCoeficiente = "";
            }
            return PartialView("TipoCoeficienteTierra", model);
        }

        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ValorCoeficiente(ValuacionModel model, long? IdMetadatoCoeficienteTierra, string IdTipoCoeficienteTierra)
        {
            if (model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra == 0)
            {
                model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra = IdMetadatoCoeficienteTierra ?? 0;
                model.TipoCoeficienteTierra.IdTipoCoeficienteTierra = IdTipoCoeficienteTierra;
            }
            //List<CoeficientesTierra> lista = GetCoeficientesTierraValor(model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra);
            //foreach (var VBM in lista.OrderBy(t => double.Parse(t.Desde)).ThenBy(t => double.Parse(t.Hasta)).ThenBy(t => t.Coeficiente))
            //{
            //    model.ValorCoeficiente.Valor1Coeficiente.Add(double.Parse(VBM.Desde).ToString());
            //    model.ValorCoeficiente.Valor2Coeficiente.Add(double.Parse(VBM.Hasta).ToString());
            //    model.ValorCoeficiente.Valor3Coeficiente.Add(Decimal.Parse(VBM.Coeficiente.ToString()));
            //}

            var tiposvaluacion = GetTipoValuacion();
            var tv = tiposvaluacion.Where(x => x.Destino == ("T" + model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra)).FirstOrDefault();
            if (tv == null)
            {
                ViewBag.Title = "Coeficiente de Tierra";
                ViewBag.Description = "No posee configuración para la carga de este coeficiente";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }
            tv.TipoParametro1 = tv.TipoParametro1 ?? "";
            tv.TipoParametro2 = tv.TipoParametro2 ?? "";

            model.CoeficienteStore = new CoeficienteStoreModel();
            model.CoeficienteStore.TipoParametro1 = tv.TipoParametro1.ToLower();
            model.CoeficienteStore.TipoParametro2 = tv.TipoParametro2.ToLower();
            if (tv.TipoParametro1.ToLower() == "combo")
            {
                ViewBag.listaParametro1 = new SelectList(GetAtributosParametro1(tv.TipoValuacionID), "Key", "Value");
            }
            if (tv.TipoParametro2.ToLower() == "combo")
            {
                ViewBag.listaParametro2 = new SelectList(GetAtributosParametro2(tv.TipoValuacionID), "Key", "Value");
            }

            ViewBag.NombreParametro1 = tv.NombreParametro1;
            ViewBag.NombreParametro2 = tv.NombreParametro2;


            List<ValorBasicoTierraStore> lista = GetCoeficientesStore(model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra, "T");
            model.TipoValuacionId = tv.TipoValuacionID;

            foreach (var VBTS in lista)
            {
                model.CoeficienteStore.ValorComparacion1.Add("" + VBTS.Comparador1);
                model.CoeficienteStore.Parametro1.Add("" + VBTS.Parametro1Desde);
                model.CoeficienteStore.Parametro1Desde.Add("" + VBTS.Parametro1Desde);
                model.CoeficienteStore.Parametro1Hasta.Add("" + VBTS.Parametro1Hasta);
                model.CoeficienteStore.Parametro2.Add("" + VBTS.Parametro2Desde);
                model.CoeficienteStore.Parametro2Desde.Add("" + VBTS.Parametro2Desde);
                model.CoeficienteStore.Parametro2Hasta.Add("" + VBTS.Parametro2Hasta);
                model.CoeficienteStore.Cantidad.Add((VBTS.Valor ?? 0).ToString());

                model.CoeficienteStore.ValorComparacion2.Add("" + VBTS.Comparador2);
            }
            return PartialView("CoeficienteTierraPorStore", model);

        }

        //[OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        //public ActionResult RangoCoeficiente(ValuacionModel model, long? IdMetadatoCoeficienteTierra, string IdTipoCoeficienteTierra)
        //{
        //    if (model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra == 0)
        //    {
        //        model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra = IdMetadatoCoeficienteTierra ?? 0;
        //        model.TipoCoeficienteTierra.IdTipoCoeficienteTierra = IdTipoCoeficienteTierra;
        //    }
        //    List<CoeficientesTierra> lista = GetCoeficientesTierraRango(model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra);
        //    foreach (var VBM in lista.OrderBy(t => double.Parse(t.Desde)).ThenBy(t => double.Parse(t.Hasta)).ThenBy(t => t.Coeficiente))
        //    {
        //        model.RangoCoeficiente.Rango1Coeficiente.Add(double.Parse(VBM.Desde).ToString());
        //        model.RangoCoeficiente.Rango2Coeficiente.Add(double.Parse(VBM.Hasta).ToString());
        //        model.RangoCoeficiente.Rango3Coeficiente.Add(Decimal.Parse(VBM.Coeficiente.ToString()));
        //    }
        //    return PartialView("RangoCoeficiente", model);
        //}
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult BusquedaParcela()
        {
            var model = new ValuacionModel();
            ViewBag.TipoParcela = new SelectList(GetTipoParcela(), "TipoParcelaID", "Descripcion");
            //ViewBag.vigencias = new SelectList(padrones.Select(x => new { IdPadron = x.IdPadron, Vigencia = (x.VigenciaDesde ?? System.DateTime.Now).ToString("dd/MM/yyyy") + " - " + (x.VigenciaHasta ?? System.DateTime.Now).ToString("dd/MM/yyyy") }), "IdPadron", "Vigencia");

            return PartialView("BusquedaParcela", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ValuacionPadronTemporal()
        {
            var model = new ValuacionModel();

            return PartialView("ValuacionPadronTemporal", model);
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult CalcularPadronTemporal(ValuacionModel model)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            var valuacionPadronCabecera = new ValuacionPadronCabecera();
            valuacionPadronCabecera.VigenciaDesde = model.BusquedaPadron.VigenciaDesde;
            //valuacionPadronCabecera.VigenciaHasta = model.BusquedaPadron.VigenciaHasta;
            if (usuario == null)
            {
                usuario = new UsuariosModel();
                usuario.Id_Usuario = 1;
            }
            valuacionPadronCabecera.Usuario_Alta = usuario.Id_Usuario;

            Dictionary<string, string> valores = CalcularTotalesTemporal(valuacionPadronCabecera);

            model.BusquedaPadron.CantErrores = long.Parse(valores["CantErrores"]);
            model.BusquedaPadron.ErroresPath = valores["ErroresPath"];


            if (valores.Keys.Contains("SumaSuperficieSemiCubierta"))
            {
                model.BusquedaPadron.SumaSuperficieTierra = float.Parse(valores["SumaSuperficieTierra"]);
                model.BusquedaPadron.SumaSuperificeCubierta = float.Parse(valores["SumaSuperificeCubierta"]);
                model.BusquedaPadron.SumaSuperficieSemiCubierta = float.Parse(valores["SumaSuperficieSemiCubierta"]);
                model.BusquedaPadron.SumaValorTierra = Decimal.Parse(valores["SumaValorTierra"]);
                model.BusquedaPadron.SumaValorMejora = Decimal.Parse(valores["SumaValorMejora"]);
                model.BusquedaPadron.SumaValorTotal = Decimal.Parse(valores["SumaValorTotal"]);
                model.BusquedaPadron.MaxSuperficieTierra = float.Parse(valores["MaxSuperficieTierra"]);
                model.BusquedaPadron.MaxSuperificeCubierta = float.Parse(valores["MaxSuperificeCubierta"]);
                model.BusquedaPadron.MaxSuperficieSemiCubierta = float.Parse(valores["MaxSuperficieSemiCubierta"]);
                model.BusquedaPadron.MaxValorTierra = Decimal.Parse(valores["MaxValorTierra"]);
                model.BusquedaPadron.MaxValorMejora = Decimal.Parse(valores["MaxValorMejora"]);
                model.BusquedaPadron.MaxValorTotal = Decimal.Parse(valores["MaxValorTotal"]);
                model.BusquedaPadron.MinSuperficieTierra = float.Parse(valores["MinSuperficieTierra"]);
                model.BusquedaPadron.MinSuperificeCubierta = float.Parse(valores["MinSuperificeCubierta"]);
                model.BusquedaPadron.MinSuperficieSemiCubierta = float.Parse(valores["MinSuperficieSemiCubierta"]);
                model.BusquedaPadron.MinValorTierra = Decimal.Parse(valores["MinValorTierra"]);
                model.BusquedaPadron.MinValorMejora = Decimal.Parse(valores["MinValorMejora"]);
                model.BusquedaPadron.MinValorTotal = Decimal.Parse(valores["MinValorTotal"]);
                model.BusquedaPadron.PromSuperficieTierra = float.Parse(valores["PromSuperficieTierra"]);
                model.BusquedaPadron.PromSuperificeCubierta = float.Parse(valores["PromSuperificeCubierta"]);
                model.BusquedaPadron.PromSuperficieSemiCubierta = float.Parse(valores["PromSuperficieSemiCubierta"]);
                model.BusquedaPadron.PromValorTierra = Decimal.Parse(valores["PromValorTierra"]);
                model.BusquedaPadron.PromValorMejora = Decimal.Parse(valores["PromValorMejora"]);
                model.BusquedaPadron.PromValorTotal = Decimal.Parse(valores["PromValorTotal"]);
                model.BusquedaPadron.NulosSuperficieTierra = float.Parse(valores["NulosSuperficieTierra"]);
                model.BusquedaPadron.NulosSuperificeCubierta = float.Parse(valores["NulosSuperificeCubierta"]);
                model.BusquedaPadron.NulosSuperficieSemiCubierta = float.Parse(valores["NulosSuperficieSemiCubierta"]);
                model.BusquedaPadron.NulosValorTierra = float.Parse(valores["NulosValorTierra"]);
                model.BusquedaPadron.NulosValorMejora = float.Parse(valores["NulosValorMejora"]);
                model.BusquedaPadron.NulosValorTotal = float.Parse(valores["NulosValorTotal"]);
                model.BusquedaPadron.CantInmuebles = long.Parse(valores["CantInmuebles"].ToString());
            }

            return PartialView("ValuacionPadronTemporal", model);
        }

        public ActionResult GrabarPadronTemporal(ValuacionModel model)
        {
            GrabarPadronTemporal(new ValuacionPadronCabecera() { IdPadron = model.BusquedaPadron.IdPadron });
            return RedirectToAction("PadronSearch", new { model = model });
        }
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult PadronConsolidar(ValuacionModel model)
        {
            ConsolidarPadron(new ValuacionPadronCabecera() { IdPadron = model.BusquedaPadron.IdPadron, Usuario_Alta = Usuario.Id_Usuario });
            model.BusquedaPadron.esConsolidado = true;
            return RedirectToAction("PadronSearch", new { model = model });
        }
        public ActionResult PadronEliminar(ValuacionModel model)
        {
            EliminarPadron(new ValuacionPadronCabecera() { IdPadron = model.BusquedaPadron.IdPadron, Usuario_Baja = Usuario.Id_Usuario });
            return RedirectToAction("BuscarPadron");
        }

        public ActionResult ImprimeBusquedaPadron(ValuacionModel model)
        {
            return View("ImprimeBusquedaPadron", model);
        }

        public ActionResult PadronImprimir(ValuacionModel model)
        {
            if (model.BusquedaPadron.IdPadron == 0)
            {
                RedirectToAction("BuscarPadron");
            }

            List<ValuacionPadronDetalle> detalles = GetValuacionPadronDetalleByIdPadron(model.BusquedaPadron.IdPadron);
            var DetallesPadron = ConvertToDataTable(detalles);

            var padron = GetValuacionPadronCabeceraById(model.BusquedaPadron.IdPadron);
            var grid = new GridView();
            grid.DataSource = DetallesPadron;
            grid.DataBind();
            var nombre = ("Padron " + (padron.VigenciaDesde.HasValue ? padron.VigenciaDesde.Value.ToString("dd/MM/yyyy") : "") + "_" + padron.Estado).Replace("/", "-");
            nombre = nombre.Replace(" ", "_");
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + nombre + ".xls");
            Response.ContentType = "application/ms-excel";
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("BuscarPadron");
        }
        [HttpGet]
        public JsonResult PadronImprimir(long idPadron)
        {

            List<ValuacionPadronDetalle> detalles = GetValuacionPadronDetalleByIdPadron(idPadron);
            var DetallesPadron = ConvertToDataTable(detalles);

            var padron = GetValuacionPadronCabeceraById(idPadron);
            var grid = new GridView();
            grid.DataSource = DetallesPadron;
            grid.DataBind();
            var nombre = ("Padron " + (padron.VigenciaDesde.HasValue ? padron.VigenciaDesde.Value.ToString("dd/MM/yyyy") : "") + "_" + padron.Estado).Replace("/", "-");
            nombre = nombre.Replace(" ", "_");
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + nombre + ".xls");
            Response.ContentType = "application/ms-excel";
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("BuscarPadron", JsonRequestBehavior.AllowGet);
        }
        public ActionResult BuscarParcelas(ValuacionModel model)
        {

            ViewBag.listaParcelas = GetValuacionParcelaBuscar(new ValuacionParcela() { Id_Inmueble = model.Parcela.Id_Inmueble, Tipo_Parcela = model.Parcela.Tipo_Parcela, Partida = model.Parcela.Partida, Ejido = model.Parcela.Ejido, Sector = model.Parcela.Sector, Fraccion = model.Parcela.Fraccion, Circunscripcion = model.Parcela.Circunscripcion, Manzana = model.Parcela.Manzana, Parcela = model.Parcela.Parcela, Nombre = model.Parcela.Nombre, Apellido = model.Parcela.Apellido });
            return PartialView("ResultadoParcela", model);

        }

        public ActionResult DetalleValuacionParcela(long idParcela)
        {

            List<ParametrosGeneralesModel> pg = GetParametrosGenerales();
            List<TipoValuacion> tiposvaluacion = GetTipoValuacion();

            ValuacionModel model = new ValuacionModel();
            Parcela parcela = GetParcelaById(idParcela);
            Session["model"] = parcela;
            List<ValuacionPadronDetalle> lista = GetValuacionPadronDetalle(parcela.ParcelaID);
            List<ValuacionPadronCabecera> listaCabeceras = GetValuacionPadronCabecera();

            //VEAMOS, si esto no es exactamente prolijo, es mas, es arreglar usando un parche para tapar la fuga.
            //pero dadas las configuraciones que estubieron haciendo en valuaciones para los diferentes entornos, tuve que agregar un tipo_municipio en ge_parametro.
            //Por este parametro va ser afectado el comportamiento de Valuaciones, "resolviendo" el problemas de 0's y null's y alguna otra configuracion.
            //Solo usado para los entornos de GeoSIT CHUBUT.
            //TIPO_MUNICIPIO = 1 (default)
            //En el codigo sera identificado con el siguiente elemento: <parche></parche>


            var list = listaCabeceras.Where(w => w.Estado == "CONS" && w.VigenciaHasta == null).FirstOrDefault();
            if (list != null)
            {
                model.EdicionParcela.IdPadron = list.IdPadron;
            }
            else
            {
                //return PartialView("~/Views/Valuacion/MensajeSinValuacion.cshtml");
                ViewBag.Title = "Sin Valuacion";
                ViewBag.Description = "La parcela seleccionada aún no ha sido valuada.";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");
            }

            var parametroParcela = pg.Where(x => x.Clave == "FILTRO_PARCELA_DESTINO").Select(x => x.Valor).FirstOrDefault();
            var parametroMejora = pg.Where(x => x.Clave == "FILTRO_MEJORA_DESTINO").Select(x => x.Valor).FirstOrDefault();

            model.EdicionParcela.listaUts = GetUTByIdParcela(parcela.ParcelaID);

            if (model.EdicionParcela.listaUts.Count == 0)
            {
                ViewBag.Title = "Sin Unidades Tributarias";
                ViewBag.Description = "La parcela seleccionada no posee Unidad Tributarias Registradas.";
                ViewBag.ReturnUrl = Url.Action("Index", "Home");
                return PartialView("InformationMessageView");

            }
            model.EdicionParcela.listaVal = lista;

            /* 
             * La agrupacion deberia ser por otro campo que NO sea la fecha de alta (un campo de version por ejemplo) porque podria traer 
             * problemas si se da de alta algo por fuera del sistema (TOAD, SQL PLUS o similares)
             * Por ahora queda así, pero en caso de cambiar: REVISAR Y AJUSTAR SEGUN SEA NECESARIO EN TODO EL PROCESO VALUATORIO (TEMPORAL Y CONSOLIDACION INCLUSIVE)
            */

            model.EdicionParcela.listaPadrones = lista.GroupBy(x => new { x.IdPadron, x.Fecha_Alta }, (padron, valuaciones) => valuaciones.FirstOrDefault())
                                                      .OrderByDescending(p => p.Fecha_Vigencia_Desde)
                                                      .ToList();

            model.EdicionParcela.Nomenclatura = GetNomenclatura(parcela.ParcelaID).Nombre;
            List<TipoParcelaModel> tipos = GetTipoParcela();


            model.EdicionParcela.Tipo_Parcela = tipos.Where(t => t.TipoParcelaID == parcela.TipoParcelaID).FirstOrDefault().Descripcion;
            model.EdicionParcela.Id_Parcela = parcela.ParcelaID;
            model.EdicionParcela.Id_Zona_Atributo = parcela.AtributoZonaID;

            List<ValorBasicoMejora> VBM = GetValorBasicoMejora();

            List<ValorBasicoTierraModulo> VBTM = GetValorBasicoTierraModulo();

            List<ValorBasicoTierraSuperficie> VBTS = GetValorBasicoTierraSuperficie();
            List<ValorBasicoTierraSuperficie> VBTSVias = GetValorBasicoTierraSuperficieVias();

            CoeficientesCalculados coeficientes = GetCoeficientesByParcela(parcela.ParcelaID);

            coeficientes = coeficientes ?? new CoeficientesCalculados();

            model.EdicionParcela.C1T = coeficientes.nCoeficiente1T.GetValueOrDefault() == 0 ? 1m : coeficientes.nCoeficiente1T.GetValueOrDefault();
            model.EdicionParcela.C2T = coeficientes.nCoeficiente2T.GetValueOrDefault() == 0 ? 1m : coeficientes.nCoeficiente2T.GetValueOrDefault();
            model.EdicionParcela.C3T = coeficientes.nCoeficiente3T.GetValueOrDefault() == 0 ? 1m : coeficientes.nCoeficiente3T.GetValueOrDefault();
            model.EdicionParcela.C4T = coeficientes.nCoeficiente4T.GetValueOrDefault() == 0 ? 1m : coeficientes.nCoeficiente4T.GetValueOrDefault();
            var coefMejoraDefecto = GetCoeficiente1Individual(System.DateTime.Now.Year);

            model.EdicionParcela.C1M = coefMejoraDefecto;
            model.EdicionParcela.C2M = 1m;
            model.EdicionParcela.C3M = 1m;
            model.EdicionParcela.C4M = 1m;
            List<ValorBasicoStore> vbst = null;

            if (tiposvaluacion.Where(t => t.MetodoValuacion == "TierraPorStore" || t.MetodoValuacion == "MejoraValoresBasicosStore" || t.MetodoValuacion == "SuperficieSemiCubiertaStore").FirstOrDefault() != null)
            {
                vbst = GetValoresBasicosStoreByParcela(parcela.ParcelaID);

            }
            var metodoValuacionTierra = "";
            TipoValuacion tipovalTierra;
            if (pg.Where(x => x.Clave == "FILTRO_PARCELA").Select(x => x.Valor).FirstOrDefault() == "1")
            {
                var properties = parcela.GetType().GetProperty(parametroParcela);
                object value = properties.GetValue(parcela);
                var IdFiltroParcela = value.ToString();

                tipovalTierra = tiposvaluacion.Where(k => k.IdFiltroParcela == IdFiltroParcela && k.Destino == "T").FirstOrDefault();
                if (tipovalTierra == null)
                {
                    ViewBag.Title = "Valuación de Tierra Individual";
                    ViewBag.Description = "No posee configuración para la valuación de tierra para la parcela seleccionada.";
                    ViewBag.ReturnUrl = Url.Action("Index", "Home");
                    return PartialView("InformationMessageView");
                }
                metodoValuacionTierra = tipovalTierra.MetodoValuacion.ToLower();

            }
            else
            {
                //<parche>
                var valor = pg.Where(x => x.Clave == "TIPO_MUNICIPIO").Select(x => x.Valor).FirstOrDefault();
                if (valor == "2")
                {
                    tipovalTierra = tiposvaluacion.Where(k => k.IdFiltroParcela == "0" && k.Destino == "T").FirstOrDefault();
                }
                else
                {
                    tipovalTierra = tiposvaluacion.Where(k => k.IdFiltroParcela == null && k.Destino == "T").FirstOrDefault();
                }
                //</parche>
                if (tipovalTierra == null)
                {
                    ViewBag.Title = "Valuación de Tierra Individual";
                    ViewBag.Description = "No posee configuración para la valuación de tierra para la parcela seleccionada.";
                    ViewBag.ReturnUrl = Url.Action("Index", "Home");
                    return PartialView("InformationMessageView");
                }
                metodoValuacionTierra = tipovalTierra.MetodoValuacion.ToLower();
            }
            var metodoValuacionMejora = "";
            TipoValuacion tipovalMejora;
            var bMejorasPorTipo = pg.Where(x => x.Clave == "FILTRO_MEJORA").Select(x => x.Valor).FirstOrDefault() == "1";
            if (bMejorasPorTipo)
            {
                var properties = parcela.GetType().GetProperty(parametroMejora);
                object value = properties.GetValue(parcela);
                var IdFiltroParcela = value.ToString();
                tipovalMejora = tiposvaluacion.Where(k => k.IdFiltroParcela == IdFiltroParcela && k.Destino == "M").FirstOrDefault();

            }
            else
            {
                //<parche>
                var valor = pg.Where(x => x.Clave == "TIPO_MUNICIPIO").Select(x => x.Valor).FirstOrDefault();
                if (valor == "2")
                {
                    tipovalMejora = tiposvaluacion.Where(k => k.IdFiltroParcela == "0" && k.Destino == "M").FirstOrDefault();
                }
                else
                {
                    tipovalMejora = tiposvaluacion.Where(k => k.IdFiltroParcela == null && k.Destino == "M").FirstOrDefault();
                }
                //</parche>

            }
            if (tipovalMejora != null)
            {
                metodoValuacionMejora = tipovalMejora.MetodoValuacion.ToLower();
                tipovalTierra.TipoParametro1 = tipovalTierra.TipoParametro1 ?? String.Empty;
                tipovalTierra.TipoParametro2 = tipovalTierra.TipoParametro2 ?? String.Empty;
                tipovalMejora.TipoParametro1 = tipovalMejora.TipoParametro1 ?? String.Empty;
                tipovalMejora.TipoParametro2 = tipovalMejora.TipoParametro2 ?? String.Empty;
            }

            model.MetodoValuacionMejora = metodoValuacionMejora;
            if (metodoValuacionTierra == "tierrapormodulo")
            //if (GetTipoValorBasicoTierra().Where(i => i.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault().IdTipoValorBasicoTierra == 2)
            {
                model.EdicionParcela.ValModulos = true;

                //"1" Menor a
                //"2" Mayor a
                //"3" Menor o igual a
                //"4" Mayor o igual a
                //"5" Igual a
                //"6" Entre
                var cantModulos = VBTM.Where(x => decimal.Parse(x.Desde) <= parcela.Superficie && x.Comparador == "6" && x.IdAtributoZona == parcela.AtributoZonaID && decimal.Parse(x.Hasta ?? x.Desde) >= parcela.Superficie && x.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault();
                if (cantModulos == null)
                {
                    cantModulos = VBTM.Where(x => decimal.Parse(x.Desde) == parcela.Superficie && x.Comparador == "5" && x.IdAtributoZona == parcela.AtributoZonaID && x.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault();
                }
                if (cantModulos == null)
                {
                    cantModulos = VBTM.Where(x => decimal.Parse(x.Desde) <= parcela.Superficie && x.Comparador == "4" && x.IdAtributoZona == parcela.AtributoZonaID && x.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault();
                }
                if (cantModulos == null)
                {
                    cantModulos = VBTM.Where(x => decimal.Parse(x.Desde) >= parcela.Superficie && x.Comparador == "3" && x.IdAtributoZona == parcela.AtributoZonaID && x.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault();
                }
                if (cantModulos == null)
                {
                    cantModulos = VBTM.Where(x => decimal.Parse(x.Desde) < parcela.Superficie && x.Comparador == "2" && x.IdAtributoZona == parcela.AtributoZonaID && x.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault();
                }
                if (cantModulos == null)
                {
                    cantModulos = VBTM.Where(x => decimal.Parse(x.Desde) > parcela.Superficie && x.Comparador == "1" && x.IdAtributoZona == parcela.AtributoZonaID && x.IdTipoParcela == parcela.TipoParcelaID).FirstOrDefault();
                }
                if (cantModulos != null)
                {
                    model.EdicionParcela.CantModulos = cantModulos.Modulos;
                }
            }
            else if (metodoValuacionTierra == "tierraporsuperficie" || metodoValuacionTierra == "tierraporsuperficievia")
            {
                //tierraporsuperficie
                //tierrapormodulo
                //tierraporsuperficievia

                ValorBasicoTierraSuperficie mt = null;
                if (metodoValuacionTierra == "tierraporsuperficie")
                {
                    mt = VBTS.Where(x => x.IdTipoParcela == parcela.TipoParcelaID && x.IdAtributoZona == parcela.AtributoZonaID).FirstOrDefault();
                    if (mt == null)
                    {
                        mt = VBTS.Where(x => (x.IdTipoParcela == 0 || x.IdTipoParcela == null) && x.IdAtributoZona == parcela.AtributoZonaID).FirstOrDefault();
                    }
                }
                else
                {
                    int idx = 0;
                    OA.Domicilio dom = null;
                    while (mt == null && idx < lista.Count)
                    {
                        dom = GetDomicilioFisicoByUTId(lista[idx++].IdUnidadTributaria);
                        if (dom != null && dom.numero_puerta != null && Regex.IsMatch(dom.numero_puerta, @"^\d+$"))
                        {
                            long nropuerta = long.Parse(dom.numero_puerta);
                            var paridad = "P";
                            if ((nropuerta % 2) == 0)
                            {
                                paridad = "P";
                            }
                            else
                            {
                                paridad = "I";
                            }
                            var altura = GetTramoViaByIdVia(dom.ViaId ?? 0).Where(z => (z.AlturaDesde) <= nropuerta && (z.AlturaHasta) >= nropuerta && z.Paridad == paridad).FirstOrDefault();
                            mt = VBTSVias.Where(x => x.IdTipoParcela == parcela.TipoParcelaID && (long.Parse(x.Parametro1) == dom.ViaId && long.Parse(x.Parametro2) == altura.TramoViaId)).FirstOrDefault();
                        }
                    }
                }
                if (mt != null) model.EdicionParcela.ValorMetro2 = mt.Valor;
            }
            else
            {
                //tierraporstore
                var valorStore = vbst.Where(r => r.widParcela == parcela.ParcelaID).FirstOrDefault();
                if (valorStore != null)
                {
                    model.EdicionParcela.ValorMetro2 = Convert.ToDecimal(valorStore.ValorBasicoTierra ?? 0f);
                }
                else
                {
                    model.EdicionParcela.ValorMetro2 = 0m;
                }
            }

            model.EdicionParcela.Medida = float.Parse(parcela.Superficie.ToString());

            if (lista != null && lista.Count > 0)
            {
                var valuacionactual = lista.Where(j => j.Fecha_Vigencia_Hasta == null).FirstOrDefault();
                if (valuacionactual != null)
                {
                    model.EdicionParcela.ValorTierra = valuacionactual.ValorTierra;
                    model.EdicionParcela.ValorMejoras = valuacionactual.ValorMejora;
                    model.EdicionParcela.VigenciaDesde = valuacionactual.Fecha_Vigencia_Desde ?? System.DateTime.Now;
                    model.EdicionParcela.VigenciaHasta = valuacionactual.Fecha_Vigencia_Hasta;
                }
            }


            model.EdicionParcela.listaMejoras = GetMejorasByParcelaId(parcela.ParcelaID);



            if (metodoValuacionMejora == "mejoravaloresbasicos" || metodoValuacionMejora == "superficiesemicubierta")
            {
                ViewBag.listaParametro1 = new SelectList(GetTiposMejoras(), "TipoMejoraID", "Descripcion");
                var estadosConservacion = GetEstadosConservacion();
                ViewBag.listaParametro2 = new SelectList(estadosConservacion, "EstadoConservacionID", "Descripcion");
                var estadoconcevacion = estadosConservacion.FirstOrDefault().EstadoConservacionID.ToString();
                var valorbasicoMejora2 = VBM.FirstOrDefault(a => a.Desde == estadoconcevacion);
                if (valorbasicoMejora2 != null)
                {
                    model.EdicionParcela.ValorBasicoMejora = valorbasicoMejora2.Coeficiente;
                    model.EdicionParcela.CoefSemiC = valorbasicoMejora2.Semicubierto;
                }
                ViewBag.Parametro1MejoraNombre = "Tipo";
                ViewBag.Parametro1MejoraTipo = "combo";
                ViewBag.Parametro2MejoraNombre = "Categoría";
                ViewBag.Parametro2MejoraTipo = "combo";

            }
            else
            {
                if (tipovalMejora != null)
                {
                    if (tipovalMejora.TipoParametro1.ToLower() == "combo")
                    {
                        ViewBag.listaParametro1 = new SelectList(GetAtributosParametro1(tipovalMejora.TipoValuacionID), "Key", "Value");
                    }
                    else
                    {
                        ViewBag.listaParametro1 = "";
                    }
                    if (tipovalMejora.TipoParametro2.ToLower() == "combo")
                    {
                        ViewBag.listaParametro2 = new SelectList(GetAtributosParametro2(tipovalMejora.TipoValuacionID), "Key", "Value");
                    }
                    else
                    {
                        ViewBag.listaParametro2 = "";
                    }

                    if (tipovalMejora.TipoParametro1 == "ID_PARCELA" || tipovalMejora.TipoParametro2 == "ID_PARCELA")
                    {
                        tipovalMejora.TipoParametro1 = String.Empty;
                        tipovalMejora.TipoParametro2 = String.Empty;
                    }

                    var valorStore = vbst != null ? vbst.Where(r => r.widParcela == parcela.ParcelaID).FirstOrDefault() : null;

                    if (tipovalMejora.TipoParametro1 == String.Empty && tipovalMejora.TipoParametro2 == String.Empty)
                    {
                        //var valorStore = vbst.Where(r => r.widParcela == parcela.ParcelaID).FirstOrDefault();
                        if (vbst != null)
                        {

                            if (valorStore != null)
                            {
                                if (valorStore.ValorBasicoMejora != null)
                                {
                                    model.EdicionParcela.ValorBasicoMejora = Convert.ToDecimal(valorStore.ValorBasicoMejora ?? 0f);
                                    model.EdicionParcela.CoefSemiC = Convert.ToDecimal(valorStore.Semicubierto ?? 0f);
                                }
                                else
                                {
                                    ValorBasicoStore vbstore = GetValoresBasicosStoreByParametro(parcela.ParcelaID.ToString(), string.Empty);
                                    model.EdicionParcela.ValorBasicoMejora = Convert.ToDecimal(vbstore.ValorBasicoMejora ?? 0f);
                                    model.EdicionParcela.CoefSemiC = Convert.ToDecimal(vbstore.Semicubierto ?? 0f);
                                }
                            }
                            else
                            {
                                ValorBasicoStore vbstore = GetValoresBasicosStoreByParametro(parcela.ParcelaID.ToString(), string.Empty);
                                model.EdicionParcela.ValorBasicoMejora = Convert.ToDecimal(vbstore.ValorBasicoMejora ?? 0f);
                                model.EdicionParcela.CoefSemiC = Convert.ToDecimal(vbstore.Semicubierto ?? 0f);
                            }
                        }
                        else
                        {
                            ValorBasicoStore vbstore = GetValoresBasicosStoreByParametro(parcela.ParcelaID.ToString(), string.Empty);
                            model.EdicionParcela.ValorBasicoMejora = Convert.ToDecimal(vbstore.ValorBasicoMejora ?? 0f);
                            model.EdicionParcela.CoefSemiC = Convert.ToDecimal(vbstore.Semicubierto ?? 0f);
                        }
                    }
                    else
                    {
                        //<parche>
                        if (pg.Where(x => x.Clave == "TIPO_MUNICIPIO").Select(x => x.Valor).FirstOrDefault() == "3")
                        {
                            model.EdicionParcela.ValorBasicoMejora = Convert.ToDecimal(valorStore.ValorBasicoMejora ?? 0f);
                            model.EdicionParcela.CoefSemiC = Convert.ToDecimal(valorStore.Semicubierto ?? 0f);
                        }
                        else
                        {
                            model.EdicionParcela.ValorBasicoMejora = 0m;
                            model.EdicionParcela.CoefSemiC = 0m;
                        }
                        //</parche>

                    }
                    ViewBag.Parametro1MejoraNombre = tipovalMejora.NombreParametro1;
                    ViewBag.Parametro1MejoraTipo = tipovalMejora.TipoParametro1;
                    ViewBag.Parametro2MejoraNombre = tipovalMejora.NombreParametro2;
                    ViewBag.Parametro2MejoraTipo = tipovalMejora.TipoParametro2;
                }
            }

            /* 
             * los coeficientes de mejoras no son por unidades Tributarias, o al menos no es como se pensó, por lo que da igual con qué unidad 
             * tributaria se recuperen los datos, pero NO puede ser CERO(0) ya que la consulta filtra por una UT.... 
             * SI SI YA SE..... 100% COHERENTE!
             */
            var ut = model.EdicionParcela.listaUts.FirstOrDefault();
            ut = ut ?? new UnidadTributaria();
            var coeficientesmejoras = GetCoeficientesMejoras(parcela.ParcelaID, ut.UnidadTributariaId);

            if (ut.UnidadTributariaId > 0)
            {
                model.ResponsablesFiscales = GetResponsablesFiscales(ut.UnidadTributariaId) ?? new ResponsableFiscalViewModel[0];
                foreach (var responsableFiscal in model.ResponsablesFiscales)
                {
                    responsableFiscal.UnidadTributariaId = ut.UnidadTributariaId;
                }
            }


            for (int j = 0; j < model.EdicionParcela.listaMejoras.Count; j++)
            {


                //MejoraValoresBasicos
                //MejoraValoresBasicosStore
                //SinValuacionMejora
                //SuperficieSemiCubierta
                //SuperficieSemiCubiertaStore
                if (metodoValuacionMejora == "mejoravaloresbasicos" || metodoValuacionMejora == "superficiesemicubierta")
                {
                    var valorbasicoMejora = VBM.Where(a => a.Desde == model.EdicionParcela.listaMejoras[j].Parametro2).FirstOrDefault();
                    if (valorbasicoMejora != null)
                    {
                        model.EdicionParcela.listaMejoras[j].ValorBasico = valorbasicoMejora.Coeficiente;
                        model.EdicionParcela.listaMejoras[j].CoeficienteSemi = valorbasicoMejora.Semicubierto;
                    }
                }
                else if (metodoValuacionMejora == "mejoravaloresbasicosstore" || metodoValuacionMejora == "superficiesemicubiertastore")
                {
                    var mejoraid = model.EdicionParcela.listaMejoras[j].MejoraID;
                    ValorBasicoStore vMejora = vbst.Where(r => r.widMejora == mejoraid).FirstOrDefault();
                    model.EdicionParcela.listaMejoras[j].ValorBasico = Convert.ToDecimal(vMejora.ValorBasicoMejora ?? 0f);
                    model.EdicionParcela.listaMejoras[j].CoeficienteSemi = Convert.ToDecimal(vMejora.Semicubierto ?? 0f);
                }
                else
                {
                    model.EdicionParcela.listaMejoras[j].ValorBasico = 0;
                    model.EdicionParcela.listaMejoras[j].CoeficienteSemi = 0;
                    //SinValuacionMejora
                }
                model.EdicionParcela.listaMejoras[j].C1 = 1m;
                model.EdicionParcela.listaMejoras[j].C2 = 1m;
                model.EdicionParcela.listaMejoras[j].C3 = 1m;
                model.EdicionParcela.listaMejoras[j].C4 = 1m;

                if (coeficientesmejoras != null)
                {
                    var coefmejora = coeficientesmejoras.Where(x => x.widMejora == model.EdicionParcela.listaMejoras[j].MejoraID).FirstOrDefault();
                    if (coefmejora != null)
                    {
                        model.EdicionParcela.listaMejoras[j].C1 = coefmejora.nCoeficiente1M.GetValueOrDefault() == 0 ? 1m : coefmejora.nCoeficiente1M.GetValueOrDefault();
                        model.EdicionParcela.listaMejoras[j].C2 = coefmejora.nCoeficiente2M.GetValueOrDefault() == 0 ? 1m : coefmejora.nCoeficiente2M.GetValueOrDefault();
                        model.EdicionParcela.listaMejoras[j].C3 = coefmejora.nCoeficiente3M.GetValueOrDefault() == 0 ? 1m : coefmejora.nCoeficiente3M.GetValueOrDefault();
                        model.EdicionParcela.listaMejoras[j].C4 = coefmejora.nCoeficiente4M.GetValueOrDefault() == 0 ? 1m : coefmejora.nCoeficiente4M.GetValueOrDefault();
                    }
                }
            }

            model.EdicionParcela.Desc_Zona_Atributo = GetAtributosZonaObjeto().Where(w => w.FeatId == parcela.AtributoZonaID).Select(w => w.Nombre).FirstOrDefault();

            return PartialView("EdicionValuacionParcela", model);
        }

        [HttpPost]
        public ActionResult AsignarResponsableFiscal([System.Web.Http.FromBody] ResponsableFiscalViewModel responsableFiscal)
        {
            if (responsableFiscal != null)
            {
                string url = "api/UnidadTributariaPersona/AsignarResponsableFiscal";
                var result = cliente.PostAsJsonAsync<ResponsableFiscalViewModel>(url, responsableFiscal).Result;
                result.EnsureSuccessStatusCode();
                return Json(true);
            }
            return Json(false);
        }


        //METODOS DE ACCESO A LA API ↓

        public ValuacionCoeficientes GetCoeficientesByUT(long UnidadtribitariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesByUT/" + UnidadtribitariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (ValuacionCoeficientes)resp.Content.ReadAsAsync<ValuacionCoeficientes>().Result;
        }
        public CoeficientesCalculados GetCoeficientesByParcela(long idParcela)
        {
            /* Le paso 0 como unidad tributaria porque no lo esta usando y no voy a perder tiempo, ahora, en ver quien mas usa este metodo */
            /* el metodo se creo con dos parametros , ya que el store procedure al final del proceso recibe , parcela y unidad tributaria  , la joda de dejar el parametro UT aca era saber que existe la posibilidad de usarlo */
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesByParcela/?idParcela=" + idParcela + "&idUnidadTributaria=0").Result;
            resp.EnsureSuccessStatusCode();
            return (CoeficientesCalculados)resp.Content.ReadAsAsync<CoeficientesCalculados>().Result;
        }

        public ValorBasicoStore GetValoresBasicosStoreByParametro(String parametro1, String parametro2)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValoresBasicosStoreByParametro/?parametro1=" + parametro1 + "&parametro2=" + parametro2).Result;
            resp.EnsureSuccessStatusCode();
            return (ValorBasicoStore)resp.Content.ReadAsAsync<ValorBasicoStore>().Result;
        }
        public ValorBasicoStore GetValoresBasicosStoreByParametroFull(String parametro1, String parametro2, long anio, float Superficie, float SuperficieSemi)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValoresBasicosStoreByParametroFull/?parametro1=" + parametro1 + "&parametro2=" + parametro2 + "&anio=" + anio
                + "&Superficie=" + Superficie + "&SuperficieSemi=" + Superficie).Result;
            resp.EnsureSuccessStatusCode();
            return (ValorBasicoStore)resp.Content.ReadAsAsync<ValorBasicoStore>().Result;
        }
        public List<ValorBasicoStore> GetValoresBasicosStoreByParcela(long idParcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValoresBasicosStoreByParcela/?idParcela=" + idParcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoStore>)resp.Content.ReadAsAsync<List<ValorBasicoStore>>().Result;
        }
        public List<CoeficientesCalculados> GetCoeficientesMejoras(long idParcela, long unidadTributaria)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesMejoras/?idParcela=" + idParcela + "&idUnidadTributaria=" + unidadTributaria).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoeficientesCalculados>)resp.Content.ReadAsAsync<List<CoeficientesCalculados>>().Result;
        }
        public Nomenclatura GetNomenclatura(long idParcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetNomenclatura/" + idParcela).Result;
            resp.EnsureSuccessStatusCode();
            return (Nomenclatura)resp.Content.ReadAsAsync<Nomenclatura>().Result;
        }
        public List<TiposMejoras> GetTiposMejoras()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTiposMejoras").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposMejoras>)resp.Content.ReadAsAsync<List<TiposMejoras>>().Result;
        }

        public List<EstadosConservacion> GetEstadosConservacion()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetEstadosConservacion").Result;
            resp.EnsureSuccessStatusCode();
            return (List<EstadosConservacion>)resp.Content.ReadAsAsync<List<EstadosConservacion>>().Result;
        }


        public Valuacion GetValuacionByUTId(long UnidadTributariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionByUTId/" + UnidadTributariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (Valuacion)resp.Content.ReadAsAsync<Valuacion>().Result;
        }
        public List<ValuacionMejoraModel> GetMejorasByParcelaId(long ParcelaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetMejorasByParcelaId/" + ParcelaID).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionMejoraModel>)resp.Content.ReadAsAsync<List<ValuacionMejoraModel>>().Result;
        }
        public List<ValuacionMejoraModel> GetMejorasByUTId(long UnidadTributariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetMejorasByUTId/" + UnidadTributariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionMejoraModel>)resp.Content.ReadAsAsync<List<ValuacionMejoraModel>>().Result;
        }

        public Persona GetPersonaByDomicilioId(long DomicilioId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetPersonaByDomicilioId/" + DomicilioId).Result;
            resp.EnsureSuccessStatusCode();
            return (Persona)resp.Content.ReadAsAsync<Persona>().Result;
        }
        public OA.Domicilio GetDomicilioFisicoByUTId(long UnidadTributariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetDomicilioFisicoByUTId/" + UnidadTributariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (OA.Domicilio)resp.Content.ReadAsAsync<OA.Domicilio>().Result;
        }
        public OA.Domicilio GetDomicilioFiscalByUTId(long UnidadTributariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetDomicilioFiscalByUTId/" + UnidadTributariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (OA.Domicilio)resp.Content.ReadAsAsync<OA.Domicilio>().Result;
        }
        public List<ValuacionPadronDetalle> GetValuacionPadronDetalleByIdPadron(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionPadronDetalleByIdPadron/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionPadronDetalle>)resp.Content.ReadAsAsync<List<ValuacionPadronDetalle>>().Result;
        }

        public UnidadTributaria GetUnidadTributariaById(long UnidadTributariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetUTById/" + UnidadTributariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (UnidadTributaria)resp.Content.ReadAsAsync<UnidadTributaria>().Result;
        }

        public List<Valuacion> GetValuacionesByIdParcela(long ParcelaId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionesByIdParcela/" + ParcelaId).Result;
            resp.EnsureSuccessStatusCode();
            return (List<Valuacion>)resp.Content.ReadAsAsync<List<Valuacion>>().Result;
        }
        public List<Valuacion> GetValuacionesByUTId(long UnidadTributariaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionesByUTId/" + UnidadTributariaID).Result;
            resp.EnsureSuccessStatusCode();
            return (List<Valuacion>)resp.Content.ReadAsAsync<List<Valuacion>>().Result;
        }
        public Parcela GetParcelaById(long idparcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetParcelaById/" + idparcela).Result;
            resp.EnsureSuccessStatusCode();
            return (Parcela)resp.Content.ReadAsAsync<Parcela>().Result;
        }
        public List<UnidadTributaria> GetUTByIdParcela(long idparcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetUTByIdParcela/" + idparcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<UnidadTributaria>)resp.Content.ReadAsAsync<List<UnidadTributaria>>().Result;
        }
        public List<ValuacionPadronCabecera> GetValuacionPadronCabeceraByIdParcela(long idparcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionPadronCabeceraByIdParcela/" + idparcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionPadronCabecera>)resp.Content.ReadAsAsync<List<ValuacionPadronCabecera>>().Result;
        }
        public List<TipoParcelaModel> GetTipoParcela()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoParcelas").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoParcelaModel>)resp.Content.ReadAsAsync<List<TipoParcelaModel>>().Result;
        }
        public Dictionary<string, string> GetFiltroValuacionTierra()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetFiltroValuacionTierra").Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;
        }
        public Dictionary<string, string> GetFiltroValuacionMejora()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetFiltroValuacionMejora").Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;
        }

        public List<ValuacionParcela> GetValuacionParcelaBuscar(ValuacionParcela model)
        {

            //List<ParcelaModel> lista = new List<ParcelaModel>();

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/GetValuacionParcelaBuscar", model).Result;
            resp.EnsureSuccessStatusCode();
            //return (List<ParcelaInmueble>)resp.Content.ReadAsAsync<List<ParcelaInmueble>>().Result;

            List<ValuacionParcela> parcelas = (List<ValuacionParcela>)resp.Content.ReadAsAsync<List<ValuacionParcela>>().Result;
            if (parcelas == null)
            {
                parcelas = new List<ValuacionParcela>();
            }
            return parcelas;

        }

        public ResponsableFiscalViewModel[] GetResponsablesFiscales(long idUnidadTributaria)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/UnidadTributariaPersona/Get?idUnidadTributaria=" + idUnidadTributaria).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<ResponsableFiscalViewModel[]>().Result;
        }

        public List<ParcelaModel> GetParcela()
        {

            List<ParcelaModel> lista = new List<ParcelaModel>();

            List<Parcela> parcelas = GetParcelas();
            var BusquedaParcela = new ParcelaModel();
            foreach (var parcela in parcelas)
            {
                //BusquedaParcela.Id_Parcela = parcela.IdParcela;
                BusquedaParcela.Tipo_Parcela = parcela.TipoParcelaID;
                BusquedaParcela.Id_Inmueble = parcela.ParcelaID;
                BusquedaParcela.Partida = "0001212/0001";
                BusquedaParcela.Ejido = "";
                BusquedaParcela.Sector = "";
                BusquedaParcela.Fraccion = "";
                BusquedaParcela.Circunscripcion = "";
                BusquedaParcela.Manzana = "";
                BusquedaParcela.Parcela = parcela.Atributos;
                BusquedaParcela.Nombre = "Nombre";
                BusquedaParcela.Apellido = "Apellido";

                lista.Add(BusquedaParcela);
            }

            return lista;

            //HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetTipoDoc").Result;
            //resp.EnsureSuccessStatusCode();
            //return (List<TipoDocModel>)resp.Content.ReadAsAsync<IEnumerable<TipoDocModel>>().Result;
        }

        public ActionResult GrabaValuacionParcela(ValuacionModel model)
        {
            /***************************************** ESTE METODO ES UN ASCO *****************************************/
            /* TO-DO: 
             * PASAR LA LOGICA A LA API Y HACER QUE TANTO EL MASIVO TEMPORAL COMO LA INDIVIDUAL USEN EL MISMO METODO.  
             * POR AHORA LO ADAPTÉ UN POCO PARA ARREGLAR UN PAR DE COSAS, PERO EL SERRUCHO Y EL CAMBIO GRUESO SE DEJA 
             * PARA MAS ADELANTE. HAY QUE SER MUY CABEZA PARA METER TODA LA LOGICA DE VALUACION EN EL CLIENTE Y AUN MAS 
             * TENERLA EN 2 LUGARES DISTINTOS!!!!!!!!!!!!                                                                       
             * POR SI FUERA POCO, LA VALUACION Y LAS MEJORAS SE HACEN EN TRANSACCIONES DIFERENTES. SIMPLEMENTE GENIAL,  
             * COMO SE GENERE ALGUN ERROR EN LA GRABACION DE LAS MEJORAS, LAS VALUACIONES QUEDAN EN ESTADO INCONSISTENTE
             */

            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long idUsuario = 0;
            if (usuario != null)
            {
                idUsuario = usuario.Id_Usuario;
            }

            ValuacionPadronDetalle val;
            OA.Domicilio domicilioFisico;
            OA.Domicilio domicilioFiscal;
            Parcela parcela = GetParcelaById(model.EdicionParcela.Id_Parcela);
            List<UnidadTributaria> utList = GetUTByIdParcela(parcela.ParcelaID);
            List<ValuacionPadronDetalle> detalles = new List<ValuacionPadronDetalle>();
            for (int i = 0; i < utList.Count; i++)
            {
                val = new ValuacionPadronDetalle();

                domicilioFisico = GetDomicilioFisicoByUTId(utList[i].UnidadTributariaId);
                domicilioFiscal = GetDomicilioFiscalByUTId(utList[i].UnidadTributariaId);

                if (domicilioFisico != null) val.DomicilioInmueble = domicilioFisico.ViaNombre;
                if (domicilioFiscal != null) val.DomicilioFiscal = domicilioFiscal.ViaNombre;

                val.IdUnidadTributaria = utList[i].UnidadTributariaId;
                val.ValorTierra = model.EdicionParcela.ValorTierra;
                val.ValorMejora = model.EdicionParcela.ValorMejoras;
                val.ValorMejoraPropiedad = model.EdicionParcela.ValorMejoras * ((utList[i].PorcentajeCopropiedad == 0 ? 100m : utList[i].PorcentajeCopropiedad) / 100m);
                val.ValorTierraPropiedad = model.EdicionParcela.ValorTierra * ((utList[i].PorcentajeCopropiedad == 0 ? 100m : utList[i].PorcentajeCopropiedad) / 100m);
                val.ValorTotal = model.EdicionParcela.ValorTierra + model.EdicionParcela.ValorMejoras;
                val.IdPadron = model.EdicionParcela.IdPadron;
                val.IdParcela = model.EdicionParcela.Id_Parcela;
                val.IdAtributoZona = model.EdicionParcela.Id_Zona_Atributo;
                val.TipoParcela = parcela.TipoParcelaID.ToString();
                val.PartidaProvincial = utList[i].CodigoProvincial;
                val.PartidaMunicipal = utList[i].CodigoMunicipal;
                val.PorcentajeCodominio = float.Parse(utList[i].PorcentajeCopropiedad.ToString());
                val.SuperficieTierra = float.Parse(parcela.Superficie.ToString());
                if (model.EdicionParcela.listaMejoras != null)
                {
                    val.SuperificeCubierta = float.Parse(model.EdicionParcela.listaMejoras.Sum(x => x.Medida) + "");
                    val.SuperficieSemiCubierta = float.Parse(model.EdicionParcela.listaMejoras.Sum(x => x.MedidaSemiCubierta) + "");
                }
                val.Fecha_Vigencia_Hasta = null;
                val.Usuario_Alta = idUsuario;

                detalles.Add(val);
                ValuacionComarcal(parcela.ParcelaID, utList[i].CodigoMunicipal, val);
            }
            SetValuacionPadronDetalle(detalles);


            if (model.EdicionParcela.listaMejoras != null)
            {
                List<Mejora> mejoras = new List<Mejora>();
                Mejora m;
                foreach (var item in model.EdicionParcela.listaMejoras)
                {
                    m = new Mejora();
                    m.MejoraID = item.MejoraID;
                    m.ParcelaID = model.EdicionParcela.Id_Parcela;
                    m.SubCategoriaID = 1; //esto queda hardcodeado. no se para que se usa pero asi estaba
                    m.Parametro1 = item.Parametro1;
                    m.Parametro2 = item.Parametro2;
                    if (model.EdicionParcela.Tipo_Parcela == "URBANA")
                    {
                        m.UnidadMedida = "mts";
                    }
                    else
                    {
                        m.UnidadMedida = "ha";
                    }
                    m.Medida = item.Medida;
                    m.MedidaSemiCubierta = item.MedidaSemiCubierta;
                    m.Anio = item.Anio;
                    mejoras.Add(m);
                }
                MejorasGrabar(mejoras);
            }
            model.Mensaje = "OK";
            return RedirectToAction("DetalleValuacionParcela", new { idParcela = model.EdicionParcela.Id_Parcela, Mensaje = model.Mensaje });
        }

        private string ValuacionComarcal(long parcelaId, string partida, ValuacionPadronDetalle val)
        {
            int vigencia = 0;
            if (val.Fecha_Vigencia_Desde.HasValue)
            {
                vigencia = Convert.ToInt32(string.Format("{0:0000}{1:00}", val.Fecha_Vigencia_Desde.Value.Year, val.Fecha_Vigencia_Desde.Value.Month));
            }
            var parametros = new Dictionary<string, object>
            {
                { "parcelaId", parcelaId },
                { "partida", partida },
                { "vigencia", vigencia },
                { "avalt", Convert.ToString(val.ValorTierra, CultureInfo.InvariantCulture) },
                { "avalm", Convert.ToString(val.ValorTotal, CultureInfo.InvariantCulture) },
                { "supt", Convert.ToString(val.SuperficieTierra, CultureInfo.InvariantCulture) },
                { "supm", Convert.ToString(val.SuperificeCubierta, CultureInfo.InvariantCulture) }
            };
            string queryString = string.Join("&", parametros.Select(x => string.Format("{0}={1}", x.Key, x.Value)).ToArray());
            var result = cliente.PostAsync("api/InterfaseRentas/Avaluo?" + queryString, new StringContent(string.Empty)).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<string>().Result;
        }

        public ActionResult GrabaMejorasValoresBasicos(ValuacionModel model)
        {
            List<ValorBasicoMejora> lista = new List<ValorBasicoMejora>();
            ValorBasicoMejora vbts;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long idUsuario = 0;
            if (usuario != null)
            {
                idUsuario = usuario.Id_Usuario;
            }
            for (int i = 0; i < model.MejoraValoresBasicos.ValorBasicoMejora1.Count() - 1; i++)
            {

                vbts = new ValorBasicoMejora();
                vbts.Desde = model.MejoraValoresBasicos.ValorBasicoMejora1[i];
                if (model.MejoraValoresBasicos.ValorBasicoMejora2.Count > 0)
                {
                    vbts.Hasta = model.MejoraValoresBasicos.ValorBasicoMejora2[i];
                }
                vbts.Coeficiente = model.MejoraValoresBasicos.ValorBasicoMejora3[i];
                vbts.Fecha_Alta = System.DateTime.Now;
                vbts.IdTipoValorBasicoMejora = 1;
                vbts.Semicubierto = model.SuperficieSemiCubiertaPorcentaje == 0m ? 100m : model.SuperficieSemiCubiertaPorcentaje;
                vbts.Usuario_Alta = idUsuario;
                if (vbts.Coeficiente != 0)
                {
                    lista.Add(vbts);
                }
            }

            MejorasValoresBasicosGrabar(lista);

            model.Mensaje = "OK";
            return RedirectToAction("MejoraValoresBasicos", new { Mensaje = model.Mensaje });

        }

        //public ActionResult GrabaRangoCoeficiente(ValuacionModel model)
        //{
        //    List<CoeficientesTierra> lista = new List<CoeficientesTierra>();
        //    CoeficientesTierra vct;
        //    var usuario = ((UsuariosModel)Session["usuarioPortal"]);
        //    long idUsuario = 0;
        //    if (usuario != null)
        //    {
        //        idUsuario = usuario.Id_Usuario;
        //    }
        //    var nroCoeficiente = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra;
        //    var descCoeficiente = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra;
        //    Coeficientes coef = new Coeficientes();
        //    coef.Descripcion = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra;
        //    coef.Nro_Coeficiente = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra;

        //    coef.Id_Tipo_Coeficiente = 3;
        //    coef = CoeficientesGrabar(coef);
        //    for (int i = 0; i < model.RangoCoeficiente.Rango1Coeficiente.Count() - 1; i++)
        //    {
        //        if (model.RangoCoeficiente.Rango1Coeficiente[i] != "" && model.RangoCoeficiente.Rango3Coeficiente[i] > 0)
        //        {
        //            vct = new CoeficientesTierra();

        //            vct.Coeficiente = float.Parse(model.RangoCoeficiente.Rango3Coeficiente[i] + "");
        //            vct.Id_Coeficiente = coef.Id_Coeficiente;
        //            vct.Desde = model.RangoCoeficiente.Rango1Coeficiente[i];
        //            vct.Hasta = model.RangoCoeficiente.Rango2Coeficiente[i];
        //            vct.Fecha_Alta = System.DateTime.Now;
        //            vct.Usuario_Alta = idUsuario;
        //            lista.Add(vct);
        //        }
        //    }

        //    ValorCoeficienteTierraGrabar(lista);

        //    model.Mensaje = "OK";
        //    return RedirectToAction("RangoCoeficiente", new { Mensaje = model.Mensaje, IdMetadatoCoeficienteTierra = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra, IdTipoCoeficienteTierra = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra });

        //}

        public ActionResult GrabaRangoCoeficienteMejora(ValuacionModel model)
        {

            List<CoeficientesMejora> lista = new List<CoeficientesMejora>();
            CoeficientesMejora vct;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long idUsuario = 0;
            if (usuario != null)
            {
                idUsuario = usuario.Id_Usuario;
            }
            var nroCoeficiente = model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente;
            var descCoeficiente = model.TipoCoeficienteMejora.IdTipoCoeficienteMejora;
            Coeficientes coef = new Coeficientes();
            coef.Descripcion = model.TipoCoeficienteMejora.IdTipoCoeficienteMejora;
            coef.Nro_Coeficiente = model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente;

            coef.Id_Tipo_Coeficiente = 4;
            coef = CoeficientesGrabar(coef);
            for (int i = 0; i < model.RangoCoeficienteMejora.Rango1CoeficienteMejora.Count() - 1; i++)
            {
                if (model.RangoCoeficienteMejora.Rango1CoeficienteMejora[i] != "" && model.RangoCoeficienteMejora.Rango3CoeficienteMejora[i] > 0)
                {
                    vct = new CoeficientesMejora();

                    vct.Coeficiente = float.Parse(model.RangoCoeficienteMejora.Rango3CoeficienteMejora[i] + "");
                    vct.Id_Coeficiente = coef.Id_Coeficiente;
                    vct.Desde = model.RangoCoeficienteMejora.Rango1CoeficienteMejora[i];
                    vct.Hasta = model.RangoCoeficienteMejora.Rango2CoeficienteMejora[i];
                    vct.Fecha_Alta = System.DateTime.Now;
                    vct.Usuario_Alta = idUsuario;
                    lista.Add(vct);
                }
            }

            ValorCoeficienteMejoraGrabar(lista);
            model.Mensaje = "OK";
            return RedirectToAction("RangoCoeficienteMejora", new { Mensaje = model.Mensaje, IdMejoraCoeficiente = model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente, IdTipoCoeficienteMejora = model.TipoCoeficienteMejora.IdTipoCoeficienteMejora });

        }

        public ActionResult GrabaTierraPorSuperficie(ValuacionModel model, string returnUrl)
        {

            List<ValorBasicoTierraSuperficie> lista = new List<ValorBasicoTierraSuperficie>();
            ValorBasicoTierraSuperficie vbts;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long idUsuario = 0;
            if (usuario != null)
            {
                idUsuario = usuario.Id_Usuario;
            }
            for (int i = 0; i < model.TierraPorSuperficie.Zona.Count(); i++)
            {
                if (model.TierraPorSuperficie.Zona[i] > 0 && model.TierraPorSuperficie.Valor[i] > 0)
                {
                    vbts = new ValorBasicoTierraSuperficie();
                    vbts.IdAtributoZona = model.TierraPorSuperficie.Zona[i];
                    vbts.Valor = model.TierraPorSuperficie.Valor[i];
                    vbts.IdTipoValorBasicoTierra = 1;
                    vbts.Fecha_Alta = System.DateTime.Now;
                    vbts.Usuario_Alta = idUsuario;
                    //if (model.IdFiltroParcela != null)
                    //{
                    //    vbts.IdTipoParcela = long.Parse(model.IdFiltroParcela);
                    //}
                    vbts.IdTipoParcela = long.Parse(model.IdFiltroParcela ?? "0");
                    lista.Add(vbts);
                }
            }

            TierraPorSuperficieGrabar(lista);


            //return RedirectToAction("ValuacionPadronTemporal", model);
            model.Mensaje = "OK";
            return RedirectToAction("TierraPorSuperficie", new { Mensaje = model.Mensaje, IdFiltroParcela = model.IdFiltroParcela });

        }

        public ActionResult GrabaTierraPorSuperficieVia(ValuacionModel model, string returnUrl)
        {

            List<ValorBasicoTierraSuperficie> lista = new List<ValorBasicoTierraSuperficie>();
            ValorBasicoTierraSuperficie vbts;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long idUsuario = 0;
            if (usuario != null)
            {
                idUsuario = usuario.Id_Usuario;
            }
            for (int i = 0; i < model.TierraPorSuperficie.Via.Count(); i++)
            {
                if (model.TierraPorSuperficie.Valor[i] > 0)
                {
                    vbts = new ValorBasicoTierraSuperficie();
                    vbts.Parametro1 = model.TierraPorSuperficie.Via[i];
                    vbts.Parametro2 = model.TierraPorSuperficie.Eje_Via[i];
                    vbts.IdTipoValorBasicoTierra = 1;
                    vbts.Valor = model.TierraPorSuperficie.Valor[i];
                    vbts.Fecha_Alta = System.DateTime.Now;
                    vbts.Usuario_Alta = idUsuario;
                    vbts.IdTipoParcela = long.Parse((model.IdFiltroParcela ?? "0"));
                    //if (model.IdFiltroParcela != null)
                    //{
                    //    vbts.IdTipoParcela = long.Parse(model.IdFiltroParcela);
                    //}
                    lista.Add(vbts);
                }
            }

            TierraPorSuperficieGrabarVia(lista);

            //List<ValorBasicoTierraSuperficie> lista = GetValorBasicoTierraSuperficieVias();
            //foreach (var VBT in lista)
            //{

            //    model.TierraPorSuperficie.Zona.Add(VBT.IdAtributoZona);
            //    model.TierraPorSuperficie.Valor.Add(VBT.Valor);
            //    model.TierraPorSuperficie.Via.Add(VBT.Parametro1);
            //    model.TierraPorSuperficie.Eje_Via.Add(VBT.Parametro2);
            //}

            //ViewBag.listaVias = GetVias();
            //return PartialView("TierraPorSuperficieVia", model);

            //return RedirectToAction("ValuacionPadronTemporal", model);
            model.Mensaje = "OK";
            return RedirectToAction("TierraPorSuperficieVia", new { Mensaje = model.Mensaje, IdFiltroParcela = model.IdFiltroParcela });

        }
        public ActionResult GrabaTierraPorStore(ValuacionModel model, string returnUrl)
        {

            List<ValorBasicoTierraStore> lista = new List<ValorBasicoTierraStore>();
            ValorBasicoTierraStore vbtst;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();
            for (int i = 0; i < model.TierraPorStore.Cantidad.Count(); i++)
            {
                if (model.TierraPorStore.Cantidad[i] != string.Empty)
                {
                    vbtst = new ValorBasicoTierraStore();
                    if (model.TierraPorStore.ValorComparacion1.Count() > 0)
                    {
                        vbtst.Comparador1 = model.TierraPorStore.ValorComparacion1[i];

                        if (model.TierraPorStore.ValorComparacion1[i] == "6")
                        {
                            vbtst.Parametro1Desde = model.TierraPorStore.Parametro1Desde[i];
                            vbtst.Parametro1Hasta = model.TierraPorStore.Parametro1Hasta[i];
                        }
                        else
                        {
                            vbtst.Parametro1Desde = model.TierraPorStore.Parametro1[i];
                        }
                    }
                    else
                    {
                        if (model.TierraPorStore.Parametro1.Count() > 0)
                        {
                            vbtst.Parametro1Desde = model.TierraPorStore.Parametro1[i];
                        }
                    }

                    if (model.TierraPorStore.ValorComparacion2.Count() > 0)
                    {
                        vbtst.Comparador2 = model.TierraPorStore.ValorComparacion2[i];

                        if (model.TierraPorStore.ValorComparacion2[i] == "6")
                        {
                            vbtst.Parametro2Desde = model.TierraPorStore.Parametro2Desde[i];
                            vbtst.Parametro2Hasta = model.TierraPorStore.Parametro2Hasta[i];
                        }
                        else
                        {
                            vbtst.Parametro2Desde = model.TierraPorStore.Parametro2[i];
                        }
                    }
                    else
                    {

                        if (model.TierraPorStore.Parametro2.Count() > 0)
                        {
                            vbtst.Parametro2Desde = model.TierraPorStore.Parametro2[i];
                        }
                    }
                    vbtst.IdTipoValuacion = model.TipoValuacionId;
                    vbtst.Valor = float.Parse(model.TierraPorStore.Cantidad[i]);
                    vbtst.Fecha_Alta = System.DateTime.Now;
                    vbtst.Usuario_Alta = usuario.Id_Usuario;
                    vbtst.IdFiltroParcela = model.IdFiltroParcela ?? "0";
                    lista.Add(vbtst);
                }
            }

            TierraPorStoreGrabar(lista);


            model.Mensaje = "OK";
            return RedirectToAction("TierraPorStore", new { Mensaje = model.Mensaje, IdFiltroParcela = model.IdFiltroParcela });

        }
        public ActionResult GrabaMejoraPorStore(ValuacionModel model, string returnUrl)
        {

            List<ValorBasicoMejoraStore> lista = new List<ValorBasicoMejoraStore>();
            ValorBasicoMejoraStore vbmst;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();
            for (int i = 0; i < model.MejoraPorStore.Cantidad.Count(); i++)
            {
                if (model.MejoraPorStore.Cantidad[i] != string.Empty)
                {
                    vbmst = new ValorBasicoMejoraStore();
                    if (model.MejoraPorStore.ValorComparacion1.Count() > 0)
                    {
                        vbmst.Comparador1 = model.MejoraPorStore.ValorComparacion1[i];

                        if (model.MejoraPorStore.ValorComparacion1[i] == "6")
                        {
                            vbmst.Parametro1Desde = model.MejoraPorStore.Parametro1Desde[i];
                            vbmst.Parametro1Hasta = model.MejoraPorStore.Parametro1Hasta[i];
                        }
                        else
                        {
                            vbmst.Parametro1Desde = model.MejoraPorStore.Parametro1[i];
                        }
                    }
                    else
                    {
                        if (model.MejoraPorStore.Parametro1.Count() > 0)
                        {
                            vbmst.Parametro1Desde = model.MejoraPorStore.Parametro1[i];
                        }
                    }

                    if (model.MejoraPorStore.ValorComparacion2.Count() > 0)
                    {
                        vbmst.Comparador2 = model.MejoraPorStore.ValorComparacion2[i];

                        if (model.TierraPorStore.ValorComparacion2[i] == "6")
                        {
                            vbmst.Parametro2Desde = model.MejoraPorStore.Parametro2Desde[i];
                            vbmst.Parametro2Hasta = model.MejoraPorStore.Parametro2Hasta[i];
                        }
                        else
                        {
                            vbmst.Parametro2Desde = model.MejoraPorStore.Parametro2[i];
                        }
                    }
                    else
                    {
                        if (model.MejoraPorStore.Parametro2.Count() > 0)
                        {
                            vbmst.Parametro2Desde = model.MejoraPorStore.Parametro2[i];
                        }
                    }
                    vbmst.Semicubierto = model.SuperficieSemiCubiertaPorcentaje;
                    vbmst.IdTipoValuacion = model.TipoValuacionId;
                    vbmst.Valor = float.Parse(model.MejoraPorStore.Cantidad[i]);
                    vbmst.Fecha_Alta = System.DateTime.Now;
                    vbmst.Usuario_Alta = usuario.Id_Usuario;
                    vbmst.IdFiltroParcela = model.IdFiltroParcela ?? "0";
                    lista.Add(vbmst);
                }
            }

            MejoraPorStoreGrabar(lista);


            model.Mensaje = "OK";
            return RedirectToAction("MejoraValoresBasicosStore", model);

        }

        public ActionResult GrabaTierraPorModulo(ValuacionModel model)
        {
            List<ValorBasicoTierraModulo> lista = new List<ValorBasicoTierraModulo>();
            ValorBasicoTierraModulo vbtm;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            long idUsuario = 0;
            if (usuario != null)
            {
                idUsuario = usuario.Id_Usuario;
            }
            for (int i = 0; i < model.TierraPorModulo.Zona.Count() - 1; i++)
            {
                if (model.TierraPorModulo.Zona[i] > 0 && model.TierraPorModulo.CantidadModulos[i] > 0)
                {
                    vbtm = new ValorBasicoTierraModulo();
                    vbtm.IdAtributoZona = model.TierraPorModulo.Zona[i];
                    vbtm.Modulos = model.TierraPorModulo.CantidadModulos[i];
                    /*
                     * ERNESTO.- Como dijo el enorme Tato: "Sería un chiste si no fuera una joda grande como una casa"
                     * Es gracioso que para la carga del modelo hagan esto:
                            model.TierraPorModulo.Superficie.Add(VBM.Desde);
                            model.TierraPorModulo.SuperficieDesde.Add(VBM.Desde);

                     * y que para la determinacion del tipo de dato a grabar hagan esto:
                            vbtm.Desde = model.TierraPorModulo.SuperficieDesde[i] == "" ? model.TierraPorModulo.Superficie[i] : model.TierraPorModulo.SuperficieDesde[i];
                     * 
                     * A PROGRAMAR A LO CABEZA NO ME GANAN!
                     * ES UN ASCO, PERO LO HAGO ASI O TENGO QUE MATAR A QUIEN LO HIZO
                     */
                    vbtm.Desde = model.TierraPorModulo.ValorComparacion[i] == "6" /*ES OPERADOR <<ENTRE>>*/ ? model.TierraPorModulo.SuperficieDesde[i] : model.TierraPorModulo.Superficie[i];
                    /*
                     * FIN DE CODIGO CABEZA....¿FIN?
                     */


                    vbtm.Hasta = model.TierraPorModulo.SuperficieHasta[i];
                    vbtm.Comparador = model.TierraPorModulo.ValorComparacion[i];
                    vbtm.IdTipoValorBasicoTierra = 2;
                    vbtm.IdTipoParcela = long.Parse(model.IdFiltroParcela ?? "0");
                    //if (model.IdFiltroParcela != null)
                    //{
                    //    vbtm.IdTipoParcela = long.Parse(model.IdFiltroParcela);
                    //}
                    vbtm.Fecha_Alta = System.DateTime.Now;
                    vbtm.Usuario_Alta = idUsuario;
                    lista.Add(vbtm);
                }
            }

            TierraPorModuloGrabar(lista);

            model.Mensaje = "OK";
            return RedirectToAction("TierraPorModulo", new { Mensaje = model.Mensaje, IdFiltroParcela = model.IdFiltroParcela });

        }

        public ActionResult GrabaSuperficieSemiCubierta(ValuacionModel model)
        {

            //ValorBasicoMejora vbm = new ValorBasicoMejora(); ;
            //var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            //long idUsuario = 0;
            //if (usuario != null)
            //{
            //    idUsuario = usuario.Id_Usuario;
            //}

            //vbm.Semicubierto = model.SuperficieSemiCubierta.SuperficieSemiCubiertaPorcentaje;
            //SuperficieSemiCubiertaGrabar(vbm);
            TipoValuacion tv = GetTipoValuacionByID(model.TipoValuacionId);

            if (tv.MetodoValuacion.ToLower() == "superficiesemicubierta")
            {
                return RedirectToAction("MejoraValoresBasicos", model);
            }
            else
            {
                return RedirectToAction("MejoraValoresBasicosStore", model);
            }
            //model.Mensaje = "OK";
            //return RedirectToAction("MejoraValoresBasicos", model);


        }

        public ActionResult GrabaCoeficienteTierra(ValuacionModel model)
        {

            List<CoeficientesStore> lista = new List<CoeficientesStore>();
            CoeficientesStore coefStore;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();
            Coeficientes coef = new Coeficientes();
            coef.Descripcion = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra;
            coef.Nro_Coeficiente = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra;

            coef.Id_Tipo_Coeficiente = 1;
            coef = CoeficientesGrabar(coef);
            for (int i = 0; i < model.CoeficienteStore.Cantidad.Count(); i++)
            {
                if (model.CoeficienteStore.Cantidad[i] != string.Empty)
                {
                    coefStore = new CoeficientesStore();
                    if (model.CoeficienteStore.ValorComparacion1.Count() > 0)
                    {
                        coefStore.Comparador1 = model.CoeficienteStore.ValorComparacion1[i];

                        if (model.CoeficienteStore.ValorComparacion1[i] == "6")
                        {
                            coefStore.Parametro1Desde = model.CoeficienteStore.Parametro1Desde[i];
                            coefStore.Parametro1Hasta = model.CoeficienteStore.Parametro1Hasta[i];
                        }
                        else
                        {
                            coefStore.Parametro1Desde = model.CoeficienteStore.Parametro1[i];
                        }
                    }
                    else
                    {
                        if (model.CoeficienteStore.Parametro1.Count() > 0)
                        {
                            coefStore.Parametro1Desde = model.CoeficienteStore.Parametro1[i];
                        }
                    }

                    if (model.CoeficienteStore.ValorComparacion2.Count() > 0)
                    {
                        coefStore.Comparador2 = model.CoeficienteStore.ValorComparacion2[i];

                        if (model.CoeficienteStore.ValorComparacion2[i] == "6")
                        {
                            coefStore.Parametro2Desde = model.CoeficienteStore.Parametro2Desde[i];
                            coefStore.Parametro2Hasta = model.CoeficienteStore.Parametro2Hasta[i];
                        }
                        else
                        {
                            coefStore.Parametro2Desde = model.CoeficienteStore.Parametro2[i];
                        }
                    }
                    else
                    {

                        if (model.CoeficienteStore.Parametro2.Count() > 0)
                        {
                            coefStore.Parametro2Desde = model.CoeficienteStore.Parametro2[i];
                        }
                    }
                    coefStore.IdTipoValuacion = model.TipoValuacionId;
                    coefStore.Valor = float.Parse(model.CoeficienteStore.Cantidad[i]);
                    coefStore.Fecha_Alta = System.DateTime.Now;
                    coefStore.Usuario_Alta = usuario.Id_Usuario;
                    coefStore.TipoCoeficiente = "T";
                    coefStore.NroCoeficiente = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra;
                    coefStore.IdCoeficiente = coef.Id_Coeficiente;
                    lista.Add(coefStore);
                }
            }

            CoeficienteStoreGrabar(lista);

            //List<CoeficientesTierra> lista = new List<CoeficientesTierra>();
            //CoeficientesTierra vct;
            //var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            //long idUsuario = 0;
            //if (usuario != null)
            //{
            //    idUsuario = usuario.Id_Usuario;
            //}
            //var nroCoeficiente = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra;
            //var descCoeficiente = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra;
            //Coeficientes coef = new Coeficientes();
            //coef.Descripcion = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra;
            //coef.Nro_Coeficiente = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra;

            //coef.Id_Tipo_Coeficiente = 1;
            //coef = CoeficientesGrabar(coef);
            //for (int i = 0; i < model.ValorCoeficiente.Valor1Coeficiente.Count() - 1; i++)
            //{
            //    if (model.ValorCoeficiente.Valor1Coeficiente[i] != "" && model.ValorCoeficiente.Valor3Coeficiente[i] > 0)
            //    {
            //        vct = new CoeficientesTierra();

            //        vct.Coeficiente = float.Parse(model.ValorCoeficiente.Valor3Coeficiente[i] + "");
            //        vct.Id_Coeficiente = coef.Id_Coeficiente;
            //        vct.Desde = model.ValorCoeficiente.Valor1Coeficiente[i].ToString();
            //        vct.Hasta = model.ValorCoeficiente.Valor2Coeficiente[i].ToString();
            //        vct.Fecha_Alta = System.DateTime.Now;
            //        vct.Usuario_Alta = idUsuario;
            //        lista.Add(vct);
            //    }
            //}



            //ValorCoeficienteTierraGrabar(lista);



            model.Mensaje = "OK";

            return RedirectToAction("ValorCoeficiente", new { Mensaje = model.Mensaje, IdMetadatoCoeficienteTierra = model.MetadatoCoeficienteTierra.IdMetadatoCoeficienteTierra, IdTipoCoeficienteTierra = model.TipoCoeficienteTierra.IdTipoCoeficienteTierra });

        }

        public ActionResult GrabaCoeficienteMejora(ValuacionModel model)
        {

            List<CoeficientesStore> lista = new List<CoeficientesStore>();
            CoeficientesStore coefStore;
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();
            Coeficientes coef = new Coeficientes();
            coef.Descripcion = model.TipoCoeficienteMejora.IdTipoCoeficienteMejora;
            coef.Nro_Coeficiente = model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente;

            coef.Id_Tipo_Coeficiente = 3;
            coef = CoeficientesGrabar(coef);
            for (int i = 0; i < model.CoeficienteStore.Cantidad.Count(); i++)
            {
                if (model.CoeficienteStore.Cantidad[i] != string.Empty)
                {
                    coefStore = new CoeficientesStore();
                    if (model.CoeficienteStore.ValorComparacion1.Count() > 0)
                    {
                        coefStore.Comparador1 = model.CoeficienteStore.ValorComparacion1[i];

                        if (model.CoeficienteStore.ValorComparacion1[i] == "6")
                        {
                            coefStore.Parametro1Desde = model.CoeficienteStore.Parametro1Desde[i];
                            coefStore.Parametro1Hasta = model.CoeficienteStore.Parametro1Hasta[i];
                        }
                        else
                        {
                            coefStore.Parametro1Desde = model.CoeficienteStore.Parametro1[i];
                        }
                    }
                    else
                    {
                        if (model.CoeficienteStore.Parametro1.Count() > 0)
                        {
                            coefStore.Parametro1Desde = model.CoeficienteStore.Parametro1[i];
                        }
                    }

                    if (model.CoeficienteStore.ValorComparacion2.Count() > 0)
                    {
                        coefStore.Comparador2 = model.CoeficienteStore.ValorComparacion2[i];

                        if (model.CoeficienteStore.ValorComparacion2[i] == "6")
                        {
                            coefStore.Parametro2Desde = model.CoeficienteStore.Parametro2Desde[i];
                            coefStore.Parametro2Hasta = model.CoeficienteStore.Parametro2Hasta[i];
                        }
                        else
                        {
                            coefStore.Parametro2Desde = model.CoeficienteStore.Parametro2[i];
                        }
                    }
                    else
                    {

                        if (model.CoeficienteStore.Parametro2.Count() > 0)
                        {
                            coefStore.Parametro2Desde = model.CoeficienteStore.Parametro2[i];
                        }
                    }
                    coefStore.IdTipoValuacion = model.TipoValuacionId;
                    coefStore.Valor = float.Parse(model.CoeficienteStore.Cantidad[i]);
                    coefStore.Fecha_Alta = System.DateTime.Now;
                    coefStore.Usuario_Alta = usuario.Id_Usuario;
                    coefStore.TipoCoeficiente = "M";
                    coefStore.NroCoeficiente = model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente;
                    coefStore.IdCoeficiente = coef.Id_Coeficiente;
                    lista.Add(coefStore);
                }
            }

            CoeficienteStoreGrabar(lista);
            model.Mensaje = "OK";
            return RedirectToAction("ValorCoeficienteMejora", new { Mensaje = model.Mensaje, IdMejoraCoeficiente = model.MetadatoMejoraCoeficiente.IdMejoraCoeficiente, IdTipoCoeficienteMejora = model.TipoCoeficienteMejora.IdTipoCoeficienteMejora });

        }

        public JsonResult GetViasEje(long id)
        {
            return Json(GetEjesVia(id));
        }
        public JsonResult GetValorBasicoMejoraByIdEstado(long id)
        {
            /* 
             * jQuery vuelve por el handler de error si la respuesta es Json(null), asi que en caso de ser null, 
             * instancio un objeto "default" para no cambiar la firma del metodo y el codigo js asociado a la llamada
             */
            return Json(GetValorBasicoMejoraByIdEstadoConservacion(id) ?? new ValorBasicoMejora(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValorBasicoMejoraByParam(string parametro1, string parametro2)
        {

            return Json(GetValoresBasicosStoreByParametro(parametro1, parametro2) ?? new ValorBasicoStore(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetValorBasicoMejoraByParamTODOS(string parametro1, string parametro2, long anio, float superficie, float SuperficieSemi)
        {

            return Json(GetValoresBasicosStoreByParametroFull(parametro1, parametro2, anio, superficie, SuperficieSemi) ?? new ValorBasicoStore(), JsonRequestBehavior.AllowGet);
        }

        //METODOS DE ACCESO A LA API ↓
        public List<UnidadTributaria> UTsFechasGrabar(List<UnidadTributaria> uts)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/UTsFechasGrabar/", uts).Result;
            resp.EnsureSuccessStatusCode();
            return (List<UnidadTributaria>)resp.Content.ReadAsAsync<List<UnidadTributaria>>().Result;
        }
        public Valuacion ValuacionGrabar(Valuacion valuacion)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/ValuacionGrabar/", valuacion).Result;
            resp.EnsureSuccessStatusCode();
            return (Valuacion)resp.Content.ReadAsAsync<Valuacion>().Result;
        }

        public void SetValuacionPadronDetalle(List<ValuacionPadronDetalle> detallesValuaciones)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/SetValuacionPadronDetalle/", detallesValuaciones).Result;
            resp.EnsureSuccessStatusCode();
            return;
        }
        public List<Mejora> MejorasGrabar(List<Mejora> mejoras)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/MejorasGrabar/", mejoras).Result;
            resp.EnsureSuccessStatusCode();
            return (List<Mejora>)resp.Content.ReadAsAsync<List<Mejora>>().Result;
        }
        public Coeficientes CoeficientesGrabar(Coeficientes mejora)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/CoeficientesGrabar/", mejora).Result;
            resp.EnsureSuccessStatusCode();
            return (Coeficientes)resp.Content.ReadAsAsync<Coeficientes>().Result;
        }

        public List<CoeficientesStore> CoeficienteStoreGrabar(List<CoeficientesStore> lista)
        {

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/CoeficienteStoreGrabar/", lista).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoeficientesStore>)resp.Content.ReadAsAsync<List<CoeficientesStore>>().Result;
        }

        public void ValorCoeficienteMejoraGrabar(List<CoeficientesMejora> coef)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/ValorCoeficienteMejoraGrabar/", coef).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void ValorCoeficienteTierraGrabar(List<CoeficientesTierra> coef)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/ValorCoeficienteTierraGrabar/", coef).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void MejorasValoresBasicosGrabar(List<ValorBasicoMejora> mejora)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/MejorasValoresBasicosGrabar/", mejora).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void SuperficieSemiCubiertaGrabar(ValorBasicoMejora mejora)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/SuperficieSemiCubiertaGrabar/", mejora).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void TierraPorModuloGrabar(List<ValorBasicoTierraModulo> lista)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/TierraPorModuloGrabar/", lista).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void TierraPorSuperficieGrabar(List<ValorBasicoTierraSuperficie> lista)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/TierraPorSuperficieGrabar/", lista).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void TierraPorSuperficieGrabarVia(List<ValorBasicoTierraSuperficie> lista)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/TierraPorSuperficieGrabarVia/", lista).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void TierraPorStoreGrabar(List<ValorBasicoTierraStore> lista)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/TierraPorStoreGrabar/", lista).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void MejoraPorStoreGrabar(List<ValorBasicoMejoraStore> lista)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/MejoraPorStoreGrabar/", lista).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void ConsolidarPadron(ValuacionPadronCabecera model)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/ConsolidarPadron/", model).Result;
            resp.EnsureSuccessStatusCode();
        }
        public void EliminarPadron(ValuacionPadronCabecera model)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/EliminarPadron/", model).Result;
            resp.EnsureSuccessStatusCode();
        }
        public List<ValuacionPadronCabecera> GetValuacionPadronCabecera()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionPadronCabecera").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionPadronCabecera>)resp.Content.ReadAsAsync<List<ValuacionPadronCabecera>>().Result;

        }
        public ValuacionPadronCabecera GetValuacionPadronCabeceraById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionPadronCabeceraById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ValuacionPadronCabecera)resp.Content.ReadAsAsync<ValuacionPadronCabecera>().Result;

        }

        public List<ValuacionPadronDetalle> GetValuacionPadronDetalle(long idParcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValuacionPadronDetalleByIdParcela/" + idParcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValuacionPadronDetalle>)resp.Content.ReadAsAsync<List<ValuacionPadronDetalle>>().Result;

        }
        public List<ValorBasicoTierraModulo> GetValorBasicoTierraModulo()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraModulo").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraModulo>)resp.Content.ReadAsAsync<List<ValorBasicoTierraModulo>>().Result;

        }
        public List<ValorBasicoTierraStore> GetValorBasicoTierraStoreXTipo(long idTipoValuacion, string idFiltroParcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraStoreXTipo/?idTipoValuacion=" + idTipoValuacion + "&id=" + idFiltroParcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraStore>)resp.Content.ReadAsAsync<List<ValorBasicoTierraStore>>().Result;

        }
        public List<ValorBasicoTierraStore> GetCoeficientesStoreXTipoValuacion(long idTipoValuacion)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesStoreXTipoValuacion/?idTipoValuacion=" + idTipoValuacion).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraStore>)resp.Content.ReadAsAsync<List<ValorBasicoTierraStore>>().Result;

        }
        public List<ValorBasicoTierraStore> GetCoeficientesStore(long NroCoeficiente, string idCoeficiente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesStore/?NroCoeficiente=" + NroCoeficiente + "&idCoeficiente=" + idCoeficiente).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraStore>)resp.Content.ReadAsAsync<List<ValorBasicoTierraStore>>().Result;

        }
        public List<ValorBasicoMejoraStore> GetValorBasicoMejoraStoreXTipo(long idTipoValuacion, String idFiltroParcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoMejoraStoreXTipo/?idTipoValuacion=" + idTipoValuacion + "&id=" + idFiltroParcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoMejoraStore>)resp.Content.ReadAsAsync<List<ValorBasicoMejoraStore>>().Result;

        }

        public List<ValorBasicoTierraModulo> GetValorBasicoTierraModulXTipo(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraModulXTipo/?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraModulo>)resp.Content.ReadAsAsync<List<ValorBasicoTierraModulo>>().Result;

        }
        public List<ValorBasicoTierraSuperficie> GetValorBasicoTierraSuperficie()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraSuperficie").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraSuperficie>)resp.Content.ReadAsAsync<List<ValorBasicoTierraSuperficie>>().Result;

        }
        public List<ValorBasicoTierraSuperficie> GetValorBasicoTierraSuperficieVias()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraSuperficieVias").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraSuperficie>)resp.Content.ReadAsAsync<List<ValorBasicoTierraSuperficie>>().Result;

        }
        public List<ValorBasicoTierraSuperficie> GetValorBasicoTierraSuperficieXTipo(long TipoParcelaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraSuperficieXTipo/" + TipoParcelaID).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraSuperficie>)resp.Content.ReadAsAsync<List<ValorBasicoTierraSuperficie>>().Result;

        }
        public List<ValorBasicoTierraSuperficie> GetValorBasicoTierraSuperficieViasXTipo(long TipoParcelaID)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoTierraSuperficieViasXTipo/" + TipoParcelaID).Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoTierraSuperficie>)resp.Content.ReadAsAsync<List<ValorBasicoTierraSuperficie>>().Result;

        }
        public Dictionary<string, string> CalcularTotalesTemporal(ValuacionPadronCabecera model)
        {
            cliente.Timeout = new TimeSpan(1, 0, 0);
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/CalcularTotalesTemporal/", model).Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;

        }
        public Dictionary<string, float> GrabarPadronTemporal(ValuacionPadronCabecera model)
        {

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/GrabarPadronTemporal/", model).Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, float>)resp.Content.ReadAsAsync<Dictionary<string, float>>().Result;

        }
        public Dictionary<string, string> RecuperarTotales(long idPadron)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/RecuperarTotalesTemporal/" + idPadron).Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;

        }

        public List<Parcela> GetParcelas()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetParcelas").Result;
            resp.EnsureSuccessStatusCode();
            return (List<Parcela>)resp.Content.ReadAsAsync<List<Parcela>>().Result;
        }

        public Parcela GetParcelaByIdUt(long idUnidadTributaria)
        {
            using (var resp = cliente.GetAsync($"api/Parcela/GetParcelaByUt?idUnidadTributaria={idUnidadTributaria}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<Parcela>().Result;
            }
        }

        public List<ValorBasicoMejora> GetValorBasicoMejora()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoMejora").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ValorBasicoMejora>)resp.Content.ReadAsAsync<List<ValorBasicoMejora>>().Result;

        }

        public ValorBasicoMejora GetValorBasicoMejoraSupSemiCubierta()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoMejoraSupSemiCubierta").Result;
            resp.EnsureSuccessStatusCode();
            return (ValorBasicoMejora)resp.Content.ReadAsAsync<ValorBasicoMejora>().Result;

        }
        public ValorBasicoMejoraStore GetValorBasicoMejoraSupSemiCubiertaStore()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoMejoraSupSemiCubiertaStore").Result;
            resp.EnsureSuccessStatusCode();
            return (ValorBasicoMejoraStore)resp.Content.ReadAsAsync<ValorBasicoMejoraStore>().Result;

        }
        public List<Coeficientes> GetCoeficientes()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientes").Result;
            resp.EnsureSuccessStatusCode();
            return (List<Coeficientes>)resp.Content.ReadAsAsync<List<Coeficientes>>().Result;

        }
        public List<CoeficientesTierra> GetCoeficientesTierraValor(long NroCoeficiente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesTierraValor/" + NroCoeficiente).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoeficientesTierra>)resp.Content.ReadAsAsync<List<CoeficientesTierra>>().Result;

        }
        public List<CoeficientesTierra> GetCoeficientesTierraRango(long NroCoeficiente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesTierraRango/" + NroCoeficiente).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoeficientesTierra>)resp.Content.ReadAsAsync<List<CoeficientesTierra>>().Result;

        }

        public List<CoeficientesMejora> GetCoeficientesMejoraValor(long NroCoeficiente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesMejoraValor/" + NroCoeficiente).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoeficientesMejora>)resp.Content.ReadAsAsync<List<CoeficientesMejora>>().Result;

        }
        public List<CoeficientesMejora> GetCoeficientesMejoraRango(long NroCoeficiente)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientesMejoraRango/" + NroCoeficiente).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoeficientesMejora>)resp.Content.ReadAsAsync<List<CoeficientesMejora>>().Result;

        }

        public List<OA.Objeto> GetAtributosZonaObjeto()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetAtributosZonaObjeto").Result;
            resp.EnsureSuccessStatusCode();
            return (List<OA.Objeto>)resp.Content.ReadAsAsync<IEnumerable<OA.Objeto>>().Result;
        }
        public Dictionary<string, string> GetAtributosParametro1(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetAtributosParametro1/?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;
        }
        public Dictionary<string, string> GetAtributosParametro2(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetAtributosParametro2/?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (Dictionary<string, string>)resp.Content.ReadAsAsync<Dictionary<string, string>>().Result;
        }

        public List<Via> GetVias()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetVias").Result;
            resp.EnsureSuccessStatusCode();
            return (List<Via>)resp.Content.ReadAsAsync<IEnumerable<Via>>().Result;
        }
        public Via GetViasById(long Viaid)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetViasById/" + Viaid).Result;
            resp.EnsureSuccessStatusCode();
            return (Via)resp.Content.ReadAsAsync<Via>().Result;
        }

        public List<EjesVia> GetEjesVia(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetEjesVia/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<EjesVia>)resp.Content.ReadAsAsync<IEnumerable<EjesVia>>().Result;

        }
        public List<EjesVia> GetEjesVia()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetEjesVias").Result;
            resp.EnsureSuccessStatusCode();
            return (List<EjesVia>)resp.Content.ReadAsAsync<IEnumerable<EjesVia>>().Result;

        }
        public List<TramoVia> GetTramoVia(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTramoVia/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TramoVia>)resp.Content.ReadAsAsync<List<TramoVia>>().Result;
        }
        public List<TramoVia> GetTramoViaByIdVia(long Viaid)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTramoViaByIdVia/" + Viaid).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TramoVia>)resp.Content.ReadAsAsync<List<TramoVia>>().Result;
        }

        public List<TramoVia> GetTramosVias()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTramosVias").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TramoVia>)resp.Content.ReadAsAsync<IEnumerable<TramoVia>>().Result;
        }
        public List<TipoValorBasicoTierra> GetTipoValorBasicoTierra()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoValorBasicoTierra").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoValorBasicoTierra>)resp.Content.ReadAsAsync<IEnumerable<TipoValorBasicoTierra>>().Result;
        }
        public List<TipoValuacion> GetTipoValuacion()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoValuacion").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoValuacion>)resp.Content.ReadAsAsync<IEnumerable<TipoValuacion>>().Result;
        }
        public TipoValuacion GetTipoValuacionByID(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoValuacionByID/?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (TipoValuacion)resp.Content.ReadAsAsync<TipoValuacion>().Result;
        }
        public List<TipoValuacion> GetTipoValuacionByFiltro(string destino, string idFiltroParcela)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoValuacionByFiltro/?Destino=" + destino + "&idFiltroParcela=" + idFiltroParcela).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoValuacion>)resp.Content.ReadAsAsync<IEnumerable<TipoValuacion>>().Result;
        }
        public TipoValorBasicoTierra GetTipoValorBasicoTierraById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoValorBasicoTierraById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (TipoValorBasicoTierra)resp.Content.ReadAsAsync<TipoValorBasicoTierra>().Result;
        }
        public TipoValorBasicoTierra GrabarTipoValorBasicoTierra(TipoValorBasicoTierra tvbt)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ValuacionService/GrabarTipoValorBasicoTierra/", tvbt).Result;
            resp.EnsureSuccessStatusCode();
            return (TipoValorBasicoTierra)resp.Content.ReadAsAsync<TipoValorBasicoTierra>().Result;
        }

        public ValorBasicoMejora GetValorBasicoMejoraByIdEstadoConservacion(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetValorBasicoMejoraByIdEstadoConservacion/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (ValorBasicoMejora)resp.Content.ReadAsAsync<ValorBasicoMejora>().Result;
        }
        public Decimal GetCoeficiente1Individual(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficiente1Individual/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (Decimal)resp.Content.ReadAsAsync<Decimal>().Result;
        }
        public List<ParametrosGeneralesModel> GetParametrosGenerales()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetParametrosGenerales").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ParametrosGeneralesModel>)resp.Content.ReadAsAsync<IEnumerable<ParametrosGeneralesModel>>().Result;
        }
        public JsonResult GetCoef1(long anio)
        {
            return Json(GetCoeficiente1Individual(anio), JsonRequestBehavior.AllowGet);
        }
        public DataTable ConvertToDataTable(List<ValuacionPadronDetalle> data)
        {
            //PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            //    Partida Provincial; 
            //Partida Municipal;
            //Domicilio Inmueble; Domicilio Fiscal;
            //Porcentaje de Copropiedad; Titular; Responsable Fiscal;
            //Superficie Tierra; Superficie Cubierta; Valor Tierra; 
            //Valor mejora; Valor Total Parcela; Valor Tierra Propiedad;
            //Valor Mejoras Propiedad
            //foreach (PropertyDescriptor prop in properties){
            //   table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            //}
            //table.Columns.Add(  "IdPadronDetalle", typeof(long) );
            //table.Columns.Add(  "IdPadron", typeof(long) );
            //table.Columns.Add(  "IdParcela", typeof(long) );
            //table.Columns.Add(  "IdUnidadTributaria" , typeof(long));
            //table.Columns.Add(  "IdAtributoZona", typeof(long) );
            //table.Columns.Add(  "TipoParcela", typeof(String) );

            table.Columns.Add("PartidaProvincial", typeof(String));
            table.Columns.Add("PartidaMunicipal", typeof(String));

            table.Columns.Add("DomicilioInmueble", typeof(String));
            table.Columns.Add("DomicilioFiscal", typeof(String));
            table.Columns.Add("PorcentajeCodominio", typeof(float));
            table.Columns.Add("Titular", typeof(String));
            table.Columns.Add("ResponsableFiscal", typeof(String));
            table.Columns.Add("SuperficieTierra", typeof(float));
            table.Columns.Add("SuperificeCubierta", typeof(float));
            table.Columns.Add("SuperficieSemiCubierta", typeof(float));
            //table.Columns.Add("Uso", typeof(String));
            //table.Columns.Add("anioConstruccion", typeof(long));
            table.Columns.Add("ValorTierra", typeof(Decimal));
            table.Columns.Add("ValorMejora", typeof(Decimal));
            table.Columns.Add("ValorTotal", typeof(Decimal));
            //table.Columns.Add("Usuario_Alta" , typeof(long));
            //table.Columns.Add("Fecha_Alta", typeof(DateTime));
            //table.Columns.Add("Usuario_Modificacion",typeof(long));
            //table.Columns.Add("Fecha_Modificacion",typeof(DateTime));
            //table.Columns.Add("Usuario_Baja",typeof(long));
            //table.Columns.Add("Fecha_Baja", typeof(DateTime));
            table.Columns.Add("ValorTierraPropiedad", typeof(Decimal));
            table.Columns.Add("ValorMejoraPropiedad", typeof(Decimal));
            //table.Columns.Add("Fecha_Vigencia_Desde", typeof(DateTime));
            //table.Columns.Add("Fecha_Vigencia_Hasta", typeof(DateTime));

            foreach (ValuacionPadronDetalle item in data)
            {
                DataRow row = table.NewRow();
                //foreach (PropertyDescriptor prop in properties){
                //    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                //}
                //table.Rows.Add(row);
                table.Rows.Add(item.PartidaProvincial, item.PartidaMunicipal, item.DomicilioInmueble, item.DomicilioFiscal,
                    item.PorcentajeCodominio, item.Titular, item.ResponsableFiscal, item.SuperficieTierra, item.SuperificeCubierta, item.SuperficieSemiCubierta
                    , item.ValorTierra, item.ValorMejora, item.ValorTotal, item.ValorTierraPropiedad, item.ValorMejoraPropiedad);
            }
            return table;

        }

        public Decimal GetCoeficientes2(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetCoeficientes2/?id=" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (Decimal)resp.Content.ReadAsAsync<Decimal>().Result;
        }

        public List<DDJJ> GetDDJJByIdUt(long idUnidadTributaria)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/DeclaracionJurada/GetDeclaracionesJuradas?idUnidadTributaria=" + idUnidadTributaria).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<DDJJ>>().Result;
        }

        public Decimal GetTipoMunicipalidad()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ValuacionService/GetTipoMunicipalidad/").Result;
            resp.EnsureSuccessStatusCode();
            return (Decimal)resp.Content.ReadAsAsync<Decimal>().Result;
        }

        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }
        public FileResult AbrirReporte()
        {
            Response.AppendHeader("Content-Disposition", new ContentDisposition { FileName = ArchivoDescarga.NombreArchivo, Inline = true }.ToString());
            return File(ArchivoDescarga.Contenido, ArchivoDescarga.MimeType);
        }

        public ActionResult GenerarReporteValuatorio(long idUnidadTributaria, long? idTramite)
        {
            using (var resp = cliente.GetAsync($"api/UnidadTributaria/Get?id={idUnidadTributaria}&incluirDominios=true").Result)
            {
                var ut = resp.Content.ReadAsAsync<UnidadTributaria>().Result;
                var parcela = ut.Parcela;
                var dominios = ut.Dominios.ToList();
                var tipoUt = ut.TipoUnidadTributariaID;
                decimal superficie = 0;
                if (tipoUt == 3)
                {
                    superficie = Convert.ToDecimal(ut.Superficie.Value);
                }
                else if ((tipoUt == 2 || tipoUt == 1))
                {
                    superficie = parcela.Superficie;
                }

                var DDJJ = GetDDJJByIdUt(ut.UnidadTributariaId);

                if ((!dominios.Any() || superficie == 0) && parcela.ClaseParcelaID != 6)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }

                if (!DDJJ.Any() && parcela.ClaseParcelaID != 6)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

            }

            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            using (var resp = apiReportes.GetAsync($"api/CertificadoValuatorio/Get?idUnidadTributaria={idUnidadTributaria}&idTramite={idTramite}&usuario={usuario}").Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarCertificadoValuatorio);
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"CertificadoValuatorio_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GenerarHistoricoValuaciones(long idUnidadTributaria, long? idTramite)
        {
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
            using (var resp = apiReportes.GetAsync($"api/InformeHistoricoValuaciones/Get?idUnidadTributaria={idUnidadTributaria}&idTramite={idTramite}&usuario={usuario}").Result)
            {
                if (!resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                AuditoriaHelper.Register(((UsuariosModel)Session["usuarioPortal"]).Id_Usuario, string.Empty, Request, TiposOperacion.Consulta, Autorizado.Si, Eventos.GenerarInformeHistoricoValuaciones);
                ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"InformeHistoricoValuaciones_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }
        public ActionResult GenerarInformeValuacionParcelario(long? idParcela = null, long? idTramite = null)
        {
            var parcela = Session["Parcela"] as Parcela;
            if (idParcela != null)
            {
                var resp = cliente.GetAsync("api/Parcela/Get/" + idParcela).Result;
                resp.EnsureSuccessStatusCode();
                parcela = resp.Content.ReadAsAsync<Parcela>().Result;
            }
            else if (parcela == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No se ha seleccionado ninguna parcela.");
            }

            if (parcela.UnidadesTributarias.Any())
            {
                string tipo = "Rural";
                if (parcela.UnidadesTributarias.Any(ut => ut.TipoUnidadTributariaID == 2)) // es ph (puede ser urbana o no)
                {
                    tipo = "Ph";
                }
                else if (parcela.TipoParcelaID != 2 && parcela.TipoParcelaID != 3)
                {
                    tipo = "Urbana";
                }
                long idUt = parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID != 3).OrderBy(x => x.TipoUnidadTributariaID).First().UnidadTributariaId;
                using (var resp = cliente.GetAsync("api/DeclaracionJurada/GetDeclaracionesJuradas?idUnidadTributaria=" + idUt).Result)
                {
                    var DDJJ = resp.Content.ReadAsAsync<List<DDJJ>>().Result;
                    if (!DDJJ.Any())
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                    }
                }
                string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
                using (var apiReportes = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesUrl"]) })
                using (var resp = apiReportes.GetAsync($"api/InformeValuacion{tipo}/Get?idUnidadTributaria={idUt}&idTramite={idTramite}&usuario={usuario}").Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                    string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                    ArchivoDescarga = new ArchivoDescarga(Convert.FromBase64String(bytes64), $"{parcela.UnidadesTributarias.First().CodigoProvincial}_InformeValuacion{tipo}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "application/pdf");
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No se puede generar el informe de valuación para esta parcela.");
            }
        }
    }
}
