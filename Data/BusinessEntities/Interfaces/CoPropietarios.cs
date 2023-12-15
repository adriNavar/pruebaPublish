using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Interfaces
{
    public class CoPropietarios
    {
        public String Padron { get; set; }
        public String Estado { get; set; }
        public String Ind_Leyenda { get; set; }
        public String Ind_Identificacion { get; set; }
        public String Nombre { get; set; }
        public String Tributaria_Categoria { get; set; }
        public String Tributaria_Id { get; set; }
        public double? Porcentaje { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
