using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class Inspeccion : IEntity
    {
        public long InspeccionID { get; set; }
        public long InspectorID { get; set; }
        public int TipoInspeccionID { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public DateTime FechaHoraInicioOriginal { get; set; }
        public DateTime FechaHoraFinOriginal { get; set; }
        public int UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        public TipoInspeccion TipoInspeccion { get; set; }
        public EstadoInspeccion EstadoInspeccion { get; set; }
        public Inspector Inspector { get; set; }

        public int? Objeto { get; set; }
        public int? Tipo { get; set; }
        public string Identificador { get; set; }

        public int SelectedEstado { get; set; }
        public DateTime? FechaHoraDeInspeccion { get; set; }
        public string ResultadoInspeccion { get; set; }

        [NotMapped]
        public int UsuarioUpdate { get; set; }
        [NotMapped]
        public string selectedUT { get; set; }
        [NotMapped]
        public string selectedDocs { get; set; }

        public ICollection<InspeccionExpedienteObra> InspeccionExpedienteObras { get; set; }
    }
}
