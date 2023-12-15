using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.MapasTematicos;


namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class Consultas
    {

        public List<long> idUsuarios{ get; set; }
        public List<long> FuncionAsociada{ get; set; }
        public String fechaDesde{ get; set; }
        public String fechaHasta { get; set; }
        //Resultado para datatable
        public Usuarios usuario { get; set; }
        public Funciones funcion { get; set; }
        public Auditoria auditoria { get; set; }
        public Componente componente { get; set; }
        public long cantidadObjetos { get; set; }
        public String nombreFuncion { get; set; }
        public String contenido { get; set; }
        public int tipoInforme { get; set; }

    }
}
