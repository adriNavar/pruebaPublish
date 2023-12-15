using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class FileDescriptor : IEntity
    {
        public Nullable<long> FileDescriptorId { get; set; }
        public string Nombre { get; set; }
        public string Path { get; set; }

    }
}
