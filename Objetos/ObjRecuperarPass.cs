using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProcess.Objetos
{
    public class ObjRecuperarPass
    {
        ObjRecuperarPass()
        {
            IdUsuario = 0;
            Correo = "";
        }
    public int IdUsuario { get; set; }
    public string Correo { get; set; }


    }
}
