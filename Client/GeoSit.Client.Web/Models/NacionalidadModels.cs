using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class NacionalidadModels
    {
        public NacionalidadModels()
        {
            Nacionalidades = new NacionalidadModel();
        }
        public NacionalidadModel Nacionalidades { get; set; }
    }

    public class NacionalidadModel
    {
        public long NacionalidadId { get; set; }
        public string Descripcion { get; set; }
    }
}