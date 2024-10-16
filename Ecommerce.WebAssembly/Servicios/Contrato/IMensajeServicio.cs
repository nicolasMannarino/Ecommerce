using System.Threading.Tasks;
using Ecommerce.DTO;

namespace Ecommerce.WebAssembly.Servicios.Contrato
{
    public interface IMensajeServicio
    {
        Task EnviarMensajeAsync(string cola, string nombre, string destinatario, string mensaje);
    }
}
