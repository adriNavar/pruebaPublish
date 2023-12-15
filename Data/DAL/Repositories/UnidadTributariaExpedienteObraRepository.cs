using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class UnidadTributariaExpedienteObraRepository : IUnidadTributariaExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public UnidadTributariaExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<UnidadTributaria> GetUnidadTributariaExpedienteObrasByIdExpedienteObra(long idExpedienteObra)
        {
            var unidadTributarias = _context.UnidadesTributarias;
            var unidadTributariaExpedienteObras = _context.UnidadesTributariasExpedienteObra;
            var expedienteObras = _context.ExpedientesObra;

            var query = from u in unidadTributarias
                        join utEo in unidadTributariaExpedienteObras
                        on u.UnidadTributariaId equals utEo.UnidadTributariaId
                        join eo in expedienteObras on utEo.ExpedienteObraId equals eo.ExpedienteObraId
                        where utEo.ExpedienteObraId == idExpedienteObra
                        select u;

            return query.ToList();
        }

        public UnidadTributariaExpedienteObra GetUnidadTributariaExpedienteObraById(long idExpedienteObra, long idUnidadTributaria)
        {
            return _context.UnidadesTributariasExpedienteObra.Find(idExpedienteObra, idUnidadTributaria);
        }

        public UnidadTributariaExpedienteObra GetUnidadTributariaExpedienteObraById(long idUnidadTributaria)
        {
            return _context.UnidadesTributariasExpedienteObra.FirstOrDefault(x => x.UnidadTributariaId == idUnidadTributaria);
        }


        public void InsertUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra, ExpedienteObra expedienteObra)
        {
            unidadTributariaExpedienteObra.ExpedienteObra = expedienteObra;
            _context.UnidadesTributariasExpedienteObra.Add(unidadTributariaExpedienteObra);
        }

        public void InsertUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra)
        {
            _context.UnidadesTributariasExpedienteObra.Add(unidadTributariaExpedienteObra);
        }

        public void UpdateUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra)
        {
            _context.Entry(unidadTributariaExpedienteObra).State = EntityState.Modified;
        }

        public void DeleteUnidadTributariaExpedienteObra(long idExpedienteObra, long idUnidadTributaria)
        {
            DeleteUnidadTributariaExpedienteObra(GetUnidadTributariaExpedienteObraById(idExpedienteObra, idUnidadTributaria));
        }

        public void DeleteUnidadTributariasByExpedienteObraId(long idExpedienteObra)
        {
            var uteos = _context.UnidadesTributariasExpedienteObra.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var u in uteos)
            {
                _context.Entry(u).State = EntityState.Deleted;
            }
        }

        public void DeleteUnidadTributariaExpedienteObra(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra)
        {
            if (unidadTributariaExpedienteObra != null)
                _context.Entry(unidadTributariaExpedienteObra).State = EntityState.Deleted;
        }
    }
}
