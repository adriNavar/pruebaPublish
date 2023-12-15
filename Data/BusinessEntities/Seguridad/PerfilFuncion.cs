using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class PerfilFuncion
    {
        public long Id_Perfil_Funcion { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Funcion { get; set; }
        public long Id_Funcion_Padre { get; set; }
        public string Funcion_Nombre { get; set; }
        public string Funcion_Padre_Nombre { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
    }
}
