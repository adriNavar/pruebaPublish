using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class TramiteUnidadTributaria : IEntity
    {
        public long Id_Tramite_Uts { get; set; }
        public long Id_Tramite { get; set; }
        public long Id_Unidad_Tributaria { get; set; }
        public Nullable<long> Id_Usu_Alta { get; set; }
        public Nullable<DateTime> Fecha_Alta { get; set; }
        public Nullable<long> Id_Usu_Modif { get; set; }
        public Nullable<DateTime> Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }

        public Tramite Tramite { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }

    }
}
