using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Ploteo
{
    public class PloteoMasivoZona : IEntity
    {
        /*public int IdObjeto { get; set; }

        public int IdComponente { get; set; }*/

        public int ID { get; set; }

        public string APICID { get; set; }

        public double? KMRed { get; set; }
    }
}
