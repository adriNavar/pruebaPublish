using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class DetalleObjetoConfig
    {
        public string id { get; set; }
        public bool cierra { get; set; }
        public bool edita { get; set; }
        public bool extraFieldsRelaciones { get; set; }
        public bool minimiza { get; set; }

        public DetalleObjetoConfig(string id) 
            : this(id, false, false, false, false) { }
        public DetalleObjetoConfig(string id, bool cierra, bool edita, bool extraFieldsRelaciones, bool minimiza)
        {
            this.id = id;
            this.cierra = cierra;
            this.edita = edita;
            this.extraFieldsRelaciones = extraFieldsRelaciones;
            this.minimiza = minimiza;
        }
    }
}