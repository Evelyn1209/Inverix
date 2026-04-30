using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inverix.Classes
{
    public class AuditoriaClass
    {
        public int IdAuditoria { get; set; }
        public string SeccionAfectada { get; set; }
        public string CuentaAfectada { get; set; }
        public string AccionRealizada { get; set; }
        public string DetalleDelCambio { get; set; }
        public string CreadoOriginalmentePor { get; set; }
        public DateTime FechaHoraDeCreacion { get; set; }
        public string ActualizadoPor { get; set; }
        public DateTime FechaHoraDeActualizacion { get; set; }
    }
}
