using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class FileData : IEntity
    {
        public Nullable<long> FileDataId { get; set; }
        public Nullable<long> FileColumnId { get; set; }
        public string Valor { get; set; }
        public long IndiceFila { get; set; }

    }
}
