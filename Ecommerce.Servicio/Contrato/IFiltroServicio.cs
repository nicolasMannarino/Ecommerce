using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.DTO;

namespace Ecommerce.Servicio.Contrato
{
    public interface IFiltroServicio
    {
        Task<List<FiltroDTO>> Lista(string buscar);
        Task<FiltroDTO> Obtener(int id);
        Task<FiltroDTO> Crear(FiltroDTO modelo);
        Task<bool> Editar(FiltroDTO modelo);
        Task<bool> Eliminar(int id);
    }
}
