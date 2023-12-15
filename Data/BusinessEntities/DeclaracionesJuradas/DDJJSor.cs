using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Via;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJSor
    {
        public long IdSor { get; set; }
        public long IdDeclaracionJurada { get; set; }
        public long? IdLocalidad { get; set; }
        public long? IdCamino { get; set; }
        public long? DistanciaCamino { get; set; }
        public long? DistanciaLocalidad { get; set; }
        public long? DistanciaEmbarque { get; set; }
        public long? NumeroHabitantes { get; set; }
        public long? IdMensura { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJ DeclaracionJurada { get; set; }

        public ICollection<VALSuperficies> Superficies { get; set; }

        public ICollection<DDJJSorCar> SorCar { get; set; }

        public Via.Via Via { get; set; }

        public Objeto Objeto { get; set; }

        public Mensura Mensuras { get; set; }


    }
}
