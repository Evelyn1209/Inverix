using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class CotizanteClass
    {
        // Identificadores
        public int IdCotizante { get; set; }
        public int IdTipoIdentificacion { get; set; }

        // Propiedad para la Vista (proviene del JOIN con Tipo_Identificacion)
        public string TipoIdentificacionNombre { get; set; }

        // Datos Personales
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public bool Estado { get; set; }

        // Propiedad calculada para mostrar en el GridView
        public string NombreCompleto
        {
            get { return $"{Nombre} {PrimerApellido} {SegundoApellido}".Trim(); }
        }

        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}