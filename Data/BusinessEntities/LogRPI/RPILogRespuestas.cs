using System;
using System.Net;

namespace GeoSit.Data.BusinessEntities.LogRPI
{
    public class RPILogRespuestas : IEntity
    {
        public int LogId { get; set; }
        public string UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoOperacionId { get; set; }
        public string Valor { get; set; }
        public int CodigoRespuesta { get; set; }

        public RPITipoOperacion TipoOperaciones { get; set; }

        public RPILogRespuestas() { }

        public RPILogRespuestas(string usuario, TipoDeOperacion toperacion, string valor, HttpStatusCode codigorespuesta)
        {
            UsuarioId = usuario;
            Fecha = System.DateTime.Now;
            TipoOperacionId = (int)toperacion;
            Valor = valor;
            CodigoRespuesta = (int)codigorespuesta;

        }

    }
}
