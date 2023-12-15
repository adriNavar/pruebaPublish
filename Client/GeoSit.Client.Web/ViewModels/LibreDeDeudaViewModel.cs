using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class LibreDeDeudaViewModel : ObjetoEspecificoViewModel
    {
        [Display(Name = "Ente emisor")]
        public int EnteEmisor { get; set; }
        [Display(Name = "Fecha de vigencia")]
        public DateTime? FechaVigencia { get; set; }
        [Display(Name = "Fecha de emisión")]
        public DateTime? FechaEmision { get; set; }
        [Display(Name = "Nº de certificado")]
        public string NroCertificado { get; set; }

        [Display(Name = "Superficie")]
        public string Superficie { get; set; }

        [Display(Name = "Valuacion($)")]
        public string Valuacion { get; set; }

        [Display(Name = "Deuda($)")]
        public string Deuda { get; set; }

        public ICollection<SelectListItem> EntesEmisores { get; set; }

        public LibreDeDeudaViewModel()
        {
            EntesEmisores = new List<SelectListItem>();
        }
    }
}