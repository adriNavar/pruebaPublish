
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Funciones : IEntity
    {
        public long Id_Funcion { get; set; }
        public string Nombre { get; set; }
        public long Id_Aplicacion { get; set; }
        public long Id_Funcion_Padre { get; set; }


        //public SEEvento Evento { get; set; }
    }
    


}

    
