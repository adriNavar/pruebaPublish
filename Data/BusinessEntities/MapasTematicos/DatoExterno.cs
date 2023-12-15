using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class DatoExterno : IEntity
    {
        public long DatoExternoId { get; set; }
        public long DatoExternoConfiguracionId { get; set; }
        public string idComponente { get; set; }
        public string Valor { get; set; }
        
    }
}
