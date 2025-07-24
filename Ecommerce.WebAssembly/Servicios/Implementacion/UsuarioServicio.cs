using Ecommerce.DTO;
using Ecommerce.WebAssembly.Servicios.Contrato;
using System.Net.Http.Json;

namespace Ecommerce.WebAssembly.Servicios.Implementacion
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly HttpClient _httpClient;

        public UsuarioServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Cambiado a nuevo endpoint AuthController
        public async Task<ResponseDTO<SesionDTO>> Autorizacion(LoginDTO modelo)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", modelo);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<SesionDTO>>();
            return result!;
        }

        public async Task<ResponseDTO<UsuarioDTO>> Crear(UsuarioDTO modelo)
        {
            var response = await _httpClient.PostAsJsonAsync("usuario", modelo);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<UsuarioDTO>>();
            return result!;
        }

        // Ahora recibe el id en la URL y el modelo en el cuerpo
        public async Task<ResponseDTO<bool>> Editar(UsuarioDTO modelo)
        {
            var response = await _httpClient.PutAsJsonAsync($"usuario/{modelo.IdUsuario}", modelo);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<bool>>();
            return result!;
        }

        public async Task<ResponseDTO<bool>> Eliminar(int id)
        {
            return await _httpClient.DeleteFromJsonAsync<ResponseDTO<bool>>($"usuario/{id}");
        }

        // Parámetros en query string, ejemplo: /usuario?rol=admin&buscar=juan
        public async Task<ResponseDTO<List<UsuarioDTO>>> Lista(string rol = null, string buscar = "")
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(rol))
                query["rol"] = rol;

            if (!string.IsNullOrWhiteSpace(buscar))
                query["buscar"] = buscar;

            var queryString = query.ToString();

            var url = string.IsNullOrWhiteSpace(queryString) ? "usuario" : $"usuario?{queryString}";

            return await _httpClient.GetFromJsonAsync<ResponseDTO<List<UsuarioDTO>>>(url);
        }

        public async Task<ResponseDTO<UsuarioDTO>> Obtener(int id)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<UsuarioDTO>>($"usuario/{id}");
        }
    }
}
