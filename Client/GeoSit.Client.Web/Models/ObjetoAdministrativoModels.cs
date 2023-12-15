using System;

namespace GeoSit.Client.Web.Models
{
    public class ObjetoAdministrativoModels
    {
        public ObjetoAdministrativoModels()
        {
            ObjAdministrativo = new ObjetoAdministrativoModel();
        }
        public ObjetoAdministrativoModel ObjAdministrativo { get; set; }
    }

    public class ObjetoAdministrativoModel
    {
        public long FeatId { get; set; }
        public long TipoObjetoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Nomenclatura { get; set; }
        public long? ClassId { get; set; }
        public long? RevisionNumber { get; set; }
        public long? ObjetoPadreId { get; set; }
        public long? UsuarioAltaId { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionId { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioBajaId { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string Atributos { get; set; }
    }
}