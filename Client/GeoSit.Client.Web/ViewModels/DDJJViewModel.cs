using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class DDJJ : ObjetoEspecificoViewModel
    {
        public long? IdDDJJ { get; set; }
        public long? Poligono { get; set; }
        public long? IdVersion { get; set; }
        public string DeclaracionJurada { get; set; }

        public List<SelectListItem> Versiones { get; set; }
    }
}