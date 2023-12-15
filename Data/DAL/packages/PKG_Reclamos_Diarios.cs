using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using GeoSit.Data.DAL.Contexts;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.packages
{
    public class PKG_Reclamos_Diarios
    {
        public Point PostPuntoDeDireccion(GetPointRequest request)
        {
            Point response = new Point();
            try
            {
                using (var ctx = GeoSITMContext.CreateContext())
                {

                    string param = @"'" + request.Distrito + "','" + request.Calle.GetLast(6) + "','" + request.Numero + "','" + request.Interseccion.GetLast(6) + "','" + request.Manzana + "'";
                    string q = "SELECT PKG_RECLAMOS_DIARIOS.fnc_getpoint (" + param + ").x X,PKG_RECLAMOS_DIARIOS.fnc_getpoint (" + param + ").y Y FROM DUAL";
                    response = ctx.Database.SqlQuery<Point>(q).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                response.X = -1;
                response.Y = -1;
            }
            

            return response;
        }
    }
    public static class StringExtension
    {
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }
}
