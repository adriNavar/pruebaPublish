using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJSorOtrasCar
    {
        public long IdDDJJSorOtrasCar { get; set; }
        public long IdVersion { get; set; }
        public string OtrasCarRequerida { get; set; }
        public decimal Requerido { get; set; }


        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJVersion Version { get; set; }
    }
}
