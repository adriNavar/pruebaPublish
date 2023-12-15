
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class SuperficieExpedienteObra : IEntity
    {
        public long SuperficieExpedienteObraID { get; set; }
        public DateTime Fecha { get; set; }
        public double Superficie { get; set; }
        public int? CantidadPlantas { get; set; }
        public int TipoSuperficieID { get; set; }
        public long ExpedienteObraID { get; set; }
        public int UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación
        public virtual ExpedienteObra ExpedienteObra { get; set; }
        public virtual TipoSuperficie TipoSuperficie { get; set; }
    }
}
