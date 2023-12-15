
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class UTDomicilio : IEntity
    {
        public long DomicilioID { get; set; }
        public long TipoDomicilioID { get; set; }
        public long UnidadTributariaID { get; set; }
        public int? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}

    
