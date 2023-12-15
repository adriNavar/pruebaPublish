using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    class TipoSuperficieExpedienteObraRepository : ITipoSuperficieExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public TipoSuperficieExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoSuperficieExpedienteObra> GetTipoSuperficieExpedienteObras(long idExpedienteObra)
        {
            return _context.TipoSuperficieExpedienteObras
                           .Include(x => x.TipoSuperficie)
                           .Include(x => x.Destino)
                           .Where(x => x.ExpedienteObraId == idExpedienteObra)
                           .ToList();
        }

        public TipoSuperficieExpedienteObra GetTipoSuperficieExpedienteObraById(long idExpedienteObraSuperficie)
        {
            return _context.TipoSuperficieExpedienteObras.Find(idExpedienteObraSuperficie);
        }

        public void InsertTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra, ExpedienteObra expedienteObra)
        {
            tipoSuperficieExpedienteObra.ExpedienteObra = expedienteObra;
            _context.TipoSuperficieExpedienteObras.Add(tipoSuperficieExpedienteObra);
        }

        public void InsertTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra)
        {
            _context.TipoSuperficieExpedienteObras.Add(tipoSuperficieExpedienteObra);
        }

        public void UpdateTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra)
        {
            _context.Entry(tipoSuperficieExpedienteObra).State = EntityState.Modified;
        }

        public void DeleteTipoSuperficiesByExpedienteObraId(long idExpedienteObra)
        {
            var tipos = _context.TipoSuperficieExpedienteObras.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var tipo in tipos)
            {
                _context.Entry(tipo).State = EntityState.Deleted;
            }
        }

        public void DeleteTipoSuperficieExpedienteObraById(long idExpedienteObraSuperficie)
        {
            DeleteTipoSuperficieExpedienteObra(GetTipoSuperficieExpedienteObraById(idExpedienteObraSuperficie));
        }

        public void DeleteTipoSuperficieExpedienteObra(TipoSuperficieExpedienteObra tipoSuperficieExpedienteObra)
        {
            if (tipoSuperficieExpedienteObra != null)
                _context.Entry(tipoSuperficieExpedienteObra).State = EntityState.Deleted;
        }
    }
}
