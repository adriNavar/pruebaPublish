using GeoSit.Data.BusinessEntities.Documentos;
using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class MensuraSecuencia : IEntity
    {
        public long Departamento { get; set; }
        public string LetraMensura { get; set; }
        public long Valor { get; set; }

    }
}
