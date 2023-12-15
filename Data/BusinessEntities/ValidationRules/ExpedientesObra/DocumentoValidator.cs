using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.BusinessEntities.ValidationRules.ExpedientesObra
{
    public class DocumentoValidator : AbstractValidator<ExpedienteObraDocumento>
    {
        private readonly ExpedienteObraDocumento _expedienteObraDocumento;

        public DocumentoValidator(ExpedienteObraDocumento expedienteObraDocumento)
        {
            _expedienteObraDocumento = expedienteObraDocumento;

            RuleFor(x => x.DocumentoId)
                .Must(UnicoExpedienteObraDocumento)
                .WithMessage("Documentos: El documento ya existe para este expediente");
        }

        private bool UnicoExpedienteObraDocumento(long arg)
        {
            return _expedienteObraDocumento == null;
        }
    }
}
