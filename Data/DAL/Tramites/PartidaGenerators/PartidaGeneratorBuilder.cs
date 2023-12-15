using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GeoSit.Data.DAL.Tramites.PartidaGenerators
{
    class PartidaGeneratorBuilder
    {
        internal static IPartidaGenerator GetGenerator(UnidadTributariaTemporal ut, METramite tramite, GeoSITMContext contexto)
        {
            var generators = (from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.IsClass && !t.IsAbstract && !t.IsNested &&
                               t.Namespace == "GeoSit.Data.DAL.Tramites.PartidaGenerators.Generators"
                         select t)
                         .Select(t=> Activator.CreateInstance(t, ut, tramite, contexto) as IPartidaGenerator)
                         .ToArray();

            return generators.SingleOrDefault(gen => gen.IsValid()) ?? generators.Single(gen => gen.IsDefault());
        }
    }
}
