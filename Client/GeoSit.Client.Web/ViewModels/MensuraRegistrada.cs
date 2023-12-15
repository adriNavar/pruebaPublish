using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GeoSit.Client.Web.ViewModels
{
    public class MensuraRegistrada : ObjetoEspecificoViewModel
    {

        [Display(Name = "Nº de mensura")]
        public string Mensura { get; set; }
        public int IdMensura { get; set; }
        public string TipoMensura { get; set; }
        public long IdTipoMensura { get; set; }

    }
}