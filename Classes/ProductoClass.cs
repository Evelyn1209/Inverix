using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class ProductoClass
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int IdMoneda { get; set; }
        public string NombreMoneda { get; set; } // Para vistas
        public string CodigoMoneda { get; set; } // Para vistas
        public string SimboloMoneda { get; set; } // Para vistas
        public bool Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}