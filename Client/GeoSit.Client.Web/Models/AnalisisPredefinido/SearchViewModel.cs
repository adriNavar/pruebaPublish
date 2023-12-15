using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models.AnalisisPredefinido
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


    }  
}