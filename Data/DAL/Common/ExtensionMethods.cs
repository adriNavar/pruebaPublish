using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Linq;

namespace GeoSit.Data.DAL.Common.ExtensionMethods.Data
{
    public static class DataReaderExtensionMethods
    {
        public static DbGeometry GetGeometryFromField(this IDataReader reader, int fieldIndex, int srid)
        {
            try
            {
                if (reader.IsNullOrEmpty(fieldIndex))
                {
                    return null;
                }
                var geom = DbGeometry.FromText(reader.GetString(fieldIndex), srid);
                if (!geom.IsValid)
                {
                    throw new ApplicationException(String.Format("La geometria {0} no es valida", fieldIndex));
                }
                return geom;
            }
            catch (IndexOutOfRangeException)
            {
                throw new ApplicationException(String.Format("La geometria {0} no es un rango valido", fieldIndex));
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(string.Format("ArgumentNullException wkt: {0}", reader.GetString(fieldIndex)), ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(string.Format("ArgumentException wkt: {0}", reader.GetString(fieldIndex)), ex);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Exception wkt: {0}", reader.GetString(fieldIndex)), ex);
            }
        }
        public static DbGeometry GetGeometryFromField(this IDataReader reader, int fieldIndex, SRID srid = SRID.App)
        {
            return reader.GetGeometryFromField(fieldIndex, new SRIDParser().Parse(srid));
        }
        public static long? GetNullableInt64(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? null : (long?)reader.GetInt64(fieldIndex);
        }
        public static int? GetNullableInt32(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? null : (int?)reader.GetInt32(fieldIndex);
        }
        public static decimal? GetNullableDecimal(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? null : (decimal?)reader.GetDecimal(fieldIndex);
        }
        public static double? GetNullableDouble(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? null : (double?)reader.GetDouble(fieldIndex);
        }
        public static float? GetNullableFloat(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? null : (float?)reader.GetFloat(fieldIndex);
        }
        public static DateTime? GetNullableDateTime(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? null : (DateTime?)reader.GetDateTime(fieldIndex);
        }
        public static string GetStringOrEmpty(this IDataReader reader, int fieldIndex)
        {
            return reader.IsNullOrEmpty(fieldIndex) ? string.Empty : reader.GetValue(fieldIndex).ToString();
        }
        public static string GetTypeFormattedStringValue(this IDataReader reader, int fieldIndex)
        {
            if (reader.IsNullOrEmpty(fieldIndex)) return string.Empty;

            return reader.GetFieldType(fieldIndex) == typeof(DateTime) ?
                         reader.GetDateTime(fieldIndex).ToString() :
                         reader.GetValue(fieldIndex).ToString();

        }
        public static bool IsNullOrEmpty(this IDataReader reader, int fieldIndex)
        {
            return reader.IsDBNull(fieldIndex) || string.IsNullOrEmpty(reader.GetValue(fieldIndex).ToString());
        }
    }
}
namespace GeoSit.Data.DAL.Common.ExtensionMethods.Atributos
{
    public static class AtributoExtensionMethods
    {
        public static Atributo GetAtributoClaveByComponente(this IQueryable<Atributo> atributos, long idComponente)
        {
            var clave = atributos.FirstOrDefault(a => a.ComponenteId == idComponente && a.EsClave == 1);
            if (clave == null)
                throw new ApplicationException("No se encontró el atributo Clave");

            return clave;
        }
        public static Atributo GetAtributoGeometryByComponente(this IQueryable<Atributo> atributos, long idComponente)
        {
            var clave = atributos.FirstOrDefault(a => a.ComponenteId == idComponente && a.EsGeometry);
            if (clave == null)
                throw new ApplicationException("No se encontró el atributo Geometry");

            return clave;
        }
        public static Atributo GetAtributoLabelByComponente(this IQueryable<Atributo> atributos, long idComponente)
        {
            var clave = atributos.FirstOrDefault(a => a.ComponenteId == idComponente && a.EsLabel);
            if (clave == null)
                throw new ApplicationException("No se encontró el atributo Label");

            return clave;
        }
        public static Atributo GetAtributoFeatIdByComponente(this IQueryable<Atributo> atributos, long idComponente)
        {
            var clave = atributos.FirstOrDefault(a => a.ComponenteId == idComponente && a.EsFeatId);
            if (clave == null)
                throw new ApplicationException("No se encontró el atributo FEATID");

            return clave;
        }

        public static Atributo GetAtributoClave(this IEnumerable<Atributo> atributos)
        {
            var clave = atributos.FirstOrDefault(a => a.EsClave == 1);
            if (clave == null)
                throw new ApplicationException("No se encontró el atributo Clave");

            return clave;
        }
        public static Atributo GetAtributoFeatId(this IEnumerable<Atributo> atributos)
        {
            var clave = atributos.FirstOrDefault(a => a.EsFeatId);
            if (clave == null)
                throw new ApplicationException("No se encontró el atributo FEATID");

            return clave;
        }
        public static Atributo GetAtributoLabel(this IEnumerable<Atributo> atributos)
        {
            var label = atributos.FirstOrDefault(a => a.EsLabel);
            if (label == null)
                throw new ApplicationException("No se encontró el atributo Label");

            return label;
        }
        public static Atributo GetAtributoGeometry(this IEnumerable<Atributo> atributos)
        {
            var geometry = atributos.FirstOrDefault(a => a.EsGeometry);
            if (geometry == null)
                throw new ApplicationException("No se encontró el atributo Geometry");

            return geometry;
        }

        public static object GetFormattedValue(this Atributo atributo, object value)
        {
            if (atributo.TipoDatoId == 6)
            {
                return string.Format(" '{0}' ", value);
            }
            if (atributo.TipoDatoId == 5)
            {
                return string.Format(" TO_DATE('{0}','DD/MM/YYYY')", value);
            }
            return value;
        }
    }
}
namespace GeoSit.Data.DAL.Common.ExtensionMethods.Componentes
{
    public static class ComponenteExtensionMethods
    {
        public static Componente GetComponenteByCapa(this IQueryable<Componente> componentes, string capa)
        {
            return componentes.ToArray().Where(a => a.Capa != null).SingleOrDefault(a => a.EsComponenteByCapa(capa));
        }

        public static bool EsComponenteByCapa(this Componente componente, string capa)
        {
            return componente.Capa.ToUpper().Split(',').Contains(capa.ToUpper());
        }
    }
}

namespace GeoSit.Data.DAL.Common.ExtensionMethods.DecimalDegreesToDMS
{
    //public enum LatLon
    //{
    //    Lat,
    //    Lon
    //}
    public static class DDtoDMS
    {
        public static string ConvertToDMS(this double dd/*, LatLon ll*/)
        {
            double absDD = Math.Abs(dd);
            double mins = (absDD - Math.Floor(absDD)) * 60d;
            double secs = (mins - Math.Floor(mins)) * 60d;
            double milisecs = (secs - Math.Floor(secs)) * 10d;
            //string orient = "W";
            //bool negative = Math.Sign(dd) == -1;
            //if (ll == LatLon.Lat)
            //{
            //    orient = negative ? "S" : "N";
            //}
            //else if (!negative)
            //{
            //    orient = "E";
            //}
            //return $"{Math.Floor(absDD)}° {Math.Floor(mins)}' {Math.Floor(secs)}{(Math.Floor(milisecs) == 0 ? string.Empty : $".{(Math.Floor(milisecs) * 1000)}".Substring(0, 4))}\"{orient}";
            return $"{Math.Floor(absDD) * Math.Sign(dd)}° {Math.Floor(mins)}' {Math.Floor(secs)}{(Math.Floor(milisecs) == 0 ? string.Empty : $".{(Math.Floor(milisecs) * 100)}".Substring(0, 3))}\"";
        }
    }
}