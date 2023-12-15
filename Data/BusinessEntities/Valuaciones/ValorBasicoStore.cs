
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValorBasicoStore : IEntity{

        public long? widParcela { get; set; }
        public long? widMejora { get; set; }
        public float? ValorBasicoTierra { get; set; }
        public float? ValorBasicoMejora { get; set; }
        public float? Semicubierto { get; set; }
        public float? nCoeficiente1M { get; set; }
        public float? nCoeficiente2M { get; set; }
        public float? nCoeficiente3M { get; set; }
        public float? nCoeficiente4M { get; set; }
        
    }
}

    
