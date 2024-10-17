using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Modelo;
using Ecommerce.DTO;
using Ecommerce.Repositorio.Contrato;
using Ecommerce.Servicio.Contrato;
using AutoMapper;

namespace Ecommerce.Servicio.Implementacion
{
    public class FiltroServicio : IFiltroServicio
    {
        private readonly IGenericoRepositorio<Filtro> _modeloRepositorio;
        private readonly IMapper _mapper;
        public FiltroServicio(IGenericoRepositorio<Filtro> modeloRepositorio, IMapper mapper)
        {
            _modeloRepositorio = modeloRepositorio;
            _mapper = mapper;
        }

        public async Task<FiltroDTO> Crear(FiltroDTO modelo)
        {
            try
            {
                var dbModelo = _mapper.Map<Filtro>(modelo);
                var rspModelo = await _modeloRepositorio.Crear(dbModelo);

                if (rspModelo.IdFiltro != 0)
                    return _mapper.Map<FiltroDTO>(rspModelo);
                else
                    throw new TaskCanceledException("No se puede crear");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> Editar(FiltroDTO modelo)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdFiltro == modelo.IdFiltro);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    fromDbModelo.Nombre = modelo.Nombre;
                    var respuesta = await _modeloRepositorio.Editar(fromDbModelo);

                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo editar");
                    return respuesta;
                }
                else
                {
                    throw new TaskCanceledException("No se encontraron resultados");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdFiltro == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    var respuesta = await _modeloRepositorio.Eliminar(fromDbModelo);
                    if (!respuesta)
                        throw new TaskCanceledException("No se pudo eliminar");
                    return respuesta;
                }
                else
                {
                    throw new TaskCanceledException("No se encontraron resultados");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<FiltroDTO>> Lista(string buscar)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p =>
                p.Nombre!.ToLower().Contains(buscar.ToLower()));

                List<FiltroDTO> lista = _mapper.Map<List<FiltroDTO>>(await consulta.ToListAsync());
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<FiltroDTO> Obtener(int id)
        {
            try
            {
                var consulta = _modeloRepositorio.Consultar(p => p.IdFiltro == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                    return _mapper.Map<FiltroDTO>(fromDbModelo);
                else
                    throw new TaskCanceledException("No se encontraron coincidencias");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
