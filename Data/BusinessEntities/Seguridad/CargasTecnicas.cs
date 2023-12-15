using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class CargasTecnicas : IEntity
    {
        public long Id_Carga_Tecnica { get; set; }
        public long Id_Analisis { get; set; }
        public long Tipo_Carga { get; set; }
        public string Id_Distrito { get; set; }
        public DateTime? Fecha_Desde { get; set; }
        public DateTime? Fecha_Hasta { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime? Fecha_Alta { get; set; }
        public AnalisisTecnicos AnalisisTecnico { get; set; }
    }
}


