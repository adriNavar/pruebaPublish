using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra
{
    public class PersonaValidator : AbstractValidator<PersonaExpedienteObra>
    {
        private readonly PersonaExpedienteObra _personaInmuebleExpedienteObra;

        public PersonaValidator(PersonaExpedienteObra personaInmuebleExpedienteObra)
        {
            _personaInmuebleExpedienteObra = personaInmuebleExpedienteObra;

            RuleFor(x => x.RolId)
                .Must(UnicoPersonaInmuebleExpedienteObra)
                .WithMessage("Personas: La persona y el rol ya existen para este expediente");
        }

        private bool UnicoPersonaInmuebleExpedienteObra(long arg)
        {
            return _personaInmuebleExpedienteObra == null;
        }
    }
}
