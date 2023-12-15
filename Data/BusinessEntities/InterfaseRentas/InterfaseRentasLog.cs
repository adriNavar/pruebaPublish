using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.InterfaseRentas
{
    public class InterfaseRentasLog
    {
        public long LogID { get; set; }
        public DateTime Fecha { get; set; }
        public int TransactionID { get; set; }
        public long? ParcelaID { get; set; }
        public string Partida { get; set; }
        public string Operacion { get; set; }        
        public string WebService { get; set; }
        public string WebServiceUrl { get; set; }
        public string WebServiceClass { get; set; }
        public string Parametros { get; set; }
        public string Resultado { get; set; }
        public short Estado { get; set; }
    }
}
