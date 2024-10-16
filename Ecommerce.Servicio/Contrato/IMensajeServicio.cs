using Ecommerce.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Servicio.Contrato
{
    public interface IMensajeServicio
    {
        Task EnviarMensajeAsync(string cola, string mensaje);
    }
}
