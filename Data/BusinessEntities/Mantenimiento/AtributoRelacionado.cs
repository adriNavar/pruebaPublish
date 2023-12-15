using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Mantenimiento
{
    public class AtributoRelacionado : IEntity
    {
        public long Id_Atributo { get; set; }
        public string Descripcion { get; set; }
        
    }
  

}

    
