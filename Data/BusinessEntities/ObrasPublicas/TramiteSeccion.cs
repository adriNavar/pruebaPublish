using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class TramiteSeccion : IEntity
    {
        public long Id_Tramite_Seccion { get; set; }
        public long Id_Tramite { get; set; }
        public long Id_Tipo_Seccion { get; set; }
        public string Detalle { get; set; }
        public bool Imprime { get; set; }
        public long Id_Usu_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Id_Usu_Modif { get; set; }
        public DateTime Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }
        public TramiteTipoSeccion TipoSeccion { get; set; }
        public Tramite Tramite { get; set; }
        [NotMapped]
        public bool? Enabled { get; set; }
        [NotMapped]
        public bool? Visualizar { get; set; }
        [NotMapped]
        public bool? Editar { get; set; }
    }
}
