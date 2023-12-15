using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using Newtonsoft.Json;

namespace GeoSit.Data.DAL.Repositories
{
    public class UnidadTributariaPersonaRepository : IUnidadTributariaPersonaRepository
    {
        private readonly GeoSITMContext _context;

        public UnidadTributariaPersonaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ResponsableFiscal> GetUnidadTributariaResponsablesFiscales(long idUnidadTributaria)
        {            
            var responsablesFiscales = new List<ResponsableFiscal>();

            var unidadTributariaPersonas = _context.UnidadTributariaPersonas
                .Include(x => x.Persona)
                .Include(x => x.TipoPersona)
                .Where(x => x.UnidadTributariaID == idUnidadTributaria)
                .Where(x => x.FechaBaja == null)
                .ToList();                

            long tipoDomicilioPorDefecto = Int64.Parse(_context.ParametrosGenerales.Find(36).Valor);

            foreach (var unidadTributariaPersona in unidadTributariaPersonas)
            {
                _context.Entry(unidadTributariaPersona.Persona)
                        .Collection(x => x.PersonaDomicilios)
                        .Query()
                        .Where(x => x.TipoDomicilioId == tipoDomicilioPorDefecto)
                        .Load();

                if (unidadTributariaPersona.Persona.PersonaDomicilios != null)
                {
                    var domicilioFisico = unidadTributariaPersona.Persona.PersonaDomicilios.FirstOrDefault();
                    if (domicilioFisico != null)
                        _context.Entry(domicilioFisico).Reference(x => x.Domicilio).Load();

                    responsablesFiscales.Add(new ResponsableFiscal
                    {
                        PersonaId = unidadTributariaPersona.PersonaID,
                        TipoPersonaId = unidadTributariaPersona.TipoPersonaID,
                        TipoPersona = unidadTributariaPersona.TipoPersona.Descripcion,
                        NombreCompleto = unidadTributariaPersona.Persona.NombreCompleto,
                        //DomicilioFisico = domicilioFisico != null ? domicilioFisico.Domicilio.ViaNombre + " " + domicilioFisico.Domicilio.numero_puerta + " " + domicilioFisico.Domicilio.piso + " " + domicilioFisico.Domicilio.unidad + " " + domicilioFisico.Domicilio.localidad : string.Empty,                        
                        DomicilioFisico = domicilioFisico != null ? domicilioFisico.Domicilio.ViaNombre : string.Empty,
                        CodSistemaTributario = unidadTributariaPersona.CodSistemaTributario,
                        Domicilio = domicilioFisico.Domicilio
                    });
                }
            }                            

            return responsablesFiscales;
        }        

        public IEnumerable<ResponsableFiscal> GetUnidadTributariaResponsablesFiscalesFromView(long idUnidadTributaria)
        {
            var responsablesFiscales = new List<ResponsableFiscal>
            {
                new ResponsableFiscal
                {
                    PersonaId = 100,
                    TipoPersonaId = 1,
                    TipoPersona = "Física",
                    NombreCompleto = "JULIANA PEREZ",
                    DomicilioFisico = "ECUADOR Nro  911  Barrio PROGRESO"
                },
                new ResponsableFiscal
                {
                    PersonaId = 101,
                    TipoPersonaId = 2,
                    TipoPersona = "Jurídica",
                    NombreCompleto = "MARCOS HERNANDEZ",
                    DomicilioFisico = "AMEGHINO NORTE Nro  631  Barrio OESTE"
                }
            };

            return responsablesFiscales;
        }

        public UnidadTributariaPersona GetUnidadTributariaPersonaById(long idUnidadTributaria, long idPersona, bool baja = false)
        {
            var utp = _context.UnidadTributariaPersonas.Find(idUnidadTributaria, idPersona);
            if (baja || (utp != null && utp.FechaBaja == null))
            {
                return utp;
            }
            return null;
        }

        public void InsertUnidadTributariaPersona(UnidadTributariaPersona unidadTributariaPersona)
        {
            unidadTributariaPersona.UsuarioAltaID = unidadTributariaPersona.UsuarioModificacionID;
            unidadTributariaPersona.FechaModificacion = DateTime.Now;
            unidadTributariaPersona.FechaAlta = unidadTributariaPersona.FechaModificacion;
            _context.UnidadTributariaPersonas.Add(unidadTributariaPersona);
        }

        public void UpdateUnidadTributariaPersona(UnidadTributariaPersona unidadTributariaPersona)
        {
            unidadTributariaPersona.FechaModificacion = DateTime.Now;
            if (unidadTributariaPersona.PersonaID == unidadTributariaPersona.PersonaSavedId)
            {
                var utp = _context.Entry(unidadTributariaPersona);
                utp.State = EntityState.Modified;
                utp.Property(x => x.UsuarioAltaID).IsModified = false;
                utp.Property(x => x.FechaAlta).IsModified = false;

            }
            else
            {
                unidadTributariaPersona.UsuarioAltaID = unidadTributariaPersona.UsuarioModificacionID;
                unidadTributariaPersona.FechaAlta = unidadTributariaPersona.FechaModificacion;
                InsertUnidadTributariaPersona(unidadTributariaPersona);

                var unidadTributariaPersonaCloned = (UnidadTributariaPersona) Clone(unidadTributariaPersona);
                unidadTributariaPersonaCloned.PersonaID = unidadTributariaPersonaCloned.PersonaSavedId;
                unidadTributariaPersonaCloned.UsuarioModificacionID = unidadTributariaPersonaCloned.UsuarioModificacionID;

                DeleteUnidadTributariaPersona(unidadTributariaPersonaCloned);
            }
        }

        public void DeleteUnidadTributariaPersona(UnidadTributariaPersona unidadTributariaPersona)
        {
            if (unidadTributariaPersona == null) return;

            var utp = GetUnidadTributariaPersonaById(unidadTributariaPersona.UnidadTributariaID, unidadTributariaPersona.PersonaID);
            utp.FechaModificacion = DateTime.Now;
            utp.FechaBaja = utp.FechaModificacion;
            utp.UsuarioModificacionID = unidadTributariaPersona.UsuarioModificacionID;
            utp.UsuarioBajaID = utp.UsuarioModificacionID;

            _context.Entry(utp).State = EntityState.Modified;
            _context.Entry(utp).Property(x => x.UsuarioAltaID).IsModified = false;
            _context.Entry(utp).Property(x => x.FechaAlta).IsModified = false;
        }

        public Object Clone<T>(T obj)
        {
            var clone = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
            return clone;
        }
    }
}
