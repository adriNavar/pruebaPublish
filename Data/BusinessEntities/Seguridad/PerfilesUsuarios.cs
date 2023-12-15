
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class PerfilesUsuarios : IEntity
    {
        public long Id_Perfil { get; set; }
        public long Id_Usuario { get; set; }
        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long Cantidad_Perfiles { get; set; }
    }
    


}

    
