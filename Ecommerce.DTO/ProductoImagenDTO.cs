using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO
{
    public class ProductoImagenDTO
    {
        public int IdImagen { get; set; }
        public int IdProducto { get; set; }
        public int NumeroImagen { get; set; }
        public string RutaImagen { get; set; }
    }
}
