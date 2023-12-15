using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Interfaces;
using GeoSit.Data.DAL.Tramites.Invalid.NoProcesable;
using System;
using System.Linq;
using System.Reflection;

namespace GeoSit.Data.DAL.Tramites
{
    internal class ProcessorTramiteBuilder
    {
        internal static IProcessor GetProcessor(METramite tramite, GeoSITMContext context)
        {
            var types = (from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.IsClass && !t.IsAbstract && t.Namespace == "GeoSit.Data.DAL.Tramites.Processors"
                         select t).ToArray();

            bool found = false;
            int idx = 0;
            IProcessor processor = null;
            while (!found && idx < types.Length)
            {
                processor = Activator.CreateInstance(types[idx++], tramite, context) as IProcessor;
                found = processor.IsTipoTramite();
            }
            if (found)
            {
                processor.Configure();
                return processor;
            }
            //por ahora comento para que no fallen los objetos de trámite no implementados
            //return new TramiteInvalido();
            return new TramiteNoProcesable();
        }
    }
}
