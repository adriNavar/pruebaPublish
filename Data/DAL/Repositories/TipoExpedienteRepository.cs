using System;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoExpedienteRepository : ITipoExpedienteRepository
    {
        private readonly GeoSITMContext _context;

        public TipoExpedienteRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TipoExpediente> GetTipoExpedientes()
        {
            return _context.TiposExpediente;
        }

        public IEnumerable<TipoExpediente> GetTipoExpedientes(long idExpedienteObra)
        {
            var tipoExpedientes = _context.TiposExpediente;
            var tipoExpdienteObras = _context.TipoExpedienteObras;
            var expedienteObras = _context.ExpedientesObra;

            return from t in tipoExpedientes
                   join teo in tipoExpdienteObras on t.TipoExpedienteId equals teo.TipoExpedienteId
                   join eo in expedienteObras on teo.ExpedienteObraId equals eo.ExpedienteObraId
                   where teo.ExpedienteObraId == idExpedienteObra
                   select t;
        }

        public TipoExpediente GetTipoExpedienteById(long idTipoExpediente)
        {
            throw new NotImplementedException();
        }

        public void InsertTipoExpediente(long idTipoExpediente)
        {
            throw new NotImplementedException();
        }

        public void UpdateTipoExpediente(TipoExpediente tipoExpediente)
        {
            throw new NotImplementedException();
        }

        public void DeleteTipoExpediente(long idTipoExpediente)
        {
            throw new NotImplementedException();
        }

        public void DeleteTipoExpediente(TipoExpediente tipoExpediente)
        {
            throw new NotImplementedException();
        }
    }
}
