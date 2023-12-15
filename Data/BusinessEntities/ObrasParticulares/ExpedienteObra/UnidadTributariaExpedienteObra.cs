
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class UnidadTributariaExpedienteObra : IEntity
    {
        public long UnidadTributariaExpedienteObraID { get; set; }
        public long ExpedienteObraID { get; set; }
        public long UnidadTributariaID { get; set; }

        //Propiedades de Navegación 
        public virtual ExpedienteObra ExpedienteObra { get; set; }
        //Fin Propiedades de Navegación
    }
}
