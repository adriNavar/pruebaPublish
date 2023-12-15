using GeoSit.Data.BusinessEntities.Inmuebles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class PersonaExpedienteObra : IEntity
    {
        public long PersonaExpedienteObraID { get; set; }
        public long PersonaInmuebleID { get; set; }
        public long ExpedienteObraID { get; set; }
        public int RolID { get; set; }

        //Propiedades de Navegación 
        //public virtual PersonaInmueble PersonaInmueble { get; set; }
        public virtual ExpedienteObra ExpedienteObra { get; set; }
        public virtual Rol Rol { get; set; }
        //Fin Propiedades de Navegación
    }
}
