using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IUnidadTributariaPersonaRepository
    {
        IEnumerable<ResponsableFiscal> GetUnidadTributariaResponsablesFiscales(long idUnidadTributaria);

        IEnumerable<ResponsableFiscal> GetUnidadTributariaResponsablesFiscalesFromView(long idUnidadTributaria);

        UnidadTributariaPersona GetUnidadTributariaPersonaById(long idUnidadTributaria, long idPersona, bool baja = false);

        void InsertUnidadTributariaPersona(UnidadTributariaPersona unidadTributariaPersona);

        void UpdateUnidadTributariaPersona(UnidadTributariaPersona unidadTributariaPersona);

        void DeleteUnidadTributariaPersona(UnidadTributariaPersona unidadTributariaPersona);
    }
}
