using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class RequestObjetoResultado : IEntity
    {
        public string GUID { get; set; }
        public long IdUsuario { get; set; }
        public string TokenSesion { get; set; }
        public Componente Componente { get; set; }
        public Atributo Atributo { get; set; }
        public Componente ComponenteAtributo { get; set; }
        public bool EsImportado { get; set; }
        public MapaTematicoConfiguracion MapaTematicoConfiguracion { get; set; }
        public List<ConfiguracionFiltro> ConfiguracionFiltros { get; set; }
    }
}
