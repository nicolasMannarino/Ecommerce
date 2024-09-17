using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Modelo;

public partial class ProductoImagen
{
    public int IdProductoImagen { get; set; }
    public int IdProducto { get; set; }
    public int NumeroImagen { get; set; }
    public string? RutaImagen { get; set; }
    public virtual Producto? Producto { get; set; }
}
