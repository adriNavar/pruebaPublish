using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class ComponenteRepository : IComponenteRepository
    {
        private readonly GeoSITMContext _context;

        public ComponenteRepository(GeoSITMContext context)
        {
            _context = context;
        }


        public IEnumerable<Componente> GetComponentes()
        {
            return _context.Componente.OrderBy(c => c.Nombre);
        }

        public Componente GetComponenteById(long idComponente)
        {
            return _context.Componente.Find(idComponente);
        }

        public Componente GetComponenteByDocType(string docType)
        {
            return _context.Componente.FirstOrDefault(c => c.DocType.ToLower().Equals(docType.ToLower()));
        }

        public List<Componente> GetComponenteByDocType()
        {
            return _context.Componente.Where(x => x.DocType == "obraags" || x.DocType == "obracls").ToList();
        }

        public Componente GetComponenteByTable(string table)
        {
            return _context.Componente.FirstOrDefault(c => c.Tabla.ToLower().Equals(table.ToLower()));
        }

        public Componente GetComponenteByLayer(string layer)
        {
            Componente componente = _context.Componente.FirstOrDefault(c => c.Capa.ToLower().Equals(layer.ToLower()));
            //Busco los componentes que contengan esta capa
            if (componente == null)
            {
                componente = _context.Componente.Where(c => c.Capa.Contains(",") && c.Capa.ToLower().Contains(layer.ToLower())).ToList().FirstOrDefault(c => c.Capa.Split(',').Any(cc => cc.Trim().ToLower().Equals(layer)));
            }


            return componente;
        }

        public void InsertComponente(Componente componente)
        {
            _context.Componente.Add(componente);
        }

        public void UpdateComponente(Componente componente)
        {
            _context.Entry(componente).State = EntityState.Modified;
        }

        public void DeleteComponente(long idComponente)
        {
            var componente = _context.Componente.Find(idComponente);
            if (componente != null)
                _context.Componente.Remove(componente);
        }
    }
}
