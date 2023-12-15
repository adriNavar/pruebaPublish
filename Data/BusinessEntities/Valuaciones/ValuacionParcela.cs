
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValuacionParcela : IEntity
    {
        public long Tipo_Parcela { get; set; }
        public long Id_Inmueble { get; set; }
        public long UnidadTributariaID { get; set; }
        public string Partida { get; set; }
        public string Ejido { get; set; }
        public string Sector { get; set; }
        public string Fraccion { get; set; }
        public string Circunscripcion { get; set; }
        public string Manzana { get; set; }
        public string Parcela { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nomenclatura { get; set; }
    }
  
}

