using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class DominioRepository : IDominioRepository
    {
        private readonly GeoSITMContext _context;

        public DominioRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<DominioUT> GetDominios(long idUnidadTributaria, bool esHistorico = false)
        {
            var dominiosUt = new List<DominioUT>();
            var dominios = _context.Dominios
                .Where(x => x.UnidadTributariaID == idUnidadTributaria && (esHistorico || x.FechaBaja == null)).ToList();
            var dominioTitularRepository = new DominioTitularRepository(_context);

            foreach (var dominio in dominios)
            {
                _context.Entry(dominio).Reference(x => x.TipoInscripcion).Load();

                dominiosUt.Add(new DominioUT
                {
                    DominioID = dominio.DominioID,
                    TipoInscripcionID = dominio.TipoInscripcionID,
                    TipoInscripcion = dominio.TipoInscripcion.Descripcion,
                    Inscripcion = dominio.Inscripcion,
                    Fecha = dominio.Fecha,
                    Titulares = dominioTitularRepository.GetDominioTitulares(dominio.DominioID)
                });
            }
            return dominiosUt;
        }

        public IEnumerable<DominioUT> GetDominiosHistorico(long idUnidadTributaria)
        {
            var dominiosUt = new List<DominioUT>();

            var dominios = _context.Dominios
                                   .Include(x => x.TipoInscripcion)
                                   .Where(x => x.UnidadTributariaID == idUnidadTributaria)
                                   .OrderBy(b => b.FechaAlta).ToList();

            var dominioTitularRepository = new DominioTitularRepository(_context);
            try
            {
                foreach (var dominio in dominios)
                {
                    dominiosUt.Add(new DominioUT
                    {
                        DominioID = dominio.DominioID,
                        TipoInscripcionID = dominio.TipoInscripcionID,
                        TipoInscripcion = dominio.TipoInscripcion.Descripcion,
                        Inscripcion = dominio.Inscripcion,
                        Fecha = dominio.Fecha,
                        FechaAlta = dominio.FechaAlta,
                        FechaBaja = dominio.FechaBaja,
                        Titulares = dominioTitularRepository.GetDominioTitularesHistorico(dominio.DominioID).OrderByDescending(X => X.FechaAlta)
                    });
                }
            }
            catch (Exception)
            {
                return null;
            }
            return dominiosUt;
        }

        public IEnumerable<DominioUT> GetDominiosFromView(long idUnidadTributaria)
        {
            //var pUnidadTributariaId = new OracleParameter("unidadTributariaId", idUnidadTributaria);
            //var dominiosUt = _context.Database.SqlQuery<DominioUT>("SELECT * FROM VIEW", pUnidadTributariaId).ToList();

            var dominiosUt = new List<DominioUT>
            {
                new DominioUT
                {
                    DominioID = 100,
                    TipoInscripcionID = 1,
                    TipoInscripcion = "Matrícula",
                    Inscripcion = "1123",
                    Fecha = DateTime.Now,
                    Titulares = new List<Titular>
                    {
                        new Titular
                        {
                            DominioPersonaId = 1,
                            NombreCompleto = "OJEDA SONIA MABEL",
                            PersonaId = 351,
                            PorcientoCopropiedad = 8,
                            TipoNoDocumento = "DNI/23567456",
                            TipoTitularidadId = 1,
                            TipoTitular = ""
                        }
                    }
                },
                new DominioUT
                {
                    DominioID = 101,
                    TipoInscripcionID = 2,
                    TipoInscripcion = "Ley",
                    Inscripcion = "5412",
                    Fecha = DateTime.Now,
                    Titulares = new List<Titular>
                    {
                        new Titular
                        {
                            DominioPersonaId = 2,
                            NombreCompleto = "ATALAN VERGARA MARIA ELCIRA",
                            PersonaId = 352,
                            PorcientoCopropiedad = 10,
                            TipoNoDocumento = "DNI/25231789",
                            TipoTitularidadId = 1,
                            TipoTitular = ""
                        }
                    }
                }
            };

            return dominiosUt;
        }

        public IEnumerable<Dominio> GetDominioByDominioTitular(List<DominioTitular> domTitular)
        {
            var dominiosUt = new List<DominioUT>();
            var dominios = _context.Dominios
                .Where(s => domTitular.Select(x => x.DominioID).Contains(s.DominioID));

            var dominioTitularRepository = new DominioTitularRepository(_context);

            try
            {
                foreach (var dominio in dominios)
                {
                    _context.Entry(dominio).Reference(x => x.TipoInscripcion).Load();

                    dominiosUt.Add(new DominioUT
                    {
                        DominioID = dominio.DominioID,
                        TipoInscripcionID = dominio.TipoInscripcionID,
                        TipoInscripcion = dominio.TipoInscripcion.Descripcion,
                        Inscripcion = dominio.Inscripcion,
                        Fecha = dominio.Fecha,
                        Titulares = dominioTitularRepository.GetDominioTitulares(dominio.DominioID)
                    });
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return dominios.ToList();
        }

        public Dominio GetDominioById(long idDominio, bool baja = true)
        {
            var d = _context.Dominios.Find(idDominio);
            if (baja) return d;
            if (d != null)
                return d.FechaBaja == null ? d : null;
            return null;
        }

        public Dominio GetDominio(long idUt, string inscripcion, bool baja = true)
        {
            var d = _context.Dominios
                .FirstOrDefault(x => x.Inscripcion == inscripcion && x.UnidadTributariaID == idUt);
            if (baja) return d;
            if (d != null)
                return d.FechaBaja == null ? d : null;
            return null;
        }

        public void InsertDominio(Dominio dominio)
        {
            dominio.IdUsuarioAlta = dominio.IdUsuarioModif;
            dominio.FechaAlta = dominio.FechaModif;
            _context.Dominios.Add(dominio);
        }

        public void UpdateDominio(Dominio dominio)
        {
            dominio.FechaModif = DateTime.Now;
            _context.Entry(dominio).State = EntityState.Modified;
            _context.Entry(dominio).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(dominio).Property(p => p.IdUsuarioAlta).IsModified = false;

        }

        public void DeleteDominiosByUnidadTributariaId(long idUnidadTributaria)
        {
            throw new NotImplementedException();
        }

        public void DeleteDominio(Dominio dominio)
        {
            if (dominio == null) return;

            var d = GetDominioById(dominio.DominioID);
            d.IdUsuarioModif = dominio.IdUsuarioModif;
            d.FechaModif = dominio.FechaModif;
            d.IdUsuarioBaja = d.IdUsuarioModif;
            d.FechaBaja = d.FechaModif;

            _context.Entry(d).State = EntityState.Modified;
            _context.Entry(d).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(d).Property(p => p.IdUsuarioAlta).IsModified = false;
        }

        public ICollection<Dominio> GetDominiosUFByParcela(long idParcela, bool esHistorico = false)
        {
            var dominios = (from dominio in _context.Dominios
                            join ut in _context.UnidadesTributarias on dominio.UnidadTributariaID equals ut.UnidadTributariaId
                            where ut.ParcelaID == idParcela && (esHistorico || ut.FechaBaja == null) && (esHistorico || dominio.FechaBaja == null) &&
                                       dominio.DominioID == (from dom in _context.Dominios
                                                             where dom.UnidadTributariaID == ut.UnidadTributariaId &&
                                                            (esHistorico || dom.FechaBaja == null)
                                                             select dom.DominioID).Max()
                            select dominio).ToList();

            return dominios;

        }
    }
}