using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class DominioUT
    {
        public long DominioID { get; set; }

        public long TipoInscripcionID { get; set; }

        public string TipoInscripcion { get; set; }

        public string Inscripcion { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaAlta { get; set; }
        
        public DateTime? FechaBaja { get; set; }

        public IEnumerable<Titular> Titulares { get; set; }
    }
}
