using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class TipoPlano
    {
        public int IdTipoPlano { get; set; }
        public string Nombre { get; set; }
        public int IdPlantilla { get; set; }
        public bool Activo { get; set; }
        public string Tema { get; set; }
        public string Servicio { get; set; }
        public string CodigoPlano { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
