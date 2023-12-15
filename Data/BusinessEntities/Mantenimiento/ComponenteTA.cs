
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Mantenimiento
{
    public class ComponenteTA : IEntity
    {
        public long Id_Compoente{ get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Esquema { get; set; }
        public string Tabla { get; set; }
        public string DocType { get; set; }
        public bool Permite_Agregar { get; set; }
        public bool Permite_Eliminar { get; set; }
        public bool Permite_Modifiar { get; set; }
        

    }
  

}

    
