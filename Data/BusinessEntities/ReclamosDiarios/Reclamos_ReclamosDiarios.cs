using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ReclamosDiarios
{
    public class Reclamos_ReclamosDiarios : IEntity
    {
        public long Id_Reclamo { get; set; }
        public string Nro_Reclamo { get; set; }
        public int? Distrito { get; set; }
        public string Calle { get; set; }
        public int? Altura { get; set; }
        public string Interseccion { get; set; }
        public string Manzana { get; set; }
        public DateTime? Fecha_Ingreso { get; set; }
        public int Id_Clase { get; set; }
        public int Id_Tipo { get; set; }
        public int Id_Motivo { get; set; }
        public DateTime FechaAlta { get; set; }
    }
    


}

    
