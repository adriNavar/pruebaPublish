using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJPersonaDomicilioTemporal : ITemporalTramite
    {
        public long IdDominioTitular { get; set; }
        public long IdDomicilio { get; set; }
        public long IdTipoDomicilio { get; set; }

        public int IdTramite { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }

        public Domicilio Domicilio { get; set; }
        public TipoDomicilio TipoDomicilio { get; set; }
    }
}
