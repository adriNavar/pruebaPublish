using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class NomenclaturaOnly : IEntity
    {
        public long NomenclaturaID { get; set; }
        public long ParcelaID { get; set; }
        public string Nombre { get; set; }
        public int TipoNomenclaturaID { get; set; }
        public int? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}
