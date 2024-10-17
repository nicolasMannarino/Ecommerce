using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Servicio.Contrato;
using Ecommerce.DTO;
using Ecommerce.Servicio.Implementacion;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly IFiltroServicio _filtroServicio;
        public FiltroController(IFiltroServicio filtroServicio)
        {
            _filtroServicio = filtroServicio;
        }

        [HttpGet("Lista/{buscar?}")]
        public async Task<IActionResult> Lista(string buscar = "NA")
        {
            var response = new ResponseDTO<List<FiltroDTO>>();
            try
            {
                if (buscar == "NA")
                    buscar = "";

                response.EsCorrecto = true;
                response.Resultado = await _filtroServicio.Lista(buscar);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("Obtener/{Id:int}")]
        public async Task<IActionResult> Obtener(int Id)
        {
            var response = new ResponseDTO<FiltroDTO>();
            try
            {
                response.EsCorrecto = true;
                response.Resultado = await _filtroServicio.Obtener(Id);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
            }
            return Ok(response);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] FiltroDTO modelo)
        {
            var response = new ResponseDTO<FiltroDTO>();
            try
            {
                response.EsCorrecto = true;
                response.Resultado = await _filtroServicio.Crear(modelo);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("Editar")]
        public async Task<IActionResult> Editar([FromBody] FiltroDTO modelo)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                response.EsCorrecto = true;
                response.Resultado = await _filtroServicio.Editar(modelo);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
            }
            return Ok(response);
        }
        [HttpDelete("Eliminar/{Id:int}")]
        public async Task<IActionResult> Eliminar(int Id)
        {
            var response = new ResponseDTO<bool>();
            try
            {
                response.EsCorrecto = true;
                response.Resultado = await _filtroServicio.Eliminar(Id);
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
            }
            return Ok(response);
        }
    }
}
