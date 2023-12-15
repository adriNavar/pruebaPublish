using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;

namespace GeoSit.Data.DAL.Repositories
{
    public class DominioTitularRepository : IDominioTitularRepository
    {
        private readonly GeoSITMContext _context;

        public DominioTitularRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Titular> GetDominioTitulares(long idDominio)
        {
            var query = from d in _context.DominioTitulares
                        join dtt in _context.TiposTitularidad on d.TipoTitularidadID equals dtt.IdTipoTitularidad
                        join p in _context.Persona on d.PersonaID equals p.PersonaId
                        join tdi in _context.TipoDoc on p.TipoDocId equals tdi.Id_Tipo_Doc_Ident
                        where (d.DominioID == idDominio || idDominio == 0) && d.FechaBaja == null
                        select new Titular
                        {
                            PersonaId = d.PersonaID,
                            TipoTitularidadId = d.TipoTitularidadID.Value,
                            NombreCompleto = p.NombreCompleto,
                            TipoTitular = dtt.Descripcion,
                            TipoNoDocumento = tdi.Descripcion + "/" + p.NroDocumento,
                            PorcientoCopropiedad = d.PorcientoCopropiedad
                        };

            return query.ToList();
        }

        public IEnumerable<Titular> GetDominioTitularesHistorico(long idDominio)
        {
            try
            {
                var query = from d in _context.DominioTitulares
                                //join dtp in _context.TipoPersonas on d.TipoPersonaID equals dtp.TipoPersonaId
                            join p in _context.Persona on d.PersonaID equals p.PersonaId
                            join t in _context.TiposTitularidad on d.TipoTitularidadID equals t.IdTipoTitularidad
                            join tdi in _context.TipoDoc on p.TipoDocId equals tdi.Id_Tipo_Doc_Ident
                            where (d.DominioID == idDominio || idDominio == 0) /*&& d.FechaBaja == null*/
                            select new Titular
                            {
                                PersonaId = d.PersonaID,
                                TipoTitularidadId = d.TipoTitularidadID.Value,
                                NombreCompleto = p.NombreCompleto,
                                //TipoPersona = dtp.Descripcion,
                                TipoTitular = d.TipoTitularidad.Descripcion,
                                TipoNoDocumento = tdi.Descripcion + "/" + p.NroDocumento,
                                PorcientoCopropiedad = d.PorcientoCopropiedad,
                                FechaAlta = d.FechaAlta
                            };

                return query.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*public Titular GetDominioTitularByTitularId(long idTitular)
        {
            var dominioTitulares = _context.DominioTitulares;
            var personas = _context.Persona;
            var tiposDocsIdent = _context.TipoDoc;

            var query = from d in dominioTitulares
                        join p in personas on d.PersonaID equals p.PersonaId
                        join tdi in tiposDocsIdent on p.TipoDocId equals tdi.Id_Tipo_Doc_Ident
                        where ((d.PersonaID == idTitular || idTitular == 0) && d.FechaBaja == null)
                        select new Titular
                        {
                            PersonaId = d.PersonaID,
                            TipoPersonaId = d.TipoPersonaID,
                            NombreCompleto = p.NombreCompleto,
                            TipoPersona = p.TipoPersona.Descripcion,
                            TipoNoDocumento = tdi.Descripcion + "/" + p.NroDocumento,
                            PorcientoCopropiedad = d.PorcientoCopropiedad
                        };

            return query.FirstOrDefault();
        }*/

        public IEnumerable<DominioTitular> GetDominioTitularByTitularId(long idTitular)
        {
            try
            {
                var dominioTitulares = _context.DominioTitulares.Include(s => s.Persona)
                    .Include(r => r.Persona.TipoDocumentoIdentidad)
                    .Include(p => p.TipoTitularidad)
                    .Include(d => d.Dominio)
                    .Include(f => f.Dominio.UnidadTributaria)
                    .Include(o => o.Dominio.UnidadTributaria.Parcela)
                    .Where(x => x.PersonaID == idTitular && x.FechaBaja == null);
                return dominioTitulares.ToList();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetDominioTitularByTitularId", ex);
                return null;
            }

        }

        public IEnumerable<Titular> GetDominioTitularesFromView(long idDominio)
        {
            //Si idDominio es cero listar todos sino filtrar por idDominio

            //var pDominioId = new OracleParameter("dominioId", idDominio);
            //var titulares = _context.Database.SqlQuery<Titular>("SELECT * FROM VIEW", pDominioId).ToList();            

            return new List<Titular>
            {
                new Titular
                {
                    PersonaId = 200,
                    TipoTitularidadId = 1,
                    NombreCompleto = "ROBERTO LOPEZ",
                    TipoTitular = "Física",
                    TipoNoDocumento = "DNI/12543625",
                    PorcientoCopropiedad = 20.50M
                },
                new Titular
                {
                    PersonaId = 201,
                    TipoTitularidadId = 2,
                    NombreCompleto = "LUCIA IBAÑEZ",
                    TipoTitular = "Jurídica",
                    TipoNoDocumento = "DNI/12987678",
                    PorcientoCopropiedad = 30.31M
                },
                new Titular
                {
                    PersonaId = 203,
                    TipoTitularidadId = 2,
                    NombreCompleto = "LEON FERRER",
                    TipoTitular= "Jurídica",
                    TipoNoDocumento = "DNI/14098101",
                    PorcientoCopropiedad = 60.99M
                },
                new Titular
                {
                    PersonaId = 204,
                    TipoTitularidadId = 1,
                    NombreCompleto = "OREL AMELIA",
                    TipoTitular = "Física",
                    TipoNoDocumento = "DNI/13271864",
                    PorcientoCopropiedad = 50.50M
                }
            };
        }


        public DominioTitular GetDominioTitularById(long idDominio, long idPersona)
        {
            return _context.DominioTitulares.SingleOrDefault(x => x.DominioID == idDominio && x.PersonaID == idPersona && x.FechaBaja == null);
        }

        public void InsertDominioTitular(DominioTitular dominioTitular)
        {
            dominioTitular.UsuarioAltaID = dominioTitular.UsuarioModificacionID;
            dominioTitular.FechaModificacion = DateTime.Now;
            dominioTitular.FechaAlta = dominioTitular.FechaModificacion;
            _context.DominioTitulares.Add(dominioTitular);
        }

        public void UpdateDominioTitular(DominioTitular dominioTitular)
        {
            var dt = GetDominioTitularById(dominioTitular.DominioID, dominioTitular.PersonaID);
            dt.TipoTitularidadID = dominioTitular.TipoTitularidadID;
            dt.PorcientoCopropiedad = dominioTitular.PorcientoCopropiedad;
            dt.UsuarioModificacionID = dominioTitular.UsuarioModificacionID;
            dt.FechaModificacion = DateTime.Now;
            _context.Entry(dt).State = EntityState.Modified;

        }

        public void DeleteDominioTitular(DominioTitular dominioTitular)
        {
            var dt = GetDominioTitularById(dominioTitular.DominioID, dominioTitular.PersonaID);
            dt.UsuarioModificacionID = dominioTitular.UsuarioModificacionID;
            dt.FechaModificacion = DateTime.Now;
            dt.FechaBaja = dt.FechaModificacion;
            dt.UsuarioBajaID = dt.UsuarioModificacionID;
        }
    }
}
