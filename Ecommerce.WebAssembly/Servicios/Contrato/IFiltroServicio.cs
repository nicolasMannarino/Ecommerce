using Ecommerce.DTO;

namespace Ecommerce.WebAssembly.Servicios.Contrato
{
    public interface IFiltroServicio
    {
        Task<ResponseDTO<List<FiltroDTO>>> Lista(string buscar);
        Task<ResponseDTO<FiltroDTO>> Obtener(int id);
        Task<ResponseDTO<FiltroDTO>> Crear(FiltroDTO modelo);
        Task<ResponseDTO<bool>> Editar(FiltroDTO modelo);
        Task<ResponseDTO<bool>> Eliminar(int id);
    }
}
