using GeoSit.Data.BusinessEntities.Documentos;
using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class PartidaSecuencia : IEntity
    {
        public long Jurisdiccion { get; set; }
        public long TipoParcela { get; set; }
        public long Valor { get; set; }

    }
}
