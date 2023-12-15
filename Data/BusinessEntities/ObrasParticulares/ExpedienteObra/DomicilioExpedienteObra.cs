using GeoSit.Data.BusinessEntities.Inmuebles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class DomicilioExpedienteObra : IEntity
    {
        public long DomicilioExpedienteObraID { get; set; }

        public long DomicilioInmuebleID { get; set; }

        public long ExpedienteObraID { get; set; }

        //Propiedades de Navegación 
        //public virtual DomicilioInmueble DomicilioInmueble { get; set; }
        public virtual ExpedienteObra ExpedienteObra { get; set; }
        //Fin Propiedades de Navegación
    }
}
