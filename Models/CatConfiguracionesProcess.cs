using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class CatConfiguracionesProcess
    {
        public int IdConfidguracion { get; set; }
        public string Llave { get; set; }
        public string Valor { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int? Estado { get; set; }
    }
}
