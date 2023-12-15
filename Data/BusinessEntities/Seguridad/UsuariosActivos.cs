
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class UsuariosActivos : IEntity
    {
        public long Id_Usuario_Activo { get; set; }
        public long Id_Usuario { get; set; }

        public DateTime Fecha { get; set; }

        public String Token { get; set; }
        public DateTime Heartbeat { get; set; }
    }



}


