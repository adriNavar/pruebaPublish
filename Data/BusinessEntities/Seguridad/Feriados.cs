
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Feriados : IEntity
    {
        public long Id_Feriado { get; set; }
        public DateTime Fecha { get; set; }
        public String Descripcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

    }
    


}

    
