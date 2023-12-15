using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.Seguridad;
using System;
using System.Collections.Generic;


namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class Inspector : IEntity
    {
        public long InspectorID { get; set; }
        public string EsPlanificador { get; set; }
        public long UsuarioID { get; set; }

        public virtual Usuarios Usuario { get; set; }
        public int UsuarioUpdate { get; set; }
        public string TiposInspeccionSelected { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<Acta> Actas { get; set; }
        public ICollection<Inspeccion> Inspecciones { get; set; }
    }
}
