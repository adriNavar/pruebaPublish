
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class HorariosDetalle : IEntity
    {
        public long Id_Horario_Detalle { get; set; }
        public long Id_Horario { get; set; }
        public string Dia { get; set; }
        public DateTime Hora_Inicio { get; set; }
        public DateTime Hora_Fin { get; set; }
        public Horarios Horario { get; set; }
 

    }
    


}

    
