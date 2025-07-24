using Ecommerce.DTO.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El Nombre es requerido.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El Apellido es requerido")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El Email es requerido")]
        public string? Correo { get; set; }

        // Quitar Required para permitir edición sin modificar contraseña
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.")]
        public string? Clave { get; set; }

        // Modificar ComparePasswords para validar solo si Clave no es null ni vacío
        [ComparePasswords("Clave")]
        public string? ConfirmarClave { get; set; }
        public string? Rol { get; set; }
    }
}
