using System;

namespace ApiProcess.Objetos
{
    public class Proceso
    {
        public string ProcessId { get; set; }
        public string NombreProceso { get; set; }
        public int Navegador { get; set; }
        public string NombreTab { get; set; }
        public DateTime FechaProceso { get; set; }
        public int IdUsuario { get; set; }
        public int IdLogLogin { get; set; }
        public Proceso()
        {
            ProcessId = "";
            NombreProceso = "";
            Navegador = 0;
            NombreTab = "";
            FechaProceso = new DateTime();
            IdUsuario = 0;
            IdLogLogin = 0;
        }
    }
}
