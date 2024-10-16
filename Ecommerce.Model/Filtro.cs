using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Modelo
{
    public class Filtro
    {
        public int IdFiltro { get; set; }
        public string? Nombre { get; set; }
        public string? Tipo { get; set; }
        public virtual ICollection<CategoriaFiltro> CategoriasFiltro { get; set; }
    }
}
