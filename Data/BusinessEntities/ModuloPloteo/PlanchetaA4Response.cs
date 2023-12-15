using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class PlanchetaA4Response
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public byte[] PDF { get; set; }
    }
}
