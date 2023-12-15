using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class INMMejoraTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdMejora { get; set; }
        public int IdTramite { get; set; }
        public long? IdEstadoConservacion { get; set; }
        public long? IdDestinoMejora { get; set; }
        //public long? Ampliacion { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<INMMejoraOtraCarTemporal> OtrasCar { get; set; }
        public ICollection<INMMejoraCaracteristicaTemporal> MejorasCar { get; set; }
    }
}
