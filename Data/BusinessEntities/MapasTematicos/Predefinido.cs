using GeoSit.Data.BusinessEntities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class Predefinido
    {
        public long IdPredefinido { get; set; }
        public string Descripcion { get; set; }
        public long? ConfiguracionId { get; set; }
        public string NombreColeccion { get; set; }
        public int LeyendaConfig { get; set; }
        public int BorraRepetidos { get; set; }
        public int IdPlantillaCategoria { get; set; }
    }
}
