using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class CatPerfiles
    {
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int? Estado { get; set; }
    }
}
