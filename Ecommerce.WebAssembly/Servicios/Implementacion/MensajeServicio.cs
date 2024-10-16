using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ecommerce.WebAssembly.Servicios.Contrato;

namespace Ecommerce.WebAssembly.Servicios.Implementacion
{
    public class MensajeServicio : IMensajeServicio
    {
        private readonly HttpClient _httpClient;

        public MensajeServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task EnviarMensajeAsync(string cola, string nombre, string destinatario, string mensaje)
        {
            var contenido = new StringContent(JsonSerializer.Serialize(new
            {
                Cola = cola,
                Nombre = nombre,
                Destinatario = destinatario,
                Mensaje = mensaje
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Mensaje/Enviar", contenido);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error enviando mensaje: {error}");
            }
        }

    }
}
