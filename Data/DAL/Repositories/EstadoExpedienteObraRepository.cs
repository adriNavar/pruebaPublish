using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Interfaces;
using System;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class EstadoExpedienteObraRepository: IEstadoExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public EstadoExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<EstadoExpedienteObra> GetEstadoExpedienteObras(long idExpedienteObra)
        {
            var estados = _context.EstadosExpedienteObra
                                  .Include(x => x.EstadoExpediente)
                                  .Where(x => x.ExpedienteObraId == idExpedienteObra);
            return estados.ToList();
        }

        public EstadoExpedienteObra GetEstadoExpedienteObraById(long idExpedienteObra, long idEstadoExpediente, DateTime fecha)
        {
            return _context.EstadosExpedienteObra.Find(idExpedienteObra, idEstadoExpediente, fecha);
        }

        public void InsertEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra, ExpedienteObra expedienteObra)
        {
            estadoExpedienteObra.ExpedienteObra = expedienteObra;
            _context.EstadosExpedienteObra.Add(estadoExpedienteObra);
        }

        public void InsertEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra)
        {
            _context.EstadosExpedienteObra.Add(estadoExpedienteObra);
        }

        public void UpdateEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra)
        {
            _context.Entry(estadoExpedienteObra).State = EntityState.Modified;
        }

        public void DeleteEstadoExpedientesByExpedienteObraId(long idExpedienteObra)
        {
            var estados = _context.EstadosExpedienteObra.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var estado in estados)
            {
                _context.Entry(estado).State = EntityState.Deleted;
            }
        }

        public void DeleteEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra)
        {
            if (estadoExpedienteObra != null)
                _context.Entry(estadoExpedienteObra).State = EntityState.Deleted;
        }
    }
}
