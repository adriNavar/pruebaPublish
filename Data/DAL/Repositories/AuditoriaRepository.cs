using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly GeoSITMContext _context;

        public AuditoriaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public Auditoria GetAuditoriaById(long id)
        {
            return _context.Auditoria.Find(id);
        }

        public Auditoria GetAuditoriaByUsuarioEvento(long idUsuario, long idEvento)
        {
            return _context.Auditoria.Single(x => x.Id_Usuario == idUsuario && x.Id_Evento == idEvento);
        }

        public IEnumerable<Auditoria> GetAuditoriaByIdObjeto(long idObjeto, string Objeto)
        {
            return _context.Auditoria.Where(x => x.Id_Objeto == idObjeto && x.Objeto == Objeto)
                   .Include("usuarios")
                   .Include("eventos")
                   .Include("tramites")
                   .ToList();
        }

        public void InsertAuditoria(Auditoria auditoria)
        {
            _context.Auditoria.Add(auditoria);
        }
    }
}
