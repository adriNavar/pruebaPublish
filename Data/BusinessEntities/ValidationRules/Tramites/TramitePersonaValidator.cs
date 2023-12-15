using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.BusinessEntities.ValidationRules.Tramites
{
    public class TramitePersonaValidator : AbstractValidator<TramitePersona>
    {
        private readonly TramitePersona _tramitePersona;

        public TramitePersonaValidator(TramitePersona tramitePersona)
        {
            _tramitePersona = tramitePersona;

            RuleFor(x => x.Id_Rol)
                .Must(UnicoTramitePersona)
                .WithMessage("Personas: La persona y el rol ya existen para este certificado");
        }

        private bool UnicoTramitePersona(long arg)
        {
            return _tramitePersona == null;
        }
    }
}