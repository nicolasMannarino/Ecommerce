using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO.Utilidades
{
    public class ComparePasswordsAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public ComparePasswordsAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
            ErrorMessage = "Las contraseñas no coinciden.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherValue = validationContext.ObjectType
                .GetProperty(_otherProperty)?
                .GetValue(validationContext.ObjectInstance) as string;

            var currentValue = value as string;

            // Si la propiedad "Clave" está vacía o nula, no hacemos validación de igualdad
            if (string.IsNullOrEmpty(otherValue))
            {
                return ValidationResult.Success;
            }

            // Si "Clave" tiene valor, entonces confirmamos que coincidan
            if (!object.Equals(currentValue, otherValue))
            {
                // Asociar explícitamente el error al campo actual
                return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName! });
            }

            return ValidationResult.Success;
        }
    }

}



