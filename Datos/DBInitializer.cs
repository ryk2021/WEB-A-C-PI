using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ApiProcess.Funciones;
using ApiProcess.Models;

namespace ControlProcesosApi.Datos
{
    public class DBInitializer
    {
        public static void Initialize(ProcessManagerContext context)
        {
            context.Database.EnsureCreated();

            if (context.CatPais.Any())
            {
                return;
            }

            var _CatPaises = new CatPais[]
            {
                new CatPais{ NombrePais = "El Salvador",Estado = 1 },
                new CatPais{ NombrePais = "Guatemala",Estado = 1 }
            };

            foreach (CatPais c in _CatPaises)
            {
                context.CatPais.Add(c);
            }

            context.SaveChanges();

            int PaisSV = _CatPaises.Where(t => t.NombrePais == "El Salvador").Select(x => x.IdPais).Single();

            // Insertar Areas
            List<string> RegistroAreasSV = new List<string>();
            using (StreamReader sr = new StreamReader(File.OpenRead("Datos/CargaAreas.csv")))
            {
                string file = sr.ReadToEnd();
                RegistroAreasSV = new List<string>(file.Split("\r\n"));
            }

            List<CatAreas> ListaAreas = new List<CatAreas>();
            foreach (string area in RegistroAreasSV)
            {
                CatAreas Area = new CatAreas();
                Area.NombreArea = area;
                Area.IdPais = PaisSV;
                Area.Estado = 1;
                ListaAreas.Add(Area);
            }

            foreach (CatAreas c in ListaAreas)
            {
                context.CatAreas.Add(c);
            }

            context.SaveChanges();

            // Insertar SubAreas
            List<string> RegistroSubAreasSV = new List<string>();
            using (StreamReader sr = new StreamReader(File.OpenRead("Datos/CargaSubAreas.csv")))
            {
                string file = sr.ReadToEnd();
                RegistroSubAreasSV = new List<string>(file.Split("\r\n"));
            }

            List<CatSubAreas> ListaSubAreas = new List<CatSubAreas>();
            foreach (string itemsubarea in RegistroSubAreasSV)
            {
                CatSubAreas SubArea = new CatSubAreas();
                string[] txtReg = itemsubarea.Split("|");
                string NombreArea = txtReg[0];
                SubArea.NombreSubArea = txtReg[1];
                SubArea.IdArea = ListaAreas.Where(t => t.NombreArea == NombreArea).Select(t => t.IdArea).FirstOrDefault();
                SubArea.Estado = 1;
                ListaSubAreas.Add(SubArea);
            }

            foreach (CatSubAreas c in ListaSubAreas)
            {
                context.CatSubAreas.Add(c);
            }

            context.SaveChanges();

            // Insertar Puestos
            List<string> RegistroPuestosSV = new List<string>();
            using (StreamReader sr = new StreamReader(File.OpenRead("Datos/CargaPuestos.csv")))
            {
                string file = sr.ReadToEnd();
                RegistroPuestosSV = new List<string>(file.Split("\r\n"));
            }

            List<CatPuestos> ListaPuestos = new List<CatPuestos>();
            foreach (string itempuestos in RegistroPuestosSV)
            {
                CatPuestos Puesto = new CatPuestos();
                string[] txtReg = itempuestos.Split("|");
                string NombreSubArea = txtReg[0];
                Puesto.NombrePuesto = txtReg[1];
                Puesto.IdSubArea = ListaSubAreas.Where(t => t.NombreSubArea == NombreSubArea).Select(t => t.IdSubArea).FirstOrDefault();
                Puesto.Estado = 1;
                ListaPuestos.Add(Puesto);
            }

            foreach (CatPuestos c in ListaPuestos)
            {
                context.CatPuestos.Add(c);
            }

            context.SaveChanges();

            // Usuarios Iniciales

            var IdPuestoSV = ListaAreas.Where(t => t.NombreArea == "TECNOLOGIA")
                .Join(
                ListaSubAreas.AsQueryable(),
                _area => _area.IdArea,
                _subarea => _subarea.IdArea,
                (_area, _subarea) => new { _subarea.IdSubArea, _subarea.NombreSubArea }
                ).Where(y => y.NombreSubArea == "DESARROLLO DE SISTEMAS").Select(x => new { x.IdSubArea })
                .Join(
                ListaPuestos,
                _areaSub => _areaSub.IdSubArea,
                _puestos => _puestos.IdSubArea,
                (_areaSub, _puestos) => new { _puestos.IdSubArea, _puestos.NombrePuesto, _puestos.IdPuesto }
                ).ToList();

            int Analista = IdPuestoSV.Where(x => x.NombrePuesto == "ANALISTA DE DESARROLLO INTERNO").Select(x => x.IdPuesto).FirstOrDefault();
            int Jefe = IdPuestoSV.Where(x => x.NombrePuesto == "JEFE DE DESARROLLO DE SISTEMAS").Select(x => x.IdPuesto).FirstOrDefault();
            // Llave encriptación
            CatConfiguracionesProcess _ConfProcess = new CatConfiguracionesProcess();
            _ConfProcess.Llave = "Encriptacion";
            _ConfProcess.Valor = "AtentoSocial2020";
            context.CatConfiguracionesProcess.Add(_ConfProcess);

            context.SaveChanges();

            string LlaveEncrip = context.CatConfiguracionesProcess.Select(t => t.Valor).Single();

            // Crear Perfiles Iniciales
            var _Roles = new CatPerfiles[]
            {
                new CatPerfiles { NombrePerfil = "Administrador"},
                new CatPerfiles { NombrePerfil = "Desarrollo"},
                new CatPerfiles { NombrePerfil = "Reporteria"}
            };

            foreach (CatPerfiles c in _Roles)
            {
                context.CatPerfiles.Add(c);
            }

            context.SaveChanges();

            int PerfilIni = _Roles.Where(x => x.NombrePerfil == "Desarrollo").Select(x => x.IdPerfil).SingleOrDefault();

            // Usuarios Default
            Encriptador refEncriptador = new Encriptador();
            var _Usr = new DetUsuarios[]
            {
                new DetUsuarios {Nombres = "Aristides Alexander",Apellidos="Diaz Castillo",CodEmpleado = "503013168",Correo="aristides.diaz@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="adiaz",Clave=refEncriptador.Encrypt("admin", false, _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Ana Margarita",Apellidos="Ramirez Molina",CodEmpleado = "503012958",Correo="margarita.ramirez@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="aramirez",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Carlos Vicente",Apellidos="Chavez Navas",CodEmpleado = "503010608",Correo="vicente.navas@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="cchavez",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Carlos Ernesto",Apellidos="Guerrero Solorzano",CodEmpleado = "503013694",Correo="cguerrero@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="cguerrero",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Oscar Francisco",Apellidos="Flores Duran",CodEmpleado = "503018232",Correo="oscar.flores@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="oflores",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Bryan Ricardo",Apellidos="Rodas Velasquez",CodEmpleado = "503018253",Correo="bryan.rodas@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="brodas",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Denis Gerson",Apellidos="Perez Presentacion",CodEmpleado = "503020395",Correo="denis.perez@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="dperez",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Edgard Armando",Apellidos="García Amaya",CodEmpleado = "503020396",Correo="edgard.garcia@atento.com",IdPais = PaisSV,IdPuesto=Jefe,IdPerfil=PerfilIni,Usuario="egarcia",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Pedro Angel",Apellidos="Molina Saravia",CodEmpleado = "503020680",Correo="pedro.molina@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="pmolina",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Roberto Antonio",Apellidos="de la Cruz Lemus",CodEmpleado = "503020681",Correo="antonio.lemus@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="rlemus",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1},
                new DetUsuarios {Nombres = "Erika Esmeralda",Apellidos="Lopez Pimentel",CodEmpleado = "503020682",Correo="erika.lopez@atento.com",IdPais = PaisSV,IdPuesto=Analista,IdPerfil=PerfilIni,Usuario="elopez",Clave=refEncriptador.Encrypt("admin", false,  _ConfProcess.Valor),Activo=1}
            };

            foreach (DetUsuarios c in _Usr)
            {
                context.DetUsuarios.Add(c);
            }

            context.SaveChanges();

            // Datos Navegadores
            var _DetProcesoIns = new DetTipoProceso[]
            {
                    new DetTipoProceso {NombreProceso = "chrome",TipoProceso = "Internet"},
                    new DetTipoProceso {NombreProceso = "msedge",TipoProceso="Internet"},
                    new DetTipoProceso {NombreProceso = "iexplore",TipoProceso="Internet"},
                    new DetTipoProceso {NombreProceso = "brave",TipoProceso="Internet"}
            };

            foreach (DetTipoProceso c in _DetProcesoIns)
            {
                context.DetTipoProceso.Add(c);
            }

            context.SaveChanges();
        }
    }
}
