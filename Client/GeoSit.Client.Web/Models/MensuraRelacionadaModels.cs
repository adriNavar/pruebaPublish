using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class MensuraRelacionadaModels
    {
        public long IdMensuraRelacionada { get; set; }
        public long IdMensuraOrigen { get; set; }
        public long IdMensuraDestino { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public MensuraModel MensuraOrigen { get; set; }

        public MensuraModel MensuraDestino { get; set; }

        public string MensuraOrigenDescripcion { get; set; }
        public string MensuraOrigenTipo { get; set; }
        public string MensuraOrigenEstado { get; set; }

        public string MensuraDestinoDescripcion { get; set; }
        public string MensuraDestinoTipo { get; set; }
        public string MensuraDestinoEstado { get; set; }
    }
}