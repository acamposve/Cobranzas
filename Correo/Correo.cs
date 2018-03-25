using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using Entidades;

namespace Cobranzas
{
    public partial class Cobranzas_Correo : ServiceBase
    {
        private Timer serviceTimer;
        public Boolean Ocupado;
        public Dictionary<Int32, Thread> Hilos = new Dictionary<int, Thread>();

        public Cobranzas_Correo()
        {
            InitializeComponent();
        }
        public Int32 idProceso;
        protected override void OnStart(string[] args)
        {
            TimerCallback timerDelegate = new TimerCallback(LeerCorreos);
            serviceTimer = new Timer(timerDelegate, null, 10 * 1000, 7 * 60 * 1000);
        }

        protected override void OnStop()
        {
        }
        protected string Parametro(string Parametro, CobranzasDataContext db)
        {
            try
            {
                return db.Parametros.Single(x => x.Clave == Parametro).Valor;
            }
            catch
            {
                return null;
            }
        }
        protected Int32 ObtenerProceso()
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Int32 idProceso = db.Logs.Where(x => x.TipoProceso == "Correos").Max(x => x.idProceso) + 1;
                    return idProceso;
                }
                catch
                {
                    return 1;
                }
            }
        }
        protected void Log(string Msg, Int32 idProceso, Int32 Indice = 0)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Logs Log = new Entidades.Logs();
                Log.idProceso = idProceso;
                Log.Descripcion = Msg;
                Log.Fecha = DateTime.Now;
                Log.Indice = Indice;
                Log.TipoProceso = "Correos";
                db.Logs.InsertOnSubmit(Log);
                db.SubmitChanges();
            }
            //System.IO.File.AppendAllText(String.Format(AppDomain.CurrentDomain.BaseDirectory + @"\Log{0}.log", DateTime.Now.ToString("yyyyMMdd")), Msg + "\n");
        }
        private void LeerCorreo(object IdOperador)
        {
            Int32 idProcesoPersonal = idProceso;
            try
            {
                Int32 idOperador = (int)IdOperador;
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<Entidades.CorreosFiltros> Filtros = db.CorreosFiltros./*Where(x => x.idOperador == idOperador).*/ToList();

                    String Ruta = Parametro("RutaCorreos", db);
                    Entidades.Operadores op = db.Operadores.Single(x => x.idOperador == idOperador);
                    Log("Leyendo: " + op.Nombre, idProcesoPersonal, idOperador);
                    String Servidor = (op.POP3Host ?? Parametro("POP3Host", db));
                    if (Servidor == null)
                    {
                        Log("Saltando, por no tener definido un servidor", idProcesoPersonal, idOperador);
                        return;
                    }
                    Int32 Puerto = (op.POP3Port ?? Convert.ToInt32(Parametro("POP3Port", db)));
                    Boolean SSL = op.POP3SSL ?? (Parametro("POP3SSL", db) == "1");
                    String Usuario = op.POP3Login;
                    String Password = op.POP3Password;
                    if (op.POP3Password == null)
                    {
                        Log("Saltando, por no tener contraseña", idProcesoPersonal, idOperador);
                        return;
                    }
                    DateTime? UltimaFecha = op.UltimaFechaCorreoEntrante;
                    using (OpenPop.Pop3.Pop3Client POP3 = new OpenPop.Pop3.Pop3Client())//Iniciando el servidor POP3;
                    {
                        POP3.Connect(Servidor, Puerto, SSL);
                        POP3.Authenticate(Usuario, Password);
                        int Count = POP3.GetMessageCount();

                        Int32 Inicio = Count;
                        while (true)
                        {
                            OpenPop.Mime.Header.MessageHeader mh = POP3.GetMessageHeaders(Inicio);
                            if (db.Correos.Any(x => x.idOperador == idOperador && x.IdPop3 == mh.MessageId)) break;
                            Inicio--;
                            if (Inicio == 0) break;
                        }
                        Inicio++;
                        //Inicio = UltimaFecha == null ? 1 : BuscarIdPorFecha(1, Count, POP3, UltimaFecha.Value);
                        //Inicio -= 4;
                        //if (Inicio < 1) Inicio = 1;


                        Log(op.Login + " Inicio:" + Inicio + ", Total:" + Count, idProcesoPersonal, idOperador);
                        Int32 ErroresSeguidos = 0;
                        if (Inicio > Count)
                        {
                            Log("No hay correos nuevos para: " + op.Login, idProcesoPersonal, idOperador);
                        }
                        for (int i = Inicio; i <= Count; i++)//últimos 5 correos para verificar.
                        {
                            if (ErroresSeguidos == 5)
                            {
                                Log("Abortando Lectura de " + op.Login + " Por 5 erorres consecutivos", idProcesoPersonal, idOperador);
                                break;
                            }
                            try
                            {
                                OpenPop.Mime.Header.MessageHeader mh = POP3.GetMessageHeaders(i);

                                /*if (UltimaFecha != null && mh.DateSent.ToLocalTime() <= UltimaFecha)
                                {
                                    Log("Saltando Mensaje", idProcesoPersonal, i);
                                    continue;
                                }*/
                                if (db.Correos.Any(x => x.idOperador == idOperador && x.IdPop3 == mh.MessageId))
                                {
                                    Log("Saltando Mensaje de " + op.Login + " " + i.ToString() + "/" + Count.ToString(), idProcesoPersonal, idOperador);
                                    continue;
                                }
                                Log("Leyendo Mensaje de " + op.Login + " " + i.ToString() + "/" + Count.ToString(), idProcesoPersonal, idOperador);

                                OpenPop.Mime.Message m = POP3.GetMessage(i);
                                UltimaFecha = mh.DateSent.ToLocalTime();
                                String idLimpio = Limpiar(mh.MessageId);
                                String Directorio = UltimaFecha.Value.ToString("yyyyMMdd") + "\\";
                                String Prefijo = UltimaFecha.Value.ToString("mmss") + "_";
                                String RutaCompleta = Ruta + Directorio + Prefijo + idLimpio + ".eml";
                                Int32 idCorreoNuevo = 0;
                                if (!File.Exists(RutaCompleta))
                                {
                                    Directory.CreateDirectory(Ruta + Directorio);
                                    m.Save(new FileInfo(RutaCompleta));
                                }
                                Entidades.Correos Correo = new Entidades.Correos();
                                Correo.IdPop3 = mh.MessageId;
                                Correo.idOperador = idOperador;
                                Correo.Asunto = Limitar(mh.Subject, 2000);
                                Correo.Remitente = Limitar(mh.From.Raw, 500);
                                Correo.FechaCreacion = mh.DateSent.ToLocalTime();
                                Correo.Destinatarios = Limitar(String.Join(",", mh.To.Select(x => x.Raw)), 5000);
                                Correo.DestinatariosCopia = Limitar(String.Join(",", mh.Cc.Select(x => x.Raw)), 5000);
                                Correo.DestinatariosCopiaOculta = Limitar(String.Join(",", mh.Bcc.Select(x => x.Raw)), 5000);
                                Correo.RutaEml = Directorio + Prefijo + idLimpio;
                                var Personas = Filtros.Where(x => mh.From.Address.ToLower() == x.De.ToLower()).Select(x => x.idPersona);
                                foreach (int idPersona in Personas)
                                {
                                    Correo.Correos_Personas.Add(new Entidades.Correos_Personas() { idPersona = idPersona });
                                }
                                db.Correos.InsertOnSubmit(Correo);
                                Correo.Fecha = DateTime.Now;
                                db.SubmitChanges();
                                idCorreoNuevo = Correo.idCorreo;

                                db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, op);
                                op.UltimaFechaCorreoEntrante = UltimaFecha;
                                db.SubmitChanges();
                                Log("Leido Mensaje de " + op.Login + " " + i.ToString() + "/" + Count.ToString() + "#" + idCorreoNuevo, idProcesoPersonal, idOperador);
                                ErroresSeguidos = 0;
                            }
                            catch (OpenPop.Pop3.Exceptions.InvalidLoginException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: IL Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.InvalidUseException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: IU Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.LoginDelayException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: LD Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.PopServerException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: PS Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.PopServerLockedException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: PSL Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.PopServerNotAvailableException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: PSNA Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.PopServerNotFoundException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: SNF Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (OpenPop.Pop3.Exceptions.PopClientException Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: PC Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                            catch (Exception Ex)
                            {
                                ErroresSeguidos++;
                                Log("Error en Mensaje: Op: " + op.Login + "(" + i.ToString() + "/" + Count.ToString() + "), Mensaje:" + Ex.Message, idProcesoPersonal, idOperador);
                            }
                        }
                        POP3.Disconnect();
                    }
                    Log("Fin Lectura: " + op.Nombre, idProcesoPersonal, idOperador);
                }
            }
            catch (Exception Ex)
            {
                if (Ex.Message == "Server did not accept user credentials")
                {
                    try
                    {
                        using (CobranzasDataContext db = new CobranzasDataContext())
                        {
                            db.Operadores.Single(x => x.idOperador == (Int32)IdOperador).POP3Password = null;
                            db.SubmitChanges();
                            db.Avisos.InsertOnSubmit(new Entidades.Avisos
                            {
                                Aviso = "El servidor ha rechazado su contraseña, por favor actualice su contraseña de correo nuevamente",
                                FechaAviso = DateTime.Now.AddMinutes(2),
                                FechaCrea = DateTime.Now,
                                FechaOriginal = DateTime.Now.AddMinutes(2),
                                idOperador = (Int32)IdOperador,
                                idOperadorCrea = 1,
                                VecesMostrada = 0
                            });
                            db.SubmitChanges();
                        }
                    }
                    catch { }
                }
                Log("Error General: Op:" + IdOperador + ", Mensaje:" + Ex.Message, idProcesoPersonal, (int)IdOperador);
            }
        }
        private String Limpiar(String Limpiar)
        {
            return Limpiar
                    .Replace("\\", "")
                    .Replace("/", "")
                    .Replace(":", "")
                    .Replace("*", "")
                    .Replace("\"", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("?", "")
                    .Replace("|", "");
        }
        private String Limitar(String Cadena, Int32 Longitud)
        {
            return (Cadena ?? "").Substring(0, Longitud > Cadena.Length ? Cadena.Length : Longitud);
        }
        private Int32 BuscarIdPorFecha(int Inicio, int Fin, OpenPop.Pop3.Pop3Client POP3, DateTime Fecha)
        {
            if (Inicio >= Fin) return Inicio;
            Int32 Medio = (Inicio + Fin) / 2;
            OpenPop.Mime.Header.MessageHeader mh = POP3.GetMessageHeaders(Medio);
            if (mh.DateSent.ToLocalTime() <= Fecha)
            {
                return BuscarIdPorFecha(Medio + 1, Fin, POP3, Fecha);
            }
            else
            {
                return BuscarIdPorFecha(Inicio, Medio - 1, POP3, Fecha);
            }
        }
        private void LeerCorreos(object state)
        {
            try
            {
                idProceso = ObtenerProceso();
                Log("Proceso Iniciado", idProceso);

                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Entidades.Operadores op in db.Operadores.Where(x => x.POP3Login != null && x.POP3Password != null))//Recorrer los operadores disponibles para revisar su correo
                    {
                        Thread Hilo;
                        if (Hilos.ContainsKey(op.idOperador) && Hilos[op.idOperador].ThreadState != System.Threading.ThreadState.Stopped) continue;
                        if (Hilos.ContainsKey(op.idOperador) && Hilos[op.idOperador].ThreadState == System.Threading.ThreadState.Stopped) Hilos.Remove(op.idOperador);

                        Hilo = new Thread(LeerCorreo);
                        Hilo.Start(op.idOperador);
                        Hilos.Add(op.idOperador, Hilo);
                    }
                }
            }
            catch (Exception Ex)
            {
                try
                {
                    Log("0" + Ex.Message, idProceso, 0);
                }
                catch { }
            }
        }
    }
}
