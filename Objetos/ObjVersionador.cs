using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlProcesosApi.Objetos
{
    public class ObjVersionador
    {
        public ObjVersionador()
        {
            version = "";
            base64 = "";
            error = "";


        }
        public string version { get; set; }
        public string  base64 { get; set; }
        public string base64msi { get; set; }
        public string error { get; set; }

    }
}
