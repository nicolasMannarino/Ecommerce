using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO
{
    public class CatalogoFiltroRequestDTO
    {
        public string Categoria { get; set; } = "";
        public string Buscar { get; set; } = "";
        public List<ProductoFiltroValorDTO> Filtros { get; set; } = new();
    }

}
