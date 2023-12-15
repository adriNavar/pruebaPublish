using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.Common;

namespace GeoSit.Client.Web.ViewModels
{
    public class UnidadFuncional : ObjetoEspecificoViewModel
    {
        public OperacionParcela Operacion { get; set; }

        [Display(Name = "Partida Inmobiliaria")]
        public string UnidadFuncionalCodigo { get; set; }
        public int IdUnidadFuncional { get; set; }



        [Display(Name = "Tipo")]
        public int? Tipo { get; set; }

        [Display(Name = "Vigencia")]
        public DateTime? Vigencia { get; set; }

        [Display(Name = "ID's/Plano")]
        public string Plano { get; set; }

        [Display(Name = "Piso")]
        public string Piso { get; set; }

        [Display(Name = "Unidad")]
        public string Unidad { get; set; }

        [Display(Name = "Superficie")]
        public string Superficie { get; set; }


        public ICollection<SelectListItem> Tipos { get; set; }


    }
}