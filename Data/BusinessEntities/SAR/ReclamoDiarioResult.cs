using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.SAR
{
    public class ReclamoDiarioResult
    {
        public int? Distrito { get; set; }
        public string Calle { get; set; }
        public int? Altura { get; set; }
        public string Interseccion { get; set; }
        public string Manzana { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Clase { get; set; }
        public string Tipo { get; set; }
        public string Motivo { get; set; }
        public string Reclamo { get; set; }
        public int Clase_Id { get; set; }
        public int Tipo_Id { get; set; }
        public int Motivo_Id { get; set; }
    }
}
