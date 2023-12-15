using System;
using System.Linq;
using System.Data.Entity;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly GeoSITMContext _context;

        public PersonaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<PersonaExpedienteRolDomicilio> SearchPersona(string nombre)
        {
            nombre = nombre.ToLower();
            var tokens = nombre.Split(' ');

            var personaInmuebles = _context.Persona;
            var personaDomicilios = _context.PersonaDomicilio;
            var domicilioInmuebles = _context.Domicilios;

            return from personaInmueble in personaInmuebles
                   join personaDomicilio in personaDomicilios
                   on personaInmueble.PersonaId equals personaDomicilio.PersonaId
                   join domicilioInmueble in domicilioInmuebles
                   on personaDomicilio.DomicilioId equals domicilioInmueble.DomicilioId
                   where tokens.All(token => personaInmueble.NombreCompleto.ToLower().Contains(token))
                   select new PersonaExpedienteRolDomicilio
                   {
                       PersonaInmuebleId = personaInmueble.PersonaId,
                       NombreCompleto = personaInmueble.NombreCompleto,
                       DomicilioFisico = personaDomicilio != null ? personaDomicilio.Domicilio.ViaNombre : ""
                   };
        }

        public PersonaExpedienteRolDomicilio GetPersona(long idPersona)
        {
            var persona = _context.Persona.Find(idPersona);
            _context.Entry(persona).Collection(x => x.PersonaDomicilios).Load();
            var domicilio = persona.PersonaDomicilios != null ?
                persona.PersonaDomicilios.FirstOrDefault(pd => pd.TipoDomicilioId == 1) : null;
            if (domicilio != null) _context.Entry(domicilio).Reference(x => x.Domicilio).Load();
            return new PersonaExpedienteRolDomicilio
            {
                PersonaInmuebleId = persona.PersonaId,
                NombreCompleto = persona.NombreCompleto,
                DomicilioFisico = domicilio != null ? domicilio.Domicilio.ViaNombre : string.Empty
            };
        }

        public Persona GetPersonaDatos(long idPersona)
        {
            var persona = _context.Persona
                                   .Include("TipoDocumentoIdentidad")
                                   .Include("TipoPersona")
                                   .Include("PersonaDomicilios")
                                   .Include("PersonaDomicilios.Domicilio")
                                   .Include("PersonaDomicilios.TipoDomicilio")
                                   .SingleOrDefault(x => x.PersonaId == idPersona);

            persona.PersonaNacionalidad = _context.Nacionalidad.Where(n => n.NacionalidadId == persona.Nacionalidad).Select(n => n.Descripcion).FirstOrDefault();

            switch (persona.Sexo)
            {
                case 1:
                    persona.PersonaSexo = "Femenino";
                    break;
                case 2:
                    persona.PersonaSexo = "Masculino";
                    break;
                case 3:
                    persona.PersonaSexo = "Sin Identificar";
                    break;
                default:
                    persona.PersonaSexo = ("Sin Identificar");
                    break;
            }

            switch (persona.EstadoCivil)
            {
                case 1:
                    persona.PersonaEstadoCivil = "Casado/a";
                    break;
                case 2:
                    persona.PersonaEstadoCivil = "Separado/a";
                    break;
                case 3:
                    persona.PersonaEstadoCivil = "Divorciado/a";
                    break;
                case 4:
                    persona.PersonaEstadoCivil = "Viudo/a";
                    break;
                case 5:
                    persona.PersonaEstadoCivil = "Soltero/a";
                    break;
                case 6:
                    persona.PersonaEstadoCivil = "Sin Identificar";
                    break;

                default:
                    persona.PersonaEstadoCivil = ("Sin Identificar");
                    break;
            }

            persona.NombreCompleto = persona.NombreCompleto.ToUpper();

            return persona;
        }

        public IEnumerable<Persona> GetPersonasByTramite(int tramite)
        {
            int tipoEntradaPersona = Convert.ToInt32(Entradas.Persona);

            var query = from entradaTramite in _context.TramitesEntradas
                        join objetoEntrada in _context.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join persona in _context.Persona on entradaTramite.IdObjeto.Value equals persona.PersonaId
                        where objetoEntrada.IdEntrada == tipoEntradaPersona && tramite == entradaTramite.IdTramite
                        select persona;

            return query.Include(x => x.TipoPersona).ToList();
        }

        public IEnumerable<Persona> GetPersonasCompletas(long[] personas)
        {
            var listaPersonas = new List<Persona>();

            int MAX_CANT = 900;
            int procesados = 0;
            while (procesados < personas.Length)
            {
                var stepIds = personas.Skip(procesados).Take(MAX_CANT).ToArray();
                procesados += stepIds.Length;

                listaPersonas.AddRange(_context.Persona
                                               .Where(persona => stepIds.Contains(persona.PersonaId))
                                               .Include(p => p.TipoDocumentoIdentidad)
                                               .Include(p => p.PersonaDomicilios.Select(pd => pd.Domicilio))
                                               .Include(p => p.PersonaDomicilios.Select(pd => pd.TipoDomicilio))
                                               .ToList());
            }

            return listaPersonas;
        }
    }
}
