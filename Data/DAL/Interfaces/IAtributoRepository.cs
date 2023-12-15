using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Repositories
{
    public interface IAtributoRepository
    {
        IEnumerable<Atributo> GetAtributos();

        IEnumerable<Atributo> GetAtributosByIdComponente(long idComponente);

        Atributo GetAtributoById(int idAtributo);

        void InsertAtributo(Atributo atributo);

        void InsertAtributo(Atributo atributo, Componente componente);

        void UpdateAtributo(Atributo atributo);

        void DeleteAtributo(int idAtributo);

        IEnumerable<Atributo> GetVisiblesByComponente(long idComponente);
    }
}
