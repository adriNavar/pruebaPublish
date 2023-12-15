using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class FuncAdicParametroRepository : IFuncAdicParametroRepository
    {
        private readonly GeoSITMContext _context;

        public FuncAdicParametroRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<FuncAdicParametro> GetFuncAdicParametros()
        {
            return _context.FuncAdicParametros.ToList();
        }

        public FuncAdicParametro GetFuncAdicParametroById(int idFuncAdicParametro)
        {
            return _context.FuncAdicParametros.Find(idFuncAdicParametro);
        }

        public void InsertFuncAdicParametro(FuncAdicParametro funcAdicParametro)
        {
            _context.FuncAdicParametros.Add(funcAdicParametro);
        }

        public void UpdateFuncAdicParametro(FuncAdicParametro funcAdicParametro)
        {
            _context.Entry(funcAdicParametro).State = EntityState.Modified;
        }

        public void DeleteFuncAdicParametro(int id)
        {
            var funcAdicParametro = _context.FuncAdicParametros.Find(id);
            if (funcAdicParametro != null)
                _context.FuncAdicParametros.Remove(funcAdicParametro);
        }
    }
}
