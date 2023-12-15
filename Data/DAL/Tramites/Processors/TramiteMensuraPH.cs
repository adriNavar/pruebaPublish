using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.DAL.Tramites.Processors.Abstract;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Processors
{
    internal class TramiteMensuraPH : TramiteProcessor
    {
        public TramiteMensuraPH(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto, Convert.ToInt32(TiposTramites.TramiteMensura)) { }

        public override bool IsTipoTramite()
        {
            return new[]
            {
                Convert.ToInt32(ObjetosTramites.DivisionEdificacionExistenteAConstruirRegimenPHMensuraRegistrada),
                Convert.ToInt32(ObjetosTramites.DivisionEdificacionExistenteRegimenPHMensuraRegistrada),
            }.Contains(_tramite.IdObjetoTramite);
        }
        protected override void ConfigurePreProcessingActions()
        {
            _acciones.AddRange(new Accion[]
            {
                new RegistrarMensura(_tramite, _contexto),
                new ActualizarParcelasOrigen(_tramite, _contexto),
                new ActualizarUnidadesTributariasOrigen(_tramite, _contexto),
                new DuplicarGraficoParcelaOrigen(_tramite, _contexto),
            });
        }
        protected override void ConfigurePostProcessingActions()
        {
            _accionesInformes.Add(new GenerarInformeAdjuducacion(_tramite, _contexto));
        }
    }
}
