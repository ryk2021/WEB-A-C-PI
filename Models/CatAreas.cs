using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProcess.Models
{
    public partial class CatAreas
    {
        public int IdArea { get; set; }
        public int IdPais { get; set; }
        public string NombreArea { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int? Estado { get; set; }      
    }
}
