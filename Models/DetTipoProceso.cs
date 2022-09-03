using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class DetTipoProceso
    {
        public int IdTipoProceso { get; set; }
        public string NombreProceso { get; set; }
        public string TipoProceso { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? Estado { get; set; }
    }
}
