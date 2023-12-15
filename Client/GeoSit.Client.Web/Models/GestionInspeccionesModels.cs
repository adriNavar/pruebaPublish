using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.Inspecciones
{
    public class GestionInspeccionesModel
    {
        public ICollection<SelectListItem> CmbTipoInspectores { get; set; }
        public ICollection<SelectListItem> CmbInspectores { get; set; }
        public ICollection<SelectListItem> CmbInspectoresActivos { get; set; }
        public ICollection<SelectListItem> CmbObjetos { get; set; }
        public ICollection<SelectListItem> CmbTipos { get; set; }
        public SelectList CmbEstados { get; set; }

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
        public int SelectedEstado { get; set; }
        public int SelectedEstadoHidden { get; set; }
        public string Documentos { get; set; }
        public string ResultadoInspeccion { get; set; }

        public string selectedUT { get; set; }

        public long usuario { get; set; }
        public string Planificador { get; set; }

        public List<InspeccionUnidadesTributarias> InspeccionUnidadeTributarias { get; set; }
        
    }

    
    public class TipoInspeccionModel
    {
        public long TipoInspeccionID { get; set; }
        public string Descripcion { get; set; }
    }

    public class EstadoInspeccionModel
    {
        public long EstadoInspeccionID { get; set; }
        public string Descripcion { get; set; }
    }

    public class InspectorModel
    {
        public long InspectorID { get; set; }
        public string EsPlanificador { get; set; }
        public long UsuarioID { get; set; }
        public virtual UsuariosModel Usuario { get; set; }

        public List<TipoInspeccionModel> TiposInspeccion { get; set; }

        public InspectorModel()
        {
            TiposInspeccion = new List<TipoInspeccionModel>();
        }

        public long UsuarioUpdate { get; set; }
        public string TiposInspeccionSelected { get; set; }

        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

    }

    public class InspectorTipoInspeccion
    {
        public long InspectorTipoInspeccionID { get; set; }
        public long InspectorID { get; set; }
        public long TipoInspeccionID { get; set; }

        public string Descripcion { get; set; }

        
    }

    public class CalendarioModel
    {
        public string success { get; set; }
        public List<EventoModel> result { get; set; }
    }
    public class EventoModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string cssclass { get; set; }
        public string start { get; set; }
        public string end { get; set; }
    }

    public class InspeccionModel
    {
        public long InspeccionID { get; set; }
        public long InspectorID { get; set; }
        public long TipoInspeccionID { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public DateTime FechaHoraInicioOriginal { get; set; }
        public DateTime FechaHoraFinOriginal { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        public TipoInspeccionModel TipoInspeccion { get; set; }
        public InspectorModel Inspector { get; set; }

        public int? Objeto { get; set; }
        public int? Tipo { get; set; }
        public string Identificador { get; set; }

        public int SelectedEstado { get; set; }
        public DateTime? FechaHoraDeInspeccion { get; set; }
        public string ResultadoInspeccion { get; set; }

        public long UsuarioUpdate { get; set; }

        public string selectedUT { get; set; }
        public string selectedDocs { get; set; }

        public List<InspeccionUnidadesTributarias> InspeccionUnidadeTributarias { get; set; }
        public List<EstadoInspeccionModel> EstadosInspeccion { get; set; }

        public string EsPlanificador { get; set; }

        public List<InspeccionDocumento> InspeccionDocumento { get; set; }
    }

    public class InspeccionUnidadesTributarias
    {
        public long Id { get; set; }
        public long InspeccionID { get; set; }
        public long UnidadTributariaId { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }
    }

    public class InspeccionDocumento
    {
        public long Id { get; set; }
        public long InspeccionID { get; set; }
        public long id_documento { get; set; }
        public GeoSit.Data.BusinessEntities.Documentos.Documento documento { get; set; }
    }

    public class UnidadTributaria
    {
        public long UnidadTributariaId { get; set; }
        public string CodigoMunicipal { get; set; }
        public string CodigoProvincial { get; set; }
    }

    public class AdministracionInspectoresModel
    {
        public List<InspectorModel> Inspectores { get; set; }
        public List<TipoInspeccionModel> TiposInspecciones { get; set; }
        public string SelectedUsuario { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> cmbUsuarios { get; set; }
        public string idInspector { get; set; }
        public string TiposInspeccionesSelected { get; set; }

        
        
        public AdministracionInspectoresModel()
        {
            Inspectores = new List<InspectorModel>();
        }
    }

    public class InformeInspeccionModel
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public long[] Inspectores { get; set; }
        public int[] Tipos { get; set; }
    }
}