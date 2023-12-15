
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class UsuariosPerfiles : IEntity
    {
        public long Id_Usuario_Perfil { get; set; }
        public long Id_Usuario { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Horario { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public Usuarios Usuarios { get; set; }
        public Horarios Horarios { get; set; }

    }
    


}

    
