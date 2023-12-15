using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators.Abstract;
using System;

namespace GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators
{
    class PartidaUrbanaGenerator : PartidaGenerator
    {
        public PartidaUrbanaGenerator(UnidadTributariaTemporal ut, METramite tramite, GeoSITMContext contexto)
            : base(ut, tramite, contexto) { }

        public override string Generate()
        {
            return new MesaEntradasRepository(Contexto)
                        .GenerarPartidaInmobiliaria(UnidadTributaria.JurisdiccionID.Value, Convert.ToInt64(TipoParcelaEnum.Urbana));
        }

        public override bool IsValid()
        {
            /* 
               sólo fuerzo generacion de partida de tipo urbana si la UT es UF (tipo 3), que se usan para PH o Conj Inmobiliarios o
               si es la unidad tributaria madre (tipo 2) de un Conjunto Inmobiliario 
            */
            return UnidadTributaria.TipoUnidadTributariaID == Convert.ToInt32(TipoUnidadTributariaEnum.UnidadFuncionalPH) ||
                   UnidadTributaria.TipoUnidadTributariaID == Convert.ToInt32(TipoUnidadTributariaEnum.PropiedaHorizontal) &&
                   UnidadTributaria.Parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.ConjuntoInmobiliario);
        }
    }
}
