using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class Provincia : IEntity
    {
        public long ProvinciaId { get; set; }
        public string Descripcion { get; set; }
    }
}
