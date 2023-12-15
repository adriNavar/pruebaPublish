using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using System;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IEstadoExpedienteObraRepository
    {
        IEnumerable<EstadoExpedienteObra> GetEstadoExpedienteObras(long idExpedienteObra);

        EstadoExpedienteObra GetEstadoExpedienteObraById(long idExpedienteObra, long idEstadoExpediente, DateTime fecha);

        void InsertEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra, ExpedienteObra expedienteObra);

        void InsertEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra);

        void UpdateEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra);

        void DeleteEstadoExpedientesByExpedienteObraId(long idExpedienteObra);

        void DeleteEstadoExpedienteObra(EstadoExpedienteObra estadoExpedienteObra);
    }
}
