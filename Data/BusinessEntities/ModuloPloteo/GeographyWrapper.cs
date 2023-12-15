using System.Spatial;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class GeometryWrapper
    {
        public static implicit operator GeometryPoint(GeometryWrapper wrapper)
        {
            return GeographConvert.ConvertPointTo(wrapper._dbGeo);
        }

        public static implicit operator GeometryWrapper(GeometryPoint pt)
        {
            return new GeometryWrapper(GeographConvert.ConvertFrom(pt));
        }

        public static implicit operator GeometryLineString(GeometryWrapper wrapper)
        {
            return GeographConvert.ConvertLineStringTo(wrapper._dbGeo);
        }

        public static implicit operator GeometryWrapper(GeometryLineString lineString)
        {
            return new GeometryWrapper(GeographConvert.ConvertFrom(lineString));
        }


        public static implicit operator GeometryPolygon(GeometryWrapper wrapper)
        {
            return GeographConvert.ConvertPolygonTo(wrapper._dbGeo);
        }

        public static implicit operator GeometryWrapper(GeometryPolygon polygon)
        {
            return new GeometryWrapper(GeographConvert.ConvertFrom(polygon));
        }


        public static implicit operator DbGeometry(GeometryWrapper wrapper)
        {
            return wrapper._dbGeo;
        }

        public static implicit operator GeometryWrapper(DbGeometry dbGeo)
        {
            return new GeometryWrapper(dbGeo);
        }

        protected GeometryWrapper(DbGeometry dbGeo)
        {
            _dbGeo = dbGeo;
        }

        private readonly DbGeometry _dbGeo;
    }
}
