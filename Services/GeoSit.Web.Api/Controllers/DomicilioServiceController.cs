using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Web.Api.Common;
using GeoSit.Data.DAL.Contexts;
using System;

namespace GeoSit.Web.Api.Controllers
{
    public class DomicilioServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/Domicilio
        [ResponseType(typeof(ICollection<Domicilio>))]
        public IHttpActionResult GetDomicilios()
        {
            return Ok(db.Domicilios.Where(a => a.UsuarioBajaId == 0).ToList());
        }

        // GET api/GetDomicilioById
        [ResponseType(typeof(Domicilio))]
        public IHttpActionResult GetDomicilioById(long id)
        {
            Domicilio dom = db.Domicilios.Where(a => a.DomicilioId == id && (a.UsuarioBajaId == null || a.UsuarioBajaId == 0)).FirstOrDefault();
            if (dom == null)
            {
                return NotFound();
            }
            db.Entry(dom).Reference(d => d.TipoDomicilio).Load();
            return Ok(dom);
        }


        // GET api/GetDatosDomicilioByDescripcion
        [ResponseType(typeof(ICollection<Domicilio>))]
        public IHttpActionResult GetDatosDomicilioByViaNombre(string id)
        {
            List<Domicilio> dom = db.Domicilios.Where(a => (a.ViaNombre.ToUpper().Contains(id.ToUpper())) && (a.UsuarioBajaId == null || a.UsuarioBajaId == 0)).ToList();
            if (dom == null)
            {
                return NotFound();
            }
            return Ok(dom);
        }

        // GET api/GetDatosDomicilioByPropiedades
        [ResponseType(typeof(Domicilio))]
        public IHttpActionResult GetDatosDomicilioByPropiedades(string nombrevia, string puerta, string localidad, long tipodomicilio, string piso, string depto)
        {
            string fil_piso = piso ?? string.Empty;
            string fil_depto = depto ?? string.Empty;

            Domicilio dom = db.Domicilios.Where(a => a.ViaNombre.ToUpper() == nombrevia.ToUpper() && a.numero_puerta.ToUpper() == puerta.ToUpper() && a.ViaNombre.ToUpper() == localidad.ToUpper() && a.TipoDomicilioId == tipodomicilio && (a.UsuarioBajaId == null || a.UsuarioBajaId == 0) && a.piso == fil_piso && a.unidad == fil_depto).FirstOrDefault();
            if (dom == null)
            {
                return NotFound();
            }
            return Ok(dom);
        }

        [HttpPost]
        public IHttpActionResult SetDomicilio_Save(Domicilio parametros)
        {
            try
            {
                string tipoOperacion = TiposOperacion.Alta, evento = Eventos.AltaDomicilio, mensaje = Mensajes.AltaDomicilioOK;
                parametros.FechaModif = DateTime.Now;
                var current = db.Domicilios.SingleOrDefault(x => x.DomicilioId == parametros.DomicilioId && x.FechaBaja == null);
                if (current == null)
                {
                    current = db.Domicilios.Add(new Domicilio()
                    {
                        UsuarioAltaId = parametros._Id_Usuario,
                        FechaAlta = parametros.FechaModif
                    });
                }
                else
                {
                    tipoOperacion = TiposOperacion.Modificacion;
                    evento = Eventos.ModificarDomicilio;
                    mensaje = Mensajes.ModificarDomicilioOK;
                }
                current.ViaNombre = string.IsNullOrEmpty(parametros.ViaNombre) ? string.Empty : parametros.ViaNombre;
                current.numero_puerta = string.IsNullOrEmpty(parametros.numero_puerta) ? null : parametros.numero_puerta;
                current.piso = string.IsNullOrEmpty(parametros.piso) ? null : parametros.piso;
                current.unidad = string.IsNullOrEmpty(parametros.unidad) ? null : parametros.unidad;
                current.barrio = string.IsNullOrEmpty(parametros.barrio) ? null : parametros.barrio;
                current.localidad = string.IsNullOrEmpty(parametros.localidad) ? null : parametros.localidad;
                current.municipio = string.IsNullOrEmpty(parametros.municipio) ? null : parametros.municipio;
                current.provincia = string.IsNullOrEmpty(parametros.provincia) ? null : parametros.provincia;
                current.pais = parametros.pais;
                current.ubicacion = string.IsNullOrEmpty(parametros.ubicacion) ? null : parametros.ubicacion;
                current.codigo_postal = string.IsNullOrEmpty(parametros.codigo_postal) ? null : parametros.codigo_postal;
                current.ViaId = parametros.ViaId == 0 ? null : parametros.ViaId;
                current.IdLocalidad = parametros.IdLocalidad == 0 ? null : parametros.IdLocalidad;
                current.ProvinciaId = parametros.ProvinciaId == 0 ? null : parametros.ProvinciaId;
                current.TipoDomicilioId = parametros.TipoDomicilioId;

                current.UsuarioModifId = parametros._Id_Usuario;
                current.FechaModif = parametros.FechaModif;


                db.SaveChanges(new Auditoria(parametros._Id_Usuario, evento, mensaje, parametros._Machine_Name, 
                               parametros._Machine_Name, Autorizado.Si, null, parametros, "Domicilio", 1, tipoOperacion));

                return Ok(current);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("DomicilioServiceController-SetDomicilio_Save", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteDomicilio_Save(Domicilio parametros)
        {
            try
            {
                var current = db.Domicilios.SingleOrDefault(x => x.DomicilioId == parametros.DomicilioId && x.FechaBaja == null);
                if (current != null)
                {
                    db.SaveChanges(new Auditoria(parametros._Id_Usuario, Eventos.BajaDomicilio, Mensajes.BajaDomicilioOK, parametros._Machine_Name,
                   parametros._Machine_Name, Autorizado.Si, null, parametros, "Domicilio", 1, TiposOperacion.Baja));
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("DomicilioServiceController-DeleteDomicilio_Save", ex);
                return InternalServerError();
            }
            return Ok();
        }

    }
}