using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    [Serializable]
    public class SubtipoObjetoInfraestructura : IEntity
    {
        public long ID_Subtipo_Objeto { get; set; }
        public long ID_Tipo_Objeto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Esquema { get; set; }
        public string Layer { get; set; }
        public int Clase { get; set; }

        public virtual ICollection<ObjetoInfraestructura> ObjetosInfraestructura { get; set; }
        public virtual TipoObjetoInfraestructura TipoObjeto { get; set; }
    }
}
