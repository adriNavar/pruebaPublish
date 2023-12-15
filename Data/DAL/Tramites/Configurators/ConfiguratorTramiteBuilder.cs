using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Configurators.Default;
using GeoSit.Data.DAL.Tramites.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace GeoSit.Data.DAL.Tramites.Configurators
{
    class ConfiguratorTramiteBuilder
    {
        internal static IConfigurator GetConfigurator(int tipoProcesador, METramite tramite, GeoSITMContext contexto)
        {
            var configurators = (from t in Assembly.GetExecutingAssembly().GetTypes()
                                 where t.IsClass && !t.IsAbstract && !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith("GeoSit.Data.DAL.Tramites.Configurators.ObjetoTramite")
                                 select t)
                                 .Select(t => Activator.CreateInstance(t, tipoProcesador, tramite, contexto) as IConfigurator)
                                 .Where(cfg => cfg.IsConfiguradorTipoTramite())
                                 .ToArray();

            return configurators.SingleOrDefault(cfg => cfg.IsObjetoTramite()) ?? configurators.SingleOrDefault(cfg => cfg.IsConfiguradorTramiteDefault()) ?? new DefaultConfigurator();
        }
    }
}
