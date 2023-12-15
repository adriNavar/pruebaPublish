using System;

namespace GeoSit.Client.Web.Models
{
    public class DivisionModels
    {
        public DivisionModels()
        {
            DatoDivision = new DivisionModel();
        }
        public DivisionModel DatoDivision { get; set; }
    }

    public class DivisionModel
    {
        public long FeatId { get; set; }
        public long TipoObjetoId { get; set; }
        public long TipoDivisionId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Nomenclatura { get; set; }
        public int ClassId { get; set; }
        public int RevisionNumber { get; set; }
        public long ObjetoPadreId { get; set; }
        public int UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacionId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBajaId { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}