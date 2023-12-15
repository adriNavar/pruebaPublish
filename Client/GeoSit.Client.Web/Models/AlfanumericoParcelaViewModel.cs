using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Models
{
    public class AlfanumericoParcelaViewModel
    {
        public long TipoOperacionId { get; set; }

        public SelectList TipoOperacionList { get; set; }

        public long TipoParcelaId { get; set; }

        public SelectList TipoParcelaList { get; set; }

        public long ClaseParcelaId { get; set; }

        public SelectList ClaseParcelaList { get; set; }

        public long JurisdiccionId { get; set; }

        public SelectList JurisdiccionList { get; set; }

        public long EstadoParcelaId { get; set; }

        public SelectList EstadoParcelaList { get; set; }

        public string NumeroExpediente { get; set; }

        public string FechaExpediente { get; set; }

        public string NumeroParcela { get; set; }
        public decimal Superficie { get; set; }

        public string Vigencia { get; set; }
    }
}