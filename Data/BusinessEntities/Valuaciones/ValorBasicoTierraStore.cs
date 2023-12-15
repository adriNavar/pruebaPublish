
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValorBasicoTierraStore : IEntity
    {	
        public long IdValorBasicoTierraStore { get; set; }
        public long? IdTipoValuacion { get; set; }
        public String IdFiltroParcela { get; set; }
        public String Comparador1 { get; set; }
        public String Parametro1Desde { get; set; }
        public String Parametro1Hasta { get; set; }
        public String Comparador2 { get; set; }
        public String Parametro2Desde { get; set; }
        public String Parametro2Hasta { get; set; }
        public float?  Valor { get; set; }
        public long? Usuario_Alta { get; set; }
        public DateTime? Fecha_Alta { get; set; }
        public long? Usuario_Modificacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        

    }
}