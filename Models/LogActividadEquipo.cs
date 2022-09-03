using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class LogActividadEquipo
    {
        public int IdLogActividad { get; set; }
        public int IdUsuario { get; set; }
        public int IdLogLogin { get; set; }
        public DateTime? FechaIncial { get; set; }
        public DateTime? FechaFinal { get; set; }
    }
}
