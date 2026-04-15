using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class UsuarioClass
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public bool Estado { get; set; }
        public string Rol { get; set; } // Propiedad extendida para la vista de listado
        public string RolCodigo { get; set; } // Código del rol: 'ADM', 'COT'
        public int? IdCotizante { get; set; } // Id del perfil Cotizante (si aplica)
    }
}
