using System;


namespace GeoSit.Data.BusinessEntities.LogRPI
{
    public class RPITipoOperacion : IEntity
    {
        public int TipoOperacionId { get; set; }
        public string Descripcion { get; set; }
        public string Sentido { get; set; }

        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModifId { get; set; }
        public DateTime FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }

    }
}
