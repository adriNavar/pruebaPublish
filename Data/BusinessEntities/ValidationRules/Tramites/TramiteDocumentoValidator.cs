using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.BusinessEntities.ValidationRules.Tramites
{
    public class TramiteDocumentoValidator : AbstractValidator<TramiteDocumento>
    {
        private readonly TramiteDocumento _documento;

        public TramiteDocumentoValidator(TramiteDocumento documento)
        {
            _documento = documento;

            RuleFor(x => x.Id_Documento)
                .Must(UnicoTramiteDocumento)
                .WithMessage("Documentos: El documento ya existe para este certificado");
        }

        private bool UnicoTramiteDocumento(long arg)
        {
            return _documento == null;
        }
    }
}
