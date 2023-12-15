using GeoSit.Data.BusinessEntities.MapasTematicos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GeoSit.Data.BusinessEntities.Redes
{
    public class Relacion : IEntity
    {
        public long Id { get; set; }
        public string TablaPadre { get; set; }
        public long? IdTablaPadre { get; set; }
        public string TablaHijo { get; set; }
        public long? IdTablaHijo { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public virtual string FechaModifString
        {
            get
            {
                return FechaAlta.ToString("dd/MM/yyyy");
            }
        }

        public string Codigo { get; set; }
        
        public Componente Componente { get; set; }
    }
}
