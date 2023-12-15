using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class PaisModels
    {
        public PaisModels()
        {
            Paises = new PaisModel();
        }
        public PaisModel Paises { get; set; }
    }

    public class PaisModel
    {
        public long PaisId { get; set; }
        public string Descripcion { get; set; }
    }
}