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
    class ServicioExpedienteObraRepository : IServicioExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public ServicioExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ServicioExpedienteObra> GetServicioExpedienteObras(long idExpedienteObra)
        {
            var servicios = _context.ServicioExpedienteObras
                                    .Include(x => x.Servicio)
                                    .Where(x => x.ExpedienteObraId == idExpedienteObra);
            return servicios.ToList();
        }

        public ServicioExpedienteObra GetServicioExpedienteObraById(long idExpedienteObra, long idServicioExpediente)
        {
            return _context.ServicioExpedienteObras.Find(idExpedienteObra, idServicioExpediente);
        }

        public void InsertServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra, ExpedienteObra expedienteObra)
        {
            servicioExpedienteObra.ExpedienteObra = expedienteObra;
            _context.ServicioExpedienteObras.Add(servicioExpedienteObra);
        }

        public void InsertServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra)
        {
            _context.ServicioExpedienteObras.Add(servicioExpedienteObra);
        }

        public void UpdateServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra)
        {
            _context.Entry(servicioExpedienteObra).State = EntityState.Modified;
        }

        public void DeleteServicioExpedienteObra(ServicioExpedienteObra servicioExpedienteObra)
        {
            if (servicioExpedienteObra != null)
                _context.Entry(servicioExpedienteObra).State = EntityState.Deleted;
        }

        public void DeleteServicioExpedientesByExpedienteObraId(long idExpedienteObra)
        {
            var servicios = _context.ServicioExpedienteObras.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var servicio in servicios)
            {
                _context.Entry(servicio).State = EntityState.Deleted;
            }
        }
    }
}
