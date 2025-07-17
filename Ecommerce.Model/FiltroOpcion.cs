using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Modelo
{
    public class FiltroOpcion
    {
        [Key]
        public int Id { get; set; }  

        public int IdFiltro { get; set; }

        [Required]
        public string Valor { get; set; } = string.Empty;

        public Filtro Filtro { get; set; } = null!;
        public int IdCategoria { get; set; }

    }
}
