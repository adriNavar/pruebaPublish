using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Interfaces;

namespace GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators.Abstract
{
    abstract class PartidaGenerator : IPartidaGenerator
    {
        protected METramite Tramite;
        protected GeoSITMContext Contexto;
        protected UnidadTributariaTemporal UnidadTributaria;

        protected PartidaGenerator(UnidadTributariaTemporal ut, METramite tramite, GeoSITMContext contexto)
        {
            UnidadTributaria = ut;
            Tramite = tramite;
            Contexto = contexto;
        }

        public abstract string Generate();

        public virtual bool IsDefault()
        {
            return false;
        }

        public virtual bool IsValid()
        {
            return false;
        }
    }
}
