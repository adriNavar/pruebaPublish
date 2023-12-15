using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class OCObjeto
    {
        public long FeatId { get; set; }
        public long IdSubtipoObjeto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Atributos { get; set; }
        public DbGeometry Geometry { get; set; }
        public string GeomTxt { get; set; }
        public Nullable<int> ClassId { get; set; }
        public Nullable<int> RevisionNumber { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}
