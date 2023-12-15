using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class TipoOperacion : IEntity
    {
        public long TipoOperacionId { get; set; }
        public string Nombre { get; set; }
        public long TipoFiltroId { get; set; }
        public long CantidadValores { get; set; }

    }
}
