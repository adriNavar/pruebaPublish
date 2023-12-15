using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Common.Enums;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Repositories
{
    public interface ILayerGrafRepository
    {
        #region se comenta porque no se usa. borrar mas adelante
        //LayerGraf[] GetLayerGrafByCoords(string tableName, string campoFeatId, string campoNombre, string campoGeometry, double x1, double y1, double x2, double y2, string mask);

        //LayerGraf[] GetLayerGrafByCoords(string layerGrafName, double x1, double y1, double x2, double y2, string mask);

        //LayerGraf[] GetLayerGrafByBuffer(string tableName, string campoFeatId, string campoNombre, string campoGeometry, string tableNameMaster, string campoFeatIdMaster, string campoGeometryMaster, long featIdMaster, int distBuffer, string mask);

        //LayerGraf[] GetLayerGrafById(string tableName, string campoFeatId, string campoNombre, string campoGeometry, string campoId, string id, bool cambioCoords);

        //LayerGraf[] GetLayerGrafById(string layerGrafName, string id, bool cambioCoords); 

        //double[] GetCoordsBoxByLstIdObjAndIdComp(List<int> idsObj, int idComp);

        //double[] GetCoordsByRelaciones(List<Relacion> relaciones);

        //LayerGraf GetPuerta(string pIdUbicacionPloteo);

        //LayerGraf GetParcelaSAR(string pIdUbicacionPloteo);

        //Canieria[] GetDatosCañeriaPlotByCoords(List<string> lstCoordsBuffImpresion);

        //int GetIdManzanaByIdObjetoGraf(int pIdObjetoGraf);
        #endregion

        LayerGraf[] GetLayerGrafById(Layer layer, Componente componente, string id, List<string> lstCoordsGeometry);

        LayerGraf[] GetLayerGrafByMapaTematico(string guid);

        LayerGraf[] GetLayerGrafByCoords(Layer layer, Componente componente, double x1, double y1, double x2, double y2);

        //string GetDireccionByIdObjGraf(string pIdObjetoGraf);

        LayerGraf[] GetLayerGrafByCoordsAndIds(Layer layer, Componente componente, List<string> lstCoordsGeometry, string anio, List<long> ids);

        LayerGraf[] GetLayerGrafByIds(Layer layer, Componente componente, string ids, List<string> lstCoordsGeometry);

        string GetLayerGrafTextById(Componente componente, string id, long idAtributo);

        string GetLayerGrafDistritoById(Componente componente, string id);

        string GetLayerGrafDistritoByCoords(double x, double y);
        string GetLayerGrafRegionByCoords(double x, double y);

        string GetLayerGrafKMMallaByCoords(int pIdMalla, Componente compCañeria, List<string> lstTipo);

        LayerGraf[] GetLayerGrafByObjetoBase(Layer layer, Componente componente, Componente componenteBase, List<string> lstCoordsGeometry, string idObjetoBase, string anio, bool esInformeAnual);

        LayerGraf[] GetLayerGrafByObjetoBaseIntersect(Layer layer, Componente componente, Componente componenteBase, string idObjetoBase, string anio, bool esInformeAnual);

        double[] TransformCoords(double x1, double y1, double x2, double y2, SRID origen, SRID destino);

        LayerGraf GetLayerGrafByComponentePrincipal(Layer layer, Componente componente, string guid);
    }
}
