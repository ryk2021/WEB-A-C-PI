using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class DetProcesoDetalle
    {
        public int IdProceso { get; set; }
        public int IdUsuario { get; set; }
        public int IdLogLogin { get; set; }
        public string ProcessId { get; set; }
        public string NombreProceso { get; set; }
        public int? Navegador { get; set; }
        public string NombreTab { get; set; }
        public DateTime? FechaProceso { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int? VentanaActual { get; set; }
    }
}
