using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class DetErrorLog
    {
        public int IdError { get; set; }
        public int? IdUsuario { get; set; }
        public string DescError { get; set; }
        public DateTime? FechaError { get; set; }
    }
}
