using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEObjetoEntrada : IBajaLogica
    {
        public int IdObjetoEntrada { get; set; }
        public int IdObjetoTramite { get; set; }
        public int IdEntrada { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        
    }
}
