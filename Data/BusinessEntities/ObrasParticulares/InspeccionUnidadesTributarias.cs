using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class InspeccionUnidadesTributarias : IEntity
    {
        public long Id { get; set; }
        public long InspeccionID { get; set; }
        public long UnidadTributariaID { get; set; }
        public int? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        public UnidadTributaria UnidadTributaria { get; set; }
        public Inspeccion Inspeccion { get; set; }
    }
}
