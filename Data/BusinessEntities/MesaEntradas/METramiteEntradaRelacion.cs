using System;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class METramiteEntradaRelacion : IBajaLogica
    {
        public int IdTramiteEntradaRelacion { get; set; }
        public virtual METramiteEntrada TramiteEntrada { get; set; }
        public int IdTramiteEntrada { get; set; }
        public virtual METramiteEntrada TramiteEntradaPadre { get; set; }
        public int IdTramiteEntradaPadre { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
