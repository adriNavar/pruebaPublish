using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Web.Api.Models
{
    public class ClaseParcela
    {
        public ClaseParcela()
        {
            TiposMedidasLineales = new List<TipoMedidaLinealConParcela>();
        }

        public long IdClaseParcela { get; set; }
        public string Descripcion { get; set; }

        public List<TipoMedidaLinealConParcela> TiposMedidasLineales { get; set; }

    }

    public class TipoMedidaLineal
    {
        public long IdTipoMedidaLineal { get; set; }

        public string Descripcion { get; set; }

        public long? Orden { get; set; }


    }

    public class TipoMedidaLinealConParcela : TipoMedidaLineal, IAforo
    {

        public long IdClasesParcelasMedidaLineal { get; set; }

        public bool RequiereLongitud { get; set; }

        public bool RequiereAforo { get; set; }
       

        // Nuevo para la UI

        public double? ValorMetros { get; set; }
        public double? ValorAforo { get; set; }

        public string Calle { get; set; }
        public string Desde { get; set; }
        public string Hasta { get; set; }
        public string Paridad { get; set; }
        public long? IdVia { get; set; }
        public long? IdTramoVia { get; set; }
        public string Altura { get; set; }
    }


    public class Aforo : IAforo
    {
        public long? IdVia { get; set; }

        public long? IdTramoVia { get; set; }

        public double? ValorAforo { get; set; }
        
        public string Calle { get; set; }

        public string Altura { get; set; }

        public string Desde { get; set; }
        
        public string Hasta { get; set; }
        
        public string Paridad { get; set; }
    
    }


    public interface IAforo
    {
        long? IdVia { get; set; }

        long? IdTramoVia { get; set; }

        double? ValorAforo { get; set; }

        string Calle { get; set; }

        string Altura { get; set; }

        string Desde { get; set; }

        string Hasta { get; set; }

        string Paridad { get; set; }

    }



}