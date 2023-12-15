using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class LayerConfiguracionModels
    {
        public int LayerConfiguracionId { get; set; }
        public int Agrupacion1 { get; set; }
        public Nullable<int> Agrupacion2 { get; set; }
        public string Descripcion { get; set; }
        public string SourceType { get; set; }
        public string SourceConfig { get; set; }
        public string StyleConfig { get; set; }
        public bool Visible { get; set; }
        public int ZIndex { get; set; }
        public Nullable<int> MinScale { get; set; }
        public Nullable<int> MaxScale { get; set; }
    }
}