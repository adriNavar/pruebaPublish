using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IPersonaExpedienteObraRepository
    {
        IEnumerable<PersonaExpedienteRolDomicilio> GetPersonaExpedienteObras(long idExpedienteObra);

        PersonaExpedienteObra GetPersonaExpedienteObraById(long idExpedienteObra, long idPersona);

        PersonaExpedienteObra GetPersonaExpedienteObraById(long idExpedienteObra, long idPersona, long idRol);

        void InsertPersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra,
            ExpedienteObra expedienteObra);

        void InsertPersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra);

        void UpdatePersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra);

        void DeletePersonaByExpedienteObraId(long idExpedienteObra);

        void DeletePersonaExpedienteObraById(long idExpedienteObra, long idPersona);

        void DeletePersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra);
    }
}
