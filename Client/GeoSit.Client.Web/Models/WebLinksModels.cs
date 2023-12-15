using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class WebLinksModels
    {
        public WebLinksModels()
        {
            DatosWebLinks = new WebLinksModel();
        }
        public WebLinksModel DatosWebLinks { get; set; }
    }

    public class WebLinksModel
    {
        public long idLink { get; set; }
        public string Descripcion { get; set; }
        public string URL { get; set; }
    }
}