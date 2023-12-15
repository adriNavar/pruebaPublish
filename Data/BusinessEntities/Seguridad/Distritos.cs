
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Distritos : IEntity
    {
        public string Id_Distrito { get; set; }
        public string Geometry { get; set; }
        public string Nombre { get; set; }
        public string Abrev { get; set; }
        public long Id_Region { get; set; }
        public long? Id_Provincia { get; set; }
        public long? Prestacion { get; set; }
        public long? Apic_Gid { get; set; }
        public long? Apic_Id { get; set; }
        public Regiones Region { get; set; }

    }
    


}

    
