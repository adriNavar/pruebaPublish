using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class DatoExternoConfiguracion : IEntity
    {
        public long DatoExternoConfiguracionId { get; set; }
        public long Componente { get; set; }
        public string Nombre { get; set; }
        public long TipoDato { get; set; }
        public long Usuario { get; set; }

        public long AtributoId { get; set; }
        
    }
}
