using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.LogicalTransactionUnits
{
    public class UnidadExpedienteObra
    {
        public OperationItem<ExpedienteObra> OperacionExpedienteObra { get; set; }
        public Operaciones<ControlTecnico> OperacionesControlesTecnicos { get; set; }
        public Operaciones<ExpedienteObraDocumento> OperacionesDocumentos { get; set; }
        public Operaciones<DomicilioExpedienteObra> OperacionesDomicilios { get; set; }
        public Operaciones<EstadoExpedienteObra> OperacionesEstados { get; set; }
        public Operaciones<InspeccionExpedienteObra> OperacionesInspecciones { get; set; }
        public Operaciones<Liquidacion> OperacionesLiquidaciones { get; set; }
        public Operaciones<ObservacionExpediente> OperacionesObservaciones { get; set; }
        public Operaciones<PersonaExpedienteObra> OperacionesPersonas { get; set; }
        public Operaciones<ServicioExpedienteObra> OperacionesServicios { get; set; }
        public Operaciones<TipoSuperficieExpedienteObra> OperacionesSuperficies { get; set; }
        public Operaciones<TipoExpedienteObra> OperacionesTipos { get; set; }
        public Operaciones<UnidadTributariaExpedienteObra> OperacionesUnidadesTributarias { get; set; }

        public UnidadExpedienteObra()
        {
            this.OperacionesControlesTecnicos = new Operaciones<ControlTecnico>();
            this.OperacionesDocumentos = new Operaciones<ExpedienteObraDocumento>();
            this.OperacionesDomicilios = new Operaciones<DomicilioExpedienteObra>();
            this.OperacionesEstados = new Operaciones<EstadoExpedienteObra>();
            this.OperacionesInspecciones = new Operaciones<InspeccionExpedienteObra>();
            this.OperacionesLiquidaciones = new Operaciones<Liquidacion>();
            this.OperacionesObservaciones = new Operaciones<ObservacionExpediente>();
            this.OperacionesPersonas = new Operaciones<PersonaExpedienteObra>();
            this.OperacionesServicios = new Operaciones<ServicioExpedienteObra>();
            this.OperacionesSuperficies = new Operaciones<TipoSuperficieExpedienteObra>();
            this.OperacionesTipos = new Operaciones<TipoExpedienteObra>();
            this.OperacionesUnidadesTributarias = new Operaciones<UnidadTributariaExpedienteObra>();
            this.OperacionExpedienteObra = new OperationItem<ExpedienteObra>();
        }
    }
}
