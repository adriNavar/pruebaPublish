using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class TipoDocumentoModels
    {
        public TipoDocumentoModels()
        {
            TiposDocumentos = new TiposDocumentosModel();
        }
        public TiposDocumentosModel TiposDocumentos { get; set; }
    }

    public class TiposDocumentosModel
    {
        public long TipoDocumentoId { get; set; }
        public string Descripcion { get; set; }
        public bool EsEditable { get; set; }

        public bool EsEliminable { get; set; }
        public string Esquema { get; set; }
    }
}