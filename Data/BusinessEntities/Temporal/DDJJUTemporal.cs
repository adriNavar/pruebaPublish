using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJUTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdU { get; set; }
        public int IdTramite { get; set; }
        public decimal? SuperficiePlano { get; set; }
        public decimal? SuperficieTitulo { get; set; }
        public int? AguaCorriente { get; set; }
        public int? Cloaca { get; set; }
        public long? NumeroHabitantes { get; set; }
        public byte[] Croquis { get; set; }
        public long? IdMensura { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public MensuraTemporal Mensura { get; set; }
        public ICollection<DDJJUFraccionTemporal> Fracciones { get; set; }
    }
}
