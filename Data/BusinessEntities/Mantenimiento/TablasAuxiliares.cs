
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Mantenimiento
{
    public class TablasAuxiliares : IEntity
    {
        public List<string> ValoresTablas { get; set; }

        public List<string> CamposTablas { get; set; }
        public string ComponentesId { get; set; }
        public string TablaID { get; set; }
        public long Id_Usuario { get; set; }
    }
}


