using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IFuncionAdicionalRepository
    {
        IEnumerable<FuncionAdicional> GetFuncionAdicionales();

        FuncionAdicional GetFuncionAdicionalById(int idFuncionAdicional);

        void InsertFuncionAdicional(FuncionAdicional funcionAdicional);

        void UpdateFuncionAdicional(FuncionAdicional funcionAdicional);

        void DeleteFuncionAdicional(int id);
    }
}