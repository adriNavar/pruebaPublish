using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
 
    public class TipoValuacion : IEntity
    {
        public long TipoValuacionID { get; set; }
        public String Descripcion { get; set; }
        public String IdFiltroParcela { get; set; }
        public String MetodoValuacion { get; set; }
        public String Parametro1 { get; set; }
        public String TipoParametro1 { get; set; }
        public String NombreParametro1 { get; set; }
        public String Parametro2 { get; set; }
        public String TipoParametro2 { get; set; }
        public String NombreParametro2 { get; set; }
        public String Destino { get; set; }
    }
}
