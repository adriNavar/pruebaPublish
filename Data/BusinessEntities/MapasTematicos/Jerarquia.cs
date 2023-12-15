using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class Jerarquia : IEntity
    {
        public long JerarquiaId { get; set; }
        public long ComponenteSuperiorId { get; set; }
        public long ComponenteInferiorId { get; set; }
        public long AtributoSuperiorId { get; set; }
        public long AtributoInferiorId { get; set; }
        public string TablaRelacion { get; set; }
        public string EsquemaTblRel { get; set; }
    }
}
