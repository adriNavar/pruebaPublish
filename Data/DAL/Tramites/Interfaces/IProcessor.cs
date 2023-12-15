using System.Collections.Generic;

namespace GeoSit.Data.DAL.Tramites.Interfaces
{
    interface IProcessor
    {
        bool IsTipoTramite();
        void Process();
        void Configure();
        void Configure(IEnumerable<IAction> actions);
    }
}
