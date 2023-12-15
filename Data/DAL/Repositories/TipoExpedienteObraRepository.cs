using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    class TipoExpedienteObraRepository : ITipoExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public TipoExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoExpedienteObra> GetTipoExpedienteObras(long idExpedienteObra)
        {
            return _context.TipoExpedienteObras.Include(x => x.TipoExpediente).Where(x => x.ExpedienteObraId == idExpedienteObra);
        }

        public TipoExpedienteObra GetTipoExpedienteObraById(long idExpedienteObra, long idTipoExpediente)
        {
            return _context.TipoExpedienteObras.Find(idExpedienteObra, idTipoExpediente);
        }

        public void InsertTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra, ExpedienteObra expedienteObra)
        {
            tipoExpedienteObra.ExpedienteObra = expedienteObra;
            _context.TipoExpedienteObras.Add(tipoExpedienteObra);
        }

        public void InsertTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra)
        {
            _context.TipoExpedienteObras.Add(tipoExpedienteObra);
        }

        public void UpdateTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra)
        {
            _context.Entry(tipoExpedienteObra).State = EntityState.Modified;
        }

        public void DeleteTipoExpedienteObra(TipoExpedienteObra tipoExpedienteObra)
        {
            if (tipoExpedienteObra != null)
                _context.Entry(tipoExpedienteObra).State = EntityState.Deleted;
        }

        public void DeleteTipoExpedientesByExpedienteObraId(long idExpedienteObra)
        {
            var tipos = _context.TipoExpedienteObras.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var tipo in tipos)
            {
                _context.Entry(tipo).State = EntityState.Deleted;
            }
        }
    }
}
