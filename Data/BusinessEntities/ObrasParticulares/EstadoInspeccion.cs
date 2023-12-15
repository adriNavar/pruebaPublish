using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class EstadoInspeccion : IEntity
    {
        public int EstadoInspeccionID { get; set; }
        public string Descripcion { get; set; }
    }
}
