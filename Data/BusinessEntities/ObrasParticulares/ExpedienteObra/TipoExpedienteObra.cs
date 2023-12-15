
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class TipoExpedienteObra : IEntity
    {
        public long TipoExpedienteObraID { get; set; }
        public long ExpedienteObraID { get; set; }
        public int TipoExpedienteID { get; set; }
        public int UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación 
        public virtual ExpedienteObra ExpedienteObra { get; set; }
        public virtual TipoExpediente TipoExpediente { get; set; }
        //Fin Propiedades de Navegación		
    }
}
