using GeoSit.Data.BusinessEntities.Seguridad;
using System.Linq;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class ParametroRepository : IParametroRepository
    {
        private readonly GeoSITMContext _context;

        public ParametroRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ParametrosGenerales GetParametro(long idParametro)
        {
            return _context.ParametrosGenerales.Find(idParametro);
        }
        public ParametrosGenerales GetParametro(string clave)
        {
            return _context.ParametrosGenerales.SingleOrDefault(p => p.Clave == clave);
        }

        public string GetValor(long idParametro)
        {
            return this.GetParametro(idParametro)?.Valor;
        }
        public string GetValor(string clave)
        {
            return this.GetParametro(clave)?.Valor;
        }

        public string GetParametroByDescripcion(string descripcion)
        {
            return _context.ParametrosGenerales.FirstOrDefault(p => p.Descripcion == descripcion)?.Valor;
        }
    }
}
