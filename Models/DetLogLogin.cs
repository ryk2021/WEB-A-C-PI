using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class DetLogLogin
    {
        public int IdLogLogin { get; set; }
        public int IdUsuario { get; set; }
        public DateTime? FechaLogin { get; set; }
    }
}
