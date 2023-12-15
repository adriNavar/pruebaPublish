using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class VIRValuacion : IEntity
    {
        public long ValuacionId { get; set; }
        public int CorridaId { get; set; }
        public long InmuebleId { get; set; }
        public string Partida { get; set; }
        public DateTime? VigenciaDesde { get; set; }
        public int? Zona { get; set; }
        public string ValuacionTipo { get; set; }
        public double? SuperficieTierra { get; set; }
        public string UnidadMedidaSupTierra { get; set; }
        public double? ValorTierra { get; set; }
        public string TipoMejoraUso { get; set; }
        public double? SuperficieMejora { get; set; }
        public string UnidadMedidaSupMejora { get; set; }
        public double? ValorMejoras { get; set; }
        public double? ValorTotal { get; set; }

        public bool Vigente { get; set; }
    }
}
