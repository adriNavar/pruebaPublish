using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IExpedienteObraDocumentoRepository
    {
        IEnumerable<DocumentoExpedienteObra> GetExpedienteObraDocumentos(long idExpedienteObra);

        ExpedienteObraDocumento GetExpedienteObraDocumentoById(long idExpedienteObra, long idDocumento);

        void InsertExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento, ExpedienteObra expedienteObra);

        void InsertExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento);

        void UpdateExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento);

        void DeleteExpedienteObraDocumento(long idExpedienteObra);

        void DeleteExpedienteObraDocumento(long idExpedienteObra, long idDocumento);

        void DeleteExpedienteObraDocumento(ExpedienteObraDocumento expedienteObraDocumento);
    }
}
