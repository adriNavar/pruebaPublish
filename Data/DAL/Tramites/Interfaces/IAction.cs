using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Enums;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Tramites.Interfaces
{
    interface IAction
    {
        Ambito Ambito { get; }
        ResultadoValidacion Resultado { get; }
        List<string> Errores { get; }
        bool Execute();
    }
}
