using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class Valuacion : IEntity
    {
        public long ValuacionID { get; set; }
        public long UnidadTributariaID { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public Decimal ValorTierra { get; set; }
        public Decimal? ValorMejoras { get; set; }
        public long? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; } 

        [NotMapped]
        public Decimal? ValorFiscalTotal { get; set; }
    }
}
