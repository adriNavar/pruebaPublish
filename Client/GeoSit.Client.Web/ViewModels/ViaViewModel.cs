using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class ViaViewModel : ObjetoEspecificoViewModel
    {
        [Display(Name = "Tipo")]
        public int Tipo { get; set; }

        [Display(Name = "ID s/Plano")]
        public string Plano { get; set; }

        public ICollection<SelectListItem> Tipos { get; set; }
    }
}