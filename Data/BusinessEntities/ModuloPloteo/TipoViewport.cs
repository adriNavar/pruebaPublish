using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class TipoViewport
    {
        public int IdTipoViewport { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBaja { get; set;}
        public DateTime? FechaBaja { get; set;}
    }
}
