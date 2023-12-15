using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class FuncionAdicional
    {
        public int IdFuncionAdicional { get; set; }

        public string Nombre { get; set; }

        [JsonIgnore]
        public ICollection<Plantilla> Plantillas { get; set; }

        public ICollection<FuncAdicParametro> FuncAdicParametros { get; set; }
    }
}
