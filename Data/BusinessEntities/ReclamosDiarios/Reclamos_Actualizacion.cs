using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ReclamosDiarios
{
    public class Reclamos_Actualizacion : IEntity
    {
        public DateTime Fecha_Reclamos_Diarios { get; set; }
        public DateTime Fecha_Parametros { get; set; }
        public string Nombre { get; set; }

    }
    


}

    
