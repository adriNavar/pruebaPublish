using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJ
    {
        public long IdDeclaracionJurada { get; set; }
        public long IdVersion { get; set; }
        public long IdOrigen { get; set; }
        public long IdUnidadTributaria { get; set; }
        public string IdPoligono { get; set; }
        public DateTime? FechaVigencia { get; set; }
        public long? IdTramiteObjeto { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<DDJJSor> Sor { get; set; }
        public ICollection<DDJJU> U { get; set; }
        public ICollection<INMMejora> Mejora { get; set; }


        public ICollection<DDJJDesignacion> Designacion { get; set; }
        public ICollection<DDJJDominio> Dominios { get; set; }
        public ICollection<VALValuacion> Valuaciones { get; set; }


        public DDJJOrigen Origen { get; set; }
        public DDJJVersion Version { get; set; }
    }
}
