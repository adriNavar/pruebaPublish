using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IObservacionRepository
    {
        IEnumerable<ObservacionExpediente> GetObservaciones(long idExpedienteObra);

        ObservacionExpediente GetObservacionById(long idObservacion);

        void InsertObservacion(ObservacionExpediente observacionExpediente, ExpedienteObra expedienteObra);

        void InsertObservacion(ObservacionExpediente observacionExpediente);

        void UpdateObservacion(ObservacionExpediente observacionExpediente);

        void DeleteObservacionesByExpedienteObraId(long idExpedienteObra);

        void DeleteObservacionById(long idObservacion);

        void DeleteObservacion(ObservacionExpediente observacionExpediente);
    }
}
