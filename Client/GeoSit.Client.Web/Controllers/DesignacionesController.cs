using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.Inmuebles;
using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers
{
    public class DesignacionesController : Controller
    {
        // GET: Designaciones
        public ActionResult Index(Designacion designacion)
        {
            ModelState.Clear();
            designacion = designacion ?? new Designacion();
            try
            {
                var parametros = new SeguridadController().GetParametrosGenerales();
                bool activaDesignaciones = parametros
                                            .Where(pg => pg.Agrupador == "MANTENEDOR_PARCELARIO")
                                            .Any(pmt => pmt.Clave == Recursos.ActivarDesignaciones && pmt.Valor == "1") && SeguridadController.ExisteFuncion(Seguridad.VisualizarDesignaciones);
                if (!activaDesignaciones)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                bool esNueva = designacion.IdDesignacion == 0;
                ViewData["EsNueva"] = esNueva;

                var parcela = Session["Parcela"] as Parcela;
                bool parcelaPrescripcion = (esNueva ? designacion.IdTipoDesignador : parcela.ClaseParcelaID) == 2;
                using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
                {
                    if (parcela == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    else if ((!parcela.Designaciones?.Any() ?? false) && !((Session["UnidadMantenimientoParcelario"] as UnidadMantenimientoParcelario)?.OperacionesDesignaciones?.Any() ?? false))
                    {
                        using (var result = client.GetAsync($"api/designacion/GetDesignacionesParcela?idParcela={parcela.ParcelaID}").Result)
                        {
                            result.EnsureSuccessStatusCode();
                            parcela.Designaciones = result.Content.ReadAsAsync<List<Designacion>>().Result;
                        }
                    }
                    using (var result = client.GetAsync($"api/designacion/GetTiposDesignador").Result)
                    {
                        result.EnsureSuccessStatusCode();
                        var tipos = result.Content.ReadAsAsync<IEnumerable<TipoDesignador>>().Result;
                        var disponibles = tipos.Where(t => parcela.Designaciones.All(d => t.IdTipoDesignador != d.IdTipoDesignador));
                        if (parcelaPrescripcion)
                        {
                            disponibles = disponibles.Where(x => x.Nombre != "TITULO");
                        }
                        bool puedeCambiar = designacion.IdDesignacion != 0 && disponibles.Any() || disponibles.Count() > 1;
                        ViewData["PuedeCambiarDesignador"] = puedeCambiar;
                        if (puedeCambiar)
                        {
                            if (parcelaPrescripcion)
                            {
                                ViewData["TiposDesignador"] = new SelectList(tipos.Where(x => x.Nombre != "TITULO"), "IdTipoDesignador", "Nombre", designacion.IdTipoDesignador);
                            }
                            else
                            {
                                ViewData["TiposDesignador"] = new SelectList(tipos, "IdTipoDesignador", "Nombre", designacion.IdTipoDesignador);
                            }
                        }
                        if (designacion.IdDesignacion == 0)
                        {
                            designacion.IdParcela = parcela.ParcelaID;
                            designacion.TipoDesignador = disponibles.First();
                            designacion.IdTipoDesignador = disponibles.First().IdTipoDesignador;
                        }
                    }
                    bool esRural = parcela.TipoParcelaID == 2 || parcela.TipoParcelaID == 3;
                    ViewData["EsRural"] = esRural;

                    #region Load Departamentos
                    var departamentos = new List<OA.Objeto>();
                    using (var result = client.GetAsync($"api/objetoadministrativoservice/GetObjetoByTipo/{Convert.ToInt64(parametros.Single(p => p.Clave == "ID_TIPO_OBJETO_DEPARTAMENTO").Valor)}").Result)
                    {
                        result.EnsureSuccessStatusCode();
                        departamentos = result.Content.ReadAsAsync<IEnumerable<OA.Objeto>>().Result.OrderBy(d => d.Nombre).ToList();
                    }
                    if (esNueva)
                    {
                        designacion.IdDepartamento = departamentos.First().FeatId;
                    }
                    else
                    {
                        designacion.IdDepartamento = designacion?.IdDepartamento ?? departamentos.SingleOrDefault(d => string.Compare(d.Nombre, designacion.Departamento) == 0)?.FeatId;
                    }
                    ViewData["Departamentos"] = new SelectList(departamentos, "FeatId", "Nombre", designacion.IdDepartamento);
                    #endregion

                    #region Load Localidades
                    var localidades = new List<OA.Objeto>();
                    long idTipoObjetoLocalidad = Convert.ToInt64(parametros.Single(p => p.Clave == "ID_TIPO_OBJETO_LOCALIDAD").Valor);
                    ViewData["IdTipoObjetoLocalidad"] = idTipoObjetoLocalidad;
                    if (designacion.IdDepartamento.HasValue)
                    {
                        designacion.Departamento = designacion.Departamento ?? departamentos.Single(d => d.FeatId == designacion.IdDepartamento.Value).Nombre;
                        localidades = loadHijosByPadre(designacion.IdDepartamento, idTipoObjetoLocalidad).ToList();
                    }
                    if (esNueva)
                    {
                        designacion.IdLocalidad = localidades.First().FeatId;
                    }
                    else
                    {
                        designacion.IdLocalidad = designacion?.IdLocalidad ?? localidades.SingleOrDefault(d => string.Compare(d.Nombre, designacion.Localidad) == 0)?.FeatId;
                    }
                    designacion.Localidad = designacion.Localidad ?? localidades.SingleOrDefault(d => d.FeatId == (designacion.IdLocalidad ?? -1))?.Nombre;
                    ViewData["Localidades"] = new SelectList(localidades, "FeatId", "Nombre", designacion.IdLocalidad);
                    #endregion

                    #region Load Secciones
                    if (esRural)
                    {
                        var secciones = new List<OA.Objeto>();
                        long idTipoObjetoSeccion = Convert.ToInt64(parametros.Single(p => p.Clave == "ID_TIPO_OBJETO_SECCION").Valor);
                        ViewData["IdTipoObjetoSeccion"] = idTipoObjetoSeccion;
                        if (designacion.IdDepartamento.HasValue)
                        {
                            secciones = loadHijosByPadre(designacion.IdDepartamento, idTipoObjetoSeccion).ToList();
                        }
                        if (esNueva)
                        {
                            designacion.IdSeccion = secciones.First().FeatId;
                        }
                        else
                        {
                            designacion.IdSeccion = designacion?.IdSeccion ?? secciones.SingleOrDefault(d => string.Compare(d.Nombre, designacion.Seccion) == 0)?.FeatId;
                        }
                        designacion.Seccion = designacion.Seccion ?? secciones.SingleOrDefault(d => d.FeatId == (designacion.IdSeccion ?? -1))?.Nombre;
                        ViewData["Secciones"] = new SelectList(localidades, "FeatId", "Nombre", designacion.IdSeccion);
                    }
                    #endregion
                }
                return PartialView(designacion);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public JsonResult CargarObjetos(long idPadre, long tipo)
        {
            return Json(new SelectList(loadHijosByPadre(idPadre, tipo), "FeatId", "Nombre").ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarTipoDesignador(Designacion designacion)
        {
            try
            {
                using (var cliente = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
                using (var result = cliente.GetAsync($"api/designacion/GetTiposDesignador").Result)
                {
                    result.EnsureSuccessStatusCode();
                    var designadores = result.Content.ReadAsAsync<IEnumerable<TipoDesignador>>().Result;
                    designacion.TipoDesignador = designadores.Single(d => d.IdTipoDesignador == designacion.IdTipoDesignador);
                }
                var parcela = Session["Parcela"] as Parcela;
                if (parcela.Designaciones.Any(d => d.IdTipoDesignador == designacion.IdTipoDesignador && d.IdDesignacion != designacion.IdDesignacion))
                {
                    return Json(new { error = true, designador = designacion.TipoDesignador.Nombre });
                }
                return Json(designacion);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("ValidarTipo", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        private IEnumerable<OA.Objeto> loadHijosByPadre(long? idPadre, long hijosTipo)
        {
            if (idPadre.HasValue)
            {
                using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]) })
                using (var result = client.GetAsync($"api/objetoadministrativoservice/GetObjetoByIdPadreTipo/{idPadre}/?tipo={hijosTipo}").Result)
                {
                    result.EnsureSuccessStatusCode();
                    return result.Content.ReadAsAsync<IEnumerable<OA.Objeto>>().Result.OrderBy(o => o.Nombre);
                }
            }
            else
            {
                return new OA.Objeto[0];
            }
        }
    }
}