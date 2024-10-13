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
        [Required(ErrorMessage = "Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Apellido")]
        public string? Apellido { get; set; }
        [Required(ErrorMessage = "Email")]
        public string? Correo { get; set; }
        [Required(ErrorMessage = "Contraseña")]
        public string? Clave { get; set; }
        [Required(ErrorMessage = "Confirmar Contraseña")]
        public string? ConfirmarClave { get; set; }
        public string? Rol { get; set; }
    }
}
