
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class TipoCoeficientes : IEntity
    {	
        public long Id_Tipo_Coeficiente { get; set; }
        public String Descripcion { get; set; }

    }
}

    
