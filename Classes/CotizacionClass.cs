using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class CotizacionClass
    {

        // Llaves y Relaciones
        public int IdCotizacion { get; set; }
        public string NumeroCotizacion { get; set; }
        public int IdUsuario { get; set; }
        public int IdTasa { get; set; }
        public int IdProducto { get; set; }
        public int IdPlazo { get; set; }
        public int IdCotizante { get; set; }

        // Fechas y Montos
        public DateTime FechaCotizacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public decimal Monto { get; set; }
        public decimal TasaAplicada { get; set; }
        public decimal PorcentajeImpuesto { get; set; }

        // Totales Calculados
        public decimal MontoTotalInteresBruto { get; set; }
        public decimal MontoTotalImpuesto { get; set; }
        public decimal MontoTotalNeto { get; set; }

        // Propiedades de Ayuda para la Vista (Read-Only)
        public string NombreCotizante { get; set; }
        public string NombreProducto { get; set; }
        public string DescripcionPlazo { get; set; }

        // Campos de Auditoría
        public bool Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Lista de detalles (Opcional, para manejo Maestro-Detalle en C#)
        public List<DetalleCotizacionClass> Detalles { get; set; }

        public CotizacionClass()
        {
            Detalles = new List<DetalleCotizacionClass>();
        }
    }
}