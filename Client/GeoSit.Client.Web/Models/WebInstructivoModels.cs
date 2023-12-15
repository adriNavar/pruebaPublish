using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class WebInstructivoModels
    {
        public WebInstructivoModels()
        {
            DatosWebInstructivo = new WebInstructivoModel();
        }
        public WebInstructivoModel DatosWebInstructivo { get; set; }
    }

    public class WebInstructivoModel
    {
        public long IdInstructivo { get; set; }
        public string Seccion { get; set; }
        public string Descripcion { get; set; }
        public string URL { get; set; }
    }
}