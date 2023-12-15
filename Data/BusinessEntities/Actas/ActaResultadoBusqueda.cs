using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Actas
{
    public class ActaResultadoBusqueda : IEntity
    {
        public int numero { get; set; }
        public DateTime fechaAlta { get; set; }
        public int? plazo { get; set; }
        public string inspector { get; set; }
        public string estado { get; set; }
    }
}
