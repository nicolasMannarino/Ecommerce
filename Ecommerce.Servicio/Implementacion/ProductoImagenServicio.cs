using Ecommerce.DTO;
using Ecommerce.Servicio.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Servicio.Implementacion
{
    public class ProductoImagenServicio : IProductoImagenServicio
    {
        public Task<ProductoDTO> Crear(ProductoImagenDTO modelo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Editar(ProductoImagenDTO modelo)
        {
            throw new NotImplementedException();
        }
    }
}
