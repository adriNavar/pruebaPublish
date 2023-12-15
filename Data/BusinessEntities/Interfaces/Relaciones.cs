using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Interfaces
{
    public class Relaciones
    {

        //public String Mi_Imp_Leyenda { get; set; }
        //public String Mi_Imp_Identificacion { get; set; }
        //public String Mi_Imp_Definitivo { get; set; }
        //public String Otro_Imp_Tipo { get; set; }
        //public String Otro_Imp_Leyeda { get; set; }
        //public String Otro_Imp_Identificacion { get; set; }
        //public String Otro_Imp_Nombre { get; set; }
        //public String Otro_Imp_Definitivo{ get; set; }
        //public String Es { get; set; }
        //public String Relacion { get; set; }

        public String ORIGEN { get; set; }
        public String ESTADO_ORIGEN { get; set; }
        public String DESTINO { get; set; }
        public String ESTADO_DESTINO { get; set; }
        public String TIPO_RELACION { get; set; }
        public String OPERACION { get; set; }
        public String ALTA_FECHA { get; set; }
        public String FECHA_DESDE { get; set; }
        public String FECHA_HASTA { get; set; }
    }
}
