using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IPersonaRepository
    {
        IEnumerable<PersonaExpedienteRolDomicilio> SearchPersona(string nombre);

        PersonaExpedienteRolDomicilio GetPersona(long idPersona);

        Persona GetPersonaDatos(long idPersona);
        IEnumerable<Persona> GetPersonasByTramite(int tramite);
        IEnumerable<Persona> GetPersonasCompletas(long[] personas);
    }
}
