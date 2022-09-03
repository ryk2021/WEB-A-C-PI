using System;
using System.Collections.Generic;

namespace ApiProcess.Models
{
    public partial class DetUsuarios
    {
        public int IdUsuario { get; set; }
        public int? IdPuesto { get; set; }
        public int? IdPerfil { get; set; }
        public int IdPais { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string CodEmpleado { get; set; }
        public string Correo { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int Activo { get; set; }
    }
}
