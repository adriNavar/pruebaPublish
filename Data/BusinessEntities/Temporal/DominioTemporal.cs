using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DominioTemporal : ITemporalTramite
    {
        public long DominioID { get; set; }
        public long UnidadTributariaID { get; set; }
        public int IdTramite { get; set; }
        public long TipoInscripcionID { get; set; }
        public string Inscripcion { get; set; }
        public DateTime Fecha { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<DominioTitularTemporal> Titulares { get; set; }
    }
}