using System;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class UnidadTributariaDomicilio
    {
        public long DomicilioID { get; set; }
        public long UnidadTributariaID { get; set; }
        public long TipoDomicilioID { get; set; }
        public long? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion{ get; set; }
        public long?  UsuarioBajaID{ get; set; }
        public DateTime? FechaBaja{ get; set; }

        /*Propiedades de Navegacion*/
        public Domicilio Domicilio { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }
    }
}
