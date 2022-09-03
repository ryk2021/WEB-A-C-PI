using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class CatSubAreas
    {
        public int IdSubArea { get; set; }
        public int IdArea { get; set; }
        public string NombreSubArea { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int? Estado { get; set; }
    }
}
