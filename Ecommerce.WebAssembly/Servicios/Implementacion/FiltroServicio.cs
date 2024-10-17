using Ecommerce.DTO;
using Ecommerce.WebAssembly.Servicios.Contrato;
using System.Net.Http.Json;

namespace Ecommerce.WebAssembly.Servicios.Implementacion
{
    public class FiltroServicio : IFiltroServicio
    {
        private readonly HttpClient _httpClient;
        public FiltroServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseDTO<FiltroDTO>> Crear(FiltroDTO modelo)
        {
            var response = await _httpClient.PostAsJsonAsync("Filtro/Crear", modelo);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<FiltroDTO>>();
            return result!;
        }

        public async Task<ResponseDTO<bool>> Editar(FiltroDTO modelo)
        {
            var response = await _httpClient.PutAsJsonAsync("Filtro/Editar", modelo);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<bool>>();
            return result!;
        }

        public async Task<ResponseDTO<bool>> Eliminar(int id)
        {
            return await _httpClient.DeleteFromJsonAsync<ResponseDTO<bool>>($"Filtro/Eliminar/{id}");
        }

        public async Task<ResponseDTO<List<FiltroDTO>>> Lista(string buscar)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<List<FiltroDTO>>>($"Filtro/Lista/{buscar}");
        }

        public async Task<ResponseDTO<FiltroDTO>> Obtener(int id)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<FiltroDTO>>($"Filtro/Obtener/{id}");
        }
    }
}
