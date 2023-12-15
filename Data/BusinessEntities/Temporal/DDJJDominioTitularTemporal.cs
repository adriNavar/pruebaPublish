using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJDominioTitularTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdDominioTitular { get; set; }
        public long IdDominio { get; set; }
        public long IdPersona { get; set; }
        public short IdTipoTitularidad { get; set; }
        public decimal PorcientoCopropiedad { get; set; }
        public int IdTramite { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<DDJJPersonaDomicilioTemporal> PersonaDomicilios { get; set; }
        public TipoTitularidad TipoTitularidad { get; set; }
    }
}
