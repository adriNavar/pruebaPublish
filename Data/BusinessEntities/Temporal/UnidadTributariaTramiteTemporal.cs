using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class UnidadTributariaTramiteTemporal
    {
        public string Planoid { get; set; }
        public string CodProvincial { get; set; }
        public string Nomenclatura { get; set; }

        public decimal PorcentajeCop { get; set; } //Código de DGCyC corrientes

    }
}
