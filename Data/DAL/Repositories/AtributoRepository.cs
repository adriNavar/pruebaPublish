using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class AtributoRepository : IAtributoRepository
    {
        private readonly GeoSITMContext _context;

        public AtributoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Atributo> GetAtributos()
        {
            return _context.Atributo.ToList();
        }

        public IEnumerable<Atributo> GetAtributosByIdComponente(long idComponente)
        {
            return _context.Atributo.Where(x => x.ComponenteId == idComponente);
        }

        public Atributo GetAtributoById(int idAtributo)
        {
            return _context.Atributo.Find(idAtributo);
        }

        public void InsertAtributo(Atributo atributo)
        {
            _context.Atributo.Add(atributo);
        }

        public void InsertAtributo(Atributo atributo, Componente componente)
        {
            atributo.Componente = componente;
            _context.Atributo.Add(atributo);
        }

        public void UpdateAtributo(Atributo atributo)
        {
            _context.Entry(atributo).State = EntityState.Modified;
        }

        public void DeleteAtributo(int idAtributo)
        {
            var atributo = _context.Atributo.Find(idAtributo);
            if (atributo != null)
                _context.Atributo.Remove(atributo);
        }

        public IEnumerable<Atributo> GetVisiblesByComponente(long idComponente)
        {
            var query = from atributo in _context.Atributo
                        join componente in _context.Componente on atributo.ComponenteId equals componente.ComponenteId
                        where atributo.EsVisible && (atributo.ComponenteId == idComponente || idComponente == 0)
                        orderby componente.Nombre
                        select new { AtributoId = atributo.AtributoId, Nombre = componente.Nombre + "." + atributo.Nombre };

            return query.ToList().Select(x => new Atributo { AtributoId = x.AtributoId, Nombre = x.Nombre });
        }
    }
}
