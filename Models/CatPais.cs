using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProcess.Models
{
    public partial class CatPais
    {
        public int IdPais { get; set; }
        public string NombrePais { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int? Estado { get; set; }
        [NotMapped]
        public string IdArea { get; set; }
        [NotMapped]
        public string IdSubArea { get; set; }
   }
}
