using System.Linq;
using FluentValidation;

namespace GeoSit.Data.BusinessEntities.ValidationRules
{
    public class FluentValidator<T>
    {
        public static string Validate(IValidator validator, T entity)
        {            
            var result = validator.Validate(new ValidationContext<T>(entity));
            return result.IsValid ? string.Empty : 
                result.Errors.Aggregate(string.Empty, (current, failure) => current + failure.ErrorMessage + "<br>");
        }
    }
}
