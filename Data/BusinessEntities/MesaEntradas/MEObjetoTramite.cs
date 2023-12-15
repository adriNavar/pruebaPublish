using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEObjetoTramite : IBajaLogica
    {
        public int IdObjetoTramite { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoTramite { get; set; }
        public string Plantilla { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public METipoTramite TipoTramite { get; set; }
    }
}
