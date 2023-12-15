using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{

    public class SearchViewModel
    {
        public string UnidadTributaria { get; set; }

        public long UnidadTributariaId { get; set; }

        public int TipoPorNumero { get; set; }

        public string NumeroDesde { get; set; }

        public string NumeroHasta { get; set; }

        public int TipoPorFecha { get; set; }

        public string FechaDesde { get; set; }

        public string FechaHasta { get; set; }

        public string Persona { get; set; }

        public long PersonaId { get; set; }

        public SelectList EstadoList { get; set; }

        public long Estado { get; set; }

        public string ExpresionRegularExpediente { get; set; }

        public string ExpresionRegularLegajo { get; set; }

        public string ExpresionRegularChapa { get; set; }

        public string ExpresionRegularExpedienteVisible { get; set; }

        public string ExpresionRegularLegajoVisible { get; set; }

        public string ExpresionRegularChapaVisible { get; set; }

        public long LoadExpediente { get; set; }
    }  
}
