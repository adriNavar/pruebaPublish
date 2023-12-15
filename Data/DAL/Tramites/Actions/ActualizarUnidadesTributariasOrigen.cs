using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class ActualizarUnidadesTributariasOrigen : AccionEntrada
    {
        private MensuraTemporal _nuevaMensura;
        public ActualizarUnidadesTributariasOrigen(METramite tramite, GeoSITMContext contexto)
            : base(Convert.ToInt32(Entradas.UnidadTributaria), tramite, contexto) { }

        public override bool Execute()
        {
            _nuevaMensura = Contexto.ChangeTracker.Entries<MensuraTemporal>().Single().Entity;
            return base.Execute();
        }
        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            var unidadTibutaria = Contexto.ChangeTracker.Entries<UnidadTributariaTemporal>().SingleOrDefault(ut => ut.Entity.UnidadTributariaId == entrada.IdObjeto)?.Entity
                                    ?? Contexto.UnidadesTributariasTemporal.Single(ut => ut.UnidadTributariaId == entrada.IdObjeto && ut.IdTramite == Tramite.IdTramite);
            Contexto.ParcelaMensurasTemporal.Add(new ParcelaMensuraTemporal()
            {
                IdParcela = unidadTibutaria.ParcelaID.Value,
                Mensura = _nuevaMensura,

                FechaAlta = _nuevaMensura.FechaAlta,
                FechaModif = _nuevaMensura.FechaAlta,
                IdUsuarioAlta = _nuevaMensura.IdUsuarioAlta,
                IdUsuarioModif = _nuevaMensura.IdUsuarioAlta
            });
            return;
        }

        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        { //recupero todas las entradas de tipo parcela, que sean Origen (no existen como hijas de ninguna otra entrada objeto)
            return from entrada in base.GetEntradas(idEntrada)
                   where !Contexto.TramitesEntradasRelacion.Any(rel => rel.IdTramiteEntrada == entrada.IdTramiteEntrada)
                   select entrada;
        }
    }
}
