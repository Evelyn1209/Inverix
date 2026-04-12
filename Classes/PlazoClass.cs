using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class PlazoClass
    {

        public int IdPlazo { get; set; }
        public int Dias { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }

        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}