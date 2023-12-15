using GeoSit.Data.DAL.Tramites.Interfaces;

namespace GeoSit.Data.DAL.Tramites.Configurators.Default
{
    class DefaultConfigurator : IConfigurator
    {
        public void Configure(IProcessor processor)
        {
            processor.Configure(new IAction[0]);
        }

        public bool IsConfiguradorTipoTramite()
        {
            return false;
        }
        public bool IsConfiguradorTramiteDefault()
        {
            return false;
        }
        public bool IsObjetoTramite()
        {
            return false;
        }
    }
}
