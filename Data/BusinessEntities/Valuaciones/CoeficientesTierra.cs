
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class CoeficientesTierra : IEntity
    {
        public long Id_Coeficiente_Tierra { get; set; }
        public long Id_Coeficiente { get; set; }
        public String Desde { get; set; }
        public String Hasta { get; set; }
        public float Coeficiente { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Modificacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }
  
}

    
