using System;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using GeoSit.Data.BusinessEntities.Inmuebles;
using Newtonsoft.Json.Linq;
using GeoSit.Client.Web.Solr;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Net;

namespace GeoSit.Client.Web.Controllers
{
    public class ParcelaGraficaController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpClient solrClient = new HttpClient();

        public ParcelaGraficaController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            solrClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["solrUrl"]);
        }

        // GET: ParcelaGrafica
        public ActionResult Index()
        {
            return PartialView();
        }

        // GET: /ParcelaGrafica/DatosParcelaGrafica
        public ActionResult DatosParcelaGrafica()
        {
            ViewBag.MensajeSalida = "";
            return PartialView();
        }

        public List<ParcelaGrafica> GetParcelasGrafica()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ParcelaGraficaService/GetParcelasGrafica").Result;
            resp.EnsureSuccessStatusCode();
            return (List<ParcelaGrafica>)resp.Content.ReadAsAsync<IEnumerable<ParcelaGrafica>>().Result;
        }

        public List<SelectListItem> GetTiposDivision()
        {
            List<SelectListItem> itemsTipos = new List<SelectListItem>();
            List<TipoDivision> Tipos = GetAllTiposDivision();
            foreach (var item in Tipos)
            {
                itemsTipos.Add(new SelectListItem { Text = item.Nombre, Value = Convert.ToString(item.TipoObjetoId) });
            }
            return itemsTipos;
        }

        public List<TipoDivision> GetAllTiposDivision()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TiposDivisionesAdministrativas/GetTiposDivision").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TipoDivision>)resp.Content.ReadAsAsync<IEnumerable<TipoDivision>>().Result;
        }


        public JsonResult GetParcelaGrafByParcelaJson(long parcelaId)
        {
            return Json(GetParcelaGrafByParcela(parcelaId));
        }
        public ParcelaGraficaModel GetParcelaGrafByParcela(long parcelaId)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ParcelaGraficaService/GetParcelaGraficaByParcelaId/" + parcelaId).Result;
                resp.EnsureSuccessStatusCode();
                return (ParcelaGraficaModel)resp.Content.ReadAsAsync<ParcelaGraficaModel>().Result;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                ParcelaGraficaModel ParcelaVacia = new ParcelaGraficaModel();
                ParcelaVacia.FeatID = 0;
                return ParcelaVacia;
            }

        }

        public JsonResult GetParcelaGrafByFeatidJson(long id)
        {
            return Json(GetParcelaGrafByFeatid(id));
        }
        public ParcelaGraficaModel GetParcelaGrafByFeatid(long id)
        {
            try
            {
                HttpResponseMessage resp = cliente.GetAsync("api/ParcelaGraficaService/GetParcelaGraficaById/" + id).Result;
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsAsync<ParcelaGraficaModel>().Result;
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("GetParcelaGrafByFeatid", ex);
                return new ParcelaGraficaModel() { FeatID = 0 };
            }

        }

        //[HttpPost]
        //public JsonResult GetManzanaCoordsByNomenclatura(string nomenclatura)
        //{
        //    var coords = string.Empty;
        //    DbGeometry geometryManzana = GetGeometryDivisionByNomenclatura(nomenclatura);
        //    if (geometryManzana != null)
        //    {
        //        bool isPoint = !geometryManzana.PointCount.HasValue;
        //        //var pointFormat = "{0},{1}";
        //        var pointFormat = "{0},{1},{2}";
        //        if (isPoint)
        //        {
        //            coords = string.Format(pointFormat, geometryManzana.XCoordinate, geometryManzana.YCoordinate, 0);
        //        }
        //        else
        //        {
        //            var lista = new List<string>();
        //            for (int i = 1; i <= geometryManzana.PointCount; i++)
        //            {
        //                var punto = geometryManzana.PointAt(i);
        //                lista.Add(string.Format(pointFormat, punto.XCoordinate, punto.YCoordinate));
        //            }
        //            coords = string.Join(",", lista.ToArray());
        //        }
        //    }
        //    //JsonResult jsonResult = new JsonResult
        //    //{
        //    //    Data = (string)resp.Content.ReadAsAsync<string>().Result
        //    //};
        //    JsonResult jsonResult = new JsonResult
        //    {
        //        Data = coords
        //    };
        //    return jsonResult;
        //}

        //[HttpPost]
        //public JsonResult GetManzanaCentroidCoordsByNomenclatura(string nomenclatura)
        //{
        //    var coords = string.Empty;
        //    DbGeometry geometryManzana = GetGeometryDivisionByNomenclatura(nomenclatura);
        //    if (geometryManzana != null)
        //    {
        //        DbGeometry geometryManzanaCentroid = geometryManzana.Centroid;
        //        var pointFormat = "{0},{1},{2}";
        //        coords = string.Format(pointFormat, geometryManzanaCentroid.XCoordinate, geometryManzanaCentroid.YCoordinate, 0);
        //    }
        //    //JsonResult jsonResult = new JsonResult
        //    //{
        //    //    Data = (string)resp.Content.ReadAsAsync<string>().Result
        //    //};
        //    JsonResult jsonResult = new JsonResult
        //    {
        //        Data = coords
        //    };
        //    return jsonResult;
        //}

        //public DbGeometry GetGeometryDivisionByNomenclatura(string nomenclatura)
        //{
        //    DbGeometry geometry = null;
        //    string parametros = string.Format("nomenclatura={0}", nomenclatura);

        //    HttpResponseMessage resp = cliente.GetAsync("api/DivisionService/GetGeometryWKTByNomenclatura?" + parametros).Result;
        //    try
        //    {
        //        resp.EnsureSuccessStatusCode();
        //        var wkt = resp.Content.ReadAsAsync<string>().Result;
        //        geometry = DbGeometry.FromText(wkt);
        //    }
        //    catch (Exception)
        //    {
        //        string msgErr = resp.ReasonPhrase;
        //    }
        //    return geometry;
        //}



        public JsonResult GetNomenclaturasJson(string nomenclatura, long tipo_division)
        {
            return Json(GetGetNomenclaturasByDivision(nomenclatura, tipo_division));
        }
        public List<NomenclaturaModel> GetGetNomenclaturasByDivision(string nombre_completo, long tipo_division)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/NomenclaturaService/GetNomenclaturaByNombre/" + nombre_completo).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<NomenclaturaModel>>().Result;
        }

        // Recupera las divisiones de acuerdo al tipo y nomenclatura.
        public JsonResult GetDivisionByNomenclaturaJson(string nomenclatura)
        {
            return Json(GetDivisionByNomenclatura(nomenclatura));
        }
        public List<DivisionModel> GetDivisionByNomenclatura(string nomenclatura)
        {
            HttpResponseMessage resp = cliente.GetAsync(string.Format("api/DivisionService/GetNomenclaturaByNomenclatura/{0}", nomenclatura)).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<DivisionModel>>().Result;
        }

        public HttpStatusCodeResult Save(long featid, long? idparcela)
        {
            if (idparcela == 0)
            {
                idparcela = null;
            }
            var parcela = new ParcelaGrafica()
            {
                FeatID = featid,
                ParcelaID = idparcela,
                _Id_Usuario = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario,
                _Ip = Request.UserHostAddress
            };
            try
            {
                parcela._Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName ?? Request.UserHostName;
            }
            catch (Exception)
            {
                parcela._Machine_Name = Request.UserHostName;
            }

            using (var resp = cliente.PostAsJsonAsync("api/ParcelaGraficaService/ParcelaGrafica_Save", parcela).Result)
            {
                if (resp.IsSuccessStatusCode || resp.StatusCode == HttpStatusCode.ExpectationFailed)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    MvcApplication.GetLogger().LogError("ParcelaGraficaController-ParcelaGraficaSave", new Exception("Error al asociar/desasociar parcela"));
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                }
            }
            //var resultado = resp.StatusCode;

            //var solr = new SolrServer();
            //if (!solr.Indexing())
            //{
            //    #region Actualizacion Parcela
            //    solr.Clear();
            //    solr.AddParam(new SolrParam("wt", "json"));
            //    solr.AddParam(new SolrParam("omitHeader", "true"));
            //    solr.AddParam(new SolrParam("df", "uid"));
            //    var doc = solr.Search(uid);

            //    var jsonOBJ = JObject.Parse(doc);
            //    var solrDoc = jsonOBJ["response"]["docs"].First;
            //    var modificaciones = new List<long>();
            //    string operador = "add";
            //    if (parcela_id.HasValue)
            //    {
            //        modificaciones.Add(id);
            //    }
            //    else
            //    {
            //        operador = "remove";
            //        foreach (var featid in (JArray)solrDoc["featids"])
            //        {
            //            modificaciones.Add(featid.ToObject<long>());
            //        }
            //    }

            //    solrDoc["featids"] = JToken.Parse($"{{\"{operador}\":[{string.Join(",", modificaciones)}]}}");

            //    var actualizacion = jsonOBJ["response"]["docs"].ToString()
            //                        .Replace(Environment.NewLine, string.Empty);

            //    solr.Clear();
            //    solr.Update(actualizacion);
            //    #endregion

            //    #region Actualizacion Unidad Tributaria
            //    solr.Clear();
            //    solr.AddParam(new SolrParam("wt", "json"));
            //    solr.AddParam(new SolrParam("omitHeader", "true"));
            //    solr.AddParam(new SolrParam("tipo", "unidadestributarias"));
            //    solr.AddParam(new SolrParam("df", "idpadre"));

            //    doc = solr.Search(uid.Replace("par_", string.Empty));

            //    jsonOBJ = JObject.Parse(doc);
            //    solrDoc = jsonOBJ["response"]["docs"].First;

            //    solrDoc["featids"] = JToken.Parse($"{{\"{operador}\":[{string.Join(",", modificaciones)}]}}");

            //    actualizacion = jsonOBJ["response"]["docs"].ToString()
            //                        .Replace(Environment.NewLine, string.Empty);

            //    solr.Clear();
            //    solr.Update(actualizacion);
            //    #endregion

            //    return resultado.ToString();
            //}
            //else
            //{
            //    return "IDX_SOLR";
            //}
            }

        public JsonResult ParcelaGraficaDeleteJson(long id, long parcela_id, string fecha, int usuario)
        {
            return Json(ParcelaGraficaDelete(id, parcela_id, fecha, usuario));
        }

        public string ParcelaGraficaDelete(long id, long parcela_id, string fecha, int usuario)
        {
            ParcelaGraficaModel parcela = new ParcelaGraficaModel();
            parcela.FeatID = id;
            parcela.ParcelaID = parcela_id;
            //parcela.FechaModificacion = Convert.ToDateTime(fecha);
            DateTime fechaActual = DateTime.Now;
            //parcela.FechaModificacion = Convert.ToDateTime(fecha);
            parcela.FechaModificacion = fechaActual;
            parcela.UsuarioModificacionID = usuario;
            //parcela.FechaBaja = Convert.ToDateTime(fecha);
            parcela.FechaBaja = fechaActual;
            parcela.UsuarioBajaID = usuario;
            parcela.ClassID = 105;
            parcela.RevisionNumber = 0;

            using (HttpResponseMessage resp = cliente.PostAsJsonAsync("api/ParcelaGraficaService/ParcelaGrafica_Delete", parcela).Result)
            {
                if (resp.IsSuccessStatusCode || resp.StatusCode == HttpStatusCode.ExpectationFailed)
                {
                    return HttpStatusCode.OK.ToString();
                }
                else
                {
                    throw new Exception(resp.StatusCode.ToString());
                }
            }
        }

        public JsonResult GetParcelaVistaJson(long id)
        {
            return Json(GetParcelaVista(id));
        }
        public string GetParcelaVista(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ParcelaGraficaService/GetNomenclaturaVista/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (string)resp.Content.ReadAsAsync<string>().Result;
        }

        public JsonResult GetDivisionFeatIdByParcelaFeatid(long featIdParc)
        {
            long featidManz = 0;
            HttpResponseMessage resp = cliente.GetAsync(string.Format("api/ParcelaGraficaService/GetDivisionFeatIdByParcelaFeatid?featIdParc={0}", featIdParc)).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
                featidManz = (long)resp.Content.ReadAsAsync<long>().Result;
            }
            catch (Exception)
            {
                string msgErr = resp.ReasonPhrase;
            }
            JsonResult jsonResult = new JsonResult
            {
                Data = featidManz
            };
            return jsonResult;
        }

        // Determina el centro de la parcela gráfica.
        public JsonResult GetCentroParcelaJson(long id)
        {
            return Json(GetCentroParcela(id));
        }
        public string GetCentroParcela(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ParcelaGraficaService/GetCentroParcela/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (string)resp.Content.ReadAsAsync<string>().Result;
        }

        public List<PersonaModel> GetDatosPersonaByAll(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/PersonaService/GetDatosPersonaByAll/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<PersonaModel>)resp.Content.ReadAsAsync<IEnumerable<PersonaModel>>().Result;
        }
    }

    public class UpdateGeometryFromCTParcela
    {
        public long FeatID { get; set; }
        public string StringGeometry { get; set; }
    }
}