using GeoSit.Data.BusinessEntities.SubSistemaWeb;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class AyudaLineaServiceController : ApiController
    {
        // GET api/AyudaLinea
        [ResponseType(typeof(ICollection<AyudaLinea>))]
        public IHttpActionResult GetAyudaLinea(long idUsuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                var query = from ayuda in ctx.AyudaLinea
                            where ayuda.IdFuncion == null || (from perfil in ctx.UsuariosPerfiles
                                                                    where perfil.Id_Usuario == idUsuario && perfil.Fecha_Baja == null
                                                                    join funcion in ctx.PerfilesFunciones on perfil.Id_Perfil equals funcion.Id_Perfil
                                                                    where funcion.Fecha_Baja == null
                                                                    select funcion.Id_Funcion).Any(f => f == ayuda.IdFuncion)
                       
                            orderby new { ayuda.Seccion, ayuda.Descripcion }
                            select ayuda;

                return Ok(query.ToList());
            }
        }

    }
}