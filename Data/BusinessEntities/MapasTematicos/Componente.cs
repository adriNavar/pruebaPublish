using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class Componente : IEntity
    {
        public long ComponenteId { get; set; }
        public long EntornoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Esquema { get; set; }
        public string Tabla { get; set; }
        public Nullable<long> Graficos { get; set; }
        public string DocType { get; set; }
        public string Capa { get; set; }
        public bool EsTemporal { get; set; }
        public bool EsLista { get; set; }
        public string TablaGrafica { get; set; }
        public long? IdComponenteGrupo { get; set; }
        public int? EsConFre { get; set; }//Se usa para mostrar los componentes con EsConfre = 1 y idServicio 1 y/o 2 en el formulario Consultas Frecuentes
        public string TablaTemporal { get; set; }
        public ICollection<Atributo> Atributos { get; set; }

        [JsonIgnore]
        public List<ComponenteConfiguracion> ComponenteConfiguracion { get; set; }
    }
}
