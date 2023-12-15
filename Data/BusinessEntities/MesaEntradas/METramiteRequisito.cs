using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class METramiteRequisito : IBajaLogica
    {
        public int IdTramiteRequisito { get; set; }
        public int IdTramite { get; set; }
        public int IdObjetoRequisito { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public METramite Tramite { get; set; }
        public MEObjetoRequisito ObjetoRequisito { get; set; }
    }
}
