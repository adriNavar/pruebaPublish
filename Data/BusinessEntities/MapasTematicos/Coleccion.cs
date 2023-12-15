using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class Coleccion : IEntity
    {
        public long ColeccionId { get; set; }
        public string Nombre { get; set; }
        public long UsuarioAlta { get; set; }
        public long? UsuarioBaja { get; set; }
        public long UsuarioModificacion { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime FechaModificacion { get; set; }
        public ICollection<ColeccionComponente> Componentes { get; set; }
    }

    public class ColeccionVista 

    {
        public long ColeccionId { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
    }
   
}