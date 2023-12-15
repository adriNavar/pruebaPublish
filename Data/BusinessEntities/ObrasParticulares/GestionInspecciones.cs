using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class GestionInspeccionesModel : IEntity
    {
        public string ReturnUrl { get; set; }

        /* Calendario */
        public string SelectedTipoInspeccionCal { get; set; }
        public string SelectedInspectorCal { get; set; }

        /* Planificacion Inspeccion */
        public string InspeccionId { get; set; }
        public string SelectedTipoInspeccion { get; set; }
        public string Inspector { get; set; }
        public string FechaHoraDesde { get; set; }
        public string FechaHoraHasta { get; set; }
        public string SelectedInspector { get; set; }
        public string FechaOrigenDesde { get; set; }
        public string FechaOrigenHasta { get; set; }
        public string UnidadesTributarias { get; set; }
        public string Descripcion { get; set; }
        public string SelectedObjeto { get; set; }
        public string SelectedTipo { get; set; }
        public string Identificador { get; set; }

        /* Resultado Inspeccion */
        public string FechaHoraDeInspeccion { get; set; }
        public string SelectedEstado { get; set; }
        public string Documentos { get; set; }
        public string ResultadoInspeccion { get; set; }

        public int usuario { get; set; }
    }
}
