using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.MapasTematicos;


namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class ListadoDeTareas
    {

        public String Login { get; set; }
        public String Nombre { get; set; }
        public String Apellido { get; set; }
        public String Ip { get; set; }
        public String nombreFuncion { get; set; }
        public String Fecha { get; set; }
        public String Objeto { get; set; }
        public long? Cantidad { get; set; }
        public String Operacion { get; set; }
        public String ObjAct { get; set; }
        public String ObjHist { get; set; }
        public long? idAuditoria { get; set; }
        public long? cantidadObModif { get; set; }
        public long? idFuncion { get; set; }
        public long? idFuncionPadre { get; set; }
        public String nombreFuncionPadre { get; set; }
        public bool TieneObjetoOrigen { get; set; }

    }
}
