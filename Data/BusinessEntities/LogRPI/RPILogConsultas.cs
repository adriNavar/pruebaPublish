using System;

namespace GeoSit.Data.BusinessEntities.LogRPI
{
    public class RPILogConsultas : IEntity
    {
        public int LogId { get; set; }
        public string UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoOperacionId { get; set; }
        public string Valor { get; set; }
        public int CodigoRespuesta { get; set; }

        public RPITipoOperacion TipoOperaciones { get; set; }

        public RPILogConsultas() { }

        public RPILogConsultas(string usuario, int toperacion, string valor, int codigorespuesta)
        {
            UsuarioId = usuario;
            Fecha = DateTime.Now;
            TipoOperacionId = toperacion;
            Valor = valor;
            CodigoRespuesta = codigorespuesta;

        }

    }
}
