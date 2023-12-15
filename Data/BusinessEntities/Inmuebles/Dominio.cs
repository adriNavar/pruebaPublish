using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class Dominio : AuditableEntity
    {
        public long DominioID { get; set; }

        public long UnidadTributariaID { get; set; }

        public long TipoInscripcionID { get; set; }

        public string Inscripcion { get; set; }

        public DateTime Fecha { get; set; }
        public string TipoInscripcionDescripcion { get; set; }

        //Propiedades de Navegación
        public UnidadTributaria UnidadTributaria { get; set; }

        public TipoInscripcion TipoInscripcion { get; set; }

        public ICollection<DominioTitular> Titulares { get; set; }

    }
}