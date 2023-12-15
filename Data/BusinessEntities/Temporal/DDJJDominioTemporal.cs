using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJDominioTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdDominio { get; set; }
        public long IdDeclaracionJurada { get; set; }
        public int IdTramite { get; set; }
        public long IdTipoInscripcion { get; set; }
        public string Inscripcion { get; set; }
        public DateTime Fecha { get; set; }
        
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }       

        public ICollection<DDJJDominioTitularTemporal> Titulares { get; set; }
        public TipoInscripcion TipoInscripcion { get; set; }

    }
}
