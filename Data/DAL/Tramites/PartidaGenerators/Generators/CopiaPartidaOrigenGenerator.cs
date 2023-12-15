using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators.Abstract;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators
{
    class CopiaPartidaOrigenGenerator : PartidaGenerator
    {
        public CopiaPartidaOrigenGenerator(UnidadTributariaTemporal ut, METramite tramite, GeoSITMContext contexto)
           : base(ut, tramite, contexto) { }

        public override string Generate()
        {
            long idEntradaParcela = Convert.ToInt64(Entradas.Parcela);
            long idObjetoEntradaParcela = (from objetoEntrada in Contexto.ObjetosEntrada
                                           where objetoEntrada.IdObjetoTramite == Tramite.IdObjetoTramite &&
                                                 objetoEntrada.FechaBaja == null &&
                                                 objetoEntrada.IdEntrada == idEntradaParcela
                                           select objetoEntrada).Single().IdObjetoEntrada;

            var parcelaDestino = Tramite.TramiteEntradas
                                        .Single(e => e.IdObjeto == UnidadTributaria.ParcelaID && e.IdObjetoEntrada == idObjetoEntradaParcela);

            var relParcelaDestino = Contexto.TramitesEntradasRelacion.Single(rel => rel.IdTramiteEntrada == parcelaDestino.IdTramiteEntrada);

            var entradaParcelaOrigen = Tramite.TramiteEntradas
                                              .Single(e => relParcelaDestino.IdTramiteEntradaPadre == e.IdTramiteEntrada);

            int idUTTipoComun = Convert.ToInt32(TipoUnidadTributariaEnum.Comun);
            var utOrigen = Contexto.ChangeTracker.Entries<UnidadTributariaTemporal>()
                                   .SingleOrDefault(utt => utt.Entity.ParcelaID == entradaParcelaOrigen.IdObjeto.Value &&
                                                           utt.Entity.TipoUnidadTributariaID == idUTTipoComun)
                                   ?.Entity ?? Contexto.UnidadesTributariasTemporal
                                                       .Single(utt => utt.ParcelaID == entradaParcelaOrigen.IdObjeto.Value &&
                                                                      utt.IdTramite == Tramite.IdTramite &&
                                                                      utt.TipoUnidadTributariaID == idUTTipoComun && utt.FechaBaja == null);

            return utOrigen.CodigoProvincial;
        }

        public override bool IsValid()
        {
            /* 
               sólo reutilizo la partida origen cuando la unidad tributaria es la madre (tipo 2) de un PH y
               la parcela es una Propiedad Horizontal
            */
            return UnidadTributaria.TipoUnidadTributariaID == Convert.ToInt32(TipoUnidadTributariaEnum.PropiedaHorizontal) &&
                   UnidadTributaria.Parcela.ClaseParcelaID == Convert.ToInt64(ClasesParcelas.PropiedadHorizontal);
        }
    }
}
