using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators.Abstract;
using System;

namespace GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators
{
    class NullPartidaGenerator : PartidaGenerator
    {
        public NullPartidaGenerator(UnidadTributariaTemporal ut, METramite tramite, GeoSITMContext contexto)
            : base(ut, tramite, contexto) { }

        public override string Generate()
        {
            return null;
        }

        public override bool IsValid()
        {
            /* 
               Es parcela proyecto o es subparcela de trámites de Cementerios Privados.
               
               NOTA: Cementerios Privados no está implementado como trámite automatizado,
                     por lo que la segunda condición no debería cumplirse nunca....
            
               pero ya me la veo venir -_-'
             */
            return UnidadTributaria.Parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.Proyecto) ||
                        UnidadTributaria.Parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.SubParcela) &&
                        (Tramite.IdObjetoTramite == Convert.ToInt32(ObjetosTramites.MensuraYDivisionRegimenConjuntoInmobiliarioCementerioPrivado) ||
                        Tramite.IdObjetoTramite == Convert.ToInt32(ObjetosTramites.DivisionRegimenConjuntoInmobiliarioCementerioPrivadoMensuraRegistrada));
        }
    }
}
