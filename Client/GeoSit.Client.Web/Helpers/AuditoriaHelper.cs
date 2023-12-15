using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Sockets;
using System.Web;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Client.Web.Helpers
{
    public static class Autorizado
    {
        public const string Si = "S";
        public const string No = "N";
    }

    public static class AuditoriaHelper
    {
        private static readonly HttpClient Cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials })
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"])
        };

        public static void Register(long idUsuario, string datosAdicionales, HttpRequestBase request, string tipoOperacion, string autorizado, string evento, string objeto = "",
                                    string objetoOrigen = "", string objetoModificado = "", int cantidad = 1)
        {
            try
            {
                var auditoria = new ObjectContent<Auditoria>(new Auditoria
                {
                    Datos_Adicionales = datosAdicionales,
                    Autorizado = autorizado,
                    Id_Evento = long.Parse(evento),
                    Id_Tipo_Operacion = long.Parse(tipoOperacion),
                    Id_Usuario = idUsuario,
                    Ip = request.UserHostAddress,
                    Machine_Name = ReverseLookup(request.UserHostAddress),
                    Objeto = objeto,
                    Objeto_Origen = objetoOrigen,
                    Objeto_Modif = objetoModificado,
                    Cantidad = cantidad,//Este campo sobra pero lo utilizan en las consultas
                    Fecha = DateTime.Now
                }, new JsonMediaTypeFormatter());

                var result = Cliente.PostAsync("api/auditoria/post", auditoria).Result;
                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("Auditar Operacion", ex);
            }
        }

        public static string ReverseLookup(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return ip;
            }
            try
            {
                return (Dns.GetHostEntry(ip).HostName ?? ip).ToLower();
            }
            catch (SocketException)
            {
                return ip.ToLower();
            }
        }
    }
}