using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class PLN_Atributo : IEntity
    {
        public PLN_Atributo()
        {
        }

        public long Id_Atributo_Zona { get; set; }
        public string Descripcion { get; set; }
    }
}
