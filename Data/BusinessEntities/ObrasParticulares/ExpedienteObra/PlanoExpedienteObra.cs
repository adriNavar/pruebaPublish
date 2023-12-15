using GeoSit.Data.BusinessEntities.Inmuebles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class PlanoExpedienteObra : IEntity
    {
        public long PlanoExpedienteObraID { get; set; }
        public long ExpedienteObraID { get; set; }
        public long PlanoInmuebleID { get; set; }

        //Propiedades de Navegación
        //public virtual PlanoInmueble PlanoInmueble { get; set; }
        public virtual ExpedienteObra ExpedienteObra { get; set; }
    }
}
