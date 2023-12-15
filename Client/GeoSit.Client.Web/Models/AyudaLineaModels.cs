using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class AyudaLineaModels
    {
        public AyudaLineaModels()
        {
            DatosAyudaLinea = new AyudaLineaModel();
        }
        public AyudaLineaModel DatosAyudaLinea { get; set; }
    }

    public class AyudaLineaModel
    {
        public int IdAyuda { get; set; }
        public string Seccion { get; set; }
        public string Descripcion { get; set; }
        public string URL { get; set; }
        public int IdFuncion { get; set; }
        public int IdTipo { get; set; }
        public int Orden { get; set; }
    }
}