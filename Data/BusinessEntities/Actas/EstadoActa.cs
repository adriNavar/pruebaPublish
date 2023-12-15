using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Actas
{
    public class EstadoActa : IEntity
    {
        public long EstadoActaId { get; set; }
        public string Descripcion { get; set; }
    }
}
