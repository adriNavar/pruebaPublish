using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IMensuraTemporalRepository
    {
        MensuraTemporal GetMensura(int idMensura, int tramite);
        TipoMensura GetTipoMensura(long idMensura);
        Documento GetDocumentoMensura(long idMensura);
        List<MensuraTemporal> GetEntradasByIdTramite(int tramite);
    }
}