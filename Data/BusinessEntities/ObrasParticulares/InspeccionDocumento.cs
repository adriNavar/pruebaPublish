using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class InspeccionDocumento : IEntity
    {
        public long Id { get; set; }
        public long InspeccionID { get; set; }
        public long id_documento { get; set; }
        public Documento documento { get; set; }
    }
}
