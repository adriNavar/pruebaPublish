using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Personas
{
    public class Profesion : IEntity
    {
        public long PersonaId { get; set; }
        public long TipoProfesionId { get; set; }
        public string Matricula { get; set; }
    }
}
