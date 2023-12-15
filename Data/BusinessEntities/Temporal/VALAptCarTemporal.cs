using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class VALAptCarTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdAptCar { get; set; }
        public long IdSorCar { get; set; }
        public long IdAptitud { get; set; }
        public int IdTramite { get; set; }
        public long Puntaje { get; set; }
      

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public VALAptitudes Aptitud { get; set; }
        public DDJJSorCaracteristicas SorCaracteristica { get; set; }

    }
}
