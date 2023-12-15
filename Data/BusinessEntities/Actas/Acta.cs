using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.BusinessEntities.Actas
{
    public class Acta : IEntity
    {
        public long ActaId { get; set; }
        public int NroActa { get; set; }
        public long InspectorId { get; set; }
        public int? Plazo { get; set; }
        public DateTime Fecha { get; set; }
        public long ActaTipoId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaId { get; set; }
        public DateTime? FechaBaja { get; set; }

        [NotMapped]
        public long SelectedEstadoActa { get; set; }
        [NotMapped]
        public string selectedUT { get; set; }
        [NotMapped]
        public string SelectedDomicilio { get; set; }
        [NotMapped]
        public string selectedPer { get; set; }
        [NotMapped]
        public string selectedDocs { get; set; }
        [NotMapped]
        public string selectedActasOrigen { get; set; }
        [NotMapped]
        public string selectedOtrosObjetos { get; set; }

        public string observaciones { get; set; }

        public ICollection<ActaUnidadTributaria> ActaUnidadesTributarias { get; set; }
        //public ActaTipo TipoActa { get; set; }
        //public Inspector Inspector { get; set; }
        //public ICollection<EstadoActa> EstadosActa { get; set; }
    }
}
