using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProcess.Objetos
{
    public class ObjTiempo
    {
        public int Id { get; set; }
        public string TimerName { get; set; }
        public string Descripcion { get; set; }
        public int? Tiempo { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? Estado { get; set; }


        public ObjTiempo()
        {
            Id = 0;
            TimerName = "";
            Descripcion = "";
            Tiempo = 0;
            DateCreated = null;
            Estado = 0;
        }
    }
}
