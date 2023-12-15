using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class METramiteEntrada : IBajaLogica
    {
        public int IdTramiteEntrada { get; set; }
        public int IdTramite { get; set; }
        public long IdComponente { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long? IdObjeto { get; set; }
        public int IdObjetoEntrada { get; set; }
        public MEObjetoEntrada ObjetoEntrada { get; set; }
        public ICollection<METramiteEntradaRelacion> TramiteEntradaRelaciones { get; set; }
    }
}
