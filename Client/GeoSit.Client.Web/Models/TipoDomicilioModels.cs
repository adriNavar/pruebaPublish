using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;


namespace GeoSit.Client.Web.Models
{
    public class TipoDomicilioModels
    {
        public TipoDomicilioModels()
        {
            TiposDomicilios = new TiposDomicilioModel();
        }
        public TiposDomicilioModel TiposDomicilios { get; set; }
    }

    public class TiposDomicilioModel
    {
        public long TipoDomicilioId { get; set; }
        public string Descripcion { get; set; }
    }
}