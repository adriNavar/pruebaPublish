using GeoSit.Data.BusinessEntities.Inmuebles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class InspeccionExpedienteObra : IEntity
    {
        public long InspeccionExpedienteObraID { get; set; }
        public long ExpedienteObraID { get; set; }
        public long TipoInspeccionInmuebleID { get; set; }

        //Propiedades de Navegación 
        //public virtual TipoInspeccionInmueble TipoInspeccionInmueble { get; set; }
        public virtual ExpedienteObra ExpedienteObra { get; set; }
        //Fin Propiedades de Navegación
    }
}
