using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Designaciones;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System;

namespace GeoSit.Data.DAL.Repositories
{
    public class DesignacionRepository : IDesignacionRepository
    {
        private readonly GeoSITMContext _context;

        public DesignacionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public Designacion GetDesignacion(long idParcela)
        {
            return _context.Designacion.FirstOrDefault(x => x.IdParcela == idParcela && x.FechaBaja == null);
        }

        public IEnumerable<Designacion> GetDesignacionesByParcela(long id)
        {
            return _context.Designacion.Include(x => x.TipoDesignador).Where(x => x.IdParcela == id && x.FechaBaja == null).ToList();
        }

        public IEnumerable<TipoDesignador> GetTiposDesignador()
        {
            return _context.TiposDesignador.ToList();
        }

        public void DeleteDesignacion(Designacion designacion)
        {
            designacion.IdUsuarioBaja = designacion.IdUsuarioModif;
            designacion.FechaModif = DateTime.Now;
            designacion.FechaBaja = designacion.FechaModif;
            _context.Entry(designacion).State = EntityState.Modified;
            _context.Entry(designacion).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(designacion).Property(p => p.IdUsuarioAlta).IsModified = false;
        }

        public void InsertDesignacion(Designacion designacion)
        {
            designacion.IdUsuarioAlta = designacion.IdUsuarioModif;
            designacion.FechaModif = DateTime.Now;
            designacion.FechaAlta = designacion.FechaModif;
            _context.Designacion.Add(designacion);
        }

        public void UpdateDesignacion(Designacion designacion)
        {
            designacion.FechaModif = DateTime.Now;
            _context.Entry(designacion).State = EntityState.Modified;
            _context.Entry(designacion).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(designacion).Property(p => p.IdUsuarioAlta).IsModified = false;
        }
    }
}