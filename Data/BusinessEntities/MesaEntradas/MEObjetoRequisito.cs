using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEObjetoRequisito : IBajaLogica
    {
        public int IdObjetoRequisito { get; set; }
        public int IdObjetoTramite { get; set; }
        public int IdRequisito { get; set; }
        public int Obligatorio { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public MEObjetoTramite ObjetoTramite { get; set; }
        public MERequisito Requisito { get; set; }
    }
}
