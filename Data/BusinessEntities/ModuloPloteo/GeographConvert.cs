using System.Diagnostics;
using System.Spatial;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class GeographConvert
    {
        public static DbGeometry ConvertFrom(Geometry geo)
        {
            string GeometryEwkt = geo.ToString();
            int semicolon = GeometryEwkt.IndexOf(';');

            string GeometryWkt = GeometryEwkt.Substring(semicolon + 1);
            return DbGeometry.FromText(GeometryWkt, int.Parse(geo.CoordinateSystem.Id));
        }

        public static GeometryPoint ConvertPointTo(DbGeometry dbGeo)
        {
            Debug.Assert(dbGeo.SpatialTypeName == "Point");
            double lat = dbGeo.YCoordinate ?? 0;
            double lon = dbGeo.XCoordinate?? 0;
            double? alt = dbGeo.Elevation;
            double? m = dbGeo.Measure;
            return GeometryPoint.Create(lat, lon, alt, m);
        }

        public static GeometryLineString ConvertLineStringTo(DbGeometry dbGeo)
        {
            Debug.Assert(dbGeo.SpatialTypeName == "LineString");
            SpatialBuilder builder = SpatialBuilder.Create();
            var pipeLine = builder.GeometryPipeline;
            pipeLine.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipeLine.BeginGeometry(SpatialType.LineString);

            int numPionts = dbGeo.PointCount ?? 0;
            for (int n = 0; n < numPionts; n++)
            {
                DbGeometry pointN = dbGeo.PointAt(n + 1);
                double lat = pointN.YCoordinate ?? 0;
                double lon = pointN.XCoordinate ?? 0;
                double? alt = pointN.Elevation;
                double? m = pointN.Measure;
                GeometryPosition position = new GeometryPosition(lat, lon, alt, m);
                if (n == 0)
                {
                    pipeLine.BeginFigure(position);
                }
                else
                {
                    pipeLine.LineTo(position);
                }
            }
            pipeLine.EndFigure();
            pipeLine.EndGeometry();
            return (GeometryLineString)(builder.ConstructedGeometry);
        }

        public static GeometryPolygon ConvertPolygonTo(DbGeometry dbGeo)
        {
            Debug.Assert(dbGeo.SpatialTypeName == "Polygon");
            SpatialBuilder builder = SpatialBuilder.Create();
            var pipeLine = builder.GeometryPipeline;
            pipeLine.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipeLine.BeginGeometry(SpatialType.Polygon);

            int numPionts = dbGeo.PointCount ?? 0;
            for (int n = 0; n < numPionts; n++)
            {
                DbGeometry pointN = dbGeo.PointAt(n + 1);
                double lat = pointN.YCoordinate?? 0;
                double lon = pointN.XCoordinate ?? 0;
                double? alt = pointN.Elevation;
                double? m = pointN.Measure;
                GeometryPosition position = new GeometryPosition(lat, lon, alt, m);
                if (n == 0)
                {
                    pipeLine.BeginFigure(position);
                }
                else
                {
                    pipeLine.LineTo(position);
                }
            }
            pipeLine.EndFigure();
            pipeLine.EndGeometry();
            return (GeometryPolygon)(builder.ConstructedGeometry);
        }

    }
}
