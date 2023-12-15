using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;
using System.Data.Common;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ISQLGeometryFieldBuilder
    {
        ISQLGeometryFieldBuilder FromField(Atributo atributo, string tableAlias);
        ISQLGeometryFieldBuilder FromWKT(string wkt, int srid);
        ISQLGeometryFieldBuilder FromWKT(string wkt, SRID srid);
        ISQLGeometryFieldBuilder FromParameter(DbParameter param, int srid);
        ISQLGeometryFieldBuilder FromParameter(DbParameter param, SRID srid);
        /// <summary>
        /// Recupera el medio, si es LINESTRING, o el centroide, cualquier otro tipo, de la geometria
        /// </summary>
        ISQLGeometryFieldBuilder Centroid();
        ISQLGeometryFieldBuilder ToWKT();
        ISQLGeometryFieldBuilder AddBuffer(double buffer);
        ISQLGeometryFieldBuilder ChangeToSRID(int srid);
        ISQLGeometryFieldBuilder ChangeToSRID(SRID srid);
        ISQLGeometryFieldBuilder IntersectionWith(ISQLGeometryFieldBuilder geometry);
        ISQLGeometryFieldBuilder ToPolygon();
        ISQLGeometryFieldBuilder Area();
        ISQLGeometryFieldBuilder AreaSqrMeters();
        ISQLGeometryFieldBuilder Length();
        ISQLGeometryFieldBuilder LengthMeters();
        ISQLGeometryFieldBuilder Perimeter();
        ISQLGeometryFieldBuilder PerimeterMeters();
        ISQLGeometryFieldBuilder NumPoints();
        ISQLGeometryFieldBuilder OverlappingArea(ISQLGeometryFieldBuilder g1);
        ISQLGeometryFieldBuilder BoundingBox();
        ISQLGeometryFieldBuilder ConvexHull();
        ISQLGeometryFieldBuilder GType();
        ISQLGeometryFieldBuilder Distance(ISQLGeometryFieldBuilder geometry);
        string FormatSpatialFilter(ISQLGeometryFieldBuilder geometry, SQLSpatialRelationships relations);
    }
}
