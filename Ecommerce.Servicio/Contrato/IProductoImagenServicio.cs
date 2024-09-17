using Ecommerce.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Servicio.Contrato
{
    public interface IProductoImagenServicio
    {
        Task<ProductoDTO> Crear(ProductoImagenDTO modelo);
        Task<bool> Editar(ProductoImagenDTO modelo);
    }
}
