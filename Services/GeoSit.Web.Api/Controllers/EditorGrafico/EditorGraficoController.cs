using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos.DTO;
using GeoSit.Data.BusinessEntities.Via.DTO;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories.EditoresGraficos;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers.EditorGrafico
{
    public class EditorGraficoController : ApiController
    {
        #region Manzanas
        [Route("api/EditorGrafico/Search/Manzanas")]
        [HttpPost]
        public IHttpActionResult SearchManzanas(DataTableParameters parametros)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoManzanasRepository(ctx).RecuperarManzanas(parametros));
            }
        }

        [Route("api/EditorGrafico/Manzanas/{featId}")]
        [HttpGet]
        public IHttpActionResult GetManzana(long featId)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoManzanasRepository(ctx).RecuperarManzana(featId));
            }
        }

        [Route("api/EditorGrafico/Manzanas")]
        [HttpPost]
        public IHttpActionResult PostManzana(Division division)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoManzanasRepository(ctx).GuardarManzana(division));
            }
        }

        [Route("api/EditorGrafico/Manzanas/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteManzana(long id, long usuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoManzanasRepository(ctx).EliminarManzana(id, usuario));
            }
        }

        #endregion

        #region Jurisdicciones
        [Route("api/EditorGrafico/Search/Jurisdicciones")]
        [HttpPost]
        public IHttpActionResult SearchJurisdicciones(DataTableParameters parametros)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoJurisdiccionesRepository(ctx).RecuperarJurisdicciones(parametros));
            }
        }

        [Route("api/EditorGrafico/Jurisdicciones/{featId}")]
        [HttpGet]
        public IHttpActionResult GetJurisdiccion(long featId)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoJurisdiccionesRepository(ctx).RecuperarJurisdiccion(featId));
            }
        }

        [Route("api/EditorGrafico/Jurisdicciones")]
        [HttpPost]
        public IHttpActionResult PostJurisdiccion(JurisdiccionDTO objeto)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoJurisdiccionesRepository(ctx).GuardarJurisdiccion(objeto));
            }
        }

        [Route("api/EditorGrafico/Jurisdicciones/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteJurisdiccion(long id, long usuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoJurisdiccionesRepository(ctx).EliminarJurisdiccion(id, usuario));
            }
        }

        #endregion

        #region Secciones
        [Route("api/EditorGrafico/Search/Secciones")]
        [HttpPost]
        public IHttpActionResult SearchSecciones(DataTableParameters parametros)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoSeccionesRepository(ctx).RecuperarSecciones(parametros));
            }
        }

        [Route("api/EditorGrafico/Secciones/{featId}")]
        [HttpGet]
        public IHttpActionResult GetSeccion(long featId)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoSeccionesRepository(ctx).RecuperarSeccion(featId));
            }
        }

        [Route("api/EditorGrafico/Secciones")]
        [HttpPost]
        public IHttpActionResult PostSeccion(SeccionDTO objeto)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoSeccionesRepository(ctx).GuardarSeccion(objeto));
            }
        }

        [Route("api/EditorGrafico/Secciones/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteSeccion(long id, long usuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoSeccionesRepository(ctx).EliminarSeccion(id, usuario));
            }
        }
        #endregion

        #region Municipios
        [Route("api/EditorGrafico/Search/Municipios")]
        [HttpPost]
        public IHttpActionResult SearchMunicipios(DataTableParameters parametros)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoMunicipiosRepository(ctx).RecuperarMunicipios(parametros));
            }
        }

        [Route("api/EditorGrafico/Municipios/{featId}")]
        [HttpGet]
        public IHttpActionResult GetMunicipio(long featId)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoMunicipiosRepository(ctx).RecuperarMunicipio(featId));
            }
        }

        [Route("api/EditorGrafico/Municipios")]
        [HttpPost]
        public IHttpActionResult PostMunicipio(MunicipioDTO objeto)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoMunicipiosRepository(ctx).GuardarMunicipio(objeto));
            }
        }

        [Route("api/EditorGrafico/Municipios/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteMunicipio(long id, long usuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoMunicipiosRepository(ctx).EliminarMunicipio(id, usuario));
            }
        }
        #endregion

        #region Barrios
        [Route("api/EditorGrafico/Search/Barrios")]
        [HttpPost]
        public IHttpActionResult SearchBarrios(DataTableParameters parametros)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoBarriosRepository(ctx).RecuperarBarrios(parametros));
            }
        }

        [Route("api/EditorGrafico/Barrios/{featId}")]
        [HttpGet]
        public IHttpActionResult GetBarrio(long featId)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoBarriosRepository(ctx).RecuperarBarrio(featId));
            }
        }

        [Route("api/EditorGrafico/Barrios")]
        [HttpPost]
        public IHttpActionResult PostBarrio(BarrioDTO objeto)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoBarriosRepository(ctx).GuardarBarrio(objeto));
            }
        }

        [Route("api/EditorGrafico/Barrios/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteBarrio(long id, long usuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoBarriosRepository(ctx).EliminarBarrio(id, usuario));
            }
        }
        #endregion

        #region TramosVia
        [Route("api/EditorGrafico/Search/TramosVias")]
        [HttpPost]
        public IHttpActionResult SearchTramosVias(DataTableParameters parametros)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoTramosViasRepository(ctx).RecuperarTramosVias(parametros));
            }
        }

        [Route("api/EditorGrafico/TramosVias/{featId}")]
        [HttpGet]
        public IHttpActionResult GetTramoVia(long featId)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoTramosViasRepository(ctx).RecuperarTramoVia(featId));
            }
        }

        [Route("api/EditorGrafico/TramosVias")]
        [HttpPost]
        public IHttpActionResult PostTramoVia(TramoViaDTO tramoVia)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoTramosViasRepository(ctx).GuardarTramoVia(tramoVia));
            }
        }

        [Route("api/EditorGrafico/TramosVias/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteTramoVia(long id, long usuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoTramosViasRepository(ctx).EliminarTramoVia(id, usuario));
            }
        }

        [Route("api/EditorGrafico/TramosVias/Paridades")]
        [HttpGet]
        public IHttpActionResult GetParidades()
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                return Ok(new EditorGraficoTramosViasRepository(ctx).RecuperarParidades());
            }
        }
        #endregion

    }
}
