using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Documentos
{
    public class InformeEstadoTramite
    {
        public string Tipo { get; set; }

        public string NroIdentificacion { get; set; }

        public string Numero { get; set; }

        public DateTime FechaInicio { get; set; }

        public string EstadoActual { get; set; }

        public DateTime FechaUltimoEstado { get; set; }
    }
}
