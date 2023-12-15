using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class InterfaseRentasLogModel
    {
        public long LogID { get; set; }
        public DateTime Fecha { get; set; }
        public int TransactionID { get; set; }
        public long? ParcelaID { get; set; }
        public string Partida { get; set; }
        public string Operacion { get; set; }
        public string WebService { get; set; }        
        public string Resultado { get; set; }
        public short Estado { get; set; }
    }
}