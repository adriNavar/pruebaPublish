using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALValuacion
    {
        public long IdValuacion { get; set; }
        public long IdUnidadTributaria { get; set; }
        public long? IdDeclaracionJurada { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal ValorTierra { get; set; }
        public decimal? ValorMejoras { get; set; }
        public decimal? ValorMejorasPropio { get; set; }
        public decimal ValorTotal { get; set; }
        public double? Superficie { get; set; }
        public double? CoefProrrateo { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJ DeclaracionJurada { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }

        public ICollection<VALValuacionDecreto> ValuacionDecretos { get; set; }

    }
}
