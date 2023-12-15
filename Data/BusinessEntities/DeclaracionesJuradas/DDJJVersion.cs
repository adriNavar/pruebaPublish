using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJVersion
    {
        public long IdVersion { get; set; }
        public string TipoDeclaracionJurada { get; set; }
        public decimal VersionDeclaracionJurada { get; set; }

        public decimal Habilitado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public long? UsuarioAlta { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<VALAptitudes> Aptitudes { get; set; }

        public ICollection<DDJJUOtrasCar> OtrasCarsU { get; set; }
        
        public ICollection<DDJJSorOtrasCar> OtrasCarsSor { get; set; }


    }
}
