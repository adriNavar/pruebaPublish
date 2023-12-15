using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories
{
    public interface ICuadraPlotRepository
    {
        CuadraPlot[] GetCuadraPlotByIdCuadra(string esquema, string tabla, string campoGeometry, string campoIdCuadra, long idCuadra);

        CuadraPlot[] GetCuadraPlotByIdManzana(string esquema, string tabla, string campoGeometry, string campoIdCuadra, string campoIdManzana, string campoIdCalle, string campoAlturaMin, string campoAlturaMax, long idManzana);

        CuadraPlot[] GetCuadraPlotByObjetoBase(Componente componenteBase, string idObjetoBase, string esquema, string tabla, string campoGeometry, string campoId, string campoIdManzana, string campoIdCalle, string campoAlturaMin, string campoAlturaMax, string campoIdParidad, int filtroGeografico);
    }
}
