using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProcess.Objetos
{
    public class ObjLogin
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }

        public ObjLogin()
        {
            Usuario = "";
            Clave = "";
        }
    }
}
