using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALDecreto
    {
        public long IdDecreto { get; set; }
        public long NroDecreto { get; set; }
        public DateTime AnioDecreto { get; set; }
        public DateTime FechaDecreto { get; set; }
        public double? Coeficiente { get; set; }
        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
        public int? Aplicado { get; set; }    

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<VALValuacionDecreto> ValuacionesDecreto { get; set; }

        public ICollection<VALDecretoZona> Zona { get; set; }
        public ICollection<VALDecretoJurisdiccion> Jurisdiccion { get; set; }



    }
}
