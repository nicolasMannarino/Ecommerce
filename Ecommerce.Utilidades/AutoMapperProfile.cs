using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.DTO;
using Ecommerce.Modelo;

namespace Ecommerce.Utilidades
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<Usuario, SesionDTO>();
            CreateMap<UsuarioDTO, Usuario>();

            CreateMap<Categoria, CategoriaDTO>();
            CreateMap<CategoriaDTO, Categoria>();

            CreateMap<Producto, ProductoDTO>().ForMember(dest => dest.Imagenes, opt => opt.MapFrom(src => src.ProductoImagenes));
            CreateMap<ProductoDTO, Producto>().ForMember(dest => dest.IdCategoriaNavigation,opt => opt.Ignore())
                                              .ForMember(dest => dest.ProductoImagenes, opt => opt.MapFrom(src => src.Imagenes));

            CreateMap<DetalleVenta, DetalleVentaDTO>();
            CreateMap<DetalleVentaDTO, DetalleVenta>();

            CreateMap<Venta, VentaDTO>();
            CreateMap<VentaDTO, Venta>();

            CreateMap<ProductoImagen, ProductoImagenDTO>();
            CreateMap<ProductoImagenDTO, ProductoImagen>();

            CreateMap<Filtro, FiltroDTO>()
                .ForMember(dest => dest.CategoriaIds, opt => opt.Ignore()); // Se setea manualmente en el servicio

            CreateMap<FiltroDTO, Filtro>()
                .ForMember(dest => dest.CategoriasFiltro, opt => opt.Ignore()); // Lo manejamos manualmente

            CreateMap<CategoriaFiltro, CategoriaFiltroDTO>();
            CreateMap<CategoriaFiltroDTO, CategoriaFiltro>();

            CreateMap<ProductoFiltroValor, ProductoFiltroValorDTO>().ReverseMap();


        }
    }
}
