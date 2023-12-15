using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class TituloViewModel : ObjetoEspecificoViewModel
    {
        [Display(Name = "Tipo")]
        public int Tipo { get; set; }
        [Display(Name = "Inscripción")]
        public string Inscripcion { get; set; }
        [Display(Name = "Fecha")]
        public DateTime? Fecha { get; set; }

        public ICollection<SelectListItem> Tipos { get; set; }
    }
}