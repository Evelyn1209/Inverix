using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class DetalleCotizacionClass
    {

        public int IdDetalleCotizacion { get; set; }
        public int IdCotizacion { get; set; }
        public int NumeroMes { get; set; }
        public decimal Monto { get; set; }
        public decimal InteresBruto { get; set; }
        public decimal Impuesto { get; set; }
        public decimal InteresNeto { get; set; }

        // Campos de Auditoría
        public bool Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}