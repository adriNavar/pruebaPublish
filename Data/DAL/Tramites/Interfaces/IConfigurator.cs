namespace GeoSit.Data.DAL.Tramites.Interfaces
{
    interface IConfigurator
    {
        bool IsConfiguradorTipoTramite();
        bool IsConfiguradorTramiteDefault();
        bool IsObjetoTramite();
        void Configure(IProcessor processor);
    }
}
