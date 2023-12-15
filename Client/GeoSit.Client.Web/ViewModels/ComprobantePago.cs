using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class ComprobantePago : ObjetoEspecificoViewModel
    {
        [Display(Name = "Tipo de tasa")]
        public int TipoTasa { get; set; }

        [Display(Name = "Identificación trámite")]
        public string Identificacion { get; set; }

        [Display(Name = "Tipo de trámite")]
        public string TipoTramite { get; set; }

        [Display(Name = "Fecha de Liquidacion")]
        public DateTime? FechaLiquidacion { get; set; }

        [Display(Name = "Fecha de Pago")]
        public DateTime? FechaPago { get; set; }

        [Display(Name = "Fecha de Vencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [Display(Name = "Medio de Pago")]
        public string MedioPago { get; set; }

        [Display(Name = "Importe($)")]
        public string Importe { get; set; }

        [Display(Name = "Estado de pago")]
        public string EstadoPago { get; set; }

        public ICollection<SelectListItem> TiposTasa { get; set; }
    }
}