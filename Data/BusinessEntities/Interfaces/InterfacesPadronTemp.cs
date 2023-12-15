using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Interfaces
{
    public class InterfacesPadronTemp : IEntity
    {

        public long IdPadronTemp { get; set; }
        public long? IdPadronTempOrigen { get; set; }
        public long IdTransaccion { get; set; }
        public String TipoTransaccion { get; set; }
        public String TipoOperacion { get; set; }
        public DateTime Fecha { get; set; } 
        public String Padron { get; set; }
        public String ParcelaNomenc { get; set; }
        public long? UsuarioModificacion { get; set; }
        public String Estado { get; set; }

        public List<InterfacesPadronTemp> Destinos { get; set; }
        public long? ParcelaID { get; set; }
    }


}