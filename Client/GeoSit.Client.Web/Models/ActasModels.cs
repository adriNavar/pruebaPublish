using GeoSit.Client.Web.Models.Inspecciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Models
{
    public class ActaModel
    {
        public List<InspectorModel> Inspectores { get; set; }
        //public List<ActaTipo> ActasTipos { get; set; }
        public ICollection<SelectListItem> CmbActasTipos { get; set; }
        public ICollection<SelectListItem> CmbInspectores { get; set; }
        public ICollection<SelectListItem> CmbEstadosActas { get; set; }

        public string SelectedEstadoActa { get; set; }
        public string SelectedTipoActa { get; set; }
        public string SelectedInspector { get; set; }
    }

    public class ActaBusqueda
    {
        public int buscaFecha { get; set; }
        public int buscaNumero { get; set; }
        public int buscaInspectores { get; set; }
        public int buscaId { get; set; }
        public int buscaUnidad { get; set; }
        public int buscaEstado { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public int numeroDesde { get; set; }
        public int numeroHasta { get; set; }
        public int idActa { get; set; }
        public int idUnidad { get; set; }
        public int idEstado { get; set; }
        public string selectedInspectoresBusqueda { get; set; }
    }
}