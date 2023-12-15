using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class Objeto : IEntity, IBajaLogica
    {
        public long FeatId { get; set; }
        public long TipoObjetoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Atributos { get; set; }
        public string Nomenclatura { get; set; }
        public long? ObjetoPadreId { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long? UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public TipoObjeto TipoObjeto { get; set; }
        public Objeto Padre { get; set; }
    }
}
