using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IEstadoDeudaRepository
    {
        IEnumerable<int> GetYears();

        IEnumerable<EstadoDeudaServicioGeneral> GetEstadoDeudaServiciosGenerales(string padron);

        IEnumerable<EstadoDeudaRenta> GetEstadoDeudaRentas(int year);
    }
}
