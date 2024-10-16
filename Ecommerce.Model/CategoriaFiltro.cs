using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Modelo
{
    public class CategoriaFiltro
    {
        public int IdFiltro { get; set; }       
        public int IdCategoria { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual Filtro Filtro { get; set; }
    }
}
