using GeoSit.Data.BusinessEntities.Documentos;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDocumentoRepository
    {
        Documento GetDocumentoById(long id);
        IEnumerable<Documento> GetDocumentosByTipo(long tipo);
        void DeleteDocumento(Documento documento);
        Documento InsertDocumento(Documento documento);
        Documento Save(Documento documento);
        DocumentoArchivo GetContent(long id);
        DocumentoArchivo GetAyudaLineaPdf(long id);
    }
}
