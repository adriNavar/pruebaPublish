using GeoSit.Data.BusinessEntities.Mapa;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Contexts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class LayersController : ApiController
    {

        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Layers
        [ResponseType(typeof(ICollection<MapLayer>))]
        public IHttpActionResult GetMapLayersByIdMapa(short id)
        {
            return Ok(db.MapLayers.Where(l => l.IdMapa == id).ToList());
        }

        [ResponseType(typeof(MapLayer))]
        public IHttpActionResult GetMapLayerConfigMapaTematico()
        {
            return Ok(db.MapLayers.Single(l => l.NombreCapa == "TEMPLATE_MT"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public IHttpActionResult GetGeometriaObjeto(long idObjeto, string capa)
        {
            string tabla = string.Empty;
            switch (capa.ToUpper())
            {
                case "VW_PARCELAS_GRAF_ALFA":
                case "VW_PARCELAS_GRAF_ALFA_RURALES":
                case "PARCELAS_HUERFANAS":
                    tabla = "vw_parcelas";
                    break;
                case "VW_MANZANAS":
                    tabla = "vw_manzanas";
                    break;
                case "VW_MEJORAS":
                    tabla = "vw_mejoras";
                    break;
                default:
                    break;
            }

            try
            {
                if (string.IsNullOrEmpty(tabla))
                {
                    return BadRequest($"La capa {capa.ToUpper()} no permite descargar geometría");
                }
                using (var builder = db.CreateSQLQueryBuilder())
                {
                    string geojson = builder.AddNoTable()
                                            .AddFields(new Atributo { Funcion = $"obtener_geometria({idObjeto},'{tabla}')", Campo = "geojson" })
                                            .ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                                            {
                                                status.Break();
                                                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader.GetString(reader.GetOrdinal("geojson"))));
                                            }).SingleOrDefault();

                    return Ok(geojson);
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"GetGeometriaObjeto", ex);
                throw;
            }
        }

        public IHttpActionResult GetGeometriasObjetos(string capa, string features)
        {
            //var features = JsonConvert.DeserializeAnonymousType(seleccion, new { seleccion = new[] { new[] { 0L } }.ToList(), capas = new string[] { string.Empty }.ToList() });
 
            string tabla = string.Empty;
            switch (capa.ToUpper())
            {
                case "VW_PARCELAS_GRAF_ALFA":
                    tabla = "vw_parcelas";
                    break;
                case "VW_PARCELAS_GRAF_ALFA_RURALES":
                    tabla = "vw_parcelas";
                    break;
                case "PARCELAS_HUERFANAS":
                    tabla = "vw_parcelas";
                    break;
                case "VW_MANZANAS":
                    tabla = "vw_manzanas";
                    break;
                case "VW_MEJORAS":
                    tabla = "vw_mejoras";
                    break;
                default:
                    break;
            }
            string geojson = string.Empty;
            try
            {
                using (var builder = db.CreateSQLQueryBuilder())
                {
                    builder.AddNoTable()
                           .AddFields(new Atributo { Funcion = $"obtener_geometrias('{features}','{tabla}')", Campo = "geojson" })
                           .ExecuteQuery((System.Data.IDataReader reader) =>
                           {
                               geojson = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader.GetString(reader.GetOrdinal("geojson"))));
                           });
                    //var dt = builder.AddNoTable().AddFields(new Atributo { Funcion = $"obtener_geometria({idObjeto},'{tabla}')", Campo = "geojson" }).ExecuteDataTable();
                    //if (dt.Rows.Count > 0)
                    //{
                    //    geojson = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dt.Rows[0][0].ToString()));
                    //}
                }
                return Ok(geojson);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError($"GetGeometriaObjeto", ex);
                throw;
            }
        }


    }
}
