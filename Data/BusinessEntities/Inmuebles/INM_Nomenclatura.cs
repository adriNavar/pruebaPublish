using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class INM_Nomenclatura : IEntity
    {
        public long Id_Nomenclatura { get; set; }
        [ForeignKey("mUnidadTributaria")]
        public long Id_Parcela { get; set; }
        public string Nomenclatura { get; set; }
        public long Id_Tipo_Nomenclatura { get; set; }
        public Nullable<long> Id_Usu_Alta { get; set; }
        public Nullable<DateTime> Fecha_Alta { get; set; }
        public Nullable<long> Id_Usu_Modif { get; set; }
        public Nullable<DateTime> Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }

        public INM_UnidadTributaria mUnidadTributaria { get; set; }
    }
}
