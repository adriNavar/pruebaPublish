
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class Coeficientes : IEntity
    {
        public long Id_Coeficiente { get; set; }
        public long Nro_Coeficiente { get; set; }
        public long Id_Tipo_Coeficiente { get; set; }
        public String Descripcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
         public long? Usuario_Modificacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }
  

}

    
