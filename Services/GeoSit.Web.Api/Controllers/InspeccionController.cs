using System.Web.Http;
using GeoSit.Data.DAL.Common;
using System.Net.Http;
using System;
using System.Linq;

namespace GeoSit.Web.Api.Controllers
{
    public class InspeccionController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public InspeccionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get(long id)
        {
            var inspecciones = _unitOfWork.InspeccionRepository.GetInspecciones(id);
            return Ok(inspecciones);
        }

        [HttpPost]
        public IHttpActionResult GetInspeccionesPorPeriodo(string[] filtros)
        {
            try
            {

                if (!DateTime.TryParse(filtros[0], out DateTime dtFrom))
                {
                    throw new HttpRequestException("El parámetro [fechaDesde] es inválido.");
                }
                if (!DateTime.TryParse(filtros[1], out DateTime dtTo))
                {
                    throw new HttpRequestException("El parámetro [fechaHasta] es inválido.");
                }

                try
                {
                    var idsUsuarios = filtros.Skip(2).Select(long.Parse);
                    if (idsUsuarios.Any())
                    {
                        var inspecciones = _unitOfWork.InspeccionRepository.GetInspeccionesPorPeriodo(idsUsuarios.ToArray(), dtFrom, dtTo);
                        if (!inspecciones.Any())
                        {
                            return NotFound();
                        }
                        return Ok(inspecciones);
                    }
                    else
                    {
                        throw new HttpRequestException("No Existen Inspectores seleccionados para la Búsqueda.");
                    }
                }
                catch (FormatException)
                {
                    throw new HttpRequestException("El parámetro [usuario] es inválido.");
                }
            }
            catch (HttpRequestException req)
            {
                return ResponseMessage(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(req.Message)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetInspeccionesPorTipo(string[] filtros)
        {
            try
            {
                if (!DateTime.TryParse(filtros[0], out DateTime dtFrom))
                {
                    throw new HttpRequestException("El parámetro [fechaDesde] es inválido.");
                }
                if (!DateTime.TryParse(filtros[1], out DateTime dtTo))
                {
                    throw new HttpRequestException("El parámetro [fechaHasta] es inválido.");
                }
                try
                {
                    var inspecciones = _unitOfWork.InspeccionRepository.GetInspeccionesPorTipo(filtros.Skip(2).Select(int.Parse).ToArray(), dtFrom, dtTo);
                    if (!(inspecciones?.Any() ?? false))
                    {
                        throw new HttpRequestException("No hay datos para los parámetros indicados.");
                    }
                    return Ok(inspecciones);
                }
                catch (FormatException)
                {
                    throw new HttpRequestException("El parámetro [tipo] es inválido.");
                }
            }
            catch (HttpRequestException req)
            {
                return ResponseMessage(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new StringContent(req.Message)
                });
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult UpdateRelacion(string inspecciones, string numero, int tipo)
        {
            //Dictionary<string, string> datos = new Dictionary<string, string>();
            inspecciones = inspecciones.Replace("and", "%");
            var array = inspecciones.Split("%".ToCharArray());
            for (int a = 0; a < array.Length; a += 2)
            {
                //UPDATE INSPECCION
                if (array[a + 1] == "A")
                {
                    _unitOfWork.InspeccionRepository.EOUpdateInspeccionRelacionAlta(array[a], numero, tipo);
                }
                //array[a]-IdInspeccion;
                //array[a + 1]-A/B;
            }
            return Ok();
        }
    }
}
