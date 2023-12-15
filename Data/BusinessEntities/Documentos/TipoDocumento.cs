using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace GeoSit.Data.BusinessEntities.Documentos
{
    public class TipoDocumento
    {
        public long TipoDocumentoId { get; set; }

        public string Descripcion { get; set; }

        public bool EsEditable { get; set; }

        public bool EsEliminable { get; set; }
        public string Esquema { get; set; }
        public DateTime? FechaBaja { get; set; }

        //Propiedades de navegación
        [JsonIgnore]
        public ICollection<Documento> Documentos { get; set; }
    }
}
