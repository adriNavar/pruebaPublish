using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Personas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJDominioTitular
    {
        public long IdDominioTitular { get; set; }
        public long IdPersona { get; set; }
        public short IdTipoTitularidad { get; set; }
        public long IdDominio { get; set; }
        public decimal PorcientoCopropiedad { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJDominio Dominio { get; set; }
        public Persona Persona { get; set; }

        public ICollection<DDJJPersonaDomicilio> PersonaDomicilio { get; set; }

        public string NombreCompleto { get; set; }
        public string TipoNoDocumento { get; set; }
        public string TipoTitularidad { get; set; }

        public TipoTitularidad TT { get; set; }

    }
}
