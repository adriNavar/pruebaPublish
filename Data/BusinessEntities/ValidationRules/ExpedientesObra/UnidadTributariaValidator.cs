using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra
{
    public class UnidadTributariaValidator : AbstractValidator<UnidadTributariaExpedienteObra>
    {
        private readonly UnidadTributariaExpedienteObra _unidadTributariaExpedienteObra;

        public UnidadTributariaValidator(UnidadTributariaExpedienteObra unidadTributariaExpedienteObra)
        {
            _unidadTributariaExpedienteObra = unidadTributariaExpedienteObra;

            RuleFor(x => x.UnidadTributariaId)
                .Must(UnicaUnidadTributariaExpedienteObra)
                .WithMessage("Unidades Tributarias: La unidad tributaria ya existe para este expediente");
        }

        private bool UnicaUnidadTributariaExpedienteObra(long arg)
        {
            return _unidadTributariaExpedienteObra == null;
        }
    }
}
