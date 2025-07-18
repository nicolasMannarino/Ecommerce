﻿using Ecommerce.DTO;
using Ecommerce.WebAssembly.Servicios.Contrato;
using System.Net.Http.Json;

namespace Ecommerce.WebAssembly.Servicios.Implementacion
{
    public class ProductoServicio : IProductoServicio
    {
        private readonly HttpClient _httpClient;

        public ProductoServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<ResponseDTO<List<ProductoDTO>>> Catalogo(string categoria, string buscar)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<List<ProductoDTO>>>($"Producto/Catalogo/{categoria}/{buscar}");
        }

        public async Task<ResponseDTO<List<FiltroDTO>>> ObtenerFiltrosPorCategoria(string nombreCategoria)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<List<FiltroDTO>>>($"Producto/FiltrosPorCategoria/{nombreCategoria}");
        }


        public async Task<ResponseDTO<ProductoDTO>> Crear(ProductoDTO modelo)
        {
            //var response = await _httpClient.PostAsJsonAsync("Producto/Crear", modelo);
            var response = await _httpClient.PostAsJsonAsync("Producto/Crear", modelo);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                // Mostrar el error en los logs o con algún servicio de alertas
                Console.WriteLine(error);
            }
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<ProductoDTO>>();
            return result!;
        }

        public async Task<ResponseDTO<bool>> Editar(ProductoDTO modelo)
        {
            var response = await _httpClient.PutAsJsonAsync("Producto/Editar", modelo);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<bool>>();
            return result!;
        }

        public async Task<ResponseDTO<bool>> Eliminar(int id)
        {
            return await _httpClient.DeleteFromJsonAsync<ResponseDTO<bool>>($"Producto/Eliminar/{id}");
        }

        public async Task<ResponseDTO<List<ProductoDTO>>> Lista(string buscar)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<List<ProductoDTO>>>($"Producto/Lista/{buscar}");
        }

        public async Task<ResponseDTO<ProductoDTO>> Obtener(int id)
        {
            return await _httpClient.GetFromJsonAsync<ResponseDTO<ProductoDTO>>($"Producto/Obtener/{id}");
        }

        public async Task<ResponseDTO<List<ProductoDTO>>> CatalogoConFiltros(string categoria, string buscar, List<ProductoFiltroValorDTO>? filtros)
        {
            var body = new CatalogoFiltroRequestDTO
            {
                Categoria = categoria,
                Buscar = buscar,
                Filtros = filtros ?? new List<ProductoFiltroValorDTO>()
            };

            var response = await _httpClient.PostAsJsonAsync("Producto/CatalogoConFiltros", body);
            var result = await response.Content.ReadFromJsonAsync<ResponseDTO<List<ProductoDTO>>>();
            return result!;
        }
    }
}
