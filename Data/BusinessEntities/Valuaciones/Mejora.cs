using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class Mejora : IEntity
    {		
        public long MejoraID { get; set; }
        public long? UnidadTributariaID { get; set; }
        public long ParcelaID { get; set; }
        public long SubCategoriaID { get; set; }
        public String  Parametro2 { get; set; }
        public String UnidadMedida { get; set; }
        public String Parametro1 { get; set; }
        public Decimal Medida { get; set; }
        public Decimal MedidaSemiCubierta { get; set; } 
        public int? Anio { get; set; } 

        public Parcela Parcela { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; } 
    }
}
