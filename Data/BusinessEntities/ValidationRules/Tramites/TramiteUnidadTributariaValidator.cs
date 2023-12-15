using FluentValidation;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.BusinessEntities.ValidationRules.Tramites
{
    public class TramiteUnidadTributariaValidator : AbstractValidator<TramiteUnidadTributaria>
    {
        private readonly TramiteUnidadTributaria _unidadTributaria;

        public TramiteUnidadTributariaValidator(TramiteUnidadTributaria unidadTributaria)
        {
            _unidadTributaria = unidadTributaria;

            RuleFor(x => x.Id_Unidad_Tributaria)
                .Must(UnicoTramiteUT)
                .WithMessage("Unidad Tributaria: La unidad tributaria ya existe para este certificado");
        }

        private bool UnicoTramiteUT(long arg)
        {
            return _unidadTributaria == null;
        }
    }
}