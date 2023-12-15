using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class FuncionAdicionalRepository : IFuncionAdicionalRepository
    {
        private readonly GeoSITMContext _context;
        private readonly IFuncAdicParametroRepository _funcAdicParametroRepository;

        public FuncionAdicionalRepository(GeoSITMContext context, IFuncAdicParametroRepository funcAdicParametroRepository)
        {
            _context = context;
            _funcAdicParametroRepository = funcAdicParametroRepository;
        }

        public IEnumerable<FuncionAdicional> GetFuncionAdicionales()
        {
            return _context.FuncionAdicionales.ToList();
        }

        public FuncionAdicional GetFuncionAdicionalById(int idFuncionAdicional)
        {
            return _context.FuncionAdicionales.Find(idFuncionAdicional);
        }

        public void InsertFuncionAdicional(FuncionAdicional funcionAdicional)
        {
            _context.FuncionAdicionales.Add(funcionAdicional);
        }

        public void UpdateFuncionAdicional(FuncionAdicional funcionAdicional)
        {
            _context.Entry(funcionAdicional).State = EntityState.Modified;
        }

        public void DeleteFuncionAdicional(int id)
        {
            var funcionAdicional = _context.FuncionAdicionales.Find(id);
            if (funcionAdicional != null)
                _context.FuncionAdicionales.Remove(funcionAdicional);
        }
    }
}
