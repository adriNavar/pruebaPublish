using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.LogicalTransactionUnits
{
    public class UnidadGeneracionCertificados
    {
        public OperationItem<Tramite> OperacionTramite { get; set; }
        public Operaciones<TramiteDocumento> OperacionesDocumentos { get; set; }
        public Operaciones<TramitePersona> OperacionesPersonas { get; set; }
        public Operaciones<TramiteUnidadTributaria> OperacionesUnidadesTributarias { get; set; }
        public Operaciones<TramiteSeccion> OperacionesSecciones { get; set; }
        public OperationItem<Documento> InformeImpreso { get; set; }
        public UnidadGeneracionCertificados()
        {
            this.OperacionesDocumentos = new Operaciones<TramiteDocumento>();
            this.OperacionesPersonas = new Operaciones<TramitePersona>();
            this.OperacionesUnidadesTributarias = new Operaciones<TramiteUnidadTributaria>();
            this.OperacionesSecciones = new Operaciones<TramiteSeccion>();
        }
    }
}
