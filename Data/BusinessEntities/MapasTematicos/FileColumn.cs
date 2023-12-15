using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class FileColumn : IEntity
    {
        public Nullable<long> FileColumnId { get; set; }
        public Nullable<long> FileDescriptorId { get; set; }
        public string Nombre { get; set; }
        public long IndiceColumna { get; set; }

        public long TipoDato { get; set; }

    }
}

