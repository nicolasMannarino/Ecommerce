using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.DTO;

namespace Ecommerce.Servicio.Contrato
{
    public interface ICorreoServicio
    {
        Task EnviarCorreoBienvenida(string destinatario, string asunto, string mensaje);
    }
}
