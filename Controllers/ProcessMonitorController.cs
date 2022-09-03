using ApiProcess.Funciones;
using ApiProcess.Models;
using ApiProcess.Objetos;
using ControlProcesosApi.Objetos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProcess.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class ProcessMonitorController : ControllerBase
    {

        public Encriptador refEncriptador = new Encriptador();
        private readonly ILogger<ProcessMonitorController> _logger;
        private readonly ProcessManagerContext Context;
        private readonly IConfiguration _config;
        public DateTime ahora = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public ProcessMonitorController(ILogger<ProcessMonitorController> logger, ProcessManagerContext ctx, IConfiguration config)
        {
            _logger = logger;
            this.Context = ctx;
            this._config = config;
        }
        //[HttpGet]
        public void RegistroError(string MensajeError)
        {
            DetErrorLog Errolog = new DetErrorLog
            {
                DescError = " Pila Error => " + MensajeError,
                FechaError = DateTime.Now,

            };
            Context.DetErrorLog.Add(Errolog);
            Context.SaveChanges();
        }

        public void RegistroErrorDesk(string MensajeError, int idusuario)
        {
            DetErrorLog Errolog = new DetErrorLog
            {
                DescError = " Pila Error => " + MensajeError,
                FechaError = DateTime.Now,
                IdUsuario = idusuario
            };
            Context.DetErrorLog.Add(Errolog);
            Context.SaveChanges();
        }

        [HttpGet]
        public async Task<ActionResult> GetPaises()
        {
            try
            {

                var query = await Task.Run(() => Context.CatPais.ToList());
                return Ok(query);
            }
            catch (Exception e)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = e.Message
                };
                //RegistroError(e.StackTrace, "WebApi");
                return Ok(error);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetDetalleProcesos()
        {
            try
            {
                var query = await Task.Run(() => Context.DetProcesoDetalle.ToList());
                return Ok(query);
            }
            catch (Exception e)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = e.Message
                };
                //RegistroError(e.StackTrace, "WebApi");
                return Ok(error);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(ObjLogin Login)
        {
            try
            {
                int AnioHoy = DateTime.Now.Year;
                int MesHoy = DateTime.Now.Month;
                int DiaHoy = DateTime.Now.Day;//.AddDays(0).Day;
                var FechaBusqueda = new DateTime(AnioHoy, MesHoy, DiaHoy);

                DetLogLogin insertLogLogin = new DetLogLogin();

                var ObjEncriptacion = Context.CatConfiguracionesProcess.Where(t => t.Llave == "Encriptacion").FirstOrDefault();
                string Clave = refEncriptador.Encrypt(Login.Clave, false, ObjEncriptacion.Valor);
                var Resultado = await Task.Run(() => Context.DetUsuarios.Where(x => x.Usuario == Login.Usuario && x.Clave == Clave).FirstOrDefault());
                if (Resultado != null)
                {

                    var ConsultaLogin = Context.DetLogLogin.Where(x => x.FechaLogin.Value >= FechaBusqueda && x.FechaLogin <= FechaBusqueda.AddDays(1) && x.IdUsuario == Resultado.IdUsuario).FirstOrDefault();// & filterBuilder1.Lte(x => x.FechaLogin, FechaBusqueda.AddDays(1)) & filterBuilder1.Eq(x => x.IdUsuario, Resultado.IdUsuario);

                    if (ConsultaLogin == null)
                    {

                        insertLogLogin.IdUsuario = Resultado.IdUsuario;
                        insertLogLogin.FechaLogin = DateTime.Now;//.AddDays(0);
                        Context.Entry(insertLogLogin).State = EntityState.Added;
                        Context.SaveChanges();
                    }
                    else
                    {
                        DetProcesoDetalle item = new DetProcesoDetalle();
                        item.ProcessId = "999999";
                        item.NombreProceso = "Reconexión";
                        item.Navegador = 0;
                        item.NombreTab = "Process Monitor";
                        item.FechaProceso = DateTime.Now;
                        item.IdUsuario = Resultado.IdUsuario;
                        item.IdLogLogin = ConsultaLogin.IdLogLogin;
                        item.FechaRegistro = DateTime.Now;
                        item.FechaActualizacion = DateTime.Now;
                        item.VentanaActual = 1;
                        Context.DetProcesoDetalle.Add(item);
                        Context.SaveChanges();
                    }

                    var Respuesta = Context.DetUsuarios.Where(x => x.IdUsuario == Resultado.IdUsuario)
                        .Join(
                    Context.DetLogLogin.Where(x => x.FechaLogin.Value >= FechaBusqueda && x.IdUsuario == Resultado.IdUsuario),
                    _DetUsuario => _DetUsuario.IdUsuario,
                    _DetLogLogin => _DetLogLogin.IdUsuario,
                    (_DetUsuario, _DetLogLogin) => new { _DetUsuario, _DetLogLogin }
                    ).Join(
                    Context.CatPais,
                    _DetUsrLogin => _DetUsrLogin._DetUsuario.IdPais,
                    _CatPais => _CatPais.IdPais,
                    (_DetUsrLogin, _CatPais) => new { _DetUsrLogin, _CatPais }
                    ).Select(t => new
                    {
                        t._DetUsrLogin._DetUsuario.IdUsuario,
                        t._DetUsrLogin._DetUsuario.Nombres,
                        t._DetUsrLogin._DetUsuario.Apellidos,
                        t._DetUsrLogin._DetUsuario.Correo,
                        t._DetUsrLogin._DetUsuario.IdPais,
                        t._DetUsrLogin._DetUsuario.Usuario,
                        t._DetUsrLogin._DetUsuario.Activo,
                        t._DetUsrLogin._DetLogLogin.IdLogLogin,
                        t._DetUsrLogin._DetLogLogin.FechaLogin,
                        t._CatPais.NombrePais
                    }).FirstOrDefault();
                    return Ok(Respuesta);
                }
                else
                {
                    return Ok(null);
                }



            }
            catch (Exception ex)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = ex.Message
                };
                RegistroError("Mensaje:  Metodo => Login " + ex.Message + " Pila de Error:" + ex.StackTrace);
                return Ok(null);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RecuperarClave(ObjRecuperarPass obj)
        {
            string LlaveEncrip = Context.CatConfiguracionesProcess.Where(x => x.Llave == "Encriptacion").Select(t => t.Valor).Single();
            string ClaveEncript = Context.DetUsuarios.Where(t => t.Correo == obj.Correo).Select(s => s.Clave).Single();
            string ClaveDecrip = refEncriptador.Decrypt(ClaveEncript, false, LlaveEncrip);

            var emailModel = new EmailModel(obj.Correo, "Clave Control Procesos", "Su clave es: " + ClaveDecrip, false);
            EmailHelper _emailHelper = new EmailHelper(_config);
            await Task.Run(() => _emailHelper.SendEmail(emailModel));
            //var Resultado = Task.Run(() => Context._DetUsuarios.AsQueryable().Where(x => x.Usuario == Login.Usuario && x.Clave == Clave).FirstOrDefault());
            //var Response = await Resultado;
            return Ok("Correo Enviado");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RegistraProceso(List<Proceso> proc)
        {
            try
            {
                if (proc.Count != 0)
                {

                    List<DetProcesoDetalle> lsproc = new List<DetProcesoDetalle>();
                    List<DetProcesoDetalle> _lsproc = new List<DetProcesoDetalle>();

                    List<int> ListId = new List<int>();
                    List<int> ListOldId = new List<int>();   

                    var _LogLogin = 0;
                    int _IdLogLogin;
                    int IDUSUARIO;


                    IDUSUARIO = proc.Select(x => x.IdUsuario).FirstOrDefault();
                    _IdLogLogin = proc.Select(x => x.IdLogLogin).FirstOrDefault();

                    var ventasactivas = Context.DetProcesoDetalle.Where(y => y.IdLogLogin == _IdLogLogin && y.IdUsuario == IDUSUARIO && y.VentanaActual ==1).ToList();

                    _LogLogin = await validar(proc);
                    if (_LogLogin != _IdLogLogin)
                    {
                        _IdLogLogin = _LogLogin;
                    }

                    //string EncuentraOldId = null;
                    var EncuentraOldId = Context.DetProcesoDetalle.Where(y => y.IdLogLogin == _IdLogLogin && y.IdUsuario == IDUSUARIO && y.VentanaActual == 1).Select(m => m.IdProceso).ToList();
                   
                    ListOldId = EncuentraOldId;
                    foreach (Proceso x in proc)
                    {
                        //EncuentraOldId = Context._DetProcesoDetalle.AsQueryable().Where(y => y.IdLogLogin == _IdLogLogin && y.IdUsuario == x.IdUsuario && y.VentanaActual == 1).Select(m => m.IdProceso).FirstOrDefault();
                        var EncuentraId = Context.DetProcesoDetalle
                            .Where(y => y.ProcessId == x.ProcessId && y.NombreProceso == x.NombreProceso
                            && y.NombreTab == x.NombreTab
                            && y.IdLogLogin == _IdLogLogin
                            && y.IdUsuario == x.IdUsuario
                            && y.VentanaActual == 1
                            )
                            .Select(x => x.IdProceso)
                            .FirstOrDefault();
                        foreach (var item in EncuentraOldId.ToList())
                        {
                            if (item == EncuentraId)
                            {
                                EncuentraOldId.Remove(item);
                            }
                        }

                        if (EncuentraId == 0)
                        {

                            DetProcesoDetalle item = new DetProcesoDetalle();
                            item.ProcessId = x.ProcessId;
                            item.NombreProceso = x.NombreProceso;
                            item.Navegador = x.Navegador;
                            item.NombreTab = x.NombreTab;
                            item.FechaProceso = x.FechaProceso;
                            item.IdUsuario = x.IdUsuario;
                            item.IdLogLogin = _IdLogLogin;
                            item.FechaRegistro = DateTime.Now;
                            item.FechaActualizacion = DateTime.Now;
                            item.VentanaActual = 1;
                            lsproc.Add(item);
                            Context.SaveChanges();
                        }
                        else
                        {
                            ListId.Add(EncuentraId);
                        }
                    }

                    if (ventasactivas.Count > 0)
                    {
                        if (ventasactivas[0].IdLogLogin != _IdLogLogin)
                        {
                            foreach (var item in ventasactivas)
                            {
                                DetProcesoDetalle procdeta = new DetProcesoDetalle();
                                procdeta = item;
                                procdeta.FechaActualizacion = DateTime.Now;
                                procdeta.VentanaActual = 0;
                                Context.DetProcesoDetalle.Update(procdeta);
                                Context.SaveChanges();
                            }
                        }                    
                    }


                    if (ListId.Count != 0)
                    {
                        var list = ListId.ToArray();
                        var elementos = Context.DetProcesoDetalle.Where(x => list.Contains(x.IdProceso)).ToList();
                        List<DetProcesoDetalle> listadetalle = new List<DetProcesoDetalle>();
                        foreach (var item in elementos)
                        {
                            DetProcesoDetalle procdeta = new DetProcesoDetalle();
                            procdeta = item;
                            procdeta.FechaActualizacion = DateTime.Now;
                            listadetalle.Add(procdeta);

                        }
                        Context.DetProcesoDetalle.UpdateRange(listadetalle);
                        Context.SaveChanges();
                    }

                    if (ListOldId.Count > 0)
                    {
                        var arrayFind = ListOldId.ToArray();
                        var elementos = Context.DetProcesoDetalle.Where(x => arrayFind.Contains(x.IdProceso)).ToList();
                        List<DetProcesoDetalle> listadetalle = new List<DetProcesoDetalle>();
                        foreach (var item in elementos)
                        {
                            DetProcesoDetalle procdeta = new DetProcesoDetalle();
                            procdeta = item;
                            procdeta.VentanaActual = 0;
                            listadetalle.Add(procdeta);

                        }
                        Context.DetProcesoDetalle.UpdateRange(listadetalle);
                        Context.SaveChanges();
                    }

                    if (lsproc.Count == 0)
                    {

                        await Task.Run(() =>
                        {

                            _lsproc = Context.DetProcesoDetalle.Where(x => x.IdLogLogin == _LogLogin).ToList();
                        });

                        return Ok(_lsproc);
                    }
                    else
                    {
                        Context.DetProcesoDetalle.AddRange(lsproc);
                        Context.SaveChanges();
                        await Task.Run(() =>
                        {
                            _lsproc = Context.DetProcesoDetalle.Where(x => x.IdLogLogin == _LogLogin).ToList();
                        });


                        return Ok(_lsproc);
                    }
                }
                else
                {
                    BloqueError error = new BloqueError
                    {
                        Error = 1,
                        DescError = "Objeto vacio"
                    };
                    RegistroError("Mensaje:  Metodo => RegistraProceso, objeto vacio, " + proc.ToString());
                    return Ok(null);
                }
            }
            catch (Exception ex)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = ex.Message
                };
                RegistroError("Mensaje:  Metodo => RegistraProceso " + ex.Message + " Pila de Error:" + ex.StackTrace);
                return Ok(error);
            }
        }

        public async Task<int> validar(List<Proceso> proc)
        {
            try
            {

                var _LogLogin = 0; int _IdLogLogin;
                DateTime FechaLogin = default; DateTime _dLogin = default; DateTime _sistema = default;
                int ConteoLogin = 0;

                _sistema = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                _IdLogLogin = proc.Select(x => x.IdLogLogin).FirstOrDefault();
                ConteoLogin = Context.DetLogLogin.Where(x => x.IdLogLogin == _IdLogLogin).Count();
                FechaLogin = Context.DetLogLogin.Where(x => x.IdLogLogin == _IdLogLogin).Select(y => y.FechaLogin.Value).FirstOrDefault();


                if (FechaLogin.Year == 0001)
                {
                    _dLogin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    _dLogin = _dLogin.AddDays(-1);
                }
                else
                {
                    if (ConteoLogin == 0)
                    {
                        _dLogin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        //_dLogin = _dLogin.AddDays(-1);
                    }
                    else
                    {
                        _dLogin = new DateTime(FechaLogin.Year, FechaLogin.Month, FechaLogin.Day);
                    }

                }
                if (_sistema > _dLogin)
                {
                    DetLogLogin insertLogLogin = new DetLogLogin();
                    insertLogLogin.IdUsuario = proc[0].IdUsuario;
                    insertLogLogin.FechaLogin = DateTime.Now;
                    Context.DetLogLogin.Add(insertLogLogin);
                    Context.SaveChanges();
                    _LogLogin = insertLogLogin.IdLogLogin;
                    RegistroError("Mensaje:  Metodo => Fechas Login: " + _IdLogLogin + " , " + _sistema.ToLongDateString() + ", " + _dLogin.ToLongDateString());
                }
                else
                {
                    _LogLogin = _IdLogLogin;
                }

                return (_LogLogin);
            }
            catch (Exception ex)
            {
                return 0;
                throw;
            }

        }

        #region LogActividadEquipo
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LogActividadEquipo(ObjActividadEquipo obj)
        {
            try
            {

                LogActividadEquipo log = new LogActividadEquipo();

                if (obj.Id != 0)
                {
                    log = Context.LogActividadEquipo.Where(x => x.IdLogActividad == obj.Id).FirstOrDefault();
                    log.FechaFinal = DateTime.Now;
                    Context.Entry(log).State = EntityState.Modified;
                    Context.SaveChanges();

                    await Task.Run(() => Context.SaveChangesAsync());
                }
                else
                {
                    log.FechaIncial = DateTime.Now;
                    log.IdUsuario = obj.IdUsuario;
                    log.IdLogLogin = obj.IdLogLogin;
                    log.FechaFinal = new DateTime(1900, 1, 1);
                    Context.Entry(log).State = EntityState.Added;
                    await Task.Run(() => Context.SaveChangesAsync());
                }

                BloqueError error = new BloqueError
                {
                    Error = 0,
                    DescError = log.IdLogActividad.ToString()
                };
                return Ok(error);
            }
            catch (Exception ex)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = ex.Message
                };
                RegistroError("Mensaje: Metodo => LogActividadEquipo" + ex.Message + " Pila de Error:" + ex.StackTrace);
                return Ok(error);
            }
        }
        #endregion

        #region DetTiempoActividad
        [HttpGet]
        public async Task<ActionResult> TiempoActividad()
        {
            try
            {
                List<DetTiempoEspera> log = new List<DetTiempoEspera>();
                JsonSerializer serializer = new JsonSerializer();
                await Task.Run(() => log = Context.DetTiempoEspera.Where(x => x.Estado == 1).ToList());


                return Ok(new { Error = 0, DescError = log });
            }
            catch (Exception ex)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = ex.Message
                };
                RegistroError("Mensaje: Metodo => TiempoActividad  " + ex.Message + " Pila de Error:" + ex.StackTrace);
                return Ok(new { Error = 1, DescError = ex.Message });
            }
        }
        #endregion

        #region UpdateAplication

        [HttpPost]
        public async Task<ActionResult> VersionApplication(ObjVersionador obj)
        {

            ObjVersionador objVersionador = new ObjVersionador();
            objVersionador = obj;
            try
            {
                string fileName = "ProcessManager.exe";
                string fileNameMsi = "ProcessManagerInstall.msi";
                string fullPath = "";
                string version = @"Version\ProcessManager.exe";
                string version2 = @"Version\ProcessManagerInstall.msi";
                string pathZip = @"Version\Process.zip";
                await Task.Run(() =>
                {

                    fullPath = Path.GetFullPath(version);
                    //if (Directory.Exists(fullPath))
                    //{
                    System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.GetFullPath(fullPath));
                    System.Diagnostics.FileVersionInfo myFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(fullPath);
                    version = myFileVersionInfo.FileVersion;

                    if (objVersionador.version != version)
                    {
                        byte[] buffer = System.IO.File.ReadAllBytes(Path.GetFullPath(pathZip));
                        string base64Encoded = Convert.ToBase64String(buffer);
                        byte[] buffermsi = System.IO.File.ReadAllBytes(Path.GetFullPath(version2));
                        string base64Encodedmsi = Convert.ToBase64String(buffermsi);

                        objVersionador.base64 = base64Encoded;
                        objVersionador.version = version;
                        objVersionador.base64msi = base64Encodedmsi;
                    }
                    //}

                    objVersionador.error = "0";
                });





            }
            catch (Exception ex)
            {
                objVersionador.base64 = "";
                objVersionador.error = "1";
                objVersionador.version = "";

                RegistroError("Mensaje:  Metodo => VersionApplication " + ex.Message + " Pila de Error:" + ex.StackTrace);
            }



            return Ok(objVersionador);
        }
        #endregion

        #region Cierra tiempos de actividad de equipo
        [HttpPost]
        public async Task<ActionResult> BuscarTiempoEsperas(Proceso proc)
        {
            try
            {
                var ventanaActiva = Context.DetProcesoDetalle.Where(x => x.IdUsuario == proc.IdUsuario && x.VentanaActual == 1).ToList();
                foreach (var item in ventanaActiva)
                {
                    DetProcesoDetalle a = new DetProcesoDetalle();
                    a = item;
                    a.VentanaActual = 0;
                    Context.DetProcesoDetalle.Update(a);
                    Context.SaveChanges();
                }

                var FechaFinal = new DateTime(1900, 1, 1);
                var tiempos = Context.LogActividadEquipo.Where(x => x.IdUsuario == proc.IdUsuario && x.FechaFinal == FechaFinal).OrderByDescending(x => x.FechaIncial).FirstOrDefault();
                if (tiempos != null)
                {

                    proc.FechaProceso = DateTime.Now;
                    proc.IdLogLogin = tiempos.IdLogLogin;
                    proc.NombreProceso = "Actividad de equipo";
                    proc.Navegador = 0;
                    proc.NombreTab = "Reanudar";
                    proc.ProcessId = "900000001";

                    var p = new List<Proceso>();
                    p.Add(proc);
                    await RegistraProceso(p);
                    await Task.Run(() =>
                    {
                        CerrarTiempoEspera(proc.IdUsuario);
                    });
                }


                return Ok();
            }
            catch (Exception e)
            {
                BloqueError error = new BloqueError
                {
                    Error = 1,
                    DescError = e.Message
                };
                return Ok(error);

            }
        }

        public void CerrarTiempoEspera(int idusuario)
        {
            try
            {
                var fe = DateTime.Now;
                var FechaFinal = new DateTime(1900, 1, 1);
                var actividades = Context.LogActividadEquipo.Where(x => x.IdUsuario == idusuario && x.FechaFinal == FechaFinal).ToList();
                var listado = actividades.Select(x => x.IdLogActividad).ToArray();
                List<LogActividadEquipo> lista = new List<LogActividadEquipo>();
                foreach (var item in actividades)
                {
                    item.FechaFinal = DateTime.Now;
                    lista.Add(item);

                }

                Context.LogActividadEquipo.UpdateRange(lista);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        #region Cerrar tiempo de almuerzo
        public async Task<List<DetProcesoDetalle>> CerrarTiempoAlmuerzo(Proceso proc)
        {
            try
            {
                List<DetProcesoDetalle> lista = new List<DetProcesoDetalle>();
                List<Proceso> listaProceso = new List<Proceso>();

                string[] idprocessAlmuerzo = { "7000000000", "7000000001" };
                lista = Context.DetProcesoDetalle.Where(x => x.FechaRegistro >= ahora && x.IdUsuario == proc.IdUsuario && idprocessAlmuerzo.Contains(x.ProcessId)).ToList();
                if (lista.Count > 0)
                {
                    var Entradas = lista.Where(x => x.ProcessId == "7000000000").ToList();
                    var Salidas = lista.Where(x => x.ProcessId == "7000000001").ToList();

                    foreach (var item in Entradas)
                    {
                        var segEntrada = item.NombreTab.Split(':')[1];
                        var bandera = false;
                        foreach (var s in Salidas)
                        {
                            var segSalida = s.NombreTab.Split(':')[1];

                            if (segEntrada == segSalida)
                            {
                                bandera = true;

                            }
                        }
                        if (!bandera)
                        {
                            Proceso p = new Proceso();
                            p.FechaProceso = DateTime.Now;
                            p.NombreProceso = "Almuerzo";
                            p.IdLogLogin = item.IdLogLogin;
                            p.IdUsuario = proc.IdUsuario;
                            p.Navegador = 0;
                            p.NombreTab = "Finaliza Almuerzo segmento:" + segEntrada;
                            p.ProcessId = "7000000001";                            
                            listaProceso.Add(p);
                            await RegistraProceso(listaProceso);
                        }

                    }


                }



                lista = Context.DetProcesoDetalle.Where(x => x.FechaRegistro >= ahora && x.IdUsuario == proc.IdUsuario && idprocessAlmuerzo.Contains(x.ProcessId)).ToList();

                return await Task.Run(() => lista);
            }
            catch (Exception ex)
            {
                RegistroError("Mensaje:  Metodo => CerrarAlmuerzo " + ex.Message + " Pila de Error:" + ex.StackTrace);
                return new List<DetProcesoDetalle>();

            }

        }
        #endregion

        #region GetTiempo Server
        public ActionResult GetFechaServer()
        {
            return Ok(DateTime.Now);
        }
        #endregion

    }


    public class BloqueError
    {
        public int Error { get; set; }
        public string DescError { get; set; }

        public BloqueError()
        {
            Error = 0;
            DescError = "";
        }
    }
}
