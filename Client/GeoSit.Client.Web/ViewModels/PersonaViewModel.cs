using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class PersonaViewModel : ObjetoEspecificoViewModel
    {
        public bool MostrarCamposTitularidad { get; set; }
        [Display(Name = "Persona")]
        public string Persona { get; set; }
        public int IdPersona { get; set; }

        [Display(Name = "Tipo Persona")]
        public int TipoPersona { get; set; }

        [Display(Name = "Tipo Titularidad")]
        public int TipoTitularidad { get; set; }

        [Display(Name = "% de Titularidad")]
        public string Titularidad { get; set; }

        public ICollection<SelectListItem> TiposPersona { get; set; }

        public ICollection<SelectListItem> TiposTitularidad { get; set; }
    }
}