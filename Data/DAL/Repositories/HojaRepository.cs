using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class HojaRepository : IHojaRepository
    {
        private readonly GeoSITMContext _context;

        public HojaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Hoja> GetHojas()
        {
            return _context.Hojas.OrderBy(h => h.Nombre).ToList();
        }

        public Hoja GetHojaById(int idHoja)
        {
            return _context.Hojas.Find(idHoja);
        }

        public void InsertHoja(Hoja hoja)
        {
            _context.Hojas.Add(hoja);
        }

        public void UpdateHoja(Hoja hoja)
        {
            _context.Entry(hoja).State = EntityState.Modified;
        }

        public void DeleteHoja(int id)
        {
            var hoja = _context.Hojas.Find(id);
            if (hoja != null)
                _context.Hojas.Remove(hoja);
        }
    }
}
