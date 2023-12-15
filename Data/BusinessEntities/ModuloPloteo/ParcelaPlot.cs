using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class ParcelaPlot
    {
        public long FeatId { get; set; }
        public string Nombre { get; set; }
        public string NroPuerta { get; set; }
        public long IdCuadra { get; set; }
        public string Expediente { get; set; }
        public string NomCatastral { get; set; }
        public int? IdClienteTipo { get; set; }
        public DbGeometry Geom { get; set; }

        public string ParcelaNro { get; set; }

        public double Superficie { get; set; }
        public bool EsBarrioCarenciado { get; set; }
        public bool EsEspacioVerde { get; set; }

        public InformacionComercial InformacionComercial { get; set; }

        //AYSA20180424
        public string Apic_id { get; set; }

    }
}
