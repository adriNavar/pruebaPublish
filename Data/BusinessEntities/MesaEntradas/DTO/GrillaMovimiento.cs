using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MesaEntradas.DTO
{
    public class GrillaMovimiento
    {
        public int IdMovimiento { get; set; }
        public string Fecha { get; set; }
        public string TipoMovimiento { get; set; }
        public string Usuario { get; set; }
        public string Destinatario { get; set; }
        public string Estado { get; set; }
        public string Remito { get; set; }

    }
}
