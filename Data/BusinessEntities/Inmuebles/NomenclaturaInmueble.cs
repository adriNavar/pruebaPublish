
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class NomenclaturaOLD : IEntity
    {
        public long Id_Nomenclatura { get; set; }
        public long Id_Parcela { get; set; }
        public String Nomenclatura_Descripcion { get; set; }
        public long Id_Tipo_Nomenclatura { get; set; }
        public long? Id_Usu_Alta { get; set; }
        public DateTime? Fecha_Alta { get; set; }
        public long? Id_Usu_Modif { get; set; }
        public DateTime? Fecha_Modif { get; set; }
        public long? Id_Usu_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
    }
			
}
