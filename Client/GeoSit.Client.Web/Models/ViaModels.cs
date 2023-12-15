using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class ViaModels
    {
        public ViaModels()
        {
            DatosVia = new ViaModel();
        }
        public ViaModel DatosVia { get; set; }


    }
    public class ViaModel
    {
        public long ViaId { get; set; }
        public long TipoViaId { get; set; }
        public long? FeatId { get; set; }
        public string Nombre { get; set; }
    }
}