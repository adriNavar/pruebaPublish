using System.Text.RegularExpressions;
using FluentValidation;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.BusinessEntities.ValidationRules.Parcela
{
    public class NomenclaturaValidator : AbstractValidator<Nomenclatura>
    {
        private readonly Nomenclatura _nomenclatura;
        //private readonly GeoSit.Data.BusinessEntities.Inmuebles.Parcela _parcela;
        public NomenclaturaValidator(Nomenclatura nomenclatura)
        {
            _nomenclatura = nomenclatura;
           
            RuleFor(n => n.TipoNomenclaturaID)
                .Must(unicoTipoEnParcela)
                .WithMessage("Nomenclatura Existente");

            RuleFor(n => n.Nombre)
                .Must(formatoNomenclatura)
                .WithMessage("El formato de la nomenclatura es inválido");            
        }        
        
        private bool unicoTipoEnParcela(Nomenclatura instance, long arg)
        {
            return _nomenclatura.Nombre != instance.Nombre || (_nomenclatura.Nombre == instance.Nombre && _nomenclatura.FechaBaja != instance.FechaBaja);
        }

        private bool formatoNomenclatura(Nomenclatura instance, string arg)
        {
            if (_nomenclatura == null ) return true;
            var regex = new Regex(instance.Tipo.ExpresionRegular);
            return regex.IsMatch(arg);
        }
    }
}
