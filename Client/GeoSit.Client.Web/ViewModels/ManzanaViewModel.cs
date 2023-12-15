using System.ComponentModel.DataAnnotations;

namespace GeoSit.Client.Web.ViewModels
{
    public class ManzanaViewModel : ObjetoEspecificoViewModel
    {
        [Display(Name = "ID s/Plano")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Debe tener cargado un valor alfanumérico")]
        public string Plano { get; set; }
    }
}