using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GeoSit.Data.BusinessEntities.MapasTematicos
{

    public class MapaTematicoConfiguracion : IEntity
    {
        public long ConfiguracionId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdConfigCategoria { get; set; }
        public long Atributo { get; set; }
        public long Agrupacion { get; set; }
        public long Distribucion { get; set; }
        public long Rangos { get; set; }
        public string Color { get; set; }
        public long Transparencia { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string ColorContorno { get; set; }
        public int CantidadContorno { get; set; }
        public long Visibilidad { get; set; }
        public long Usuario { get; set; }
        public long Externo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public virtual List<ComponenteConfiguracion> ComponenteConfiguracion { get; set; }
        public virtual List<ConfiguracionFiltro> ConfiguracionFiltro { get; set; }
        public virtual List<ConfiguracionRango> ConfiguracionRango { get; set; }
    }

}
