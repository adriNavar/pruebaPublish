using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions;
using GeoSit.Data.DAL.Tramites.Configurators.ObjetoTramite.Abstract;
using GeoSit.Data.DAL.Tramites.Interfaces;

namespace GeoSit.Data.DAL.Tramites.Configurators.ObjetoTramite.Mensura
{
    class MensuraDefault : ObjetoTramiteConfigurator
    {
        public MensuraDefault(int tipoTramite, METramite tramite, GeoSITMContext contexto)
            : base(tramite, tipoTramite, tramite.IdObjetoTramite,
                  new IAction[] 
                  {
                      new ActualizarParcelasDestino(tramite, contexto),
                      new ActualizarUnidadesTributariasDestino(tramite, contexto),
                      new ProcesarDeclaracionesJuradas(tramite, contexto),
                      new EliminarParcelasOrigen(tramite, contexto)
                  }) { }

        public override bool IsConfiguradorTramiteDefault()
        {
            return true;
        }
    }
}
