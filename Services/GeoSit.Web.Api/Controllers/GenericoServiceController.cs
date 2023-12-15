using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.Generico;
using GeoSit.Data.BusinessEntities.Seguridad;
using System;
using GeoSit.Data.DAL.Contexts;
using System.Data.Entity;
using GeoSit.Data.BusinessEntities.Mapa;
using System.Configuration;
using System.Text;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using System.Data.Entity.Spatial;

namespace GeoSit.Web.Api.Controllers
{
    public class GenericoServiceController : ApiController
    {
        // GET api/Generico
        [ResponseType(typeof(MenuItem))]
        public IHttpActionResult GetMenuItem()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    List<MenuItem> listaMenu = db.MenuItem.OrderBy(x => x.MenuItemId).ToList();
                    if (listaMenu == null)
                    {
                        return NotFound();
                    }
                    return Ok(listaMenu);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(UsuariosActivos))]
        public IHttpActionResult GetUsuariosActivosByUserId(long id)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                List<UsuariosActivos> usuariosActivos = db.UsuariosActivos.Where(x => x.Id_Usuario == id).ToList();

                if (usuariosActivos == null)
                {
                    return Ok(new List<UsuariosActivos>());
                }
                return Ok(usuariosActivos);
            }
        }
        [HttpGet]
        [ResponseType(typeof(UsuariosActivos))]
        public IHttpActionResult SetUsuariosActivos(long id)
        {
            using (var db = GeoSITMContext.CreateContext())
            {

                var ua = db.UsuariosActivos.Add(new UsuariosActivos()
                {
                    Id_Usuario = id,
                    Fecha = DateTime.Now,
                    Heartbeat = DateTime.Now,
                    Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                });
                db.SaveChanges();

                return Ok(ua);
            }
        }

        [ResponseType(typeof(List<UsuariosActivos>))]
        public IHttpActionResult CleanUsuariosActivosByUserId(long id)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                db.UsuariosActivos.RemoveRange(db.UsuariosActivos.Where(x => x.Id_Usuario == id));
                db.SaveChanges();
                return Ok();
            }
        }
        [ResponseType(typeof(List<UsuariosActivos>))]
        public IHttpActionResult CleanUsuariosActivosAll()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    foreach (var ua in db.UsuariosActivos)
                    {
                        db.Entry(ua).State = EntityState.Deleted;
                    }
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception)
            {
                return Ok();
            }
        }
        [HttpPost]
        public IHttpActionResult CleanUsuariosActivosByToken([FromBody] string token)
        {
            using (var db = GeoSITMContext.CreateContext())
            {
                try
                {
                    db.UsuariosActivos.RemoveRange(db.UsuariosActivos.Where(x => x.Token == token));
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    Global.GetLogger().LogError($"CleanUsuariosActivosByToken({token})", ex);
                }
                return Ok();
            }
        }
        public IHttpActionResult GetMapSRID()
        {
            return Ok(new SRIDDefinition() { Code = ConfigurationManager.AppSettings["DBSRID"], Definition = ConfigurationManager.AppSettings["DBSRIDDefinition"] });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public IHttpActionResult EncriptarRijndael(string texto, string key)
        {
            return Ok(Geosit.Core.Utils.Crypto.RijndaelCypher.EncryptText(texto, key));
        }

        [HttpGet]
        public IHttpActionResult DesencriptarRijndael(string texto, string key)
        {
            return Ok(Geosit.Core.Utils.Crypto.RijndaelCypher.DecryptText(texto, key));
        }

        [HttpGet]
        public IHttpActionResult RegexRandomGenerator(string regex)
        {
            string pattern = Encoding.UTF8.GetString(Convert.FromBase64String(regex));
            if(pattern == "^.+$")
            {
                return Ok();
            }
            Fare.Xeger xeger = new Fare.Xeger(pattern, new Random(0));
            string text = xeger.Generate();
            text = System.Text.RegularExpressions.Regex.Replace(text, "\\d", "9");
            text = System.Text.RegularExpressions.Regex.Replace(text, "[a-zA-Z]", "X");
            return Ok(text);
        }
    }
}