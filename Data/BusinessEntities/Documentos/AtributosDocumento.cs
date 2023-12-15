using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.BusinessEntities.Documentos
{
    public class AtributosDocumento : IEntity
    {
        public long numero_plano { get; set; }
        public string letra_plano { get; set; }
        public DateTime fecha_mensura { get; set; }
        public DateTime fecha_vigencia_original { get; set; }
        public DateTime fecha_vigencia_actual { get; set; }
        public DateTime fecha_presentacion { get; set; }
        public DateTime? fecha_aprobacion { get; set; }
        public DateTime mensuras_relacionadas { get; set; }
    }
}

