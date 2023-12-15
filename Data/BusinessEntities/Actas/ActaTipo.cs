using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Actas
{
    public class ActaTipo : IEntity
    {
        public long ActaTipoId { get; set; }
        public string Descripcion { get; set; }

        //Propiedades de navegacíon
        public ICollection<Acta> Actas { get; set; }
    }
}
