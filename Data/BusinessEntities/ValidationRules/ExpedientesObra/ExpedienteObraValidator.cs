using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra
{
    public class ExpedienteObraValidator : AbstractValidator<ExpedienteObra>
    {
        private readonly ExpedienteObra _expedientePorNoLegajo;
        private readonly ExpedienteObra _expedientePorNoExpediente;

        public ExpedienteObraValidator(ExpedienteObra expedientePorNoLegajo, ExpedienteObra expedientePorNoExpediente)
        {
            _expedientePorNoLegajo = expedientePorNoLegajo;
            _expedientePorNoExpediente = expedientePorNoExpediente;

            RuleFor(x => x.NumeroLegajo)
                .Must(UnicoNumeroLegajo)
                .WithMessage("Expediente de Obra: El número de legajo ya existe");
            RuleFor(x => x.NumeroExpediente)
                .Must(UnicoNumeroExpediente)
                .WithMessage("Expediente de Obra: El número de expediente ya existe");
        }

        private bool UnicoNumeroLegajo(ExpedienteObra instance, string arg)
        {
            if (_expedientePorNoLegajo != null)
            {
                if (instance.ExpedienteObraId == _expedientePorNoLegajo.ExpedienteObraId)
                    return true;
            }

            return _expedientePorNoLegajo == null;
        }

        private bool UnicoNumeroExpediente(ExpedienteObra instance, string arg)
        {
            if (_expedientePorNoExpediente != null)
            {
                if (instance.ExpedienteObraId == _expedientePorNoExpediente.ExpedienteObraId)
                    return true;
            }

            return _expedientePorNoExpediente == null;
        }
    }
}
