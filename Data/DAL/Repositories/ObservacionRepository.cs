using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class ObservacionRepository : IObservacionRepository
    {
        private readonly GeoSITMContext _context;

        public ObservacionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ObservacionExpediente> GetObservaciones(long idExpedienteObra)
        {
            return _context.ObservacionExpedientes.Where(x => x.ExpedienteObraId == idExpedienteObra);
        }

        public ObservacionExpediente GetObservacionById(long idObservacion)
        {
            return _context.ObservacionExpedientes.Find(idObservacion);
        }

        public void InsertObservacion(ObservacionExpediente observacionExpediente, ExpedienteObra expedienteObra)
        {
            observacionExpediente.ExpedienteObra = expedienteObra;
            _context.ObservacionExpedientes.Add(observacionExpediente);
        }

        public void InsertObservacion(ObservacionExpediente observacionExpediente)
        {
            _context.ObservacionExpedientes.Add(observacionExpediente);
        }

        public void UpdateObservacion(ObservacionExpediente observacionExpediente)
        {
            _context.Entry(observacionExpediente).State = EntityState.Modified;
        }

        public void DeleteObservacionesByExpedienteObraId(long idExpedienteObra)
        {
            var observaciones = GetObservaciones(idExpedienteObra);
            foreach (var observacion in observaciones)
            {
                _context.Entry(observacion).State = EntityState.Deleted;
            }
        }

        public void DeleteObservacionById(long idObservacion)
        {
            DeleteObservacion(GetObservacionById(idObservacion));
        }

        public void DeleteObservacion(ObservacionExpediente observacionExpediente)
        {
            if (observacionExpediente != null)
                _context.Entry(observacionExpediente).State = EntityState.Deleted;
        }
    }
}
