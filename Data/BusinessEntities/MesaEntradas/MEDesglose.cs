using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEDesglose : IEntity, IBajaLogica
    {
        public int IdDesglose { get; set; }
        public int IdTramite { get; set; }
        public int FolioDesde { get; set; }
        public int FolioHasta { get; set; }
        public int FolioCant { get; set; }
        public string Observaciones { get; set; }
        public int IdDesgloseDestino { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        //public METramite Tramite { get; set; }
        //public Usuarios Usuario_Alta { get; set; }
        public MEDesgloseDestino DesgloseDestino { get; set; }
    }
}
