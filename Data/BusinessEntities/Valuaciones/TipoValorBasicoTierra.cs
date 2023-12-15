using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class TipoValorBasicoTierra : IEntity
    {	
        public long IdTipoValorBasicoTierra { get; set; }
        public long IdTipoParcela { get; set; }
        

    }
}

    
