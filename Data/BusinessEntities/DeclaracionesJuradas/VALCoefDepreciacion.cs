﻿using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALCoefDepreciacion
    {
        public long IdCoefDepreciacion { get; set; }
        public long IdInciso { get; set; }
        public int IdEstadoConservacion { get; set; }
        public long? EdadEdificacion { get; set; }
        public double? Coeficiente { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
