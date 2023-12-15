using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IFuncAdicParametroRepository
    {
        IEnumerable<FuncAdicParametro> GetFuncAdicParametros();

        FuncAdicParametro GetFuncAdicParametroById(int idFuncAdicParametro);

        void InsertFuncAdicParametro(FuncAdicParametro funcAdicParametro);

        void UpdateFuncAdicParametro(FuncAdicParametro funcAdicParametro);

        void DeleteFuncAdicParametro(int id);
    }
}
