using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class ConfiguracionRango : IEntity
    {
        public long ConfiguracionRangoId { get; set; }
        public long ConfiguracionId { get; set; }
        public long Orden { get; set; }
        public string Desde { get; set; }
        public string Hasta { get; set; }
        public long Cantidad { get; set; }
        public string Leyenda { get; set; }
        public string ColorRelleno { get; set; }
        public string ColorLinea { get; set; }
        public long EspesorLinea { get; set; }
        public byte[] Icono { get; set; }
        public MapaTematicoConfiguracion Configuracion { get; set; }
    }
}
