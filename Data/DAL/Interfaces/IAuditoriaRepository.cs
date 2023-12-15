using GeoSit.Data.BusinessEntities.Seguridad;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IAuditoriaRepository
    {
        Auditoria GetAuditoriaById(long id);

        Auditoria GetAuditoriaByUsuarioEvento(long idUsuario, long idEvento);

        IEnumerable<Auditoria> GetAuditoriaByIdObjeto(long idObjeto, string Objeto);

        void InsertAuditoria(Auditoria auditoria);        
    }
}
