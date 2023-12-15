using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Client.Web.ViewModels
{
    public class Parcela : ObjetoEspecificoViewModel
    {
        [Display(Name = "Tipo")]
        public OperacionParcela Operacion { get; set; }

        [Display(Name = "Partida/Nomenclatura")]
        [Required(ErrorMessage = "La partida es requerida")]
        //[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Debe tener cargados datos con el siguiente formato: X9-999999-9/9999999999999999.")]
        public string Partida { get; set; }
        public int IdPartidaPersona { get; set; }


        [Display(Name = "Zona")]
        public int? Zona { get; set; }
        [Display(Name = "Tipo")]
        public int? Tipo { get; set; }
        [Display(Name = "ID's/Plano")]
        public string Plano { get; set; }


        public ICollection<SelectListItem> Zonas { get; set; }

        public ICollection<SelectListItem> Tipos { get; set; }

    }
}