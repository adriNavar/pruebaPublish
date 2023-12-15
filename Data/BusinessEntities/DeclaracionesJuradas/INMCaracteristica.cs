using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class INMCaracteristica
    {
        public long IdCaracteristica { get; set; }
        public long IdTipoCaracteristica { get; set; }
        public long IdInciso { get; set; }
        public string Descripcion { get; set; }
        public long? Numero { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public INMTipoCaracteristica TipoCaracteristica { get; set; }
        public INMInciso Inciso { get; set; }

        public ICollection<INMMejoraCaracteristica> MejoraCaracteristica { get; set; }


        // Workaround 
        public string IdCaracteristicaString {
            get 
            {
                if (this.IdCaracteristica == -1)
                {
                    return string.Empty;
                }

                return this.IdCaracteristica.ToString();
            
            }
        }

    }
}
