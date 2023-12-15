using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Actas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IActaRepository
    {
        IEnumerable<ActaExpedienteObra> GetActas(long idExpedienteObra);
        List<InformeActaVencida> GetActasFecha(string date);
    }
}
