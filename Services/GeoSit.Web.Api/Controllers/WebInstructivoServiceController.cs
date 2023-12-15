using GeoSit.Data.BusinessEntities.SubSistemaWeb;
using GeoSit.Data.DAL.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class WebInstructivoServiceController : ApiController
    {
        // GET api/WebInstructivo
        [ResponseType(typeof(ICollection<WebInstructivo>))]
        public IHttpActionResult GetWebInstructivo(long idUsuario)
        {
            using (var ctx = GeoSITMContext.CreateContext())
            {
                var query = from instructivo in ctx.WebInstructivo
                            where instructivo.IdFuncion == null || (from perfil in ctx.UsuariosPerfiles
                                                                    where perfil.Id_Usuario == idUsuario && perfil.Fecha_Baja == null
                                                                    join funcion in ctx.PerfilesFunciones on perfil.Id_Perfil equals funcion.Id_Perfil
                                                                    where funcion.Fecha_Baja == null
                                                                    select funcion.Id_Funcion).Any(f => f == instructivo.IdFuncion)
                            orderby new { instructivo.Seccion, instructivo.Descripcion }
                            select instructivo;

                return Ok(query.ToList());
            }
        }

    }
}