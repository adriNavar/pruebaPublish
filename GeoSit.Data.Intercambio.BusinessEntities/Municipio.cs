using System;

namespace GeoSit.Data.Intercambio.BusinessEntities
{
    public class Municipio
    {
        public int IdMunicipio { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public DateTime? UltimoProceso { get; set; }
    }
}
