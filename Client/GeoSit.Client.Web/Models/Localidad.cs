using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class Localidad
    {
        public long IdLocalidad { get; set; }
        public string Descripcion { get; set; }
        public int IdJurisdiccion { get; set; }
        
    }
}