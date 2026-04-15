using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class TasaClass
    {
        public int IdTasa { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } // Para vistas
        public int IdPlazo { get; set; }
        public int PlazoDias { get; set; } // Para vistas
        public decimal Porcentaje { get; set; }
        public string NombreMoneda { get; set; } // Nueva propiedad
        public bool Estado { get; set; }

        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}