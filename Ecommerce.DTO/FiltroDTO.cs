using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO
{
    public class FiltroDTO
    {
        public int IdFiltro { get; set; }
        [Required(ErrorMessage = "Ingrese nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Ingrese Tipo")]
        public string? Tipo { get; set; }
        public List<int> CategoriaIds { get; set; } = new List<int>();
    }
}
