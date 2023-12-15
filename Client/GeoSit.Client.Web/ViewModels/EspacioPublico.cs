using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class EspacioPublico : ObjetoEspecificoViewModel
    {

        [Display(Name = "Superficie")]
        public string Superficie { get; set; }

        public long IdEspacioPublico { get; set; }

    }
}