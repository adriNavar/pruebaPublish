using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Data.BusinessEntities.Ploteo
{
    public class ObjetoPloteable
    {
        public string componente { get; set; }
        public long idObjeto { get; set; }
        public string apicIdManzana { get; set; }
        public long idManzana { get; set; }
        public string apicIdParcela { get; set; }
        public string idParcela { get; set; }
    }
}