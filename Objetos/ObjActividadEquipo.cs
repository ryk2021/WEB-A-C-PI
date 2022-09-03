using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProcess.Objetos
{
    public class ObjActividadEquipo
    {
        public ObjActividadEquipo()
        {

            IdUsuario =0;
            Id = 0;
            IdLogLogin = 0;
            FechaInicial = null;
            FechaFinal = null;
        }

        public int IdUsuario { get; set; }
        public int Id { get; set; }
        public int IdLogLogin { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }

    }
}
