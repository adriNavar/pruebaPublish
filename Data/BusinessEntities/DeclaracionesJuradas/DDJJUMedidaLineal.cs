using GeoSit.Data.BusinessEntities.Via;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJUMedidaLineal
    {
        public long IdUMedidaLineal { get; set; }
        public long IdClaseParcelaMedidaLineal { get; set; }
        public long? IdVia { get; set; }
        public double? ValorMetros { get; set; }
        public long? NumeroParametro { get; set; }
        public long? IdTramoVia { get; set; }
        public double? ValorAforo { get; set; }
        public long? AlturaCalle { get; set; }

        public string Calle { get; set; }

        public long? IdUFraccion { get; set; }       

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        //public DDJJ DeclaracionJurada { get; set; }

        public VALClasesParcelasMedidaLineal ClaseParcelaMedidaLineal { get; set; }

        public TramoVia Tramo { get; set; }

        public DDJJUFracciones Fraccion { get; set; }
    }
}
