using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Actas
{
    public class ActaRolPersona : IEntity
    {
        public long ActaRolId { get; set; }
        public string Descripcion { get; set; }
    }
}
