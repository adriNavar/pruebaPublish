using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class EstadoExpedienteRepository: IEstadoExpedienteRepository
    {
        private readonly GeoSITMContext _context;

        public EstadoExpedienteRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<EstadoExpediente> GetEstadoExpedientes()
        {
            return _context.EstadosExpediente;
        }
    }
}
