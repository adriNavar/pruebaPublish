using System.ComponentModel.DataAnnotations;

namespace GeoSit.Client.Web.ViewModels
{
    public class DescripcionDelInmueble : ObjetoEspecificoViewModel
    {
        //[Display(Name = "Descripción del inmueble")]
        [Required]
        public string Descripcion { get; set; }
    }
}