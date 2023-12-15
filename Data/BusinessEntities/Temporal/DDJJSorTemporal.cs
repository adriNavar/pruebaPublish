using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJSorTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdSor { get; set; }
        public int IdTramite { get; set; }
        public long? IdLocalidad { get; set; }
        public long? IdCamino { get; set; }
        public long? DistanciaCamino { get; set; }
        public long? DistanciaLocalidad { get; set; }
        public long? DistanciaEmbarque { get; set; }
        public long? NumeroHabitantes { get; set; }
        public long? IdMensura { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public MensuraTemporal Mensura { get; set; }
        public ICollection<DDJJSorCarTemporal> SoRCars { get; set; }
        public ICollection<VALSuperficieTemporal> Superficies { get; set; }
    }
}
