using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System;
using System.Linq;

namespace GeoSit.Client.Web.Models
{
    public class MantenedorParcelarioValuacionModel
    {
        public decimal ValorTierra { get; private set; }
        public decimal? ValorMejoras { get; private set; }
        public decimal ValorTotal { get; private set; }
        public decimal? ValorMejorasPropio { get; private set; }
        public DateTime VigenciaDesde { get; private set; }
        public string Decretos { get; private set; }

        public MantenedorParcelarioValuacionModel(VALValuacion valuacion)
        {
            ValorTierra = valuacion?.ValorTierra ?? 0m;
            ValorMejoras = valuacion?.ValorMejoras;
            ValorMejorasPropio = valuacion?.ValorMejorasPropio;
            ValorTotal = valuacion?.ValorTotal ?? 0m;
            VigenciaDesde = valuacion?.FechaDesde ?? DateTime.MinValue;
            Decretos = string.Join(", ", valuacion?.ValuacionDecretos?.Select(vd => vd.Decreto.NroDecreto) ?? new long[0]);
        }
    }
}