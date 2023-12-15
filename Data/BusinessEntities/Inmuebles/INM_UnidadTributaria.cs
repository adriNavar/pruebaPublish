using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class INM_UnidadTributaria : IEntity
    {
        public long Id_Unidad_Tributaria { get; set; }
        [ForeignKey("mNomenclatura")]
        public long Id_Parcela { get; set; }
        public string Codigo_Provincial { get; set; }
        public string Codigo_Municipal { get; set; }
        public string Unidad_Funcional { get; set; }
        public float Porcentaje_PH { get; set; }
        public Nullable<long> Id_Usu_Alta { get; set; }
        public Nullable<DateTime> Fecha_Alta { get; set; }
        public Nullable<long> Id_Usu_Modif { get; set; }
        public Nullable<DateTime> Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }

        public INM_Nomenclatura mNomenclatura { get; set; }

        public virtual List<INM_Nomenclatura> mNomenclaturas { get; set; }

    }
}
