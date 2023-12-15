using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories
{
    public interface ICallePlotRepository
    {
        CallePlot[] GetCallePlotByIdCalle(string esquema, string tabla, string campoIdCalle, string campoNombre, string campoCodigo, long idCalle);
    }
}
