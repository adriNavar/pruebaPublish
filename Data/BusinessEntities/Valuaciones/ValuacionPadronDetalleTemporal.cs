
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Valuaciones
{
    public class ValuacionPadronDetalleTemporal : IEntity
    {
        public long IdPadronDetalle { get; set; }
        public long IdPadron { get; set; }
        public long IdParcela { get; set; }
        public long IdUnidadTributaria { get; set; }
        public long? IdAtributoZona { get; set; }
        public String TipoParcela { get; set; }

        public String PartidaProvincial { get; set; }
        public String PartidaMunicipal { get; set; }

        public String DomicilioInmueble { get; set; }
        public String DomicilioFiscal { get; set; }
        public float PorcentajeCodominio { get; set; }
        public String Titular { get; set; }
        public String ResponsableFiscal { get; set; }
        public float SuperficieTierra { get; set; }
        public float SuperificeCubierta { get; set; }
        public float SuperficieSemiCubierta { get; set; }
        public String Uso { get; set; }
        public long anioConstruccion { get; set; }
        public Decimal ValorTierra { get; set; }
        public Decimal ValorMejora { get; set; }
        public Decimal ValorTotal { get; set; }
        public long? Usuario_Alta { get; set; }
        public DateTime? Fecha_Alta { get; set; }
        public long? Usuario_Modificacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public Decimal? ValorTierraPropiedad { get; set; }
        public Decimal? ValorMejoraPropiedad { get; set; }
   
    }
		
}

