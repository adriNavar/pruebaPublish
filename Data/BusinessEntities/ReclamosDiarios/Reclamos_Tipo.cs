using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ReclamosDiarios
{
    public class Reclamos_Tipo : IEntity
    {
        public int Tipo_Id { get; set; }
        public string Descripcion { get; set; }
        public bool Habilitado { get; set; }
        public int Id { get; set; }

    }
    


}

    
