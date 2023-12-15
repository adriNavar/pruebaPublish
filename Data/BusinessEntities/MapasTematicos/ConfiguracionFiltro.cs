using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class ConfiguracionFiltro : IEntity
    {
        public long FiltroId { get; set; }
        public long ConfiguracionId { get; set; }
        public long FiltroTipo { get; set; }
        public long? FiltroComponente { get; set; }
        public long? FiltroAtributo { get; set; }
        public long? FiltroOperacion { get; set; }
        public long? FiltroColeccion { get; set; }
        public String Valor1 { get; set; }
        public String Valor2 { get; set; }
        public long? Ampliar { get; set; }
        public long? Tocando { get; set; }
        public long? Dentro { get; set; }
        public long? Fuera { get; set; }
        public short? PorcentajeInterseccion { get; set; }
        public long Habilitado { get; set; }
        public MapaTematicoConfiguracion Configuracion { get; set; }
        public virtual List<ConfiguracionFiltroGrafico> ConfiguracionesFiltroGrafico { get; set; }
        
    }
}
