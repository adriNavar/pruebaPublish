using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.LogicalTransactionUnits
{    
    public class UnidadMantenimientoParcelario
    {
        public Operaciones<UnidadTributaria> OperacionesUnidadTributaria { get; set; }

        public Operaciones<UnidadTributariaDomicilio> OperacionesUnidadTributariaDomicilio { get; set; }

        public Operaciones<Nomenclatura> OperacionesNomenclatura { get; set; }

        public Operaciones<ParcelaDocumento> OperacionesParcelaDocumento { get; set; }

        public Operaciones<UnidadTributariaDocumento> OperacionesUnidadTributariaDocumento { get; set; }

        public Operaciones<Domicilio> OperacionesDomicilio { get; set; }

        public Operaciones<Parcela> OperacionesParcela { get; set; } 

        public Operaciones<UnidadTributariaPersona> OperacionesUnidadTributariaPersona { get; set; }

        public Operaciones<Dominio> OperacionesDominio { get; set; }

        public Operaciones<DominioTitular> OperacionesDominioTitular { get; set; }
        
        public Operaciones<Designacion> OperacionesDesignaciones { get; set; }
        public Operaciones<ParcelaOperacion> OperacionesParcelaOrigen { get; set; }
        public Operaciones<VIRInmueble> OperacionesVIR { get; set; }

        public UnidadMantenimientoParcelario()
        {
            OperacionesUnidadTributaria = new Operaciones<UnidadTributaria>();
            OperacionesUnidadTributariaDomicilio = new Operaciones<UnidadTributariaDomicilio>();
            OperacionesNomenclatura = new Operaciones<Nomenclatura>();
            OperacionesParcelaDocumento = new Operaciones<ParcelaDocumento>();
            OperacionesUnidadTributariaDocumento = new Operaciones<UnidadTributariaDocumento>();
            OperacionesDomicilio = new Operaciones<Domicilio>();
            OperacionesParcela = new Operaciones<Parcela>();
            OperacionesUnidadTributariaPersona = new Operaciones<UnidadTributariaPersona>();
            OperacionesDominio = new Operaciones<Dominio>();
            OperacionesDominioTitular = new Operaciones<DominioTitular>();
            OperacionesDesignaciones = new Operaciones<Designacion>();
            OperacionesParcelaOrigen = new Operaciones<ParcelaOperacion>();
            OperacionesVIR = new Operaciones<VIRInmueble>();
        }
    }
}
