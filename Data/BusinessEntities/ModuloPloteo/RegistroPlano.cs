﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class RegistroPlano
    {
        public int IdRegistroPlano { get; set; }
        public int IdPeriodo { get; set; }
        public int IdTipoPlano { get; set; }
        public long IdPartido { get; set; }
        public string Adjunto { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
