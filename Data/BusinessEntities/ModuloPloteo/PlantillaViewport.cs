using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PlantillaViewport //: Plantilla
    {
        public long IdPlantillaViewport { get; set; }
        public int IdPlantilla { get; set; }
        public int IdTipoViewPort { get; set; }
        public int? Orientacion { get; set; }
        public double? ImpresionXMin { get; set; }
        public double? ImpresionYMin { get; set; }
        public double? ImpresionXMax { get; set; }
        public double? ImpresionYMax { get; set; }
        public int DistBuffer { get; set; }
        public bool OptimizarTamanioHoja { get; set; }
        public int? IdNorte { get; set; }
        public double? NorteX { get; set; }
        public double? NorteY { get; set; }
        public double? NorteAlto { get; set; }
        public double? NorteAncho { get; set; }
        public int? IdFuncionAdicional { get; set; }
        public int UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int? Transparencia { get; set; }
        public bool Visibilidad { get; set; }

        /*public long IdPlantillaViewport { get; set; }
        public int IdTipoViewPort { get; set; }
        public List<LayerViewport> layersViewport { get; set; }*/
    }
}
