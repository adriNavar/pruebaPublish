using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class MensuraDocumentoModels
    {
        public long IdMensuraDocumento { get; set; }
        public long IdMensura { get; set; }
        public long IdDocumento { get; set; }


        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        //public MensuraModel Mensura { get; set; }
        public DocumentoModel Documento { get; set; }
    }
}