using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Interfaces
{
    public class TransaccionesPendientes : IEntity
    {
        public long IdTransaccion { get; set; }
        public String Mi_Leyenda { get; set; }
        public String Mi_Identificacion { get; set; }
        public String Mi_Definitivo { get; set; }
        public String Otro_Tipo { get; set; }
        public String Otro_Leyenda { get; set; }
        public String Otro_Identificacion { get; set; }
        public String Otro_Nombre { get; set; }
        public String Otro_Definitivo { get; set; }
        public String Es { get; set; }
        public String Relacion { get; set; }
        public DateTime? Alta_Fecha { get; set; }
        public DateTime? Fecha_Desde { get; set; }
        public DateTime? Fecha_Hasta { get; set; }
        public List<InterfacesPadronTemp> listaDestino { get; set; }
        public List<InterfacesPadronTemp> listaOrigen { get; set; }

        public long? ParcelaID { get; set; }


    }


}