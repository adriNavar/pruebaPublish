
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Perfiles : IEntity
    {
        public long Id_Perfil { get; set; }
        public string Nombre { get; set; }
        public long Id_Horario { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Usuario_Modificacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

        public Horarios Horario { get; set; }



    }
    


}

    
