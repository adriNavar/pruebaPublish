﻿using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValuacionPadronCabecera : IEntity
    {
        public long IdPadron { get; set; }
        public long IdTipoValorBasicoTierra { get; set; }
        public long IdTipoValorBasicoMejora { get; set; }
        public String Descripcion { get; set; }
        public DateTime? VigenciaDesde { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public String Estado { get; set; }
        public DateTime? FechaCalculo { get; set; }
        public DateTime? FechaConsolidado { get; set; }
        public long Usuario_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
    }

}
