
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class PerfilesHist : IEntity
    {
        public long Id_Perfil_Hist { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Horario { get; set; }
        public string Nombre { get; set; }
        public long Usuario_Operacion { get; set; }
        public DateTime Fecha_Operacion { get; set; }
        
    }
    


}

    
