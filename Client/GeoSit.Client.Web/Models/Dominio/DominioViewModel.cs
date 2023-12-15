using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Models.Dominio
{
    public class DominioViewModel
    {
        public long DominioID { get; set; }

        public long UnidadTributariaID { get; set; }

        public long TipoInscripcionID { get; set; }

        public string TipoInscripcionDescripcion { get; set; }

        public string Inscripcion { get; set; }

        public DateTime Fecha { get; set; }

        public string FechaHora { get; set; }

        public Operation Operacion { get; set; }
        
        // public IEnumerable<Titular> Titulares { get; set; }
    }
}