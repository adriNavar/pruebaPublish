using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJUFraccionTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdFraccion { get; set; }
        public int IdTramite { get; set; }
        public long IdU { get; set; }
        public long NumeroFraccion { get; set; }
     
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<DDJJUMedidaLinealTemporal> MedidasLineales { get; set; }
    }
}
