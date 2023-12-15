using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IComponenteRepository
    {
        IEnumerable<Componente> GetComponentes();

        Componente GetComponenteById(long idComponente);

        Componente GetComponenteByDocType(string docType);

        List<Componente> GetComponenteByDocType();

        Componente GetComponenteByLayer(string layer);

        Componente GetComponenteByTable(string table);

        void InsertComponente(Componente componente);

        void UpdateComponente(Componente componente);

        void DeleteComponente(long idComponente);
    }
}
