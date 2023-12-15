using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.DAL.Tramites.PartidaGenerators;
using System;
using System.Data.Entity;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class ActualizarUnidadesTributariasDestino : AccionEntrada
    {
        public ActualizarUnidadesTributariasDestino(METramite tramite, GeoSITMContext contexto)
            : base(Convert.ToInt32(Entradas.UnidadTributaria), tramite, contexto) { }

        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            var ut = Contexto.UnidadesTributariasTemporal
                             .Include(u => u.Parcela)
                             .Single(u => u.UnidadTributariaId == entrada.IdObjeto && u.IdTramite == entrada.IdTramite);

            ut.JurisdiccionID = Tramite.IdJurisdiccion;
            ut.FechaVigenciaDesde = ut.FechaVigenciaHasta = null;

            ut.CodigoProvincial = PartidaGeneratorBuilder.GetGenerator(ut, Tramite, Contexto).Generate();
            return;
        }

        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        {
            return from entrada in base.GetEntradas(idEntrada)
                   join relacion in Contexto.TramitesEntradasRelacion on entrada.IdTramiteEntrada equals relacion.IdTramiteEntrada
                   join ut in Contexto.UnidadesTributariasTemporal on entrada.IdObjeto equals ut.UnidadTributariaId
                   orderby ut.TipoUnidadTributariaID, ut.PlanoId
                   select entrada;
        }
    }
}
