using System;
using GeoSit.Data.BusinessEntities.Interfaces;

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