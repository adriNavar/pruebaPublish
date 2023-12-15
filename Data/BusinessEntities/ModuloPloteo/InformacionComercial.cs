using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class InformacionComercial
    {
        public bool BA { get; set; }
        public bool PV { get; set; }
        public bool PH { get; set; }
        public bool AO { get; set; }
        public bool CC { get; set; }
        public bool BC { get; set; }
        public bool DH { get; set; }
        public bool OL { get; set; }
        public bool TI { get; set; }
        public bool ARBA { get; set; }
        public bool DI { get; set; }

        public string TI_Value { get; set; }
    }
}
