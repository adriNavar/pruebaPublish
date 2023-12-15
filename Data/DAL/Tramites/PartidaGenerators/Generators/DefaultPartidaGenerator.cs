using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators.Abstract;

namespace GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators
{
    class DefaultPartidaGenerator : PartidaGenerator
    {
        public DefaultPartidaGenerator(UnidadTributariaTemporal ut, METramite tramite, GeoSITMContext contexto)
            : base(ut, tramite, contexto) { }

        public override string Generate()
        {
            return new MesaEntradasRepository(Contexto)
                        .GenerarPartidaInmobiliaria(UnidadTributaria.JurisdiccionID.Value, UnidadTributaria.Parcela.TipoParcelaID);
        }

        public override bool IsDefault()
        {
            return true;
        }
    }
}
