using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class CatPuestos
    {
        public int IdPuesto { get; set; }
        public int IdSubArea { get; set; }
        public string NombrePuesto { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? Estado { get; set; }
    }
}
