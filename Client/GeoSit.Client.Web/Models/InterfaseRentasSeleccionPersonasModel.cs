using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeoSit.Client.Web.Models.ResponsableFiscal;

namespace GeoSit.Client.Web.Models
{
    public class InterfaseRentasSeleccionPersonasModel
    {
        public ResponsableFiscalViewModel ResponsableFiscal { get; set; }
        public InterfaseRentasPersonaModel[] Personas { get; set; }
        public bool NotFound { get; set; }
    }
}