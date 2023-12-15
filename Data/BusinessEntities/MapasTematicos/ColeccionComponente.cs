using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class ColeccionComponente : IEntity
    {
        public long ColeccionComponenteId { get; set; }
        public long ColeccionId { get; set; }
        public long ObjetoId { get; set; }
        public long ComponenteId { get; set; }
        public long UsuarioAlta { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }

        public Coleccion Coleccion { get; set; }
    }
}