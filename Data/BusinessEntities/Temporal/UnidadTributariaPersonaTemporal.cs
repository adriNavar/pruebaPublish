using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class UnidadTributariaPersonaTemporal : ITemporalTramite
    {
        public long UnidadTributariaID { get; set; }
        public int IdTramite { get; set; }
        public long PersonaID { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long TipoPersonaID { get; set; }

        public UnidadTributariaTemporal UnidadTributaria { get; set; }

        public string CodSistemaTributario { get; set; }
    }
}
