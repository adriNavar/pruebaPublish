using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class NorteRepository : INorteRepository
    {
        private readonly GeoSITMContext _context;

        public NorteRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public List<Norte> GetNortes()
        {
            return _context.Nortes.ToList();
        }

        public Norte GetNorteById(int idNorte)
        {
            return _context.Nortes.Find(idNorte);
        }

        public void InsertNorte(Norte norte)
        {
            _context.Nortes.Add(norte);
        }

        public void UpdateNorte(Norte norte)
        {
            _context.Entry(norte).State = EntityState.Modified;
        }

        public void DeleteNorte(int idNorte)
        {
            var norte = _context.Nortes.Find(idNorte);
            if (norte != null)
                _context.Nortes.Remove(norte);
        }        
    }
}
