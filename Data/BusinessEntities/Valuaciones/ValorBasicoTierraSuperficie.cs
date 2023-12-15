
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValorBasicoTierraSuperficie : IEntity
    {
        public long IdValorBasicoTierraSuperficie { get; set; }
        public long IdTipoValorBasicoTierra { get; set; }
        public long IdAtributoZona { get; set; }
        public long? IdTipoParcela { get; set; }
        public Decimal Valor { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Modificacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public String Parametro1 { get; set; }
        public String Parametro2 { get; set; }
    }
}
