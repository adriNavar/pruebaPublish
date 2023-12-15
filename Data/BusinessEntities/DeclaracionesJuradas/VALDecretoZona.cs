﻿using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALDecretoZona
    {
        public long IdDecretoZona { get; set; }
        public long IdDecreto { get; set; }
        public long IdTipoParcela { get; set; }        

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public VALDecreto Decreto { get; set; }

        public TipoParcela TipoParcela { get; set; }
    }
}
