using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    //[Serializable]
    public class TipoObjetoInfraestructura : IEntity
    {
        public long ID_Tipo_Objeto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
