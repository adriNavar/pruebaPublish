using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.DAL.Tramites.Processors.Abstract;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Processors
{
    internal class TramiteMensura : TramiteProcessor
    {
        public TramiteMensura(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto, Convert.ToInt32(TiposTramites.TramiteMensura)) { }

        public override bool IsTipoTramite()
        {
            /*por ahora filtro en este lugar los objetos de tramite posibles de procesar*/
            return new[]
            {
                Convert.ToInt32(ObjetosTramites.Mensura),
                Convert.ToInt32(ObjetosTramites.PrescripcionAdquisitiva),
                Convert.ToInt32(ObjetosTramites.PrescripcionAdquisitivaDivision),
                Convert.ToInt32(ObjetosTramites.DivisionMensuraRegistrada),
                Convert.ToInt32(ObjetosTramites.UnificacionMensuraRegistrada),
                Convert.ToInt32(ObjetosTramites.UnificacionYRedistribucionParcelariaMensuraRegistrada),
                Convert.ToInt32(ObjetosTramites.MensuraYDivision),
                Convert.ToInt32(ObjetosTramites.MensuraYUnificacion),
                Convert.ToInt32(ObjetosTramites.MensuraUnificacionYRedistribucionParcelaria),
            }.Contains(_tramite.IdObjetoTramite);
        }
        protected override void ConfigurePreProcessingActions()
        {
            _acciones.AddRange(new Accion[]
            {
                new ValidarActualizacionGrafica(_tramite, _contexto),
                new RegistrarMensura(_tramite, _contexto),
                new ActualizarParcelasOrigen(_tramite, _contexto),
                new ActualizarUnidadesTributariasOrigen(_tramite, _contexto)
            });
        }
        protected override void ConfigurePostProcessingActions()
        {
            _accionesInformes.Add(new GenerarInformeAdjuducacion(_tramite, _contexto));
        }
    }
}
