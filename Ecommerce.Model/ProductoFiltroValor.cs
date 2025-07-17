using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Modelo
{
    public class ProductoFiltroValor
    {
        public int IdProducto { get; set; }
        public int IdFiltro { get; set; }
        public string Valor { get; set; } = string.Empty;

        public Producto? Producto { get; set; }
        public Filtro? Filtro { get; set; }
    }


}
