using GeoSit.Data.BusinessEntities.MesaEntradas;
using System;

namespace GeoSit.Data.DAL.Repositories
{
    public class RelacionDto
    {
        public Guid Guid { get; set; }
        public long? IdTramiteEntrada { get; set; }
        public METramiteEntrada TramiteEntrada { get; set; }
    }
}