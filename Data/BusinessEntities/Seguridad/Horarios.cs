
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Horarios : IEntity
    {
        public long Id_Horario { get; set; }
        public string Descripcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Usuario_Modificacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public ICollection<Perfiles> Perfiles { get; set; }
        public ICollection<HorariosDetalle> HorariosDetalle { get; set; }

    }
    


}

    
