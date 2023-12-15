using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdDeclaracionJurada { get; set; }
        public int IdTramite { get; set; }
        public long IdVersion { get; set; }
        public long IdOrigen { get; set; }
        public long IdUnidadTributaria { get; set; }
        public string IdPoligono { get; set; }
        public DateTime? FechaVigencia { get; set; }
        public long? IdTramiteObjeto { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJSorTemporal Sor { get; set; }
        public DDJJUTemporal U { get; set; }
        public INMMejoraTemporal Mejora { get; set; }
        public DDJJDesignacionTemporal Designacion { get; set; }
        public ICollection<DDJJDominioTemporal> Dominios { get; set; }
        public ICollection<VALValuacionTemporal> Valuaciones { get; set; }
    }
}
