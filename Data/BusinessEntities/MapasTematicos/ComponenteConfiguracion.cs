using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class ComponenteConfiguracion : IEntity
    {
        public long ComponenteConfiguracionId { get; set; }

        public long ComponenteId { get; set; }

        public long ConfiguracionId { get; set; }

        public Componente Componente { get; set; }
        public MapaTematicoConfiguracion Configuracion { get; set; }
    }
}
