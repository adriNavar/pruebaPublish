using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class UnidadTributariaDomicilioTemporal : ITemporalTramite
    {
        public long DomicilioID { get; set; }
        public long UnidadTributariaID { get; set; }
        public int IdTramite { get; set; }
        public long TipoDomicilioID { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion{ get; set; }
        public long?  UsuarioBajaID{ get; set; }
        public DateTime? FechaBaja{ get; set; }

        /*Propiedades de Navegacion*/
        //public Domicilio Domicilio { get; set; }
        public UnidadTributariaTemporal UnidadTributaria { get; set; }
    }
}
