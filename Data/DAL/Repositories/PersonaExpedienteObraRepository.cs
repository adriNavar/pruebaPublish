using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class PersonaExpedienteObraRepository : IPersonaExpedienteObraRepository
    {
        private readonly GeoSITMContext _context;

        public PersonaExpedienteObraRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<PersonaExpedienteRolDomicilio> GetPersonaExpedienteObras(long idExpedienteObra)
        {
            var personaExpedienteObras = _context.PersonasExpedienteObra;
            var persona = _context.Persona;
            var roles = _context.Roles;
            var domicilioInmuebleExpedienteObras = _context.DomiciliosExpedienteObra;
            var domicilios = _context.Domicilios;
            
            var query = from personaInmueblesExpedienteObra in personaExpedienteObras
                join rol in roles on personaInmueblesExpedienteObra.RolId equals rol.RolId
                join personaInmueble in persona on 
                personaInmueblesExpedienteObra.PersonaInmuebleId equals personaInmueble.PersonaId
                join domicilioInmuebleExpedienteObra in domicilioInmuebleExpedienteObras
                on personaInmueblesExpedienteObra.ExpedienteObraId equals domicilioInmuebleExpedienteObra.ExpedienteObraId
                join domicilio in domicilios on domicilioInmuebleExpedienteObra.DomicilioInmuebleId equals domicilio.DomicilioId
                where personaInmueblesExpedienteObra.ExpedienteObraId == idExpedienteObra                
                select new PersonaExpedienteRolDomicilio
                {
                    PersonaInmuebleId = personaInmueblesExpedienteObra.PersonaInmuebleId,
                    ExpedienteObraId = personaInmueblesExpedienteObra.ExpedienteObraId,
                    RolId = rol.RolId,
                    Rol = rol.Descripcion,
                    NombreCompleto = personaInmueble.NombreCompleto,
                    DomicilioFisico = domicilio.TipoDomicilioId == 1 ? domicilio.ViaNombre : ""
                };

            return query.Distinct();
        }

        public PersonaExpedienteObra GetPersonaExpedienteObraById(long idExpedienteObra, long idPersona)
        {
            return _context.PersonasExpedienteObra.Find(idPersona, idExpedienteObra);
        }

        public PersonaExpedienteObra GetPersonaExpedienteObraById(long idExpedienteObra, long idPersona, long idRol)
        {
            return _context.PersonasExpedienteObra
                .FirstOrDefault(x => x.ExpedienteObraId == idExpedienteObra && x.PersonaInmuebleId == idPersona && x.RolId == idRol);
        }

        public void InsertPersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra,
            ExpedienteObra expedienteObra)
        {
            PersonaExpedienteObra.ExpedienteObra = expedienteObra;
            _context.PersonasExpedienteObra.Add(PersonaExpedienteObra);
        }

        public void InsertPersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra)
        {
            _context.PersonasExpedienteObra.Add(PersonaExpedienteObra);
        }

        public void UpdatePersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra)
        {
            _context.Entry(PersonaExpedienteObra).State = EntityState.Modified;
        }

        public void DeletePersonaByExpedienteObraId(long idExpedienteObra)
        {
            var pieos = _context.PersonasExpedienteObra.Where(x => x.ExpedienteObraId == idExpedienteObra);
            foreach (var pieo in pieos)
            {
                _context.Entry(pieo).State = EntityState.Deleted;
            }
        }

        public void DeletePersonaExpedienteObraById(long idExpedienteObra, long idPersona)
        {
            DeletePersonaExpedienteObra(GetPersonaExpedienteObraById(idExpedienteObra, idPersona));
        }

        public void DeletePersonaExpedienteObra(PersonaExpedienteObra PersonaExpedienteObra)
        {
            if (PersonaExpedienteObra != null)
                _context.Entry(PersonaExpedienteObra).State = EntityState.Deleted;
        }
    }
}
