using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IParcelaPlotRepository
    {
        IEnumerable<KeyValuePair<long, long>> GetManzanas(List<string> idsParcela, bool esApic);
        long? GetIdManzanaByParcela(long idParcela);

        //ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, int filtroGeografico, double x1, double y1, double x2, double y2);
        ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, double x1, double y1, double x2, double y2);
        ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, int filtroGeografico, double x1, double y1, double x2, double y2);
        //ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, int filtroGeografico, List<string> lstCoordsGeometry);
        //ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, int filtroGeografico, List<string> lstCoordsGeometry);
        ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, List<string> lstCoordsGeometry);
        ParcelaPlot[] GetParcelaPlotByCoords(string esquema, string tabla, double x, double y);
        bool GetSuperposicionOCObjetoByIdParcela(string esquema, string tabla, long idParcela, int idOCTipo, int porcentajeAreaOverlap);

        ParcelaPlot[] GetParcelaPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, string idObjetoBase, string tablaBarrioCarenciado);
        ParcelaPlot[] GetParcelaPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, string campoExpediente, string campoNomCatastral, string campoIdClienteTipo, int filtroGeografico, string idObjetoBase);
        ParcelaPlot[] GetParcelaPlotByObjetoBase(Componente componenteBase, string esquema, string tabla, string campoFeatId, string campoGeometry, string campoNroPuerta, string campoIdCuadra, int filtroGeografico, string idObjetoBase);

        ParcelaPlot GetParcelaPlotByIdObjGraf(string IdObjGraf);

        //ParcelaPlot[] GetInformacionComercial(ParcelaPlot[] parcelasPloteables);
        
        //IEnumerable<KeyValuePair<long, long>> GetManzanasByParcelasAPIC_ID(List<string> apicIdsParcela);

        //List<string> GetIDParcelasByApicID(List<string> idsApic);
        //List<string> GetIDParcelasByExpRef(List<string> expRef);
        //List<string> GetIDParcelasBySAPId(List<string> SAPIds);
        //List<string> GetIDParcelasByCC(List<string> CCs);
    }
}
