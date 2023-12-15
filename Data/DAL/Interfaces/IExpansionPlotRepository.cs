using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IExpansionPlotRepository
    {
        ExpansionPlot[] GetExpansionPlotByObjetoBase(Componente componenteBase, string esquema, string tabla,  string campoGeometry, string campoId, string campoNombre, int filtroGeografico, string idObjetoBase, long? filtroIdAtributo, string anio);

    }
}
