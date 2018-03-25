using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Data.OleDb;
using System.Xml.Linq;
using System.Net;
using Cobranzas;
using System.Xml.XPath;
using System.IO;
using CentralIp;
using Entidades;

namespace Cobranzas
{
    public partial class Transporte : ServiceBase
    {
        private static Boolean Actualizando = false;
        private static Boolean Avanzando = false;
        private static Boolean Trayendo = false;
        private static Boolean Llevando = false;
        private static Boolean Procesando = false;
        private Timer tmrActualizar;
        private Timer tmrAvanzar;
        private Timer tmrTraer;
        private Timer tmrLlevar;
        private Timer tmrProcesar;
        public Transporte()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            General.FormatoFechaW = "ddd dd/MM/yyyy";
            General.FormatoFecha = "dd/MM/yyyy";
            General.FormatoFechaHora = "dd/MM/yyyy hh:mmtt";
            Int32 Seg = 1000;
            Int32 Min = 60 * Seg;
            Int32 Hor = 60 * Min;
            tmrActualizar = new Timer(new TimerCallback(Actualizar), null, 2 * Min, 25 * Min);//2Min
            tmrAvanzar = new Timer(new TimerCallback(Avanzar), null, 20 * Seg, 30 * Min);//2Min
            tmrProcesar = new Timer(new TimerCallback(Procesar), null, 15 * Seg, 6 * Min);//5Min
            tmrTraer = new Timer(new TimerCallback(Traer), null, 30 * Seg, 45 * Min);//15Seg
            tmrLlevar = new Timer(new TimerCallback(Llevar), null, 1 * Min, 5 * Min); //1Min
        }
        protected Int32 ObteneridProceso(CobranzasDataContext db, String TipoProceso)
        {
            try
            {
                return db.Logs.Where(x => x.TipoProceso == TipoProceso).Max(x => x.idProceso) + 1;
            }
            catch
            {
                return 1;
            }
        }
        protected void Log(CobranzasDataContext db, String TipoProceso, int idProceso, String Descripcion, int Indice)
        {
            try
            {
                Entidades.Logs Log = new Entidades.Logs { idProceso = idProceso, Descripcion = Descripcion, Indice = Indice, Fecha = DateTime.Now, TipoProceso = TipoProceso };
                db.Logs.InsertOnSubmit(Log);
                db.SubmitChanges();
            }
            catch { }
        }
        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Log(db, "General", 0, "Recibido Comando", command);
                String Mensaje = "Respuesta:";
                switch (command)
                {
                    case 128: //Estado de Temporizadores
                        Mensaje += "\r\n Trayendo=" + (Trayendo ? "Sí" : "No");
                        Mensaje += "\r\n Actualizando=" + (Actualizando ? "Sí" : "No");
                        Mensaje += "\r\n Avanzando=" + (Actualizando ? "Sí" : "No");
                        Mensaje += "\r\n Llevando=" + (Llevando ? "Sí" : "No");
                        Mensaje += "\r\n Procesando=" + (Procesando ? "Sí" : "No");
                        break;
                    case 129: Traer(null);
                        break;
                    case 130: Actualizar(null);
                        break;
                    case 131: Avanzar(null);
                        break;
                    case 133: Llevar(null);
                        break;
                    case 134: Procesar(null);
                        break;
                    case 200: ActualizarDigital(1, false);
                        break; //Actualizar digital de cuentas inactivas
                    case 201: MoverCorreos(); break;
                    case 202: EliminarCorreos(); break;
                    case 203: break;
                    case 10: break;
                    case 11: break;
                    case 12: break;
                    case 13: break;
                    case 14: break;
                    case 15: break;
                    case 16: break;
                    case 17: break;
                    case 18: break;
                    case 19: break;
                    case 250: Proceso(1); break;
                    case 251: Proceso(2); break;
                    case 255: //Ayuda
                        Mensaje += "\r\n 128=>Estado de Temporizadores";
                        break;
                    default:
                        Mensaje += "\r\n Comando Desconocido";
                        break;
                }
                Log(db, "General", 0, Mensaje, command);
            }
        }
        protected override void OnStop()
        {
            try
            {
                tmrTraer.Dispose();
                tmrActualizar.Dispose();
                tmrAvanzar.Dispose();
                tmrLlevar.Dispose();
                tmrProcesar.Dispose();
            }
            catch (Exception Ex)
            {
            }
        }
        /*Actividades Principales*/
        private void Procesar(object state)
        {
            if (Procesando) return;
            Procesando = true;
            try
            {
                List<Int32> Origenes;
                List<Int32> CentralesIp;
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Origenes = db.Origenes.Select(x => x.idOrigen).ToList();
                    CentralesIp = db.CentralesIp.Select(x => x.idCentralIp).ToList();
                }
                foreach (Int32 idCentralIp in CentralesIp)
                {
                    ActualizarLlamadas(idCentralIp);
                }
                EliminarCorreos();
                //foreach (Int32 idOrigen in Origenes)
                //{
                //ActualizarDigital(1);
                //Proceso(1);
                //}
                ActualizacionEspecial(1);
                //Proceso(1);
                Proceso(2);
            }
            catch { }
            Procesando = false;
        }
        private void Actualizar(object state)
        {
            if (Actualizando) return;
            Actualizando = true;
            try
            {
                for (int i = 1; i <= 2; i++)
                {
                    ActualizarDatosCuentas(i);
                    ActualizarDatosPersonas(i);
                    ActualizarPersonasDesdeOrigen(i);
                    ActualizarCuentasDesdeOrigen(i);
                    ActualizarReclamosDesdeOrigen(i);
                    ActualizarPagosRDesdeOrigen(i);
                }

                ActualizarPagosTDesdeOrigen(1);
                ActualizarPagosComDesdeOrigen(1);
                ActualizarDigital(1);
                ActualizarObservacionesDesdeOrigen(1);


                //ActualizarPagosTDesdeOrigen(2);
                //ActualizarPagosComDesdeOrigen(2);
                //ActualizarDigital(2);
                //ActualizarObservacionesDesdeOrigen(2);
            }
            catch { }
            Actualizando = false;
        }
        private void Traer(object state)
        {
            if (Trayendo) return;
            Trayendo = true;
            try
            {
                TraerContactos(1);
                //TraerCuentasNuevas(2);
                for (int i = 1; i <= 2; i++)
                {
                    TraerCuentasNuevas(i);
                }
            }
            catch { }
            Trayendo = false;
        }
        private void Avanzar(object state)
        {
            if (DateTime.Now.Hour < 17 && DateTime.Now.Hour > 8) return;
            if (Avanzando) return;
            Avanzando = true;
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (db.Gestiones.Count(x => x.idStatus == 18 && x.Fecha.Date == DateTime.Now.Date) >= 300) { Avanzando = false; return; };
                    Int32 idProceso = ObteneridProceso(db, "Avanzar");
                    Log(db, "Avanzar", idProceso, "Inicio Proceso Avance", 0);

                    //{//Cuentas que deben salir de los Flujos
                    //    Log(db, "Avanzar", idProceso, "Sacando Cuentas de los Flujos", 0);
                    //    foreach (Entidades.Flujos Flujo in db.Flujos)
                    //    {
                    //        db.SacarCuentasDelFlujo(Flujo.idFlujo);
                    //    }
                    //}

                    //{//Cuentas que deben salir de los Flujos
                    //    Int32 i = 0;
                    //    var Cuentas = db.Cuentas.Where(x => x.Activa && x.idFlujo != null && (db.CumpleRegla(x.idCuenta, x.Flujos_Pasos.Flujos.idReglaSalida) ?? false)).ToList();
                    //    Log(db, "Avanzar", idProceso, "Sacando Cuentas de los Flujos, Cuentas=" + Cuentas.Count, 0);
                    //    i = 0;
                    //    foreach (Entidades.Cuentas Cuenta in Cuentas)
                    //    {
                    //        i++;
                    //        Log(db, "Avanzar", idProceso, "Sacando Cuenta: " + Cuenta.idCuenta, i);
                    //        if (Cuenta.Flujos_Pasos.Flujos.DesactivarAlSalir) Cuenta.Activa = false;
                    //        Cuenta.Flujos_Pasos = null;//Saca las cuentas del Flujo Actual
                    //        foreach (Entidades.Campanas_Cuentas CC in Cuenta.Campanas_Cuentas.Where(x => x.FechaFin == null))//Las Saca de Todas las Campañas
                    //        {
                    //            CC.FechaFin = DateTime.Now;
                    //        }
                    //        foreach (Entidades.Cuentas_Operadores CC in Cuenta.Cuentas_Operadores.Where(x => x.FechaFin == null))//Las saca de todos los operadores
                    //        {
                    //            CC.FechaFin = DateTime.Now;
                    //        }
                    //        db.SubmitChanges();
                    //    }
                    //}

                    ////Sacar cuentas de la campaña, por regla.
                    //{
                    //    foreach (Entidades.Campanas Campana in db.Campanas.Where(x => x.Activa))
                    //    {
                    //        Log(db, "Avanzar", idProceso, "Sacando Cuentas de la Campaña " + Campana.Nombre + " Por Regla", 0);
                    //        db.SacarCuentasDeCampana(Campana.idCampana);
                    //        //if (Campana.idReglaSalida == null) continue;
                    //        //List<Entidades.Campanas_Cuentas> CCS = Campana.Campanas_Cuentas.Where(x => x.FechaFin == null && (db.CumpleRegla(x.idCuenta, Campana.idReglaSalida) ?? false)).ToList();
                    //        //foreach (Entidades.Campanas_Cuentas CC in CCS)
                    //        //{
                    //        //    CC.FechaFin = DateTime.Now;
                    //        //    Log(db, "Avanzar", idProceso, "Sacando Cuenta de la Campaña " + Campana.Nombre + ", Cuenta=" + CC.idCuenta, 0);
                    //        //}
                    //    }
                    //}
                    /*Envío de notificaciones*/
                    {
                        Int32 Status = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "_STEnvioAutomatico").Valor);
                        Int32 Dias = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "DiasEntreNotificaciones").Valor);

                        //var Cuentas = db.Cuentas.Where(x => x.Activa && x.idOrigen == 1 && x.idPaso == 2 && !db.Cuentas_Gestiones.Any(y => y.Cuentas.idPersona == x.idPersona && y.Gestiones.idStatus == Status && y.Gestiones.Fecha > DateTime.Now.AddDays(-Dias))).OrderBy(x => x.idPersona).ToList();
                        List<Int32> idPersonas = db.Cuentas.Where(x => x.Activa && x.idOrigen == 1 && x.idPaso == 2 && !x.Campanas_Cuentas.Any(y => (y.idCampana == 8 || y.idCampana == 9 || y.idCampana == 6) && y.FechaFin == null) && !db.Cuentas_Gestiones.Any(y => y.Cuentas.idPersona == x.idPersona && y.Gestiones.idStatus == Status && y.Gestiones.Fecha > DateTime.Now.AddDays(-Dias))).Select(x => x.idPersona).Distinct().Take(100).ToList();
                        List<Entidades.Personas> Personas = db.Personas.Where(x => idPersonas.Contains(x.idPersona)).ToList();


                        Log(db, "Avanzar", idProceso, "Enviando Notificaciones Personas=" + Personas.Count, 0);
                        //Int32 PersonaAnterior = 0;
                        foreach (Entidades.Personas Persona in Personas)
                        {
                            try
                            {
                                Entidades.Operadores Operador;

                                try
                                {
                                    Operador = db.Cuentas_Operadores.Where(x => x.Cuentas.idPersona == Persona.idPersona && x.FechaFin == null).OrderByDescending(x => x.FechaInicio).First().Operadores;
                                }
                                catch
                                {
                                    Operador = db.Operadores.Single(x => x.idOperador == 1);
                                }

                                //if (PersonaAnterior == Persona.idPersona) continue;
                                if (db.Campanas_Cuentas.Any(x => x.Cuentas.idPersona == Persona.idPersona && (x.idCampana == 8 || x.idCampana == 9 || x.idCampana == 6) && x.FechaFin == null)) continue;
                                //if (Persona.Gestiones.Any(x => x.idStatus == Status && x.Fecha.Date >= DateTime.Now.Date.AddDays(-Dias))) continue;

                                //PersonaAnterior = Cuenta.idPersona;
                                Log(db, "Avanzar", idProceso, "Enviando Notificación a la Persona: " + Persona.idPersona, 0);
                                Entidades.Correos C = new Entidades.Correos();

                                if (Persona.Paises.Cultura.StartsWith("es"))
                                {
                                    C.Asunto = "Análisis de Antigüedad al " + DateTime.Now.Date.ToString("dd/MM/yyyy") + " Cliente: (" + Persona.Codigo + ")" + Persona.Nombre;
                                }
                                else
                                {
                                    C.Asunto = "Outstanding Accounts to " + DateTime.Now.Date.ToString("MM/dd/yyyy") + " - Veconinter - (" + Persona.Codigo + ")" + Persona.Nombre;
                                }

                                C.Mensaje = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">" +
                                    "<html><head>" + Comunes.AnalisisHead() + "</head><body>";
#warning Quemadoooooooo
                                if (Operador.idGrupo == 2)
                                {
                                    C.Mensaje += @"<a href=""http://cantv.veconinter.com.ve:8000/Encuesta.aspx?Codigo=VEN" + Persona.Codigo + @"""><img src=""http://cantv.veconinter.com.ve:8000/Encuesta.png"" /></a><br/>";
                                }
                                C.Mensaje += "<p>Estimados Sres.," + Persona.Nombre + "</p>" +
                                    "<p>Ante todo reciban un cordial saludo</p>" +
                                    "<p>Por medio de la presente hacemos llegar el estado de cuenta pendiente por pagar a Venezolana de Control Intermodal, Veconinter, C.A. para su revisión y pronto pago, podrán visualizar las facturas haciendo Click sobre cada una de ellas. </p>" +
                                    "<p>En caso de haber efectuado el pago de la deuda, favor hacer llegar el respectivo soporte (Deposito y/o Transferencia) a la siguiente dirección de correo: " + Operador.Grupos.Correo + "</p>" +
                                    "<p>Atentos a sus comentarios</p>" +
                                    Comunes.AnalisisCuerpo(Persona.idPersona, 1, db, null).ToString() +
                                    "Estimado Cliente: Lo invitamos a realizar su depósito o pago a través de nuestra PLANILLA PERSONALIZADA que ha puesto a disposición el banco BBVA Provincial para todos nuestros clientes y consignatarios, la cual encontrará adjunto a este correo. <br/> <br/>" +
                                    db.FirmaOperador(Operador.idOperador) + "</body></html>";

                                C.Destinatarios = Persona.Email;
                                C.FechaCreacion = DateTime.Now;
                                C.ResultadosAdjuntos = false;
                                C.Remitente = "Administrador<administrador@veconinter.com.ve>";
                                C.Adjuntos = Comunes.AnalisisReporte(Persona.idPersona, 1, null); //Administrador
                                C.Adjuntos += ";" + db.Parametros.Single(x => x.Clave == "RutaArchivos").Valor + "depositoProvincial.pdf";
                                C.Correos_Personas.Add(new Entidades.Correos_Personas() { idPersona = Persona.idPersona });
                                C.idOperador = 1;
                                C.Fecha = DateTime.Now;
                                db.Correos.InsertOnSubmit(C);

                                Entidades.Gestiones Gestion = new Entidades.Gestiones();
                                Gestion.idOperador = 1;
                                Gestion.idPersona = Persona.idPersona;
                                Gestion.idStatus = Status;
                                Gestion.Fecha = DateTime.Now;
                                Gestion.Descripcion = "Se envió Automáticamente el Análisis de Antigüedad";
                                db.Gestiones.InsertOnSubmit(Gestion);
                                db.SubmitChanges();

                            }
                            catch (Exception Ex)
                            {
                            }
                        }
                    }
                    //{
                    //    //Buscar Avance de cuentas por Pasos Manuales
                    //    Int32 i = 0;
                    //    foreach (Entidades.Pasos Paso in db.Pasos)
                    //    {
                    //        //Log(db, idProceso, "Avanzando Pasos, Paso=" + Paso.Nombre, 0);
                    //        //Busca todas las cuentas que están en el Paso actual y que cumplen alguna regla de avance
                    //        db.CommandTimeout = 10 * 60;
                    //                                    var Cuentas = db.Cuentas.Where(x => x.Activa && x.idFlujo != null && x.idPaso == Paso.idPaso && (Paso.Tipo == "Automático" || x.Flujos_Pasos.Pasos.FlujoAvance1.Any(y => y.idFlujo == x.idFlujo && (db.CumpleRegla(x.idCuenta, y.idRegla) ?? false)))).OrderBy(x => x.idPersona).ToList();
                    //        Log(db, "Avanzar", idProceso, "Avanzando Pasos, Paso=" + Paso.Nombre + " Cuentas=" + Cuentas.Count, 0);
                    //        Int32 PersonaAnterior = 0;
                    //        i = 0;
                    //        foreach (Entidades.Cuentas Cuenta in Cuentas)
                    //        {
                    //            i++;
                    //            Debug.Print("{0}/{1}", i, Cuentas.Count);
                    //            try
                    //            {
                    //                /*if (Cuenta.idFlujo == null)
                    //                {
                    //                    Log(db, idProceso, "Saltando Cuenta: " + Cuenta.idCuenta, i);
                    //                    continue;
                    //                }*/

                    //                foreach (Entidades.FlujoAvance FA in Cuenta.Flujos_Pasos.Pasos.FlujoAvance1)
                    //                {
                    //                    if (db.CumpleRegla(Cuenta.idCuenta, FA.idRegla) ?? false)
                    //                    {
                    //                        Log(db, "Avanzar", idProceso, "Avanzando un paso de la cuenta: " + Cuenta.idCuenta + " del paso: " + Cuenta.Flujos_Pasos.Pasos.Nombre + " al paso:" + FA.Pasos.Nombre + " por regla: " + FA.Reglas.Nombre, i);
                    //                        Cuenta.Flujos_Pasos = db.Flujos_Pasos.Single(x => x.idFlujo == FA.idFlujo && x.idPaso == FA.idPasoFinal);
                    //                        db.SubmitChanges();
                    //                    }
                    //                }
                    //                switch (Cuenta.Flujos_Pasos.Pasos.Tipo)
                    //                {
                    //                    case "Automático":
                    //                        switch (Cuenta.Flujos_Pasos.Pasos.idPaso)
                    //                        {
                    //                            case 1: //StandBy Inicial 
                    //                                break;
                    //                            case 2: //Envío notificación
                    //                                if (Cuenta.Personas.EnviosAutomaticos ?? false)
                    //                                {
                    //                                    //Excepciones de campañas
                    //                                    if (Cuenta.Campanas_Cuentas.Any(x => x.idCampana == 8 || x.idCampana == 9 || x.idCampana == 6)) break;
                    //                                    String TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                    //                                    Int32 Status = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "_STEnvioAutomatico").Valor);
                    //                                    Int32 Dias = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "DiasEntreNotificaciones").Valor);
                    //                                    if (PersonaAnterior == Cuenta.idPersona) break;
                    //                                    if (Cuenta.Personas.Gestiones.Any(x => x.idStatus == Status && x.Fecha.Date >= DateTime.Now.Date.AddDays(-Dias))) break;
                    //                                    Log(db, "Avanzar", idProceso, "Enviando Notificación de la cuenta: " + Cuenta.idCuenta, i);
                    //                                    Entidades.Correos C = new Entidades.Correos();
                    //                                    C.Asunto = "Análisis de Antigüedad al " + DateTime.Now.Date.ToString("dd/MM/yyyy") + " Cliente: (" + Cuenta.Personas.Codigo + ")" + Cuenta.Personas.Nombre;
                    //                                    Entidades.Operadores Operador;

                    //                                    try
                    //                                    {
                    //                                        Operador = db.Cuentas_Operadores.Where(x => x.idCuenta == Cuenta.idCuenta && x.FechaFin == null).OrderByDescending(x => x.FechaInicio).First().Operadores;
                    //                                    }
                    //                                    catch
                    //                                    {
                    //                                        Operador = db.Operadores.Single(x => x.idOperador == 1);
                    //                                    }

                    //                                    C.Mensaje = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">" +
                    //                                        "<html><head>" + Comunes.AnalisisHead() + "</head><body>" +
                    //                                        "<p>Estimados Sres.," + Cuenta.Personas.Nombre + "</p>" +
                    //                                        "<p>Ante todo reciban un cordial saludo</p>" +
                    //                                        "<p>Por medio de la presente hacemos llegar el estado de cuenta pendiente por pagar a Venezolana de Control Intermodal, Veconinter, C.A. para su revisión y pronto pago, podrán visualizar las facturas haciendo Click sobre cada una de ellas. </p>" +
                    //                                        "<p>En caso de haber efectuado el pago de la deuda, favor hacer llegar el respectivo soporte (Deposito y/o Transferencia) a la siguiente dirección de correo: " + Operador.Grupos.Correo + "</p>" +
                    //                                        "<p>Atentos a sus comentarios</p>" +
                    //                                        Comunes.AnalisisCuerpo(Cuenta.idPersona, 1, db, null).ToString() + db.FirmaOperador(Operador.idOperador) + "</body></html>";

                    //                                    C.Destinatarios = Cuenta.Personas.Email;
                    //                                    C.FechaCreacion = DateTime.Now;
                    //                                    C.ResultadosAdjuntos = false;
                    //                                    C.Remitente = "Administrador<administrador@veconinter.com.ve>";
                    //                                    C.Adjuntos = Comunes.AnalisisReporte(Cuenta.idPersona, 1, null); //Administrador
                    //                                    C.Adjuntos += ";" + db.Parametros.Single(x => x.Clave == "RutaArchivos").Valor + "depositoProvincial.pdf";
                    //                                    C.Correos_Personas.Add(new Entidades.Correos_Personas() { idPersona = Cuenta.idPersona });
                    //                                    C.idOperador = 1;
                    //                                    C.Fecha = DateTime.Now;
                    //                                    db.Correos.InsertOnSubmit(C);

                    //                                    Entidades.Gestiones Gestion = new Entidades.Gestiones();
                    //                                    Gestion.idOperador = 1;
                    //                                    Gestion.idPersona = Cuenta.idPersona;
                    //                                    Gestion.idStatus = Status;
                    //                                    Gestion.Fecha = DateTime.Now;
                    //                                    Gestion.Descripcion = "Se envió Automáticamente el Análisis de Antigüedad";
                    //                                    db.Gestiones.InsertOnSubmit(Gestion);
                    //                                    db.SubmitChanges();
                    //                                }
                    //                                break;
                    //                            case 3: //Llamada IVR
                    //                                break;
                    //                            case 4: //Envio SMS
                    //                                break;
                    //                        }
                    //                        break;
                    //                    case "BackOffice":
                    //                        Log(db, "Avanzar", idProceso, "Enviando a modo BackOffice la cuenta: " + Cuenta.idCuenta, i);
                    //                        AsignarTipoOperador(db, Cuenta, "BO");
                    //                        break;
                    //                    case "Legal":
                    //                        Log(db, "Avanzar", idProceso, "Enviando a legal la cuenta: " + Cuenta.idCuenta, i);
                    //                        AsignarTipoOperador(db, Cuenta, "LE");
                    //                        break;
                    //                    case "Operador":
                    //                        Log(db, "Avanzar", idProceso, "Enviando a Operador la cuenta: " + Cuenta.idCuenta, i);
                    //                        AsignarTipoOperador(db, Cuenta, "OP");
                    //                        break;
                    //                    case "Supervisor":
                    //                        Log(db, "Avanzar", idProceso, "Enviando a Supervisor la cuenta: " + Cuenta.idCuenta, i);
                    //                        AsignarTipoOperador(db, Cuenta, "SU");
                    //                        break;
                    //                }
                    //            }
                    //            catch
                    //            {
                    //            }
                    //            PersonaAnterior = Cuenta.idPersona;
                    //        }
                    //    }
                    //}
                }
            }
            catch { }
            Avanzando = false;
        }
        //private void Distribuir(object state)
        //{
        //    //if (Distribuyendo) return;
        //    //Distribuyendo = true;
        //    try
        //    {
        //        using (CobranzasDataContext db = new CobranzasDataContext())
        //        {
        //            db.CommandTimeout = 10000;
        //            Int32 idProceso = ObteneridProceso(db, "Distribuir");
        //            Log(db, "Distribuir", idProceso, "Inicio Proceso Distribución", 0);
        //            Int32 i = 0;
        //            //foreach (Entidades.DistribucionCampanas Dist in db.DistribucionCampanas)
        //            //{
        //            //    i++;
        //            //    try
        //            //    {
        //            //        Entidades.Reglas Regla = Dist.Reglas;
        //            //        var Cuentas = db.Cuentas.Where(x => x.Activa && !x.Campanas_Cuentas.Any(y => y.idCampana == Dist.idCampana && y.FechaFin == null) && (db.CumpleRegla(x.idCuenta, Regla.idRegla) ?? false)).ToList();
        //            //        Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Cuentas=" + Cuentas.Count, i);
        //            //        foreach (Entidades.Cuentas Cuenta in Cuentas)
        //            //        {
        //            //            Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Cuenta=" + Cuenta.idCuenta + "(" + Cuenta.Codigo + ")", i);
        //            //            try
        //            //            {
        //            //                if (Dist.Excluir)
        //            //                {
        //            //                    foreach (Entidades.Campanas_Cuentas CC in db.Campanas_Cuentas.Where(x => x.FechaFin == null && x.idCuenta == Cuenta.idCuenta && x.idCampana != Dist.idCampana))
        //            //                    {
        //            //                        CC.FechaFin = DateTime.Now;
        //            //                    }
        //            //                }
        //            //                if (!Cuenta.Campanas_Cuentas.Any(x => x.idCampana == Dist.idCampana && x.FechaFin == null))
        //            //                { //si no está asignada a la campaña, asígnala
        //            //                    Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Cuenta=" + Cuenta.idCuenta + "(" + Cuenta.Codigo + "), Asignada", i);
        //            //                    Cuenta.Campanas_Cuentas.Add(new Entidades.Campanas_Cuentas { idCampana = Dist.idCampana, FechaInicio = DateTime.Now });
        //            //                    if (Cuenta.idFlujo == null || Cuenta.idPaso == null)
        //            //                        Cuenta.Flujos_Pasos = Dist.Flujos_Pasos;
        //            //                }
        //            //                db.SubmitChanges();
        //            //            }
        //            //            catch (Exception Ex)
        //            //            {
        //            //                Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Cuenta=" + Cuenta.idCuenta + "(" + Cuenta.Codigo + "), Error=" + Ex.Message, i);
        //            //            }
        //            //        }
        //            //    }
        //            //    catch (Exception Ex)
        //            //    {
        //            //        Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Error=" + Ex.Message, i);
        //            //    }
        //            //}
        //            foreach (Entidades.DistribucionCampanas Dist in db.DistribucionCampanas)
        //            {
        //                i++;
        //                try
        //                {
        //                    Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + ")", i);
        //                    db.DistribuirCampana(Dist.idDistribucion);
        //                }
        //                catch (Exception Ex)
        //                {
        //                    Log(db, "Distribuir", idProceso, "Procesando DC(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Error=" + Ex.Message, i);
        //                }
        //            }
        //            foreach (Entidades.DistribucionOperador Dist in db.DistribucionOperador)
        //            {
        //                i++;
        //                try
        //                {
        //                    Log(db, "Distribuir", idProceso, "Procesando DO(" + Dist.idDistribucion + ")(" + Dist.Nombre + ")", i);
        //                    db.DistribuirOperador(Dist.idDistribucion);
        //                }
        //                catch (Exception Ex)
        //                {
        //                    Log(db, "Distribuir", idProceso, "Procesando DO(" + Dist.idDistribucion + ")(" + Dist.Nombre + "), Error=" + Ex.Message, i);
        //                }
        //            }
        //        }
        //    }
        //    catch { }
        //    Distribuyendo = false;

        //}
        private void Llevar(object state)
        {
            if (Llevando) return;
            Llevando = true;
            try
            {
                List<Int32> Origenes;
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Origenes = db.Origenes.Select(x => x.idOrigen).ToList();
                }
                LlevarPagos(1);
                foreach (Int32 idOrigen in Origenes)
                {
                    LlevarReclamos(idOrigen);
                }
            }
            catch { }
            Llevando = false;
        }
        private static void AsignarTipoOperador(CobranzasDataContext db, Entidades.Cuentas Cuenta, String Tipo)
        {
            if (Cuenta.Cuentas_Operadores.Any(x => x.FechaFin == null && x.Operadores.Tipo.Contains(Tipo))) return;
            int? Supervisor = null;
            foreach (Entidades.Cuentas_Operadores CO in Cuenta.Cuentas_Operadores.Where(x => x.FechaFin == null))
            {
                Supervisor = CO.Operadores.idSupervisor;
                CO.FechaFin = DateTime.Now;
            };
            if (Tipo == "SU")
            {
                Cuenta.Cuentas_Operadores.Add(new Entidades.Cuentas_Operadores { FechaInicio = DateTime.Now, idOperador = Supervisor.Value });
            }
            db.SubmitChanges();
            if (Tipo == "SU") return;

            Entidades.Campanas_Cuentas CC = Cuenta.Campanas_Cuentas.FirstOrDefault(x => x.FechaFin == null);
            if (CC != null || !CC.Campanas.Campanas_Operadores.Any(x => x.FechaFin == null && x.Operadores.Tipo.Contains(Tipo))) //si la cuenta no está en ninguna Campaña o en la campaña no hay backoffices, Asígnasela al supervisor
            {
                if (Supervisor == null)
                {
#warning Que hacer si no tengo supervisor cuando voy a asignar un backoffice de una cuenta que no está en campaña y el operador no es backoffice
                    //break;
                }

                Cuenta.Cuentas_Operadores.Add(new Entidades.Cuentas_Operadores { FechaInicio = DateTime.Now, idOperador = Supervisor.Value });
            }
            else
            {
                //El que la haya tocado antes
                Entidades.Operadores Operador = CC.Campanas.Campanas_Operadores.Where(x => x.FechaFin == null && x.Operadores.Tipo.Contains(Tipo)).Select(x => x.Operadores).Where(x => x.Cuentas_Operadores.Any(y => y.idCuenta == Cuenta.idCuenta)).OrderBy(x => x.Cuentas_Operadores.Where(y => y.idCuenta == Cuenta.idCuenta).Max(y => y.FechaFin)).FirstOrDefault();
                if (Operador == null)
                {
                    //El Más Desocupado
                    Operador = CC.Campanas.Campanas_Operadores.Where(x => x.FechaFin == null && x.Operadores.Tipo.Contains(Tipo)).Select(x => x.Operadores).OrderBy(x => x.Cuentas_Operadores.Count(y => y.FechaFin == null)).First();
                }
                Cuenta.Cuentas_Operadores.Add(new Entidades.Cuentas_Operadores { FechaInicio = DateTime.Now, idOperador = Operador.idOperador });
            }
            db.SubmitChanges();
        }
        private void Proceso(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    String CS = "";
                    if (idOrigen == 1)
                    {
                        Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                        CS = Origen.ConnectionString;
                    }
                    else
                    {
                        CS = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=Cobranzas;Password=&&admin%%ABC;Initial Catalog=comillenium;Data Source=USMIAVS016";
                    }
                    OleDbConnection Conn = new OleDbConnection(CS);
                    Conn.Open();

                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandTimeout = 100000;
                    Comm.CommandText = @"Cobranzas._DigitalizarFacturas";
                    Comm.CommandType = CommandType.StoredProcedure;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);

                    Int32 Total = DS.Tables[0].Rows.Count;
                    Debug.Print(Total.ToString());
                    Int32 i = 0;
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        String Cuenta = (String)Fila["Serie"] + Convert.ToInt32(Fila["FacturaId"]).ToString();
                        i++;
                        Debug.Print(i.ToString() + "/" + Total.ToString());
                        OleDbCommand Comm5 = Conn.CreateCommand();
                        Comm5.CommandType = CommandType.StoredProcedure;
                        Comm5.CommandText = "Cobranzas.ObtenerDigital";
                        Comm5.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta;
                        //Comm5.Parameters.Add("RETURNVALUE", OleDbType.Integer).Direction = ParameterDirection.ReturnValue;
                        OleDbDataAdapter DA5 = new OleDbDataAdapter(Comm5);
                        try
                        {
                            DataSet DS5 = new DataSet();
                            DA5.Fill(DS5);
                            DataRow Fila5 = DS5.Tables[0].Rows[0];
                            if (Convert.ToString(Fila5["RutaConfirmacion"]) == "") continue; //Todavía no tiene Digital
                            using (WebClient client = new WebClient())
                            {
                                String S = client.DownloadString(Convert.ToString(Fila5["RutaConfirmacion"]));
                            }
                            //Cuenta.Ruta = Convert.ToString(Fila5["Archivo"]);
                            //db.SubmitChanges();
                        }
                        catch (Exception Ex)
                        {
                            Debug.Print(Ex.Message);
                        }
                    }
                    Conn.Close();
                }
                catch (Exception Ex)
                {
                    Debug.Print(Ex.Message);
                }
            }
        }
        private void ActualizarDigital(int idOrigen, Boolean Activa = true)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();
                    var Cuentas = db.Cuentas.Where(x => DateTime.Now >= x.FechaDocumento.Value.AddHours(2) && x.Activa == Activa && (x.Ruta == null || x.Ruta == "") && x.idOrigen == idOrigen); //Solo Buscar el Digital 2 horas después de que existe la factura
                    Int32 Total = Cuentas.Count();
                    Debug.Print(Total.ToString());
                    Int32 i = 0;
                    foreach (var Cuenta in Cuentas)
                    {
                        i++;
                        Debug.Print(i.ToString() + "/" + Total.ToString());
                        OleDbCommand Comm5 = Conn.CreateCommand();
                        Comm5.CommandType = CommandType.StoredProcedure;
                        Comm5.CommandText = "Cobranzas.ObtenerDigital";
                        Comm5.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta.Codigo;
                        Comm5.Parameters.Add("Pais", OleDbType.Char).Value = Cuenta.Personas.idPais; 
                        //Comm5.Parameters.Add("RETURNVALUE", OleDbType.Integer).Direction = ParameterDirection.ReturnValue;
                        OleDbDataAdapter DA5 = new OleDbDataAdapter(Comm5);
                        try
                        {
                            DataSet DS5 = new DataSet();
                            DA5.Fill(DS5);
                            DataRow Fila5 = DS5.Tables[0].Rows[0];
                            if (Convert.ToString(Fila5["RutaConfirmacion"]) == "") continue; //Todavía no tiene Digital
                            using (WebClient client = new WebClient())
                            {
                                String S = client.DownloadString(Convert.ToString(Fila5["RutaConfirmacion"]));
                            }
                            Cuenta.Ruta = Convert.ToString(Fila5["Archivo"]);
                            db.SubmitChanges();
                        }
                        catch { }
                    }
                    Conn.Close();
                }
                catch (Exception Ex)
                {
                    Debug.Print(Ex.Message);
                }
            }
        }
        private void ActualizacionEspecial(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {

                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();

                    var Cuentas = db.Cuentas.Where(x => (x.idOrigen == idOrigen) && !(db.EstaBien(x.Datos, x.Personas.Codigo) ?? false));
                    foreach (Entidades.Cuentas Cuenta in Cuentas)
                    {
                        var CodigoPersona = Cuenta.Datos.Elements("Dato").Where(x => x.Attribute("Clave").Value == "Clienteid").First().Value;
                        Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.idPais == Cuenta.Personas.idPais && x.Codigo == CodigoPersona);
                        if (Persona == null)
                        {
                            try
                            {
                                Debug.Print("Crear Persona");
                                Persona = new Entidades.Personas();
                                OleDbCommand Comm2 = Conn.CreateCommand();
                                Comm2.CommandType = CommandType.StoredProcedure;
                                Comm2.CommandText = "Cobranzas.ObtenerPersona";
                                Comm2.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = CodigoPersona;
                                Comm2.Parameters.Add("idPais", OleDbType.Char).Value = Cuenta.Personas.idPais;
                                OleDbDataAdapter DA2 = new OleDbDataAdapter(Comm2);
                                DataSet DS2 = new DataSet();
                                DA2.Fill(DS2);
                                DataRow Fila2 = DS2.Tables[0].Rows[0];
                                Persona.Email = Convert.ToString(Fila2["cliemail"]).Trim();
                                Entidades.Telefonos Tel1 = new Entidades.Telefonos();
                                Tel1.Telefono = Convert.ToString(Fila2["clitelf1"]).Trim();
                                if (Tel1.Telefono != "") Persona.Telefonos.Add(Tel1);
                                Entidades.Telefonos Tel2 = new Entidades.Telefonos();
                                Tel2.Telefono = Convert.ToString(Fila2["clitelf2"]).Trim();
                                if (Tel1.Telefono != "") Persona.Telefonos.Add(Tel2);
                                Persona.URL = Convert.ToString(Fila2["cliurl"]);
                                Persona.idPais = Cuenta.Personas.idPais;
                                Persona.Codigo = CodigoPersona;
                                Persona.DireccionFiscal = Convert.ToString(Fila2["clidireccion"]).Trim();
                                Persona.FechaCreacion = DateTime.Now;//Quemado
                                Persona.idTipoPersona = 1;//Quemado
                                Persona.Nombre = Convert.ToString(Fila2["clinombre"]).Trim();
                                Persona.Rif = Convert.ToString(Fila2["clirif"]).Trim();
                                Persona.Datos = Fila2.IsNull("Datos") ? (XElement)null : XElement.Parse(((String)Fila2["Datos"]).Trim());
                                db.Personas.InsertOnSubmit(Persona);
                                db.SubmitChanges();

                                OleDbCommand Comm3 = Conn.CreateCommand();
                                Comm3.CommandType = CommandType.StoredProcedure;
                                Comm3.CommandText = "Cobranzas.PersonaSoportes";
                                Comm3.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = CodigoPersona;
                                Comm3.Parameters.Add("idPais", OleDbType.Char).Value = Cuenta.Personas.idPais;
                                OleDbDataAdapter DA3 = new OleDbDataAdapter(Comm3);
                                DataSet DS3 = new DataSet();
                                DA3.Fill(DS3);

                                foreach (DataRow Fila3 in DS3.Tables[0].Rows)
                                {
                                    Entidades.Soportes Soporte = new Entidades.Soportes();
                                    Soporte.Codigo = Convert.ToString(Fila3["Codigo"]);
                                    Soporte.Nombre = Convert.ToString(Fila3["Nombre"]);
                                    Soporte.Ubicacion = Convert.ToString(Fila3["Ubicacion"]);
                                    Persona.Soportes.Add(Soporte);
                                }
                                db.SubmitChanges();
                                //PersonasContacto
                                OleDbCommand Comm4 = Conn.CreateCommand();
                                Comm4.CommandType = CommandType.StoredProcedure;
                                Comm4.CommandText = "Cobranzas.PersonaContactos";
                                Comm4.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = Persona.Codigo;
                                Comm4.Parameters.Add("idPais", OleDbType.Char).Value = Cuenta.Personas.idPais;
                                OleDbDataAdapter DA4 = new OleDbDataAdapter(Comm4);
                                DataSet DS4 = new DataSet();
                                DA4.Fill(DS4);

                                foreach (DataRow Fila4 in DS4.Tables[0].Rows)
                                {
                                    try
                                    {
                                        Debug.Print("Crear Contacto");
                                        Entidades.PersonasContacto Contacto = new Entidades.PersonasContacto();
                                        Contacto.idPersona = Persona.idPersona;
                                        Contacto.Nombre = Convert.ToString(Fila4["Nombre"]);
                                        Contacto.Email = Convert.ToString(Fila4["Email"]);
                                        Contacto.Cargo = Convert.ToString(Fila4["Cargo"]);
                                        Contacto.Activa = true;
                                        //Contacto.idCliente = null;
                                        if (!Fila4.IsNull("Telefono1")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono1"]), Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                        if (!Fila4.IsNull("Telefono2")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono2"]), Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                        db.PersonasContacto.InsertOnSubmit(Contacto);
                                    }
                                    catch (Exception Ex)
                                    {
                                        Debug.Print(Ex.Message);
                                    }
                                }
                                db.SubmitChanges();

                            }
                            catch (Exception Ex)
                            {
                                Debug.Print(Ex.Message);
                            }
                        }
                        Cuenta.idPersona = Persona.idPersona;
                        db.SubmitChanges();
                    }

                }
                catch { }
            }

        }
        private void ActualizarDatosCuentas(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ObtenerDatosCuenta";
                    Comm.Parameters.Add("CodigoCuenta", OleDbType.VarChar);
                    Comm.Parameters.Add("idPais", OleDbType.VarChar);
                    Conn.Open();
                    int i = 0;
                    var Cuentas = db.Cuentas.Where(x => x.Datos == null && x.idOrigen == idOrigen).ToList();
                    int total = Cuentas.Count;
                    foreach (Entidades.Cuentas Cuenta in Cuentas)
                    {
                        i++;
                        Debug.Print("{0}/{1}", i, total);
                        try
                        {
                            Comm.Parameters["CodigoCuenta"].Value = Cuenta.Codigo;
                            Comm.Parameters["idPais"].Value = Cuenta.Personas.idPais;
                            XElement xml = XElement.Parse(((String)Comm.ExecuteScalar()).Replace("&", "&amp;"));
                            Cuenta.Datos = xml;
                            db.SubmitChanges();
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
        private void ActualizarDatosPersonas(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ObtenerDatosPersona";
                    Comm.Parameters.Add("CodigoPersona", OleDbType.VarChar);
                    Comm.Parameters.Add("idPais", OleDbType.VarChar);
                    Conn.Open();
                    int i = 0;
                    var Personas = db.Personas.Where(x => x.Datos == null).ToList();
                    int total = Personas.Count;
                    foreach (Entidades.Personas Persona in Personas)
                    {
                        i++;
                        Debug.Print("{0}/{1}", i, total);
                        try
                        {
                            Comm.Parameters["CodigoPersona"].Value = Persona.Codigo;
                            Comm.Parameters["idPais"].Value = Persona.idPais;
                            XElement xml = XElement.Parse(((String)Comm.ExecuteScalar()).Replace("&", "&amp;"));
                            Persona.Datos = xml;
                            db.SubmitChanges();
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
        private void ActualizarCuentasDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActCuentas");
                try
                {
                    DateTime Fecha = DateTime.Now;
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarCuentas";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;

                    Log(db, "ActCuentas", idProceso, "Inicio Proceso Actualizacion, Origen=" + idOrigen.ToString() + ", TotalCuentas=" + DS.Tables[0].Rows.Count, 0);
                    //OSantiago 27/08/2014 Esta fecha se obtiene luego de la ejecución del SP, si se agregan registros en auditoría durante la ejecucion estos no seran tomados en cuenta porque su fecha es menor a esta, por eso se obtiene la fecha antes del SP. DateTime Fecha = DateTime.Now;
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        Log(db, "ActCuentas", idProceso, "Actualización de Cuenta: " + (String)Fila["Codigo"] + ", MontoRestante:" + Convert.ToDecimal(Fila["MontoRestante"]).ToString("N2"), i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        //Si ya existe... sáltala
                        try
                        {
                            Entidades.Cuentas Cuenta = db.Cuentas.SingleOrDefault(x => x.Codigo == Convert.ToString(Fila["Codigo"]) && x.Personas.idPais == Convert.ToString(Fila["idPais"]));
                            if (Cuenta == null) continue;
                            Cuenta.Monto = Convert.ToDecimal(Fila["Monto"]);
                            if (Cuenta.MontoRestante == 0 && Convert.ToDecimal(Fila["MontoRestante"]) > 0)
                            {
                                Cuenta.Activa = true;
                            }
                            if (Convert.ToBoolean(Fila["Anulada"]))
                            {
                                Cuenta.Activa = false;
                            }
                            Cuenta.MontoBase = Convert.ToDecimal(Fila["MontoBase"]);
                            Cuenta.MontoIva = Convert.ToDecimal(Fila["MontoIva"]);
                            Cuenta.MontoRestante = Convert.ToDecimal(Fila["MontoRestante"]);
                            Cuenta.FechaEntrega = (Fila.IsNull("FechaEntrega") ? (DateTime?)null : Convert.ToDateTime(Fila["FechaEntrega"]));
                            Cuenta.CambioDolar = Convert.ToDecimal(Fila["CambioDolar"]);
                            Cuenta.CambioLocal = Convert.ToDecimal(Fila["CambioLocal"]);
                            Cuenta.EnReclamo = Convert.ToBoolean(Fila["EnReclamo"]);
                            Cuenta.Zona = Convert.ToString(Fila["Zona"]);
                            db.SubmitChanges();
                            try //Caso de las unificaciones
                            {
                                String CodigoPersona = Convert.ToInt32(Fila["CodigoPersona"]).ToString();
                                if (CodigoPersona != Cuenta.Personas.Codigo)
                                {
                                    Cuenta.Datos = null;
                                    Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.Codigo == CodigoPersona && x.idPais == Cuenta.Personas.idPais);
                                    if (Persona != null)
                                    {
                                        Cuenta.idPersona = Persona.idPersona;
                                    }
                                    Log(db, "ActCuentas", idProceso, "Cambio de Cliente: " + Cuenta.Personas.Codigo + "->" + CodigoPersona.ToString(), i);
                                }
                                db.SubmitChanges();
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActCuentas", idProceso, "Error en Cambio de Cliente: " + Ex.Message, i);
                            }

                            OleDbCommand Comm10 = Conn.CreateCommand();
                            Comm10.CommandType = CommandType.StoredProcedure;
                            Comm10.CommandText = "Cobranzas.MarcarCuenta";
                            Comm10.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta.Codigo;
                            Comm10.Parameters.Add("Fecha", OleDbType.Date).Value = Fecha;
                            Comm10.Parameters.Add("idPais", OleDbType.VarChar).Value = Cuenta.Personas.idPais;
                            Comm10.ExecuteNonQuery();
                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActCuentas", idProceso, "Actualizacion Cuenta: " + (String)Fila["Codigo"] + " Error=" + Ex.Message, 0);
                        }
                    }
                    Conn.Close();
                }
                catch (Exception Ex)
                {
                    Log(db, "ActCuentas", idProceso, "Error en Actualización de cuentas:" + Ex.Message, 0);
                }
            }
        }
        private void ActualizarPersonasDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActPersonas");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarPersonas";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "ActPersonas", idProceso, "Inicio Proceso Actualizacion, Total Personas=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        Log(db, "ActPersonas", idProceso, "Actualización de Persona: " + (Int32)Fila["Codigo"], i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        //Si ya existe... sáltala
                        try
                        {
                            Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.Codigo == Convert.ToString(Fila["Codigo"]) && x.idPais == Convert.ToString(Fila["idPais"]));
                            if (Persona == null) continue;
                            Persona.idTipoPersona = Convert.ToInt32(Fila["idTipoPersona"]);
                            Persona.Nombre = Convert.ToString(Fila["Nombre"]).Trim();
                            Persona.Rif = Convert.ToString(Fila["Rif"]).Trim();
                            Persona.DireccionFiscal = Convert.ToString(Fila["DireccionFiscal"]).Trim();
                            Persona.URL = Convert.ToString(Fila["URL"]).Trim();
                            Persona.Email = Convert.ToString(Fila["Email"]).Trim();
                            Persona.Datos = Fila.IsNull("Datos") ? (XElement)null : XElement.Parse(((String)Fila["Datos"]).Trim());
                            Persona.Zona = Fila.IsNull("Zona") ? "" : Convert.ToString(Fila["Zona"]);
                            Persona.Contacto = Convert.ToString(Fila["Contacto"]).Trim();
                            Persona.DireccionEntrega = Convert.ToString(Fila["DireccionEntrega"]).Trim();
                            Persona.EnviosAutomaticos = Convert.ToBoolean(Fila["EnviosAutomaticos"]);

                            String Telefonos = Convert.ToString(Fila["Telefonos"]);
                            foreach (String Telefono in Comunes.ObtenerTelefonos(Telefonos))
                            {
                                if (!Persona.Telefonos.Any(x => x.Telefono == Telefono))
                                {
                                    Persona.Telefonos.Add(new Entidades.Telefonos { Telefono = Telefono, Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                }
                            }

                            //foreach (String Telefono in Telefonos.Split(','))
                            //{
                            //    if (Telefono.Trim() == "") continue;
                            //    String[] ColTelf = Telefono.Split('/');
                            //    foreach (String TelefonoReal in ColTelf)
                            //    {
                            //        if (TelefonoReal.Trim() == "") continue;
                            //        String TelefonoIns = (TelefonoReal.Length < 5) ? ColTelf[0].Substring(0, ColTelf[0].Length - TelefonoReal.Length) + TelefonoReal : TelefonoReal;
                            //        if (!Persona.Telefonos.Any(x => x.Telefono == TelefonoIns))
                            //        {
                            //            Persona.Telefonos.Add(new Entidades.Telefonos { Telefono = TelefonoIns });
                            //        }
                            //    }
                            //}
                            db.SubmitChanges();
                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActPersonas", idProceso, "Actualizacion Persona: " + ((Int32)Fila["Codigo"]) + " Error=" + Ex.Message, i);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "ActPersonas", idProceso, "Error en Actualización de Peronas:" + Ex.Message, 0);
                }
            }
        }
        private void ActualizarReclamosDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActReclamos");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarReclamos";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "ActReclamos", idProceso, "Inicio Proceso Actualizacion de Reclamos, Total Reclamos=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        String Codigo = Convert.ToString(Fila["Codigo"]);
                        String idPais = Convert.ToString(Fila["idPais"]);
                        Log(db, "ActReclamos", idProceso, "Actualización de Reclamo(" + idPais + "): " + Codigo, i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        try
                        {
                            String CodigoDepartamento = Convert.ToString(Fila["CodigoDepartamento"]);
                            String CodigoMotivo = Convert.ToString(Fila["CodigoReclamoMotivo"]);
                            String CodigoCuenta = Convert.ToString(Fila["CodigoCuenta"]);
                            String CodigoStatus = Convert.ToString(Fila["CodigoStatus"]);
                            String CodigoSolucion = Fila.IsNull("CodigoSolucion") ? (string)null : Convert.ToString(Fila["CodigoSolucion"]);
                            Int32? idReclamoSolucion = (CodigoSolucion == null) ? (Int32?)null : db.ReclamosSoluciones.First(x => x.idOrigen == idOrigen && x.Codigo == CodigoSolucion && x.idPais == idPais).idReclamoSolucion;
                            Entidades.Reclamos Reclamo = db.Reclamos.SingleOrDefault(x => x.Codigo == Codigo && x.idOrigen == idOrigen & x.idPais == idPais);
                            Boolean NuevoReclamo = Reclamo == null;

                            Entidades.ReclamosDepartamentos RD = db.ReclamosDepartamentos.FirstOrDefault(x => x.idOrigen == idOrigen && x.CodigoDepartamento == CodigoDepartamento && x.idPais == idPais);
                            if (RD == null)
                            {
                                try
                                {
                                    OleDbCommand Comm2 = Conn.CreateCommand();
                                    Comm2.CommandType = CommandType.StoredProcedure;
                                    Comm2.CommandText = "Cobranzas.ObtenerReclamosDepartamentos";
                                    Comm2.CommandTimeout = 10 * 60;
                                    Comm2.Parameters.Add("CodigoDepartamento", OleDbType.VarChar).Value = CodigoDepartamento;
                                    Comm2.Parameters.Add("Pais", OleDbType.VarChar).Value = idPais;
                                    OleDbDataAdapter DA2 = new OleDbDataAdapter(Comm2);
                                    DataSet DS2 = new DataSet();
                                    DA2.Fill(DS2);
                                    DataRow Fila2 = DS2.Tables[0].Rows[0];
                                    RD = new Entidades.ReclamosDepartamentos();
                                    RD.idPais = idPais;
                                    RD.idOrigen = idOrigen;
                                    RD.Anulado = (Boolean)Fila2["Anulado"];
                                    RD.CodigoDepartamento = CodigoDepartamento;
                                    RD.Departamento = (String)Fila2["Departamento"];
                                    db.ReclamosDepartamentos.InsertOnSubmit(RD);
                                    db.SubmitChanges();
                                }
                                catch (Exception Ex)
                                {
                                    Log(db, "ActReclamos", idProceso, Ex.Message, i);
                                }
                            }

                            if (NuevoReclamo)
                            { //Crear nuevo Reclamo si no existe
                                Reclamo = new Entidades.Reclamos();
                                Reclamo.Codigo = Codigo;
                                Reclamo.idOrigen = idOrigen;
                                Reclamo.FechaResultado = DateTime.Now;
                                Reclamo.Resultado = "Reclamo Importado desde SCI";
                                Reclamo.StatusInterno = 3;
                                Reclamo.Fecha = Convert.ToDateTime(Fila["Fecha"]);
                                Reclamo.idOperador = 1;
                                Reclamo.idReclamoMotivo = db.ReclamosMotivos.First(x => x.idOrigen == idOrigen && x.Codigo == CodigoMotivo && x.idPais == idPais).idReclamoMotivo;
                                Reclamo.Creador = Convert.ToString(Fila["Creador"]);
                                Reclamo.Descripcion = Convert.ToString(Fila["Descripcion"]);
                                Reclamo.idPais = idPais;
                                db.Reclamos.InsertOnSubmit(Reclamo);
                            }
                            Entidades.ReclamosStatus ReclamoStatus = db.ReclamosStatus.First(x => x.idOrigen == idOrigen && x.Codigo == CodigoStatus && x.idPais == idPais);

                            Reclamo.ReclamosStatus = ReclamoStatus;
                            Reclamo.idDepartamento = RD.idDepartamento;
                            Reclamo.Abierto = Convert.ToBoolean(Fila["Abierto"]);
                            Reclamo.Procede = Convert.ToBoolean(Fila["Procede"]);
                            Reclamo.Descripcion = Convert.ToString(Fila["Descripcion"]);
                            db.SubmitChanges();

                            try
                            {
                                Entidades.Cuentas Cuenta = db.Cuentas.First(x => x.idOrigen == idOrigen && x.Codigo == CodigoCuenta && x.Personas.idPais == idPais);
                                if (Reclamo.idReclamoStatus.HasValue && Reclamo.idReclamoStatus != ReclamoStatus.idReclamoStatus && Reclamo.idOperador.HasValue && Reclamo.idOperador != 1)
                                {
                                    Entidades.Avisos Aviso = new Entidades.Avisos();
                                    db.Avisos.InsertOnSubmit(Aviso);
                                    Aviso.FechaAviso = DateTime.Now.AddMinutes(5);
                                    Aviso.FechaOriginal = DateTime.Now.AddMinutes(5);
                                    Aviso.idOperadorCrea = 1;
                                    Aviso.idOperador = Reclamo.idOperador.Value;
                                    Aviso.FechaCrea = DateTime.Now;
                                    Aviso.Aviso = "El Reclamo (" + idPais + ")" + Reclamo.Codigo + " del Cliente " + Cuenta.Personas.Codigo + " Ha Cambiado de Status de " + Reclamo.ReclamosStatus.Descripcion + " al " + ReclamoStatus.Descripcion;
                                    Aviso.idPersona = Cuenta.idPersona;
                                }

                                Entidades.Cuentas_Reclamos CR = db.Cuentas_Reclamos.FirstOrDefault(x => x.idReclamo == Reclamo.idReclamo && x.idCuenta == Cuenta.idCuenta);
                                if (CR == null)
                                {
                                    CR = new Entidades.Cuentas_Reclamos();
                                    CR.idCuenta = Cuenta.idCuenta;
                                    CR.idReclamoSolucion = idReclamoSolucion;
                                    Reclamo.Cuentas_Reclamos.Add(CR);
                                }
                                else
                                {
                                    CR.idReclamoSolucion = idReclamoSolucion;
                                }
                                db.SubmitChanges();
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActReclamos", idProceso, "Actualización de Reclamo_Cuenta(" + idPais + "): " + Codigo + " Cuenta=" + CodigoCuenta + " Error=" + Ex.Message, i);
                            }
                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActReclamos", idProceso, "Actualización de Reclamo(" + idPais + "): " + Codigo + " Error=" + Ex.Message, i);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "ActReclamos", idProceso, "Error en Actualización de Reclamos:" + Ex.Message, 0);
                }
            }
        }
        private void ActualizarPagosTDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActPagosT");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarPagosTemporales";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "ActPagosT", idProceso, "Inicio Proceso Actualizacion de PagosTemporales, Total Pagos T=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        String CodigoT = Convert.ToString(Fila["CodigoT"]);
                        String CodigoR = Convert.ToString(Fila["CodigoR"]);
                        String idPais = Convert.ToString(Fila["idPais"]);

                        Log(db, "ActPagosT", idProceso, "Actualización de Pago: " + CodigoT, i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        try
                        {
                            Entidades.Pagos Pago = db.Pagos.SingleOrDefault(x => !x.Aprobado.HasValue && x.Codigo == CodigoT && x.Personas.idPais == idPais) ??
                                                  db.Pagos.SingleOrDefault(x => x.Aprobado == true && x.Codigo == CodigoR && x.Personas.idPais == idPais) ??
                                                  db.Pagos.SingleOrDefault(x => x.Aprobado == false && x.Codigo == CodigoR && x.Personas.idPais == idPais);
                            if (Pago == null)
                            {
                                continue;
                            }

                            Pago.Codigo = String.IsNullOrWhiteSpace(CodigoR) ? CodigoT : CodigoR;
                            Pago.Resultado = Convert.ToString(Fila["Descripcion"]);
                            Boolean? AprobadoAnterior = Pago.Aprobado;
                            Pago.Aprobado = Fila.IsNull("Aprobado") ? (Boolean?)null : ((Boolean)Fila["Aprobado"]);
                            Boolean Aplicado = Convert.ToBoolean(Fila["Aplicado"]);
                            Int32 StatusAnterior = Pago.idStatusPago.Value;
                            Pago.Confirmado = ((Boolean)Fila["Confirmado"]);
                            Pago.idStatusPago = Pago.Aprobado.HasValue ? (Pago.Aprobado.Value ? (Aplicado ? 8 : 6) : 7) : Pago.idStatusPago;//;Convert.ToInt32(Fila["idStatusPago"]);
#warning Valores Quemados, consultar con la tabla de status por los status apropiados
                            db.SubmitChanges();
                            if ((!AprobadoAnterior.HasValue || AprobadoAnterior == true) && Pago.Aprobado == false)
                            { //Si no había tenido respuesta, y ahora la tiene y es que no fue aprobado, crea un aviso al supervisor
                                Entidades.Operadores Operador = db.Operadores.SingleOrDefault(x => x.idOperador == (Pago.idOperadorCrea ?? Pago.idOperador ?? 1));
                                Int32 Supervisor = Operador.idSupervisor ?? Operador.idOperador;
                                Entidades.Avisos Aviso = new Entidades.Avisos
                                {
                                    Aviso = "El Pago del cliente(" + idPais + "): " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Creado Por:" + Operador.Nombre + ", Ha sido <b>rechazado</b> por el siguiente motivo: " + Pago.Resultado,
                                    FechaAviso = DateTime.Now.AddMinutes(5),
                                    FechaCancelado = null,
                                    FechaOriginal = DateTime.Now.AddMinutes(5),
                                    FechaCrea = DateTime.Now,
                                    idOperador = Supervisor,
                                    idOperadorCrea = 1,
                                    idPersona = Pago.idPersona,
                                    VecesMostrada = 0,
                                };
                                db.Avisos.InsertOnSubmit(Aviso);
                                db.SubmitChanges();
                            }
                            //                            else if (StatusAnterior != Pago.idStatusPago)
                            else if (StatusAnterior != Pago.idStatusPago && Pago.idStatusPago == 8)
                            {
                                Entidades.Avisos Aviso = new Entidades.Avisos
                                {
                                    //Aviso = "El Pago del cliente: " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha cambiado su status de: " + db.StatusPago.Single(x => x.idStatusPago == StatusAnterior).Descripcion + " a: " + db.StatusPago.Single(x => x.idStatusPago == Pago.idStatusPago).Descripcion,
                                    Aviso = "El Pago del cliente(" + idPais + "): " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha sido aplicado.",
                                    FechaAviso = DateTime.Now.AddMinutes(5),
                                    FechaCancelado = null,
                                    FechaOriginal = DateTime.Now.AddMinutes(5),
                                    FechaCrea = DateTime.Now,
                                    idOperador = Pago.idOperadorCrea ?? Pago.idOperador ?? 1,
                                    idOperadorCrea = 1,
                                    idPersona = Pago.idPersona,
                                    VecesMostrada = 0,
                                };
                                db.Avisos.InsertOnSubmit(Aviso);
                                db.SubmitChanges();
                            }
                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActPagosT", idProceso, "Actualización de Pago: " + CodigoT + " Error=" + Ex.Message, i);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "ActPagosT", idProceso, "Error en Actualización de Pagos:" + Ex.Message, 0);
                }
            }
        }
        private void ActualizarPagosRDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActPagosR");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarPagosReales";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "ActPagosR", idProceso, "Inicio Proceso Actualizacion de Pagos Reales, Total Pagos=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        String Codigo = Convert.ToString(Fila["Codigo"]);
                        String CodigoT = Convert.ToString(Fila["CodigoT"]);
                        String idPais = Convert.ToString(Fila["idPais"]);
                        Log(db, "ActPagosR", idProceso, "Actualización de Pago(" + idPais + "): " + Codigo, i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        try
                        {
                            String CodigoBanco = Convert.ToString(Fila["CodigoBancoPropio"]);
                            String NumeroCuenta = Convert.ToString(Fila["NumeroCuentaPropio"]);
                            String CodigoPersona = Convert.ToString(Fila["CodigoPersona"]);
                            String CodigoCobrador = Convert.ToString(Fila["CodigoCobrador"]);
                            Int32 idBanco = 0;
                            try
                            {
                                idBanco = db.Bancos.Single(x => x.Codigo == CodigoBanco && x.idPais == idPais).idBanco;
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Error(1): " + Ex.Message, i);
                                continue;
                            }
                            Int32 idBancoPropio = 0;
                            try
                            {
                                idBancoPropio = db.BancosPropios.Single(x => x.idBanco == idBanco && x.NroCuenta == NumeroCuenta).idBancoPropio;
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Error(2): " + Ex.Message, i);
                                continue;
                            }
                            Int32 idPersona = 0;
                            try
                            {
                                idPersona = db.Personas.Single(x => x.Codigo == CodigoPersona && x.idPais == idPais).idPersona;
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Error(3): " + Ex.Message, i);
                                continue;
                            }
                            Int32? idOperador = null;
                            if (CodigoCobrador != "")
                            {
                                try
                                {
                                    idOperador = db.Operadores_Asignaciones.SingleOrDefault(x => x.idPais == idPais && x.Codigo == CodigoCobrador).idOperador;
                                    if (idOperador == null) idOperador = db.Operadores.First(x => x.Codigo == CodigoCobrador).idOperador;
                                }
                                catch (Exception Ex)
                                {
                                    //Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Error(4): " + Ex.Message, i);
                                    //continue;
                                }
                            }
                            else
                            {
                                //Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Cobrador no Encontrado", i);
                                //continue;
                            }
                            Entidades.Pagos Pago = db.Pagos.SingleOrDefault(x => x.Codigo == Codigo && (x.Aprobado ?? false) && x.Personas.idPais == idPais);
                            if (Pago == null) Pago = db.Pagos.SingleOrDefault(x => x.Codigo == CodigoT && ((!x.Aprobado.HasValue) || (x.Aprobado == false)) && x.Personas.idPais == idPais);
                            Boolean NuevoPago = Pago == null;
                            if (NuevoPago)
                            { //Crear nuevo Pago si no existe
                                Pago = new Entidades.Pagos();
                                db.Pagos.InsertOnSubmit(Pago);
                                Pago.TipoPago = Convert.ToInt32(Fila["TipoPago"]);
                                Pago.idMoneda = Convert.ToString(Fila["idMoneda"]);
                                Pago.idPersona = idPersona;
                                Pago.idOperador = idOperador;
                                Pago.idOperadorAprobado = idOperador;
                                Pago.idOperadorCrea = idOperador;
                                Pago.FechaResultado = Convert.ToDateTime(Fila["FechaResultado"]);
                                Pago.Resultado = Convert.ToString(Fila["Resultado"]);
                            }
                            Int32? StatusAnterior;
                            try
                            {
                                Pago.Codigo = Codigo;
                                Pago.idBancoPropio = idBancoPropio;
                                Pago.idBancoOrigen = idBanco;
                                Pago.Fecha = Convert.ToDateTime(Fila["Fecha"]);
                                Pago.Referencia = Convert.ToString(Fila["Referencia"]);
                                Pago.MontoEfectivo = Convert.ToDecimal(Fila["MontoEfectivo"]);
                                Pago.MontoCheque = Convert.ToDecimal(Fila["MontoCheque"]);
                                Pago.Descripcion = Convert.ToString(Fila["Descripcion"]);
                                Pago.FechaAprobado = Convert.ToDateTime(Fila["FechaAprobado"]);
                                Pago.Confirmado = Fila.IsNull("Confirmado") ? (Boolean?)null : Convert.ToBoolean(Fila["Confirmado"]);
                                //Boolean? AprobadoAnterior = Pago.Aprobado;
                                Pago.Aprobado = Fila.IsNull("Aprobado") ? (Boolean?)null : Convert.ToBoolean(Fila["Aprobado"]);
                                StatusAnterior = Pago.idStatusPago ?? 1;
                                Pago.idStatusPago = Convert.ToBoolean(Fila["Aplicado"]) ? 8 : (Pago.idStatusPago ?? 6);
                                db.SubmitChanges();
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Error(5): " + Ex.Message, i);
                                continue;
                            }

                            //Traer las cuentas que fueron pagadas
                            try
                            {
                                OleDbCommand Comm2 = Conn.CreateCommand();
                                Comm2.CommandType = CommandType.StoredProcedure;
                                Comm2.CommandText = "Cobranzas.ActualizarPagosCuentas";
                                Comm2.CommandTimeout = 10 * 60;
                                Comm2.Parameters.Add("CodigoPago", OleDbType.VarChar).Value = Pago.Codigo;
                                Comm2.Parameters.Add("Pais", OleDbType.Char, 3).Value = idPais;
                                OleDbDataAdapter DA2 = new OleDbDataAdapter(Comm2);
                                DataSet DS2 = new DataSet();
                                DA2.Fill(DS2);

                                foreach (DataRow Fila2 in DS2.Tables[0].Rows)
                                {
                                    String CodigoCuenta = Convert.ToString(Fila2["CodigoCuenta"]);
                                    Int32 idCuenta = db.Cuentas.Single(x => x.Codigo == CodigoCuenta && x.Personas.idPais == idPais).idCuenta;
                                    Decimal Monto = Convert.ToDecimal(Fila2["Monto"]);
                                    Decimal? Retencion1 = Fila2.IsNull("Retencion1") ? (Decimal?)null : Convert.ToDecimal(Fila2["Retencion1"]);
                                    Decimal? Retencion2 = Fila2.IsNull("Retencion2") ? (Decimal?)null : Convert.ToDecimal(Fila2["Retencion2"]);
                                    Boolean Anulado = Convert.ToBoolean(Fila2["Anulado"]);
                                    DateTime Fecha = Convert.ToDateTime(Fila2["Fecha"]);
                                    Entidades.Pagos_Cuentas PC;
                                    try
                                    {
                                        PC = Pago.Pagos_Cuentas.Single(x => x.idCuenta == idCuenta);
                                        if (Anulado) { Pago.Pagos_Cuentas.Remove(PC); continue; }
                                    }
                                    catch
                                    {
                                        PC = new Entidades.Pagos_Cuentas();
                                        if (!Anulado)
                                        {
                                            Pago.Pagos_Cuentas.Add(PC);
                                        }
                                    }
                                    if (!Anulado)
                                    {
                                        PC.idCuenta = idCuenta;
                                        PC.Monto = Monto;
                                        PC.Retencion1 = Retencion1;
                                        PC.Retencion2 = Retencion2;
                                        PC.Fecha = Fecha;
                                    }
                                }
                                db.SubmitChanges();
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "ActPagosR", idProceso, "Pago: " + Codigo + ". Error(6): " + Ex.Message, i);
                                continue;
                            }
                            /*if (StatusAnterior != Pago.idStatusPago)
                            {
                                Entidades.Avisos Aviso = new Entidades.Avisos
                                {
                                    Aviso = "El Pago del cliente: " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha cambiado su status de: " + db.StatusPago.Single(x => x.idStatusPago == StatusAnterior).Descripcion + " a: " + db.StatusPago.Single(x => x.idStatusPago == Pago.idStatusPago).Descripcion,
                                    FechaAviso = DateTime.Now.AddMinutes(5),
                                    FechaCancelado = null,
                                    FechaOriginal = DateTime.Now.AddMinutes(5),
                                    FechaCrea = DateTime.Now,
                                    idOperador = Pago.idOperadorCrea ?? Pago.idOperador ?? 1,
                                    idOperadorCrea = 1,
                                    idPersona = Pago.idPersona,
                                    VecesMostrada = 0,
                                };
                                db.Avisos.InsertOnSubmit(Aviso);
                                db.SubmitChanges();
                            }*/
                            if (StatusAnterior != Pago.idStatusPago && Pago.idStatusPago == 8)
                            {
                                Entidades.Avisos Aviso = new Entidades.Avisos
                                {
                                    Aviso = "El Pago del cliente(" + idPais + "): " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha sido aplicado",
                                    FechaAviso = DateTime.Now.AddMinutes(5),
                                    FechaCancelado = null,
                                    FechaOriginal = DateTime.Now.AddMinutes(5),
                                    FechaCrea = DateTime.Now,
                                    idOperador = Pago.idOperadorCrea ?? Pago.idOperador ?? 1,
                                    idOperadorCrea = 1,
                                    idPersona = Pago.idPersona,
                                    VecesMostrada = 0,
                                };
                                db.Avisos.InsertOnSubmit(Aviso);
                                db.SubmitChanges();
                            }
#warning ValoresQuemados

                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActPagosR", idProceso, "Actualización de Pago: " + Codigo + " Error=" + Ex.Message, i);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "ActPagosR", idProceso, "Error en Actualización de Pagos:" + Ex.Message, 0);
                }
            }
        }
        private void ActualizarPagosComDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActPagosC");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarPagosCom";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "ActPagosC", idProceso, "Inicio Proceso Actualizacion de Comisiones de Pagos Total Pagos/comisiones=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        String CodigoPago = Convert.ToString(Fila["CodigoPago"]);
                        String CodigoOperador = Convert.ToString(Fila["CodigoOperador"]);
                        String CodigoCuenta = Convert.ToString(Fila["CodigoCuenta"]);
                        String Operacion = Convert.ToString(Fila["Operacion"]);
                        Log(db, "ActPagosC", idProceso, "Actualización de Pago_Cuenta: Pago: " + CodigoPago + ", Operador: " + CodigoOperador + ", Cuenta: " + CodigoCuenta, i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);

                        try
                        {
                            Int32 idPago = db.Pagos.Single(x => x.Codigo == CodigoPago).idPago;
                            Int32 idOperador = db.Operadores.Single(x => x.Codigo == CodigoOperador).idOperador;
                            Int32 idCuenta = db.Cuentas.Single(x => x.Codigo == CodigoCuenta).idCuenta;
                            Log(db, "ActPagosC", idProceso, "Actualización de Pago_Cuenta por id: Pago: " + idPago.ToString() + ", Operador: " + idOperador.ToString() + ", Cuenta: " + idCuenta.ToString(), i);

                            if (Operacion == "D")
                            {
                                db.Pagos_Cuentas.Single(x => x.idCuenta == idCuenta && x.idPago == idPago).idOperador = null;
                            }
                            else
                            {
                                db.Pagos_Cuentas.Single(x => x.idCuenta == idCuenta && x.idPago == idPago).idOperador = idOperador;
                            }
                            db.SubmitChanges();

                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActPagosC", idProceso, "Se salta porque no existe pago, operador o cuenta: " + Ex.Message, 0);
                            continue;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "ActPagosC", idProceso, "Error en el proceso: " + Ex.Message, 0);
                }
            }
        }
        private void ActualizarObservacionesDesdeOrigen(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "ActObservaciones");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ActualizarObservaciones";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "ActObservaciones", idProceso, "Inicio Proceso Actualizacion de Observaciones, Total Observaciones=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        String Codigo = Convert.ToString(Fila["Codigo"]);
                        Log(db, "ActObservaciones", idProceso, "Actualización de Observacion: " + Codigo, i);
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        try
                        {
                            String CodigoTipoObservacion = Convert.ToString(Fila["CodigoTipoObservacion"]);
                            String CodigoPersona = Convert.ToString(Fila["CodigoPersona"]);
                            String CodigoCliente = Convert.ToString(Fila["CodigoCliente"]);
                            DateTime Fecha = Convert.ToDateTime(Fila["Fecha"]);
                            Int32 idTipoObservacion = db.TiposObservaciones.SingleOrDefault(x => x.idOrigen == idOrigen && x.Codigo == CodigoTipoObservacion).idTipoObservacion;
                            Int32 idPersona = db.Personas.SingleOrDefault(x => x.Codigo == CodigoPersona).idPersona;
                            Int32 idCliente = db.Clientes.SingleOrDefault(x => x.Codigo == CodigoCliente).idCliente;

                            Entidades.PersonasObservaciones PO = db.PersonasObservaciones.SingleOrDefault(x => x.Codigo == Codigo && x.idOrigen == idOrigen);
                            Boolean Nuevo = PO == null;
                            if (Nuevo)
                            { //Crear nuevo Reclamo si no existe
                                PO = new Entidades.PersonasObservaciones();
                                db.PersonasObservaciones.InsertOnSubmit(PO);
                                PO.Codigo = Codigo;
                                PO.idOrigen = idOrigen;
                            }
                            PO.idPersona = idPersona;
                            PO.idCliente = idCliente;
                            PO.idTipoObservacion = idTipoObservacion;
                            PO.Fecha = Fecha;
                            db.SubmitChanges();
                        }
                        catch (Exception Ex)
                        {
                            Log(db, "ActObservaciones", idProceso, "Actualización de Observacion: " + Codigo + " Error=" + Ex.Message, i);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "ActObservaciones", idProceso, "Error en Actualización de Observaciones:" + Ex.Message, 0);
                }
            }
        }

        /*private List<String> ObtenerTelefonos(String Telefonos)
        {
            List<String> Result = new List<String>();
            foreach (String Telefono in Telefonos.Split(','))
            {
                if (Telefono.Trim() == "") continue;
                String[] ColTelf = Telefono.Split('/');
                foreach (String TelefonoReal in ColTelf)
                {
                    if (TelefonoReal.Trim() == "") continue;
                    String TelefonoIns = (TelefonoReal.Length < 5) ? ColTelf[0].Substring(0, ColTelf[0].Length - TelefonoReal.Length) + TelefonoReal : TelefonoReal;
                    //if (!Persona.Telefonos.Any(x => x.Telefono == TelefonoIns))
                    //{
                    Result.Add(TelefonoIns);
                    //}
                }
            }
            return Result;
        }*/

        private void TraerContactos(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "TraerContactos");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ObtenerContactos";
                    Comm.CommandTimeout = 10 * 60;
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    int i = 0;
                    Log(db, "TraerContactos", idProceso, "Inicio Proceso Importación de Contactos, Total Contactos=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        try
                        {
                            Log(db, "TraerContactos", idProceso, "Importando Contacto", i);
                            String CodigoPersona = (String)Fila["CodigoPersona"];
                            Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.Codigo == CodigoPersona);
                            if (Persona == null) continue;
                            String CodigoContacto = (String)Fila["Codigo"];
                            if (db.PersonasContacto.SingleOrDefault(x => x.Codigo == CodigoContacto) != null) continue;
                            Entidades.PersonasContacto Contacto = new Entidades.PersonasContacto();
                            Persona.PersonasContacto.Add(Contacto);
                            Contacto.Codigo = CodigoContacto;
                            Contacto.Nombre = (String)Fila["Nombre"];
                            Contacto.Cargo = "Contacto";
                            Contacto.Email = !Fila.IsNull("Email") ? (String)Fila["Email"] : "";
                            Contacto.Activa = true;
                            if (!Fila.IsNull("Telefonos"))
                            {
                                Contacto.Telefonos.Add(new Entidades.Telefonos { Importado = true, idOperador = 1, idOperadorConfirmado = 1, idPersona = Persona.idPersona, Telefono = (String)Fila["Telefonos"], Extension = !Fila.IsNull("Extension") ? (String)Fila["Extension"] : (String)null });
                            }
                            if (!Fila.IsNull("Fax"))
                            {
                                Contacto.Telefonos.Add(new Entidades.Telefonos { Importado = true, idPersona = Persona.idPersona, Telefono = (String)Fila["Fax"], idOperador = 1, idOperadorConfirmado = 1 });
                            }
                            db.SubmitChanges();
                        }
                        catch { }
                    }
                }
                catch (Exception Ex)
                {
                    Log(db, "TraerContactos", idProceso, "Error en Importación de Contactos:" + Ex.Message, 0);
                }
            }

        }
        private void ActualizarRutas(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {

                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();
                    var Cuentas = db.Cuentas.Where(x => x.Ruta == null);
                    Int32 Total = Cuentas.Count();
                    Int32 i = 0;
                    foreach (Entidades.Cuentas Cuenta in Cuentas)
                    {
                        i++;
                        Debug.Print("Cuenta:{0}/{1}", i, Total);
                        OleDbCommand Comm4 = Conn.CreateCommand();
                        Comm4.CommandType = CommandType.StoredProcedure;
                        Comm4.CommandText = "Cobranzas.ObtenerDigital";
                        Comm4.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta.Codigo;
                        OleDbDataAdapter DA4 = new OleDbDataAdapter(Comm4);
                        try
                        {
                            DataSet DS4 = new DataSet();
                            DA4.Fill(DS4);
                            DataRow Fila = DS4.Tables[0].Rows[0];
                            using (WebClient client = new WebClient())
                            {
                                String S = client.DownloadString(Convert.ToString(Fila["RutaConfirmacion"]));
                            }
                            Cuenta.Ruta = Convert.ToString(Fila["Archivo"]);
                            db.SubmitChanges();
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
        private void ActualizarPersonasContacto(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();
                    foreach (Entidades.Personas Persona in db.Personas)
                    {
                        //PersonasContacto
                        OleDbCommand Comm4 = Conn.CreateCommand();
                        Comm4.CommandType = CommandType.StoredProcedure;
                        Comm4.CommandText = "Cobranzas.PersonaContactos";
                        Comm4.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = Persona.Codigo;
                        Comm4.Parameters.Add("idPais", OleDbType.Char).Value = Persona.idPais;
                        OleDbDataAdapter DA4 = new OleDbDataAdapter(Comm4);
                        DataSet DS4 = new DataSet();
                        DA4.Fill(DS4);

                        foreach (DataRow Fila4 in DS4.Tables[0].Rows)
                        {
                            try
                            {
                                Debug.Print("Crear Contacto");
                                Entidades.PersonasContacto Contacto = new Entidades.PersonasContacto();
                                Contacto.idPersona = Persona.idPersona;
                                Contacto.Nombre = Convert.ToString(Fila4["Nombre"]);
                                Contacto.Email = Convert.ToString(Fila4["Email"]);
                                Contacto.Cargo = Convert.ToString(Fila4["Cargo"]);
                                //Contacto.idCliente = null;
                                if (!Fila4.IsNull("Telefono1")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono1"]), Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                if (!Fila4.IsNull("Telefono2")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono2"]), Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                db.PersonasContacto.InsertOnSubmit(Contacto);
                            }
                            catch (Exception Ex)
                            {
                                Debug.Print(Ex.Message);
                            }
                        }
                        db.SubmitChanges();
                    }
                }
                catch { }
            }
        }
        private void ActualizarSoportes(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();
                    foreach (Entidades.Cuentas Cuenta in db.Cuentas)
                    {
                        //Soportes
                        OleDbCommand Comm4 = Conn.CreateCommand();
                        Comm4.CommandType = CommandType.StoredProcedure;
                        Comm4.CommandText = "Cobranzas.CuentaSoportes";
                        Comm4.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta.Codigo;
                        OleDbDataAdapter DA4 = new OleDbDataAdapter(Comm4);
                        DataSet DS4 = new DataSet();
                        DA4.Fill(DS4);

                        foreach (DataRow Fila4 in DS4.Tables[0].Rows)
                        {
                            try
                            {
                                Debug.Print("Crear SoporteCuenta");
                                Entidades.Soportes Soporte = new Entidades.Soportes();
                                Soporte.Codigo = Convert.ToString(Fila4["Codigo"]);
                                Soporte.Nombre = Convert.ToString(Fila4["Nombre"]);
                                Soporte.Ubicacion = Convert.ToString(Fila4["Ubicacion"]);
                                Cuenta.Soportes.Add(Soporte);
                            }
                            catch (Exception Ex)
                            {
                                Debug.Print(Ex.Message);
                            }
                        }
                        db.SubmitChanges();
                    }
                }
                catch { }
            }
        }
        private void TraerCuentasNuevas(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "TraerCuentas");
                try
                {
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandTimeout = 10 * 60;
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "Cobranzas.ObtenerCuentas";
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    DateTime Fecha = DateTime.Now;
                    int i = 0;
                    Log(db, "TraerCuentas", idProceso, "Inicio Proceso Importación de Cuentas, Origen=" + idOrigen.ToString() + " Total Cuentas=" + DS.Tables[0].Rows.Count, 0);
                    foreach (DataRow Fila in DS.Tables[0].Rows)
                    {
                        i++;
                        Debug.Print("{0}/{1}", i, DS.Tables[0].Rows.Count);
                        String Codigo = (Convert.ToString(Fila["Codigo"])).Trim();
                        String idPais = ((String)Fila["idPais"]).Trim();
                        if (idPais.Length == 2) idPais = db.Paises.Single(x => x.Iso2 == idPais).idPais; //Si trae el código de 2 dígitos, convertirlo a 3
                        Log(db, "TraerCuentas", idProceso, "Trayendo Cuenta " + Codigo, i);
                        //Si ya existe... sáltala
                        try
                        {
                            if (db.Cuentas.Any(x => x.Codigo == Codigo && x.idOrigen == idOrigen && x.Personas.idPais == idPais))
                            {
                                try
                                {
                                    OleDbCommand Comm10 = Conn.CreateCommand();
                                    Comm10.CommandType = CommandType.StoredProcedure;
                                    Comm10.CommandText = "Cobranzas.MarcarCuenta";
                                    Comm10.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Codigo;
                                    Comm10.Parameters.Add("Fecha", OleDbType.Date).Value = Fecha;
                                    Comm10.Parameters.Add("idPais", OleDbType.VarChar).Value = idPais;
                                    Comm10.ExecuteNonQuery();
                                }
                                catch (Exception Ex)
                                {
                                    Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error MC:" + Ex.Message, i);
                                }
                                continue;
                            }

                            Boolean CrearProductoCliente = false;

                            //Inserta el cliente si no existe...
                            String CodigoCliente = ((String)Fila["CodigoCliente"]).Trim();
                            Entidades.Clientes Cliente = db.Clientes.SingleOrDefault(x => x.idPais == idPais && x.Codigo == CodigoCliente && x.idTipoCliente == Origen.idTipoCliente);
                            if (Cliente == null)
                            {
                                try
                                {
                                    Debug.Print("Crear Cliente");
                                    CrearProductoCliente = true;
                                    Cliente = new Entidades.Clientes();
                                    OleDbCommand Comm2 = Conn.CreateCommand();
                                    Comm2.CommandType = CommandType.StoredProcedure;
                                    Comm2.CommandText = "Cobranzas.ObtenerCliente";
                                    Comm2.Parameters.Add("CodigoCliente", OleDbType.VarChar).Value = CodigoCliente;
                                    Comm2.Parameters.Add("idPais", OleDbType.VarChar).Value = idPais;
                                    OleDbDataAdapter DA2 = new OleDbDataAdapter(Comm2);
                                    DataSet DS2 = new DataSet();
                                    DA2.Fill(DS2);
                                    DataRow Fila2 = DS2.Tables[0].Rows[0];
                                    Cliente.idPais = idPais;
                                    Cliente.Codigo = CodigoCliente;
                                    Cliente.DireccionFiscal = Convert.ToString(Fila2["navdireccion"]).Trim();
                                    Cliente.FechaCreacion = DateTime.Now;
                                    Cliente.idTipoCliente = Origen.idTipoCliente;
                                    Cliente.Nombre = Convert.ToString(Fila2["navnombre"]).Trim();
                                    Cliente.Rif = Convert.ToString(Fila2["navrif"]).Trim();
                                    db.Clientes.InsertOnSubmit(Cliente);
                                    db.SubmitChanges();
                                }
                                catch (Exception Ex)
                                {
                                    Debug.Print(Ex.Message);
                                    Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CCl:" + Ex.Message, i);
                                }
                            }

                            String CodigoProducto = Convert.ToString(Fila["CodigoProducto"]).Trim();
                            Entidades.Productos Producto = db.Productos.SingleOrDefault(x => x.idTipoCliente == Origen.idTipoCliente && x.Codigo == CodigoProducto && x.idPais == idPais);
                            if (Producto == null)
                            {
                                try
                                {
                                    Debug.Print("Crear Producto");
                                    CrearProductoCliente = true;
                                    Producto = new Entidades.Productos();
                                    OleDbCommand Comm2 = Conn.CreateCommand();
                                    Comm2.CommandType = CommandType.StoredProcedure;
                                    Comm2.CommandText = "Cobranzas.ObtenerProducto";
                                    Comm2.Parameters.Add("CodigoProducto", OleDbType.VarChar).Value = CodigoProducto;
                                    Comm2.Parameters.Add("idPais", OleDbType.VarChar).Value = idPais;
                                    OleDbDataAdapter DA2 = new OleDbDataAdapter(Comm2);
                                    DataSet DS2 = new DataSet();
                                    DA2.Fill(DS2);
                                    DataRow Fila2 = DS2.Tables[0].Rows[0];
                                    Producto.Nombre = Convert.ToString(Fila2["descripcion"]).Trim();
                                    Producto.Codigo = CodigoProducto;
                                    Producto.idTipoCliente = Origen.idTipoCliente;
                                    Producto.idPais = idPais;
                                    db.Productos.InsertOnSubmit(Producto);
                                    db.SubmitChanges();
                                }
                                catch (Exception Ex)
                                {
                                    Debug.Print(Ex.Message);
                                    Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CP:" + Ex.Message, i);

                                }
                            }

                            if (!CrearProductoCliente)
                            {
                                //Entidades.Productos_Clientes ProductoCliente = new Entidades.Productos_Clientes();
                            }

                            if (CrearProductoCliente || !db.Productos_Clientes.Any(x => x.idProducto == Producto.idProducto && x.idCliente == Cliente.idCliente))
                            {
                                try
                                {
                                    Debug.Print("Crear Producto-Cliente");
                                    Entidades.Productos_Clientes ProductoCliente = new Entidades.Productos_Clientes();
                                    ProductoCliente.idCliente = Cliente.idCliente;
                                    ProductoCliente.idProducto = Producto.idProducto;
                                    db.Productos_Clientes.InsertOnSubmit(ProductoCliente);
                                    db.SubmitChanges();
                                }
                                catch (Exception Ex)
                                {
                                    Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CPC:" + Ex.Message, i);
                                    Debug.Print(Ex.Message);
                                }
                            }

                            String CodigoPersona = Convert.ToString(Fila["CodigoPersona"]);
                            String Rif = Convert.ToString(Fila["IdFiscal"]);
                            //                            Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.idPais == "VEN" && x.Rif == Rif);
                            Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.idPais == idPais && x.Codigo == CodigoPersona);
                            if (Persona == null)
                            {
                                try
                                {
                                    Debug.Print("Crear Persona");
                                    Persona = new Entidades.Personas();
                                    OleDbCommand Comm2 = Conn.CreateCommand();
                                    Comm2.CommandType = CommandType.StoredProcedure;
                                    Comm2.CommandText = "Cobranzas.ObtenerPersona";
                                    Comm2.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = CodigoPersona;
                                    Comm2.Parameters.Add("idPais", OleDbType.Char).Value = idPais;
                                    OleDbDataAdapter DA2 = new OleDbDataAdapter(Comm2);
                                    DataSet DS2 = new DataSet();
                                    DA2.Fill(DS2);
                                    DataRow Fila2 = DS2.Tables[0].Rows[0];

                                    //Entidades.Telefonos Tel1 = new Entidades.Telefonos();
                                    //Tel1.Telefono = Convert.ToString(Fila2["clitelf1"]).Trim();
                                    //if (Tel1.Telefono != "") Persona.Telefonos.Add(Tel1);
                                    //Entidades.Telefonos Tel2 = new Entidades.Telefonos();
                                    //Tel2.Telefono = Convert.ToString(Fila2["clitelf2"]).Trim();
                                    //if (Tel1.Telefono != "") Persona.Telefonos.Add(Tel2);

                                    String Telefonos = Convert.ToString(Fila2["Telefonos"]).Trim();
                                    foreach (String Telefono in Comunes.ObtenerTelefonos(Telefonos))
                                    {
                                        if (!Persona.Telefonos.Any(x => x.Telefono == Telefono))
                                        {
                                            Persona.Telefonos.Add(new Entidades.Telefonos { Telefono = Telefono, Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                        }
                                    }

                                    Persona.Email = Convert.ToString(Fila2["Email"]).Trim();
                                    Persona.URL = Convert.ToString(Fila2["URL"]);
                                    Persona.idPais = idPais;
                                    Persona.Codigo = CodigoPersona;
                                    Persona.DireccionFiscal = Convert.ToString(Fila2["DireccionFiscal"]).Trim();
                                    Persona.FechaCreacion = DateTime.Now;//Quemado
                                    Persona.idTipoPersona = 1;//Quemado
                                    Persona.Nombre = Convert.ToString(Fila2["Nombre"]).Trim();
                                    Persona.Rif = Convert.ToString(Fila2["Rif"]).Trim();
                                    Persona.Datos = Fila2.IsNull("Datos") ? (XElement)null : XElement.Parse(((String)Fila2["Datos"]).Trim());
                                    Persona.Contacto = Convert.ToString(Fila2["Contacto"]).Trim();
                                    Persona.DireccionEntrega = Convert.ToString(Fila2["DireccionEntrega"]).Trim();
                                    Persona.Zona = Fila2.IsNull("Zona") ? "" : Convert.ToString(Fila2["Zona"]).ToString();
                                    Persona.EnviosAutomaticos = Convert.ToBoolean(Fila2["EnviosAutomaticos"]);
                                    db.Personas.InsertOnSubmit(Persona);
                                    db.SubmitChanges();

                                    OleDbCommand Comm3 = Conn.CreateCommand();
                                    Comm3.CommandType = CommandType.StoredProcedure;
                                    Comm3.CommandText = "Cobranzas.PersonaSoportes";
                                    Comm3.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = CodigoPersona;
                                    Comm3.Parameters.Add("idPais", OleDbType.Char).Value = idPais;

                                    OleDbDataAdapter DA3 = new OleDbDataAdapter(Comm3);
                                    DataSet DS3 = new DataSet();
                                    DA3.Fill(DS3);

                                    foreach (DataRow Fila3 in DS3.Tables[0].Rows)
                                    {
                                        Entidades.Soportes Soporte = new Entidades.Soportes();
                                        Soporte.Codigo = Convert.ToString(Fila3["Codigo"]);
                                        Soporte.Nombre = Convert.ToString(Fila3["Nombre"]);
                                        Soporte.Ubicacion = Convert.ToString(Fila3["Ubicacion"]);
                                        Persona.Soportes.Add(Soporte);
                                    }
                                    db.SubmitChanges();
                                    //PersonasContacto
                                    OleDbCommand Comm4 = Conn.CreateCommand();
                                    Comm4.CommandType = CommandType.StoredProcedure;
                                    Comm4.CommandText = "Cobranzas.PersonaContactos";
                                    Comm4.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = Persona.Codigo;
                                    Comm4.Parameters.Add("idPais", OleDbType.Char).Value = idPais;
                                    OleDbDataAdapter DA4 = new OleDbDataAdapter(Comm4);
                                    DataSet DS4 = new DataSet();
                                    DA4.Fill(DS4);

                                    foreach (DataRow Fila4 in DS4.Tables[0].Rows)
                                    {
                                        try
                                        {
                                            Debug.Print("Crear Contacto");
                                            Entidades.PersonasContacto Contacto = new Entidades.PersonasContacto();
                                            Contacto.idPersona = Persona.idPersona;
                                            Contacto.Nombre = Convert.ToString(Fila4["Nombre"]);
                                            Contacto.Email = Convert.ToString(Fila4["Email"]);
                                            Contacto.Cargo = Convert.ToString(Fila4["Cargo"]);
                                            //Contacto.idCliente = null;
                                            if (!Fila4.IsNull("Telefono1")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono1"]), Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                            if (!Fila4.IsNull("Telefono2")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono2"]), Importado = true, idOperador = 1, idOperadorConfirmado = 1 });
                                            db.PersonasContacto.InsertOnSubmit(Contacto);
                                        }
                                        catch (Exception Ex)
                                        {
                                            Debug.Print(Ex.Message);
                                            Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CCo:" + Ex.Message, i);
                                        }
                                    }
                                    db.SubmitChanges();
                                }
                                catch (Exception Ex)
                                {
                                    Debug.Print(Ex.Message);
                                    Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CPe:" + Ex.Message, i);
                                }
                            }
                            Debug.Print("Crear Cuenta");

                            Entidades.Cuentas Cuenta = new Entidades.Cuentas();
                            Cuenta.Activa = Fila.IsNull("Activa") ? false : Convert.ToBoolean(Fila["Activa"]);
                            Cuenta.CambioDolar = Convert.ToDecimal(Fila["CambioDolar"]);
                            Cuenta.CambioLocal = Convert.ToDecimal(Fila["CambioLocal"]);
                            Cuenta.Codigo = Codigo;
                            Cuenta.FechaDocumento = Fila.IsNull("FechaDocumento") ? (DateTime?)null : Convert.ToDateTime(Fila["FechaDocumento"]);
                            Cuenta.FechaFin = Fila.IsNull("FechaFin") ? (DateTime?)null : Convert.ToDateTime(Fila["FechaFin"]);
                            Cuenta.FechaInicio = Fila.IsNull("FechaInicio") ? (DateTime?)null : Convert.ToDateTime(Fila["FechaInicio"]);
                            Cuenta.FechaEntrega = Fila.IsNull("FechaEntrega") ? (DateTime?)null : Convert.ToDateTime(Fila["FechaEntrega"]);
                            Cuenta.idCliente = Cliente.idCliente;
                            Cuenta.idMoneda = Convert.ToString(Fila["idMoneda"]).Trim();
                            Cuenta.idOrigen = idOrigen;
                            Cuenta.idProducto = Producto.idProducto;
                            Cuenta.idPersona = Persona.idPersona;
                            Cuenta.Monto = Convert.ToDecimal(Fila["Monto"]);
                            Cuenta.MontoRestante = Convert.ToDecimal(Fila["MontoRestante"]);
                            Cuenta.MontoBase = Convert.ToDecimal(Fila["MontoBase"]);
                            Cuenta.MontoIva = Convert.ToDecimal(Fila["MontoIva"]);
                            Cuenta.FechaCreacion = DateTime.Now;
                            Cuenta.Anulada = false;
                            Cuenta.Ruta = Fila.IsNull("Ruta") ? "" : Convert.ToString(Fila["Ruta"]);
                            XElement Datos = null;
                            try
                            {
                                Datos = Fila.IsNull("Datos") ? (XElement)null : XElement.Parse(((String)Fila["Datos"]).Trim().Replace("&", "&amp;"));
                            }
                            catch { }
                            Cuenta.Datos = Datos;
                            Cuenta.Zona = Fila.IsNull("Zona") ? "" : Convert.ToString(Fila["Zona"]);
                            Cuenta.EnReclamo = Fila.IsNull("EnReclamo") ? false : Convert.ToBoolean(Fila["EnReclamo"]);
                            db.Cuentas.InsertOnSubmit(Cuenta);
                            db.SubmitChanges();
                            //Marcar la cuenta como que ya me la traje
                            try
                            {
                                OleDbCommand Comm10 = Conn.CreateCommand();
                                Comm10.CommandType = CommandType.StoredProcedure;
                                Comm10.CommandText = "Cobranzas.MarcarCuenta";
                                Comm10.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta.Codigo;
                                Comm10.Parameters.Add("Fecha", OleDbType.Date).Value = Fecha;
                                Comm10.Parameters.Add("idPais", OleDbType.VarChar).Value = idPais;
                                Comm10.ExecuteNonQuery();
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error MC:" + Ex.Message, i);
                            }

                            //Digital de la cuenta
                            //OleDbCommand Comm5 = Conn.CreateCommand();
                            //Comm5.CommandType = CommandType.StoredProcedure;
                            //Comm5.CommandText = "Cobranzas.ObtenerDigital";
                            //Comm5.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Cuenta.Codigo;
                            //OleDbDataAdapter DA5 = new OleDbDataAdapter(Comm5);
                            //try
                            //{
                            //    DataSet DS5 = new DataSet();
                            //    DA5.Fill(DS5);
                            //    DataRow Fila5 = DS5.Tables[0].Rows[0];
                            //    using (WebClient client = new WebClient())
                            //    {
                            //        String S = client.DownloadString(Convert.ToString(Fila5["RutaConfirmacion"]));
                            //    }
                            //    Cuenta.Ruta = Convert.ToString(Fila5["Archivo"]);
                            //    db.SubmitChanges();
                            //}
                            //catch { }

                            //Soportes
                            OleDbCommand Comm7 = Conn.CreateCommand();
                            Comm7.CommandType = CommandType.StoredProcedure;
                            Comm7.CommandText = "Cobranzas.CuentaSoportes";
                            Comm7.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Codigo;
                            Comm7.Parameters.Add("idPais", OleDbType.VarChar).Value = idPais;
                            OleDbDataAdapter DA7 = new OleDbDataAdapter(Comm7);
                            DataSet DS7 = new DataSet();
                            DA7.Fill(DS7);

                            foreach (DataRow Fila7 in DS7.Tables[0].Rows)
                            {
                                try
                                {
                                    Debug.Print("Crear SoporteCuenta");
                                    Entidades.Soportes Soporte = new Entidades.Soportes();
                                    Soporte.Codigo = Convert.ToString(Fila7["Codigo"]);
                                    Soporte.Nombre = Convert.ToString(Fila7["Nombre"]);
                                    Soporte.Ubicacion = Convert.ToString(Fila7["Ubicacion"]);
                                    Cuenta.Soportes.Add(Soporte);
                                }
                                catch (Exception Ex)
                                {
                                    Debug.Print(Ex.Message);
                                    Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CSop:" + Ex.Message, i);
                                }
                            }
                            db.SubmitChanges();
                            //Reclamos
                            try
                            {
                                OleDbCommand Comm6 = Conn.CreateCommand();
                                Comm6.CommandType = CommandType.StoredProcedure;
                                Comm6.CommandText = "Cobranzas.CuentaReclamos";
                                Comm6.Parameters.Add("CodigoCuenta", OleDbType.VarChar).Value = Codigo;
                                Comm6.Parameters.Add("idPais", OleDbType.VarChar).Value = idPais;
                                OleDbDataAdapter DA6 = new OleDbDataAdapter(Comm6);
                                DataSet DS6 = new DataSet();
                                DA6.Fill(DS6);

                                foreach (DataRow Fila6 in DS6.Tables[0].Rows)
                                {
                                    Debug.Print("Crear Reclamo");
                                    try
                                    {
                                        String CodigoReclamo = Convert.ToString(Fila6["Codigo"]);
                                        Entidades.Reclamos Reclamo = db.Reclamos.SingleOrDefault(x => x.idOrigen == idOrigen && x.Codigo == CodigoReclamo && x.idPais == idPais);
                                        if (Reclamo == null)
                                        {
                                            Reclamo = new Entidades.Reclamos();
                                            Reclamo.Codigo = CodigoReclamo;
                                            Reclamo.Descripcion = Convert.ToString(Fila6["Descripcion"]);
                                            Entidades.ReclamosMotivos RM = db.ReclamosMotivos.Single(x => x.Codigo == Convert.ToString(Fila6["CodigoReclamoMotivo"]) && x.idOrigen == idOrigen && x.idPais == idPais);
                                            Reclamo.idReclamoMotivo = RM.idReclamoMotivo;
                                            Reclamo.Fecha = Convert.ToDateTime(Fila6["Fecha"]);
                                            Reclamo.Creador = Convert.ToString(Fila6["Creador"]);
                                            Reclamo.StatusInterno = 3;
                                            Reclamo.idOrigen = idOrigen;
                                            Reclamo.idPais = idPais;
                                            Reclamo.Abierto = Convert.ToBoolean(Fila6["Abierto"]);
                                            Reclamo.Procede = Convert.ToBoolean(Fila6["Procede"]);
                                            db.Reclamos.InsertOnSubmit(Reclamo);
                                            db.SubmitChanges();
                                        }
                                        try
                                        {
                                            Entidades.Cuentas_Reclamos CR = db.Cuentas_Reclamos.SingleOrDefault(x => x.idCuenta == Cuenta.idCuenta && x.idReclamo == Reclamo.idReclamo);
                                            if (CR == null)
                                            {
                                                Debug.Print("Crear Cuenta-Reclamo");
                                                CR = new Entidades.Cuentas_Reclamos();
                                                CR.idReclamo = Reclamo.idReclamo;
                                                CR.idCuenta = Cuenta.idCuenta;
                                                CR.idReclamoSolucion = Fila6.IsNull("CodigoSolucion") ? (int?)null : db.ReclamosSoluciones.Single(x => x.Codigo == Fila6["CodigoSolucion"].ToString() && x.idOrigen == idOrigen && x.idPais == idPais).idReclamoSolucion;
                                                db.Cuentas_Reclamos.InsertOnSubmit(CR);
                                                db.SubmitChanges();
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                            Debug.Print(Ex.Message);
                                            Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CCuRe:" + Ex.Message, i);
                                        }
                                    }
                                    catch (Exception Ex)
                                    {
                                        Debug.Print(Ex.Message);
                                        Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error CRe:" + Ex.Message, i);
                                    }
                                }
                                db.SubmitChanges();
                            }
                            catch (Exception Ex)
                            {
                                Debug.Print(Ex.Message);
                                Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error Cons Rec.:" + Ex.Message, i);
                            }
                            //Pagos

                            //Soportes
                        }
                        catch (Exception Ex)
                        {
                            Debug.Print(Ex.Message);
                            Log(db, "TraerCuentas", idProceso, "Error en Cuenta " + Codigo + " Error Cons Rec.:" + Ex.Message, i);
                        }
                    }
                    Conn.Close();
                }
                catch (Exception Ex)
                {
                    Debug.Print(Ex.Message);
                    Log(db, "TraerCuentas", idProceso, "Error General: " + Ex.Message, 0);
                }
            }
        }
        private void LlevarReclamos(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Int32 idProceso = ObteneridProceso(db, "LlevarReclamos");
                try
                {
                    List<Entidades.Reclamos> Reclamos = db.Reclamos.Where(x => x.Codigo == null && x.Procede && x.Cuentas_Reclamos.Any(y => y.Cuentas.idOrigen == idOrigen)).ToList();
                    Log(db, "LlevarReclamos", idProceso, "Inicio Proceso Llevar Reclamos, Total=" + Reclamos.Count.ToString(), 0);
                    if (Reclamos.Count == 0) return;
                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);
                    Conn.Open();

                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandTimeout = 10 * 60;
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "transact_reclamos";
                    if (idOrigen == 1)
                    {
                        Comm.Parameters.Add("reclamoid", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    }
                    else
                    {
                        Comm.Parameters.Add("reclamoPaisID", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    }
                    Comm.Parameters.Add("motivoid", OleDbType.Integer);

                    if (idOrigen == 1)
                    {
                        Comm.Parameters.Add("depid", OleDbType.Numeric);
                    }
                    else
                    {
                        Comm.Parameters.Add("depPaisid", OleDbType.Numeric);
                    }

                    Comm.Parameters.Add("navieraid", OleDbType.VarChar);
                    Comm.Parameters.Add("clienteid", OleDbType.Integer);
                    Comm.Parameters.Add("otros", OleDbType.VarChar);
                    Comm.Parameters.Add("ubicacion", OleDbType.VarChar);
                    Comm.Parameters.Add("observacion", OleDbType.VarChar);
                    Comm.Parameters.Add("cerrarRec", OleDbType.SmallInt);
                    Comm.Parameters.Add("transact", OleDbType.SmallInt);
                    if (idOrigen == 2)
                    {
                        Comm.Parameters.Add("empresaPaisID", OleDbType.Integer);
                    }



                    OleDbCommand Comm2 = Conn.CreateCommand();
                    Comm2.CommandTimeout = 10 * 60;
                    Comm2.CommandType = CommandType.StoredProcedure;
                    Comm2.CommandText = "transact_reclamosxfact";

                    if (idOrigen == 1)
                    {
                        Comm2.Parameters.Add("reclamoid", OleDbType.Numeric, 10);
                    }
                    else
                    {
                        Comm2.Parameters.Add("reclamoPaisid", OleDbType.Numeric, 10);
                    }
                    Comm2.Parameters.Add("serie", OleDbType.Char, 1);
                    Comm2.Parameters.Add("facturaid", OleDbType.Numeric, 10);
                    Comm2.Parameters.Add("transact", OleDbType.SmallInt);
                    Comm2.Parameters.Add("error_insert", OleDbType.VarChar, 100).Direction = ParameterDirection.Output;
                    Comm2.Parameters.Add("aplicarTodas", OleDbType.Boolean);
                    if (idOrigen == 1)
                    {
                        Comm2.Parameters.Add("porSobreRestante", OleDbType.Boolean);
                    }
                    else
                    {
                        Comm2.Parameters.Add("empresaPaisid", OleDbType.Integer);
                    }

                    OleDbCommand Comm3 = Conn.CreateCommand();
                    Comm3.CommandTimeout = 10 * 60;
                    Comm3.CommandType = CommandType.StoredProcedure;
                    Comm3.CommandText = "transact_Soportes";
                    Comm3.Parameters.Add("transact", OleDbType.Numeric);
                    if (idOrigen == 1)
                    {
                        Comm3.Parameters.Add("soporteid", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    }
                    else
                    {
                        Comm3.Parameters.Add("soportePaisid", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    }
                    Comm3.Parameters.Add("modulo", OleDbType.VarChar);
                    Comm3.Parameters.Add("ID", OleDbType.Numeric);
                    Comm3.Parameters.Add("directorio", OleDbType.VarChar);
                    Comm3.Parameters.Add("nombre", OleDbType.VarChar);
                    Comm3.Parameters.Add("ext", OleDbType.VarChar);
                    if (idOrigen == 1)
                    {
                        Comm3.Parameters.Add("ID2", OleDbType.VarChar);
                        Comm3.Parameters.Add("ID3", OleDbType.VarChar);
                    }
                    else
                    {
                        Comm3.Parameters.Add("empresaPaisID", OleDbType.Integer);
                        Comm3.Parameters.Add("tabla", OleDbType.Integer);
                    }
                    Comm3.Parameters.Add("digitalizar", OleDbType.Boolean);
                    Comm3.Parameters.Add("navieraid", OleDbType.VarChar);

                    OleDbCommand Comm4 = Conn.CreateCommand();
                    Comm4.CommandType = CommandType.Text;
                    if (idOrigen == 1)
                    {
                        Comm4.CommandText = "update treclamos set usuapertura=? where reclamoid=?";
                        Comm4.Parameters.Add("Creador", OleDbType.VarChar, 20);
                        Comm4.Parameters.Add("Reclamoid", OleDbType.Numeric, 10);
                    }
                    else
                    {
                        Comm4.CommandText = "update treclamos set usuapertura=? where reclamopaisid=? and empresapaisid=?";
                        Comm4.Parameters.Add("Creador", OleDbType.VarChar, 20);
                        Comm4.Parameters.Add("ReclamoPaisid", OleDbType.Numeric, 10);
                        Comm4.Parameters.Add("Empresapaisid", OleDbType.Integer, 10);
                    }

                    foreach (Entidades.Reclamos Reclamo in Reclamos)
                    {
                        Int32 EmpresaPaisId = 0;
                        String Ubicacion = "Caracas";
                        if (idOrigen == 2)
                        {
                            OleDbCommand CommEP = Conn.CreateCommand();
                            CommEP.CommandTimeout = 10 * 60;
                            CommEP.CommandType = CommandType.Text;
                            CommEP.CommandText = "SELECT Empresapaisid from oempresapais as ep inner join opais as p on ep.pais=p.pais where p.iso3='" + Reclamo.idPais + "'";
                            EmpresaPaisId = Convert.ToInt32(CommEP.ExecuteScalar());
                            CommEP.CommandText = "SELECT Top 1 ES.SubrID FROM dstEstacion AS ES INNER JOIN SubRedSerie AS SS ON SS.subrID=ES.subrID INNER JOIN oOficSerie AS OS ON OS.serie=SS.serieDMDG AND OS.empresaPaisID=SS.empresaPaisID INNER JOIN oEmpresapais as EP on SS.Empresapaisid=Ep.empresapaisid INNER JOIN oPais as P on Ep.pais=p.pais WHERE ES.usuario=SUSER_NAME()and p.iso3='" + Reclamo.idPais + "'";
                            Ubicacion = Convert.ToString(CommEP.ExecuteScalar());
                        }

                        OleDbTransaction Trans = Conn.BeginTransaction();
                        try
                        {
                            Comm.Transaction = Trans;
                            Comm2.Transaction = Trans;
                            //Comm3.Transaction = Trans;
                            Comm4.Transaction = Trans;
                            //Insertar Encabezado del Reclamo.
                            Comm.Parameters["motivoid"].Value = Reclamo.ReclamosMotivos.Codigo;
                            if (idOrigen == 1)
                            {
                                Comm.Parameters["depid"].Value = 1;//Reclamo.idDepartamento;
                            }
                            else
                            {
                                Comm.Parameters["depPaisid"].Value = 1;//Reclamo.idDepartamento;
                            }
                            Comm.Parameters["navieraid"].Value = DBNull.Value;
                            Comm.Parameters["clienteid"].Value = Reclamo.Cuentas_Reclamos.First().Cuentas.Personas.Codigo;
                            Comm.Parameters["otros"].Value = DBNull.Value;
                            Comm.Parameters["ubicacion"].Value = Ubicacion;
                            Comm.Parameters["observacion"].Value = Reclamo.Descripcion;
                            Comm.Parameters["cerrarRec"].Value = 0;
                            Comm.Parameters["transact"].Value = 1;
                            if (idOrigen == 2)
                            {
                                Comm.Parameters["empresaPaisID"].Value = EmpresaPaisId;
                            }
                            Comm.ExecuteNonQuery();
                            if (idOrigen == 1)
                            {
                                Reclamo.Codigo = Comm.Parameters["reclamoid"].Value.ToString();
                            }
                            else
                            {
                                Reclamo.Codigo = Comm.Parameters["reclamoPaisid"].Value.ToString();
                            }
                            //Actualizar el creador del reclamo
                            Comm4.Parameters["Creador"].Value = Reclamo.Creador;
                            if (idOrigen == 1)
                            {
                                Comm4.Parameters["Reclamoid"].Value = Reclamo.Codigo;
                            }
                            else
                            {
                                Comm4.Parameters["ReclamoPaisid"].Value = Reclamo.Codigo;
                                Comm4.Parameters["EmpresaPaisId"].Value = EmpresaPaisId;
                            }
                            Comm4.ExecuteNonQuery();
                            //Agregar Cuentas.
                            foreach (Entidades.Cuentas_Reclamos CR in Reclamo.Cuentas_Reclamos)
                            {
                                if (idOrigen == 1)
                                {
                                    Comm2.Parameters["reclamoid"].Value = Convert.ToInt32(Reclamo.Codigo);
                                }
                                else
                                {
                                    Comm2.Parameters["reclamoPaisid"].Value = Convert.ToInt32(Reclamo.Codigo);
                                }
                                Comm2.Parameters["serie"].Value = CR.Cuentas.Codigo.Substring(0, 1);
                                Comm2.Parameters["facturaid"].Value = Convert.ToInt32(CR.Cuentas.Codigo.Substring(1));
                                Comm2.Parameters["transact"].Value = 1;
                                Comm2.Parameters["aplicarTodas"].Value = false;
                                if (idOrigen == 1)
                                {
                                    Comm2.Parameters["porSobreRestante"].Value = false;
                                }
                                else
                                {
                                    Comm2.Parameters["empresaPaisid"].Value = EmpresaPaisId;
                                }
                                Comm2.ExecuteNonQuery();
                            }
                            Trans.Commit();
                            db.SubmitChanges();
                            try
                            {
                                foreach (Entidades.Soportes Soporte in db.Soportes.Where(x => x.idCliente == Reclamo.Cuentas_Reclamos.First().Cuentas.idPersona && x.Tabla == "Reclamos" && x.idTabla == Reclamo.idReclamo))
                                {
                                    String RutaDestino = @"\\veccsvs010\SoportesSCI\ProcesarReclamo\RECLAMO_N" + Reclamo.Codigo + @"\";
                                    String RutaDestino2 = @"ftp://veccsvs010/SoportesSCI/ProcesarReclamo/RECLAMO_N" + Reclamo.Codigo;
                                    Directory.CreateDirectory(RutaDestino);
                                    File.Copy(Soporte.Ubicacion, RutaDestino + Soporte.Nombre);
                                    String Extension = Soporte.Nombre.Split('.').Last();
                                    Comm3.Parameters["transact"].Value = 1;
                                    Comm3.Parameters["modulo"].Value = "ProcesarReclamo";
                                    Comm3.Parameters["ID"].Value = Reclamo.Codigo;
                                    Comm3.Parameters["directorio"].Value = RutaDestino2;
                                    Comm3.Parameters["nombre"].Value = Soporte.Nombre;
                                    Comm3.Parameters["ext"].Value = Extension;
                                    if (idOrigen == 1)
                                    {
                                        Comm3.Parameters["ID2"].Value = "";
                                        Comm3.Parameters["ID3"].Value = "";
                                    }
                                    else
                                    {
                                        Comm3.Parameters["empresaPaisID"].Value = EmpresaPaisId;
                                        Comm3.Parameters["tabla"].Value = "treclamos";
                                    }
                                    Comm3.Parameters["digitalizar"].Value = false;
                                    Comm3.Parameters["navieraid"].Value = "";
                                    Comm3.ExecuteNonQuery();
                                    Soporte.Codigo = Comm3.Parameters["soporteid"].Value.ToString();
                                }
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "LlevarReclamos", idProceso, "Error en Reclamo " + Reclamo.idReclamo + " Error Llevando Soportes:" + Ex.Message, 0);
                            }
                            //Crear Aviso al supervisor
                            try
                            {
                                Entidades.Avisos Aviso = new Entidades.Avisos();
                                db.Avisos.InsertOnSubmit(Aviso);
                                Aviso.FechaAviso = DateTime.Now.AddMinutes(5);
                                Aviso.idOperadorCrea = Reclamo.idOperador.Value;
                                Aviso.idOperador = Reclamo.Operadores.idSupervisor ?? Reclamo.idOperador.Value;
                                Aviso.FechaCrea = DateTime.Now;
                                Aviso.Aviso = "El Reclamo " + Reclamo.Codigo + " del Cliente " + Reclamo.Cuentas_Reclamos.First().Cuentas.Personas.Codigo + "(" + Reclamo.idPais + ") Ha sido creado ";
                                Aviso.idPersona = Reclamo.Cuentas_Reclamos.First().Cuentas.idPersona;
                                Aviso.FechaOriginal = Aviso.FechaAviso;
                                db.SubmitChanges();
                            }
                            catch { }

                        }
                        catch (Exception Ex)
                        {
                            Trans.Rollback();
                            db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, Reclamo);
                            Reclamo.Resultado = Ex.Message;
                            //Reclamo.FechaResultado = DateTime.Now;
                            Reclamo.Procede = false;
                            db.SubmitChanges();
                            Log(db, "LlevarReclamos", idProceso, "Error en Reclamo " + Reclamo.idReclamo + " Error Llevando Reclamo:" + Ex.Message, 0);
                            try
                            {
                                Entidades.Avisos Aviso = new Entidades.Avisos();
                                db.Avisos.InsertOnSubmit(Aviso);
                                Aviso.FechaAviso = DateTime.Now.AddMinutes(5);
                                Aviso.idOperadorCrea = 1;
                                Aviso.idOperador = Reclamo.idOperador.Value;
                                Aviso.FechaCrea = DateTime.Now;
                                Aviso.Aviso = "No se creó el Reclamo del Cliente " + Reclamo.Cuentas_Reclamos.First().Cuentas.Personas.Codigo + "(" + Reclamo.idPais + ") por el siguiente motivo: " + Ex.Message;
                                Aviso.idPersona = Reclamo.Cuentas_Reclamos.First().Cuentas.idPersona;
                                Aviso.FechaOriginal = Aviso.FechaAviso;
                                db.SubmitChanges();
                            }
                            catch { }

                        }
                    }
                }
                catch (Exception Ex)
                {
                    Debug.Print(Ex.Message);
                    Log(db, "LlevarReclamos", idProceso, "Error General:" + Ex.Message, 0);
                }
            }//*/
        }

        private void LlevarPagos(int idOrigen)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Int32 idProceso = ObteneridProceso(db, "LlevarPagos");
                    List<Entidades.Pagos> Pagos = db.Pagos.Where(x => x.FechaAprobado == null && x.Confirmado == null && x.Codigo == null && x.Personas.Cuentas.Any(y => y.idOrigen == idOrigen)).ToList();
                    Log(db, "LlevarPagos", idProceso, "Iniciando Proceso de Llevado de Pagos, TotalPagos=" + Pagos.Count, 0);
                    if (Pagos.Count == 0) return;

                    Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                    //                    OleDbConnection Conn = new OleDbConnection("Provider=SQLOLEDB.1;Persist Security Info=False;User ID=mberroteran;Password=mberroteran;Initial Catalog=milleniumv2;Data Source=VECCSVS020");
                    OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);//"Provider=SQLOLEDB.1;Persist Security Info=False;User ID=mberroteran;Password=mberroteran;Initial Catalog=milleniumv2;Data Source=VECCSVS020");
                    Conn.Open();
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandTimeout = 10 * 60;
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.CommandText = "insertar_temp_cobro";
                    Comm.Parameters.Add("cobroid_new", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    Comm.Parameters.Add("descripcion", OleDbType.VarChar);
                    Comm.Parameters.Add("fechaCobro", OleDbType.Date).Direction = ParameterDirection.Output;
                    Comm.Parameters.Add("ptocreacion", OleDbType.Char);
                    Comm.Parameters.Add("bolivar", OleDbType.Boolean);
                    Comm.Parameters.Add("cobradorid", OleDbType.SmallInt);
                    Comm.Parameters.Add("clienteid", OleDbType.Integer);
                    //Comm.Parameters.Add("agenteid", OleDbType.Integer).Value=null;

                    OleDbCommand Comm2 = Conn.CreateCommand();
                    Comm2.CommandTimeout = 10 * 60;
                    Comm2.CommandType = CommandType.StoredProcedure;
                    Comm2.CommandText = "insertar_temp_entradasbanc";
                    Comm2.Parameters.Add("entradaID", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    Comm2.Parameters.Add("numcuenta", OleDbType.Char);
                    Comm2.Parameters.Add("bancoid", OleDbType.Char);
                    Comm2.Parameters.Add("fecharecepcion", OleDbType.Date);
                    Comm2.Parameters.Add("fechaefectiva", OleDbType.Date);
                    Comm2.Parameters.Add("numdeposito", OleDbType.Char);
                    Comm2.Parameters.Add("descripcion", OleDbType.Char);
                    Comm2.Parameters.Add("teID", OleDbType.TinyInt);
                    Comm2.Parameters.Add("cobroid", OleDbType.Numeric);
                    Comm2.Parameters.Add("montoefectivo", OleDbType.Numeric);
                    Comm2.Parameters.Add("bolivar", OleDbType.Boolean);


                    OleDbCommand Comm3 = Conn.CreateCommand();
                    Comm3.CommandTimeout = 10 * 60;
                    Comm3.CommandType = CommandType.StoredProcedure;
                    Comm3.CommandText = "insertar_temp_cheque";

                    Comm3.Parameters.Add("IDcheque", OleDbType.Numeric).Direction = ParameterDirection.Output;
                    Comm3.Parameters.Add("entradaID", OleDbType.Numeric);
                    Comm3.Parameters.Add("chequeid", OleDbType.Char);
                    Comm3.Parameters.Add("chequebco", OleDbType.Char);
                    Comm3.Parameters.Add("montocheque", OleDbType.Numeric);
                    Comm3.Parameters.Add("cobroid", OleDbType.Numeric);
                    Comm3.Parameters.Add("bancoid", OleDbType.Char);
                    Comm3.Parameters.Add("numdeposito", OleDbType.Char);
                    Comm3.Parameters.Add("chequefecha", OleDbType.Char);
                    Comm3.Parameters.Add("bolivar", OleDbType.Boolean);

                    OleDbCommand Comm4 = Conn.CreateCommand();
                    Comm4.CommandTimeout = 10 * 60;
                    Comm4.CommandType = CommandType.StoredProcedure;
                    Comm4.CommandText = "Cobranzas.Insertar_Pagos_Cuentas";
                    Comm4.Parameters.Add("CodigoPago", OleDbType.VarChar);
                    Comm4.Parameters.Add("CodigoCuenta", OleDbType.VarChar);
                    Comm4.Parameters.Add("Monto", OleDbType.Decimal);
                    Comm4.Parameters.Add("Retencion1", OleDbType.Decimal);
                    Comm4.Parameters.Add("Retencion2", OleDbType.Decimal);
                    Comm4.Parameters.Add("Pais", OleDbType.Char, 3);

                    Int32 i = 0;
                    foreach (Entidades.Pagos Pago in Pagos)
                    {
                        i++;
                        Log(db, "LlevarPagos", idProceso, "Llevando Pago " + Pago.idPago, i);

                        OleDbTransaction Trans = Conn.BeginTransaction();
                        Boolean PagoListo = false;
                        try
                        {
                            Comm.Transaction = Trans;
                            Comm2.Transaction = Trans;
                            Comm3.Transaction = Trans;
                            Comm4.Transaction = Trans;
                            //Insertar Cobro temporal...
                            Comm.Parameters["descripcion"].Value = Pago.Descripcion;
                            //XElement Elemento = Pago.Pagos_Cuentas.First().Cuentas.Datos;
                            Comm.Parameters["ptocreacion"].Value = "VECRC";//Elemento.Descendants("Dato").Where(x => x.Attribute("Clave").Value == "Port").First().Value;
                            //.Nodes().Where(x => x.Attribute("Clave").Value == "Port").First().Value;
                            //XPathEvaluate("/Datos/Dato[@Clave=\"Port\"](1)").ToString();
                            Comm.Parameters["bolivar"].Value = Pago.idMoneda == Pago.Personas.Paises.idMoneda;
                            Comm.Parameters["cobradorid"].Value = Convert.ToInt16(Pago.Operadores.Codigo);
                            Comm.Parameters["clienteid"].Value = Convert.ToInt32(Pago.Personas.Codigo);
                            Comm.ExecuteNonQuery();
                            Pago.Codigo = Comm.Parameters["cobroid_new"].Value.ToString();

                            //Insertar EntradasBanc Temporal
                            //                        , OleDbType.Numeric).Direction = ParameterDirection.Output;
                            Comm2.Parameters["numcuenta"].Value = Pago.BancosPropios.NroCuenta;
                            Comm2.Parameters["bancoid"].Value = Pago.BancosPropios.Bancos.Codigo;
                            Comm2.Parameters["fecharecepcion"].Value = Pago.Fecha;
                            Comm2.Parameters["fechaefectiva"].Value = Pago.Fecha;
                            Comm2.Parameters["numdeposito"].Value = Pago.Referencia;
                            Comm2.Parameters["descripcion"].Value = Pago.Descripcion;
                            Comm2.Parameters["teID"].Value = Convert.ToByte(Pago.TipoPago);
                            Comm2.Parameters["cobroid"].Value = Pago.Codigo;
                            Comm2.Parameters["montoefectivo"].Value = Pago.MontoEfectivo;
                            Comm2.Parameters["bolivar"].Value = Pago.idMoneda == Pago.Personas.Paises.idMoneda;
                            Comm2.ExecuteNonQuery();

                            Int32 Entrada = Convert.ToInt32(Comm2.Parameters["entradaID"].Value);
                            foreach (Entidades.Pagos_Cuentas PC in Pago.Pagos_Cuentas)
                            {
                                Comm4.Parameters["CodigoPago"].Value = Pago.Codigo;
                                Comm4.Parameters["CodigoCuenta"].Value = PC.Cuentas.Codigo;
                                Comm4.Parameters["Monto"].Value = PC.Monto;
                                Comm4.Parameters["Retencion1"].Value = PC.Retencion1;
                                Comm4.Parameters["Retencion2"].Value = PC.Retencion2;
                                Comm4.Parameters["Pais"].Value = Pago.Personas.idPais;
                                Comm4.ExecuteNonQuery();
                            }

                            foreach (Entidades.PagosDet Cheque in Pago.PagosDet)
                            {
                                Comm3.Parameters["entradaID"].Value = Entrada;
                                Comm3.Parameters["chequeid"].Value = Cheque.NroCheque;
                                Comm3.Parameters["chequebco"].Value = Cheque.Bancos.Codigo;
                                Comm3.Parameters["montocheque"].Value = Cheque.Monto;
                                Comm3.Parameters["cobroid"].Value = Pago.Codigo;
                                Comm3.Parameters["bancoid"].Value = Pago.BancosPropios.Bancos.Codigo;
                                Comm3.Parameters["numdeposito"].Value = Pago.Referencia;
                                Comm3.Parameters["chequefecha"].Value = Pago.Fecha.ToString("yyyy-MM-dd");//warning
                                Comm3.Parameters["bolivar"].Value = Pago.idMoneda == Pago.Personas.Paises.idMoneda;
                                Comm3.ExecuteNonQuery();
                                Cheque.Codigo = Comm3.Parameters["IDcheque"].Value.ToString();
                            }
                            Trans.Commit();
                            PagoListo = true;
                            Pago.idStatusPago = 3;
#warning idQuemado, Buscar Referencia en El país para poder asignarlo
                            db.SubmitChanges();

                            try
                            {
                                Int32 Status = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "_STPago").Valor);
                                Entidades.Gestiones Gestion = new Entidades.Gestiones();
                                Gestion.Descripcion = Pago.Descripcion;
                                db.Gestiones.InsertOnSubmit(Gestion);
                                Gestion.Fecha = DateTime.Now;
                                Gestion.idOperador = Pago.idOperadorCrea.Value;
                                Gestion.idPersona = Pago.idPersona;
                                Gestion.idStatus = Status;
                                foreach (Int32 idCuenta in Pago.Pagos_Cuentas.Select(x => x.idCuenta))
                                {
                                    Entidades.Cuentas_Gestiones CG = new Entidades.Cuentas_Gestiones();
                                    CG.idCuenta = idCuenta;
                                    Gestion.Cuentas_Gestiones.Add(CG);
                                }
                                db.SubmitChanges();
                            }
                            catch (Exception Ex)
                            {
                                Log(db, "LlevarPagos", idProceso, "Error Creando la Gestión: " + Ex.Message, i);
                            }

                            //try
                            //{
                            //    Entidades.Correos Correo = new Entidades.Correos();
                            //    db.Correos.InsertOnSubmit(Correo);

                            //    Correo.Destinatarios = "mberroteran";
                            //    Correo.Mensaje =
                            //        "<h3>Pago</h3><br/>" +
                            //        "Cobro Temporal Generado: " + Pago.Codigo;
                            //    Correo.Asunto = "Pago desde el Sistema de Cobranzas, cliente: " + Pago.Personas.Nombre;
                            //    Correo.idOperador = 1;
                            //    Correo.Remitente = "administrador@veconinter.com.ve";
                            //    Correo.ResultadosAdjuntos = false;
                            //    Correo.FechaCreacion = DateTime.Now;
                            //    Correo.Leido = false;
                            //    Correo.Errores = 0;
                            //    Correo.Adjuntos = "";
                            //    Correo.Fecha = DateTime.Now;

                            //    foreach (Entidades.Soportes Soporte in db.Soportes.Where(x => x.idCliente == Pago.idPersona && x.Tabla == "Pagos" && x.idTabla == Pago.idPago))
                            //    {
                            //        Correo.Adjuntos += ";" + Soporte.Ubicacion;
                            //    }
                            //    if (Correo.Adjuntos != "") Correo.Adjuntos = Correo.Adjuntos.Substring(1);
                            //    db.SubmitChanges();
                            //}
                            //catch (Exception Ex)
                            //{
                            //    File.AppendAllText(@"C:\Cobranzas\LogCorreo.log", Ex.Message);
                            //    //Log(db, "LlevarPagos", idProceso, "Error Mandando el Correo: " + Ex.Message, i);
                            //}
                        }
                        catch (Exception Ex)
                        {
                            if (!PagoListo)
                            {
                                Log(db, "LlevarPagos", idProceso, "Error Llevando Pago " + Pago.idPago + "Mensaje: " + Ex.Message, i);
                                Trans.Rollback();
                                db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, Pago);
                                Pago.Codigo = null;
                                Pago.Resultado = Ex.Message;
                                Pago.FechaResultado = DateTime.Now;
                                Pago.Confirmado = false;
                                Pago.Aprobado = false;
                                Pago.idStatusPago = 2;
                                db.SubmitChanges();
                                Entidades.Avisos Aviso = new Entidades.Avisos();

                                Aviso.Aviso = "El Pago del cliente(" + Pago.Personas.idPais + "): " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha sido rechazado por el SCI por el siguiente motivo: " + Ex.Message;
                                Aviso.FechaAviso = DateTime.Now.AddMinutes(5);
                                Aviso.FechaCancelado = null;
                                Aviso.FechaOriginal = DateTime.Now.AddMinutes(5);
                                Aviso.FechaCrea = DateTime.Now;
                                Aviso.idOperador = Pago.idOperadorCrea ?? Pago.idOperador ?? 1;
                                Aviso.idOperadorCrea = 1;
                                Aviso.idPersona = Pago.idPersona;
                                Aviso.VecesMostrada = 0;
                                db.Avisos.InsertOnSubmit(Aviso);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                }
            }
        }
        private void MoverCorreos()
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                List<String> Correos = db.Correos.Where(x => x.RutaEml != null).OrderByDescending(x => x.idCorreo).Select(x => x.RutaEml).Distinct().ToList();
                String DirInicial = @"\\VECCSVS010\CorreosCobranzas\";
                String DirFinal = @"\\192.168.6.93\CobranzasCorreos\";
                foreach (String Correo in Correos)
                {
                    String Dir = Correo.Substring(0, 8);
                    String NombreFinal = Correo.Substring(9);
                    String NombreInicial = NombreFinal.Substring(5);
                    try
                    {
                        Directory.CreateDirectory(DirFinal + Dir);
                        File.AppendAllText(@"C:\Cobranzas\LogMovimiento.log", "\r\nMoviendo De: " + DirInicial + NombreInicial + ".eml a: " + DirFinal + Correo + ".eml");
                        File.Move(DirInicial + NombreInicial + ".eml", DirFinal + Correo + ".eml");
                    }
                    catch (Exception Ex)
                    {
                        File.AppendAllText(@"C:\Cobranzas\LogMovimiento.log", "- Error " + Ex.Message);

                    };
                }
            }
        }
        private void EliminarCorreos()
        {
            return;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String RutaCorreos = db.Parametros.Single(x => x.Clave == "RutaCorreos").Valor;
                //                List<String> Correos = db.Correos.Where(x => x.RutaEml != null && x.TipoEspecial == 2).OrderBy(x => x.idCorreo).Select(x => x.IdPop3).Distinct().ToList();
                //Correos Recibidos Marcados como personal, con más de 6 meses de antigüedad y que no sea importante para nadie...
                List<Entidades.Correos> Correos = db.Correos.Where(x => x.RutaEml != null && x.TipoEspecial == 2 && x.Fecha < DateTime.Now.AddMonths(-4) && !db.Correos.Any(y => y.IdPop3 == x.IdPop3 && y.idCorreo != x.idCorreo && y.TipoEspecial != 2)).OrderBy(x => x.idCorreo).Take(2000).ToList();
                foreach (Entidades.Correos Correo in Correos)
                {
                    File.AppendAllText(@"C:\Cobranzas\LogEliminarCorreos.log", "\r\n Eliminando Correo: " + Correo.idCorreo.ToString());
                    String Ruta = RutaCorreos + Correo.RutaEml + ".eml";
                    try
                    {
                        File.Delete(Ruta);
                        db.Correos.DeleteOnSubmit(Correo);
                        db.SubmitChanges();
                    }
                    catch (Exception Ex)
                    {
                        File.AppendAllText(@"C:\Cobranzas\LogEliminarCorreos.log", "- Error " + Ex.Message);
                    }
                }
            }
        }
        private void ActualizarLlamadas(Int32 idCentralIp)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String ConnectionString = db.CentralesIp.Single(x => x.idCentralIp == idCentralIp).ConnectionString;
                Int32 idProceso = ObteneridProceso(db, "ActLlamadas");
                Log(db, "ActLlamadas", idProceso, "Inicio Proceso Actualizacion de llamadas", 0);

                List<Entidades.Llamadas> Llamadas = db.Llamadas.Where(x => x.StatusPrimario == "").ToList();
                Log(db, "ActLlamadas", idProceso, "Inicio Proceso Actualizacion, TotalLlamadas=" + Llamadas.Count, 0);
                int i = 0;
                foreach (Entidades.Llamadas Llamada in Llamadas)
                {
                    i++;
                    Log(db, "ActLlamadas", idProceso, "Actualizando Llamada=" + Llamada.Codigo, i);
                    try
                    {
                        String UniqueId = Llamada.Codigo;
                        StatusLlamada Status;
                        if (UniqueId == "")
                        {
                            CentralesIp Datos = db.Operadores.Single(x => x.idOperador == Llamada.idOperador).Oficinas.CentralesIp;
                            Status = CentralIp.StatusLlamada(ConnectionString, Llamada.Extension, Datos.PrefijoSalida + Llamada.Telefono);
                            Llamada.Codigo = Llamada.Grabacion;
                        }
                        else
                        {
                            Status = CentralIp.StatusLlamada(ConnectionString, UniqueId);
                        }
                        Llamada.Duracion = Status.Duracion;
                        Llamada.DuracionEfectiva = Status.DuracionEfectiva;
                        Llamada.StatusPrimario = Status.Status;
                        Llamada.Grabacion = Status.Status == "Contestado" ? Status.Grabacion : "";
                        Int32 Reputacion = Llamada.Telefonos.Reputacion ?? 0;
                        if (Status.Status == "Contestado")
                        {
                            if (Status.DuracionEfectiva > 45)
                            {
                                Reputacion += 5;
                            }
                            else
                            {
                                Reputacion += 1;
                            }
                        }
                        else
                        {
                            switch (Status.Status)
                            {
                                case "No Contestó":
                                    Reputacion += 1;
                                    break;
                                case "Congestionado":
                                    Reputacion += 0;
                                    break;
                                case "Falló":
                                    Reputacion -= 1;
                                    break;
                                case "Ocupado":
                                    Reputacion += 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        Llamada.Telefonos.Reputacion = Reputacion;
                        db.SubmitChanges();
                    }
                    catch (Exception Ex)
                    {
                        Log(db, "ActLlamadas", idProceso, "Error en " + Llamada.Codigo + "; " + Ex.Message, i);
                        if ((DateTime.Now - Llamada.Fecha).TotalHours >= 1)
                        {
                            Llamada.StatusPrimario = "Error";
                        }
                        db.SubmitChanges();
                    }
                }
            }
        }
    }
}

