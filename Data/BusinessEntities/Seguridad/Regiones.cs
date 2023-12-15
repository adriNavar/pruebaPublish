
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Regiones : IEntity
    {
        public long Id_Region { get; set; }
        public string Nombre { get; set; }
        public string Geometry { get; set; }
        public long Id_Concesion { get; set; }
        public long? Apic_Gid { get; set; }
        public string Apic_Id { get; set; }

        public ICollection<Distritos> Distritos { get; set; }
    }
    


}

    
