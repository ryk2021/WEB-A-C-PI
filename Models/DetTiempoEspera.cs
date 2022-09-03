using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class DetTiempoEspera
    {
        public int IdTiempo { get; set; }
        public int? Tiempo { get; set; }
        public string Descripcion { get; set; }
        public string TimerName { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Estado { get; set; }
    }
}
