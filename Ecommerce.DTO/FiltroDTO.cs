using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DTO
{
    public class FiltroDTO
    {
        public int IdFiltro { get; set; }
        [Required(ErrorMessage = "Ingrese nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Ingrese Tipo")]
        public string? TipoFiltro { get; set; }
        public List<int> CategoriaIds { get; set; } = new List<int>();
        public List<string> OpcionesDisponibles { get; set; } = new(); 
        public List<string> ValoresSeleccionados { get; set; } = new();

        // Para texto simple
        public string? ValorUnico { get; set; }

        // Para filtros de rango numérico
        public decimal? ValorDesde { get; set; }
        public decimal? ValorHasta { get; set; }

        // Para distinguir tipo de filtro (puede venir desde API o definirse local)
        public bool EsRangoNumerico { get; set; } = false;
    }
}
