using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Cobranzas.Entidades;
using Cobranzas.OT;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using CentralIp;
using System.Security.Principal;
using System.Web;
using System.Xml.Linq;
using System.Threading;
using System.Net.NetworkInformation;
using System.Text;
using System.Configuration;
using System.Data.Common;
//using Cobranzas.Servicios;
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Cobranzas.Entidades;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Xml.Linq;
using System.Web;
using System.Web.Hosting;
using System.Net;
using System.Data.Linq;
using Cobranzas.OT;*/
/*Reglas para Crear Servicios:
 * 1) El nombre debe comenzar con el objeto que afecta
 * 2) Si el objeto es compuesto puede usarse "_" Para separar los objetos
 * 3) La acción va al final y tiene las siguientes terminaciones:
 *      a) lst: Listado General (Devuelve List<Obj>)
 *      b) sel: Seleccion Detallada (Devuelve Obj)
 *      c) upd: Actualización (Devuelve Boolean)
 *      d) ins: Inserción (Devuelve Int32[id recién insertado])
 *      e) sav: Inserción o Actualización en el mismo procedimiento (Devuelve Int32[id recién insertado o id del objeto actualizado])
 *      f) del: Eliminación (Devuelve Boolean)
 *      g) nov: Verifica novedad (Devuelve Boolean) 
 *      h) cbo: Selección Especial para llenar Combos (Devuelve List<Obj>)
 */

namespace Cobranzas
{
    [ServiceContract(Namespace = "", SessionMode = SessionMode.Allowed)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, MaxItemsInObjectGraph = 2147483647)]
    public class wsCobranzas
    {
        // Para usar HTTP GET, agregue el atributo [WebGet]. (El valor predeterminado de ResponseFormat es WebMessageFormat.Json)
        // Para crear una operación que devuelva XML,
        //     agregue [WebGet(ResponseFormat=WebMessageFormat.Xml)]
        //     e incluya la siguiente línea en el cuerpo de la operación:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
              
        
        #region Avisos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otAviso> Avisos_Creados_lst(int idOperador, DateTime FechaDesde, DateTime FechaHasta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return (from c in db.Avisos
                            where
                            (c.idOperadorCrea == idOperador || c.idOperador == idOperador) &&
                            (
                                (c.FechaOriginal.Date >= FechaDesde.Date && c.FechaOriginal.Date <= FechaHasta.Date) ||
                                (c.FechaCrea.Date >= FechaDesde.Date && c.FechaCrea.Date <= FechaHasta.Date) ||
                                (c.FechaAviso.Date >= FechaDesde.Date && c.FechaAviso.Date <= FechaHasta.Date) ||
                                (c.FechaCancelado.HasValue && (c.FechaCancelado.Value.Date >= FechaDesde.Date && c.FechaCancelado.Value.Date <= FechaHasta.Date))
                            )
                            select new otAviso
                            {
                                idAviso = c.idAviso,
                                Aviso = c.Aviso,
                                OperadorCrea = c.Operadores1.Nombre,
                                Operador = c.Operadores.Nombre,
                                FechaOriginal = c.FechaOriginal,
                                FechaCrea = c.FechaCrea,
                                FechaAviso = c.FechaAviso,
                                FechaCancelado = c.FechaCancelado,
                                idPersona = c.idPersona,
                                CodigoPersona = c.Personas != null ? c.Personas.Codigo : "«Ninguno»",
                                NombrePersona = c.Personas != null ? c.Personas.Nombre : "«Ninguno»",
                                Comentario = c.Comentario
                            }).ToList();
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return new List<otAviso>();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otAviso> Avisos_lst(int idAviso)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<Int32> lstUsuarios = Comunes.Clones(Sesion.idOperador, db);
                    return (from c in db.Avisos
                            where c.idAviso > idAviso && lstUsuarios.Contains(c.idOperador) && c.FechaCancelado == null
                            select new otAviso
                            {
                                idAviso = c.idAviso,
                                Aviso = c.Aviso,
                                OperadorCrea = c.Operadores1.Nombre,
                                Operador = c.Operadores.Nombre,
                                FechaOriginal = c.FechaOriginal,
                                FechaCrea = c.FechaCrea,
                                FechaAviso = c.FechaAviso,
                                FechaCancelado = c.FechaCancelado,
                                idPersona = c.idPersona,
                                CodigoPersona = c.Personas != null ? c.Personas.Codigo : "«Ninguno»",
                                NombrePersona = c.Personas != null ? c.Personas.Nombre : "«Ninguno»"
                            }).ToList();
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return new List<otAviso>();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Avisos_ins(otAviso Aviso)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Avisos AvisoNew = new Avisos();
                    AvisoNew.Aviso = Aviso.Aviso;
                    AvisoNew.idOperadorCrea = Aviso.idOperadorCrea ?? Sesion.idOperador;
                    AvisoNew.idOperador = Aviso.idOperador ?? Sesion.idOperador;
                    AvisoNew.FechaCrea = DateTime.Now;
                    AvisoNew.FechaOriginal = Aviso.FechaAviso;
                    AvisoNew.FechaAviso = AvisoNew.FechaOriginal;
                    AvisoNew.idPersona = Aviso.idPersona;
                    AvisoNew.Prioritario = Aviso.Prioritario;
                    db.Avisos.InsertOnSubmit(AvisoNew);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Avisos_del(List<Int32> Avisos, String Comentario)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Int32 idAviso in Avisos)
                    {
                        Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                        Aviso.FechaCancelado = DateTime.Now;
                        Aviso.Comentario = Comentario;
                        db.SubmitChanges();
                    }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Personas
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otHistorialGestion> Gestiones_rpt(DateTime FechaDesde, DateTime FechaHasta, String idPais, Int32 idOperador, Boolean Supervisados, Int32? idCliente, Int32? idPersona, List<Int32> Status)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    List<String> Paises = idPais == "" ? new List<String>() : idPais.Split(',').ToList();
                    db.CommandTimeout = 5 * 60;
                    //return db.rptGestiones(FechaDesde, FechaHasta, idCliente, idPersona, (Status.Count = 0 ? (string)null : ""), idOperador, Supervisados).Select(x => new otHistorialGestion {  Codigo=x.Codigo,Cuentas=null, Descripcion=x.Descripcion, Fecha=x.Fecha, id=0,   });
                    List<Int32> lstOperadores;
                    if (idOperador == Sesion.idOperador)//Es el logueado
                    {
                        lstOperadores = Comunes.Clones(idOperador, db);
                    }
                    else
                    {
                        lstOperadores = new List<int>();
                        lstOperadores.Add(idOperador);
                    }
                    if (idCliente == 0) idCliente = null;
                    if (idPersona == 0) idPersona = null;
                    if (!Supervisados)
                    {
                        var Gestiones = db.Gestiones.Where(x => lstOperadores.Contains(x.idOperador) && (Paises.Count == 0 || Paises.Contains(x.Personas.idPais)) && x.Fecha.Date >= FechaDesde && x.Fecha.Date <= FechaHasta && (Status.Count == 0 || Status.Contains(x.idStatus)) && x.idPersona == (idPersona ?? x.idPersona) && (idCliente == null || x.Cuentas_Gestiones.Any(y => y.Cuentas.idCliente == idCliente)));
                        //var Gestiones=db.Gestiones.Where(x => x.idOperador == idOperador && x.Fecha.Date >= FechaDesde && x.Fecha.Date <= FechaHasta );
                        //if (idPersona != 0) Gestiones = Gestiones.Where(x=>x.Cuentas_Gestiones.Any(y => y.Cuentas.idPersona == (idPersona ?? y.Cuentas.idPersona)));
                        //if (idCliente != 0) Gestiones = Gestiones.Where(x=>x.Cuentas.idCliente == (idCliente ?? y.Cuentas.idCliente))
                        return Gestiones.Select(x => new otHistorialGestion
                                {
                                    Fecha = x.Fecha,
                                    Operador = x.Operadores.Nombre,
                                    Codigo = x.Personas.Codigo,
                                    Persona = x.Personas.Nombre,
                                    idPais = x.Personas.idPais,
                                    Descripcion = x.Descripcion,
                                    Status = x.Status.Nombre,
                                    Tipo = x.Status.Tipo,
                                    Cuentas = db.CuentasGestion(x.idGestion),//  x.Cuentas_Gestiones.Select(y => y.Cuentas.Codigo).ToList(),
                                    id = 0,
                                    Img = "/Img/Usuario16.png",
                                    Restante = 0//x.Personas.Cuentas.Where(y => y.Activa).Sum(y => y.MontoRestante)
                                }).ToList().Concat(db.Correos.Where(x => Status.Count == 0 && idCliente == null && x.idOperador == idOperador && x.FechaCreacion.Date >= FechaDesde && x.FechaCreacion.Date <= FechaHasta && x.Correos_Personas.Any(y => y.idPersona == (idPersona ?? y.idPersona) && (Paises.Count == 0 || Paises.Contains(y.Personas.idPais)))).Select(x => new otHistorialGestion
                                {
                                    Fecha = x.FechaCreacion,
                                    Operador = x.Operadores.Nombre,
                                    Codigo = x.Correos_Personas.Any() ? x.Correos_Personas.First().Personas.Codigo : "«Ninguna»",
                                    Persona = x.Correos_Personas.Any() ? x.Correos_Personas.First().Personas.Nombre : "«Ninguna»",
                                    idPais = x.Correos_Personas.Any() ? x.Correos_Personas.First().Personas.idPais : "«Ninguna»",
                                    Descripcion = x.Asunto,
                                    Status = x.RutaEml == null ? Resources.Recursos.SentMail : Resources.Recursos.ReceivedMail,
                                    Tipo = "Sistema",
                                    Cuentas = "",//new List<String>(),
                                    id = x.idCorreo,
                                    Img = x.RutaEml == null ? "/Img/CorreoSaliente16.png" : (x.Leido ? "/Img/CorreoLeido16.png" : "/Img/CorreoEntrante16.png"),
                                    Restante = 0
                                }).ToList()).OrderByDescending(x => x.Fecha).ToList();
                    }
                    else
                    {
                        List<Int32> Operadores;
                        if (idOperador == Sesion.idOperador)//Es el logueado
                        {
                            Operadores = Comunes.OperadoresSupervisados(idOperador, db);
                        }
                        else
                        {
                            Operadores = db.OperadoresSupervisados(idOperador, "").Select(x => x.idOperador ?? 0).ToList();
                        }

                        return db.Gestiones.Where(x => Operadores.Contains(x.idOperador) && (Paises.Count == 0 || Paises.Contains(x.Personas.idPais)) && x.Fecha.Date >= FechaDesde && x.Fecha.Date <= FechaHasta && (Status.Count == 0 || Status.Contains(x.idStatus)) && x.Cuentas_Gestiones.Any(y => y.Cuentas.idPersona == (idPersona ?? y.Cuentas.idPersona) && y.Cuentas.idCliente == (idCliente ?? y.Cuentas.idCliente))).Select(x => new otHistorialGestion
                        {
                            Fecha = x.Fecha,
                            Operador = x.Operadores.Nombre,
                            Codigo = x.Personas.Codigo,
                            Persona = x.Personas.Nombre,
                            idPais = x.Personas.idPais,
                            Descripcion = x.Descripcion,
                            Status = x.Status.Nombre,
                            Tipo = x.Status.Tipo,
                            Cuentas = db.CuentasGestion(x.idGestion),//  x.Cuentas_Gestiones.Select(y => y.Cuentas.Codigo).ToList(),
                            id = 0,
                            Img = "/Img/Usuario16.png",
                        }).ToList().Concat(db.Correos.Where(x => Status.Count == 0 && idCliente == null && Operadores.Contains(x.idOperador) && x.FechaCreacion.Date >= FechaDesde && x.FechaCreacion.Date <= FechaHasta && x.Correos_Personas.Any(y => y.idPersona == (idPersona ?? y.idPersona) && (Paises.Count == 0 || Paises.Contains(y.Personas.idPais)))).Select(x => new otHistorialGestion
                        {
                            Fecha = x.FechaCreacion,
                            Operador = x.Operadores.Nombre,
                            Codigo = x.Correos_Personas.Any() ? x.Correos_Personas.First().Personas.Codigo : "«Ninguna»",
                            Persona = x.Correos_Personas.Any() ? x.Correos_Personas.First().Personas.Nombre : "«Ninguna»",
                            idPais = x.Correos_Personas.Any() ? x.Correos_Personas.First().Personas.idPais : "«Ninguna»",
                            Descripcion = x.Asunto,
                            Status = x.RutaEml == null ? Resources.Recursos.SentMail : Resources.Recursos.ReceivedMail,
                            Tipo = "Fondo",
                            Cuentas = "",//new List<String>(),
                            id = x.idCorreo,
                            Img = x.RutaEml == null ? "/Img/CorreoSaliente16.png" : (x.Leido ? "/Img/CorreoLeido16.png" : "/Img/CorreoEntrante16.png")
                        }).ToList()).OrderByDescending(x => x.Fecha).ToList();
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otPersona Personas_sel(int idPersona, int idOperador)
        {
            String Mensaje = "";
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CommandTimeout = 10 * 60;
                    if (idPersona == 0) idPersona = Negocios.BuscarSiguienteGestion(idOperador);
                    if (idPersona == 0)
                    {
                        Mensaje = "Por ahora no le podemos entregar nuevas Cuentas para Gestionar, favor escoger sus clientes en la pestaña cartera.";
                        throw new Exception(Mensaje);
                    }

                    if (db.Cuentas.Any(x => x.idPersona == idPersona && x.Activa) && !(db.OperadorVeAPersona(idOperador, idPersona) ?? false))
                    {
                        Mensaje = "No puede ver esta cuenta.";
                        throw new Exception(Mensaje);
                    }

                    Sesion.SelPersona(idPersona == 0 ? (int?)null : idPersona);
                    Entidades.Personas c = db.Personas.Single(x => x.idPersona == idPersona);
                    List<Int32> lstOperadores = Comunes.OperadoresSupervisados(idOperador, db);// Comunes.Clones(idOperador, db);
                    Int32 idTipoCliente;
                    try
                    {
                        idTipoCliente = db.Cuentas.Where(x => db.CuentasOperador_idCuenta(idOperador, idPersona).Any(y => y.idCuenta == x.idCuenta)).Select(x => x.Clientes.idTipoCliente).First();// se asume que ningún operador va a tener que gestionar una misma persona en distintos tipos de negocios
                    }
                    catch
                    {
                        try
                        {
                            idTipoCliente = c.Cuentas.First().Clientes.idTipoCliente;
                        }
                        catch
                        {
                            try
                            {
                                idTipoCliente = db.Operadores.Single(x => x.idOperador == idOperador).Operadores_Asignaciones.First(x => x.idPais == c.idPais).idTipoCliente;
                            }
                            catch
                            {
                                idTipoCliente = 1;
                            }
                        }
                    }


                    var Result = new otPersona
                    {
                        Contacto = c.Contacto,
                        DireccionEntrega = c.DireccionEntrega,
                        MostrarDatos = "",//c.Cuentas.First().Clientes.TiposCliente.XMLMostrarDatos,
                        idPersona = c.idPersona,
                        idTipoPersona = c.idTipoPersona,
                        idTipoCliente = idTipoCliente,
                        Codigo = c.Codigo,
                        Nombre = c.Nombre,
                        Rif = c.Rif,
                        DireccionFiscal = c.DireccionFiscal,
                        Email = c.Email,
                        Datos = c.Datos,
                        ISO3 = c.idPais,
                        Pais = c.Paises.Nombre,
                        URL = c.URL,
                        Telefonos = c.Telefonos.Where(x => x.idOperadorConfirmadoEliminar == null && (x.idOperadorConfirmado != null || x.idOperador == idOperador)).Select(x => otTelefono.FromTelefono(x)).ToList(),
                        //Telefonos = new List<string>(),
                        PersonasContacto = c.PersonasContacto.Where(x => x.Activa).Select(x => new otPersonaContacto
                        {
                            idPersonaContacto = x.idPersonaContacto,
                            Nombre = x.Nombre,
                            Cargo = x.Cargo,
                            Telefonos = x.Telefonos.Where(y => y.idOperadorConfirmadoEliminar == null && (y.idOperadorConfirmado != null || y.idOperador == idOperador)).Select(y => otTelefono.FromTelefono(y)).ToList(),
                            Correo = x.Email
                        }).ToList(),
                        //PersonasContacto= new List<otPersonaContacto>(),
                        Soportes = c.Soportes.Where(x => x.idCuenta == null && x.Tabla == null).Select(x => new otSoporte { idSoporte = x.idSoporte, Nombre = x.Nombre, Ubicacion = x.Ubicacion }).ToList(),
                        //Soportes=new List<otSoporte>(),
                        Pagos = db.Pagos.Where(x => x.idPersona == idPersona && lstOperadores.Contains(x.idOperador ?? 0)).Select(x => new otPagoPersona
                        {
                            idPago = x.idPago,
                            Codigo = x.Codigo,
                            Tipo = x.TipoPago == 1 ? "Depósito" : "Transferencia",
                            Monto = x.MontoTotal ?? 0,
                            Fecha = x.Fecha,
                            Resultado = x.Resultado,
                            Moneda = x.idMoneda,
                            Referencia = x.Referencia,
                            Aprobado = x.Aprobado,
                            Confirmado = x.Confirmado,
                            Status = x.StatusPago.Descripcion
                        }).ToList(),
                        //Pagos= new List<otPagoPersona>(),
                        Reclamos = db.Reclamos.Where(x => x.Cuentas_Reclamos.Any(y => y.Cuentas.idPersona == idPersona)).Select(x => new otReclamosPersona
                        {
                            idReclamo = x.idReclamo,
                            Codigo = x.Codigo,
                            Fecha = x.Fecha,
                            Motivo = x.ReclamosMotivos.Descripcion,
                            Abierto = x.Abierto,
                            Procede = x.Procede,//  General.db.ReclamosStatus.Single(d => d.idReclamoStatus == x.Cuentas_Reclamos.Min(y => y.idReclamoStatus)).Descripcion,
                            MontoLocal = x.Cuentas_Reclamos.Sum(y => y.Cuentas.Monto * y.Cuentas.CambioLocal),
                            MonedaLocal = x.Cuentas_Reclamos.First().Cuentas.Clientes.Paises.idMoneda,
                            Resultado = x.Resultado,
                            Status = x.idReclamoStatus != null ? x.ReclamosStatus.Descripcion : "Sin Status",
                            Departamento = x.ReclamosDepartamentos.Departamento
                        }).ToList(),
                        //Reclamos=new List<otReclamosPersona>(),
                        Reportes = db.TiposReporte.Where(x => x.Persona && x.idPais == c.idPais && x.idTipoCliente == idTipoCliente).Select(x => new otReporte { idTipoReporte = x.idTipoReporte, Nombre = x.Nombre }).ToList(),
                        //Reportes=new List<otReporte>()
                        Observaciones = c.PersonasObservaciones.Select(x => new otPersonasObservaciones
                        {
                            Fecha = x.Fecha,
                            Descripcion = x.TiposObservaciones.Observacion,
                            Severidad = x.TiposObservaciones.Severidad,
                        }).ToList(),
                        Comentarios = c.NotaEspecial,
                        AvisosPropios = c.Avisos.Where(x => x.FechaCancelado == null && x.idOperador == idOperador).Select(x => x.idAviso).ToList()
                    };
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                if (Ex.Message == Mensaje)
                {
                    throw Ex;
                }
                else
                {
                    throw Ex.Informar();
                }
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCuenta> Personas_Cuentas_lst(int idPersona, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CommandTimeout = 10 * 60;
                    var result = db.vwCuentas.Where(x => db.CuentasOperador_idCuenta(idOperador, idPersona).Any(y => y.idCuenta == x.idCuenta)).OrderBy(x => x.Documento).Select(x => otCuenta.FromvwCuenta(x)).ToList();
                    return result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otHistorialGestion> Personas_Gestiones_lst(int idPersona, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Personas Persona = db.Personas.Single(x => x.idPersona == idPersona);
                    return db.Gestiones.Where(x => x.idPersona == idPersona).Select(x => new otHistorialGestion
                        {
                            Fecha = x.Fecha,
                            Operador = x.Operadores.Nombre,
                            Descripcion = x.Descripcion,
                            Status = x.Status.Nombre,
                            Tipo = x.Status.Tipo,
                            Cuentas = db.CuentasGestion(x.idGestion),  //x.Cuentas_Gestiones.Select(y => y.Cuentas.Codigo).ToList(),
                            id = x.idGestion,
                            Img = x.idOperador == 1 ? "/Img/Sistema16.png" : (db.Soportes.Any(y => y.idTabla == x.idGestion && y.Tabla == "Gestiones") ? "/Img/UsuarioAdjunto16.png" : "/Img/Usuario16.png"),
                        }).Concat(db.Correos.Where(x => x.Correos_Personas.Any(y => y.idPersona == idPersona)).Select(x => new otHistorialGestion
                        {
                            Fecha = x.FechaCreacion,
                            Operador = x.Operadores.Nombre,
                            Descripcion = x.Asunto,
                            Status = x.RutaEml == null ? Resources.Recursos.SentMail : Resources.Recursos.ReceivedMail,
                            Tipo = "Fondo",
                            Cuentas = "",//new List<String>(),
                            id = x.idCorreo,
                            Img = x.RutaEml == null ? (x.Errores == 3 ? "/Img/CorreoError16.png" : "/Img/CorreoSaliente16.png") : (x.Leido ? "/Img/CorreoLeido16.png" : "/Img/CorreoEntrante16.png")
                        })).Concat(db.GestionesViejas.Where(x => x.CodigoPersona.ToString() == Persona.Codigo && x.Pais == Persona.idPais).Select(x => new otHistorialGestion
                        {
                            Fecha = x.Fecha.Value,
                            Operador = x.Operador,
                            Descripcion = x.Descripcion,
                            Status = x.Status,
                            Tipo = "Fondo",
                            Cuentas = "",//new List<String>(),
                            id = 0,
                            Img = "/Img/Usuario16.png"
                        })).Concat(db.Llamadas.Where(x => x.Telefonos.idPersona == idPersona).Select(x => new otHistorialGestion
                        {
                            Fecha = x.Fecha,
                            Operador = x.Operadores.Nombre,
                            Descripcion = "Llamada al número: " + x.Telefono,
                            Status = x.StatusPrimario,
                            Tipo = "Fondo",
                            Cuentas = "",//new List<String>(),
                            id = x.idLlamada,
                            Img = "/Img/Telefono16.png"
                        })).Concat(db.Llamadas.Where(x => x.Telefonos.PersonasContacto.idPersona == idPersona).Select(x => new otHistorialGestion
                        {
                            Fecha = x.Fecha,
                            Operador = x.Operadores.Nombre,
                            Descripcion = "Llamada al número: " + x.Telefono,
                            Status = x.StatusPrimario,
                            Tipo = "Fondo",
                            Cuentas = "",//new List<String>(),
                            id = x.idLlamada,
                            Img = "/Img/Telefono16.png"
                        })).
                        OrderByDescending(x => x.Fecha).Take(500).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otPersona_lst> Personas_Tocadas_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<Int32> Operadores = Comunes.OperadoresSupervisados(idOperador, db);
                    //var result = db.Personas.Where(x => x.Gestiones.Any(y => Operadores.Contains(y.idOperador))).OrderBy(x => x.Nombre).Select(x => new otPersona() { idPersona = x.idPersona, Nombre = x.Nombre + "(" + x.Codigo + ")" }).ToList();
                    var result2 = db.Personas.Where(x => x.Cuentas.Any(y => y.Campanas_Cuentas.Any(z => z.Campanas.TipoCampana == 2 && z.Campanas.Campanas_Operadores.Any(w => Operadores.Contains(w.idOperador))) || y.Cuentas_Operadores.Any(z => Operadores.Contains(z.idOperador)))).Select(x => new otPersona_lst() { idPersona = x.idPersona, Nombre = x.Nombre + "(" + x.idPais + x.Codigo + ")", idPais = x.idPais }).ToList();
                    return result2.Distinct().OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCombo> Personas_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Personas.Select(x => new otCombo { Nombre = x.Nombre + '(' + x.Codigo + ')', id = x.idPersona }).OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


        [OperationContract, WebInvoke(Method = "POST")]
        public List<otAviso> Avisos_Persona_lst(int idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Avisos.Where(x => x.idPersona == idPersona && x.FechaCancelado == null).Select(x => new otAviso
                    {
                        Aviso = x.Aviso,
                        CodigoPersona = x.Personas.Codigo,
                        FechaAviso = x.FechaAviso,
                        idAviso = x.idAviso,
                        Operador = x.Operadores.Nombre,
                        OperadorCrea = x.Operadores1.Nombre,
                        FechaCancelado = x.FechaCancelado,
                        FechaCrea = x.FechaCrea,
                        FechaOriginal = x.FechaOriginal,
                        idOperador = x.idOperador,
                        idOperadorCrea = x.idOperadorCrea,
                        idPersona = x.idPersona,
                        NombrePersona = x.Personas.Nombre
                    }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptPersonasGestionadasResult> PersonasGestionadas_rpt(DateTime FechaDesde, DateTime FechaHasta, DateTime FechaFin, Int32? idCliente, Int32 idOperador, Boolean Supervisados, Boolean? Gestionados, String Paises)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CommandTimeout = 10 * 60;
                    return db.rptPersonasGestionadas(FechaDesde, FechaHasta, FechaFin, idCliente, idOperador, Supervisados, Gestionados, Paises).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 ImportarPersona_sel(String CodigoPersona, Int32 idOrigen, String idPais)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    String CS = db.Origenes.Single(x => x.idOrigen == idOrigen).ConnectionString;
                    using (OleDbConnection Conn = new OleDbConnection(CS))
                    {
                        Entidades.Personas Persona = db.Personas.SingleOrDefault(x => x.idPais == idPais && x.Codigo == CodigoPersona);
                        if (Persona != null) return Persona.idPersona;
                        try
                        {
                            Debug.Print("Crear Persona");
                            Persona = new Entidades.Personas();
                            OleDbCommand Comm2 = Conn.CreateCommand();
                            Comm2.CommandType = CommandType.StoredProcedure;
                            Comm2.CommandText = "Cobranzas.ObtenerPersona";
                            Comm2.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = CodigoPersona;
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
                                    Persona.Telefonos.Add(new Entidades.Telefonos { Telefono = Telefono });
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
                                    if (!Fila4.IsNull("Telefono1")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono1"]) });
                                    if (!Fila4.IsNull("Telefono2")) Contacto.Telefonos.Add(new Entidades.Telefonos { Telefono = Convert.ToString(Fila4["Telefono2"]) });
                                    db.PersonasContacto.InsertOnSubmit(Contacto);
                                }
                                catch (Exception Ex)
                                {
                                    Debug.Print(Ex.Message);
                                }
                            }
                            db.SubmitChanges();
                            return Persona.idPersona;
                        }
                        catch (Exception Ex)
                        {
                            return 0;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                return 0;
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean PersonasContacto_del(int idPersonaContacto)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.PersonasContacto.Single(x => x.idPersonaContacto == idPersonaContacto).Activa = false;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


#warning ese proveso no se usa
        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otTelefono> Persona_Telefonos_lst(Int32 idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Personas Persona = db.Personas.Single(x => x.idPersona == idPersona);
                    List<otTelefono> Result = Persona.Telefonos.Where(x => x.idOperadorConfirmadoEliminar == null && (x.idOperadorConfirmado != null || x.idOperador == Sesion.idOperador)).Select(x => otTelefono.FromTelefono(x)).ToList();
                    Result.AddRange(from pc in Persona.PersonasContacto from t in pc.Telefonos select otTelefono.FromTelefono(t));
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otPersonaContacto PersonaContacto_sel(Int32 idPersonaContacto)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var pc = db.PersonasContacto.Single(x => x.idPersonaContacto == idPersonaContacto);
                    return new otPersonaContacto { Cargo = pc.Cargo, Correo = pc.Email, idPersonaContacto = idPersonaContacto, Nombre = pc.Nombre, Telefonos = pc.Telefonos.Select(x => new otTelefono { CodigoArea = x.CodigoArea, CodigoPais = x.CodigoPais, Extension = x.Extension, idTelefono = x.idTelefono, Reputacion = x.Reputacion, Telefono = x.Telefono }).ToList() };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otTablaContacto_lst> TablaContacto_lst(Int32 idPersona, Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    DataTable dt = new DataTable();
                    dt = Negocios.ListarCorreos(idOperador, idPersona);


                    List<OT.otTablaContacto_lst> list = new List<otTablaContacto_lst>();
                    otTablaContacto_lst tc;
                    try
                    {
                        int i = 0;

                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            tc = new otTablaContacto_lst();
                            tc.Nombre = dt.Rows[i]["value"].ToString();
                            tc.Correo = " " + dt.Rows[i]["Email"].ToString();
                            list.Add(tc);

                        }
                        return list;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Cartera
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCarteraGrupo> Cartera_lst(int idOperador, Boolean Supervisado = false, Boolean IncluyeAutomatico = true)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CommandTimeout = 10 * 60;
                    List<Int32> lstOperadores;
                    if (Supervisado)
                    {
                        lstOperadores = new List<int>();
                        lstOperadores.Add(idOperador);
                    }
                    else
                    {
                        lstOperadores = Comunes.Clones(idOperador, db);
                    }
                    List<otCarteraGrupo> Result = new List<otCarteraGrupo>();
                    //Colas de Gestión
                    {
                        var Personas = db.Operador_Cuentas(idOperador, -2, IncluyeAutomatico, Supervisado).ToList();
                        if (Personas.Count != 0)
                        {
                            Result.Add(new otCarteraGrupo
                            {
                                Nombre = "Gestión Prioritaria",
                                idCampana = -2,
                                Personas = Personas
                            });
                        }
                    }

                    //Campañas Prioritarias
                    var Campanas = db.Campanas.Where(x => x.Activa && DateTime.Now >= x.FechaInicio && x.Campanas_Operadores.Any(y => lstOperadores.Contains(y.idOperador) && y.FechaFin == null)).OrderByDescending(x => x.Peso);
                    foreach (Entidades.Campanas Campana in Campanas.Where(x => x.Peso >= 0))
                    {
                        var Personas = db.Operador_Cuentas(idOperador, Campana.idCampana, IncluyeAutomatico, Supervisado).ToList();
                        if (Personas.Count != 0)
                        {
                            Result.Add(new otCarteraGrupo
                            {
                                Nombre = "Campaña: " + Campana.Nombre + "(" + Campana.Peso + ")",
                                idCampana = Campana.idCampana,
                                Personas = Personas
                            });
                        }
                    }
                    //Cuentas Individuales
                    {
                        var Personas = db.Operador_Cuentas(idOperador, 0, IncluyeAutomatico, Supervisado).ToList();
                        if (Personas.Count != 0)
                        {
                            Result.Add(new otCarteraGrupo
                            {
                                Nombre = "Asignados Directamente",
                                idCampana = 0,
                                Personas = Personas
                            });
                        }
                    }

                    foreach (Entidades.Campanas Campana in Campanas.Where(x => x.Peso < 0))
                    {
                        var Personas = db.Operador_Cuentas(idOperador, Campana.idCampana, IncluyeAutomatico, Supervisado).ToList();
                        if (Personas.Count != 0)
                        {
                            Result.Add(new otCarteraGrupo
                            {
                                Nombre = "Campaña: " + Campana.Nombre + "(" + Campana.Peso + ")",
                                idCampana = Campana.idCampana,
                                Personas = Personas
                            });
                        }
                    }

                    //Todas las Cuentas de Supervisados
                    List<Int32> Operadores = new List<int>();
                    foreach (Int32 Operador in lstOperadores)
                    {
                        Operadores.AddRange(db.OperadoresSupervisados(Operador, "").Select(x => x.idOperador ?? 0));
                        Operadores.Remove(Operador);
                    }

                    if (Operadores.Count != 0)
                    {
                        var Personas = db.Operador_Cuentas(idOperador, -1, IncluyeAutomatico, Supervisado).ToList();
                        if (Personas.Count != 0)
                        {
                            Result.Add(new otCarteraGrupo
                            {
                                Nombre = "Supervisados",
                                idCampana = -1,
                                Personas = Personas
                            });
                        }
                    }
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        //public String Operador(Cuentas_Operadores CO)
        //{
        //    if (CO == null) return "«Ninguno»";
        //    return CO.Operadores.Nombre;
        //}
        //public List<otCartera> Cartera(IQueryable<Personas> Personas, CobranzasDataContext db, Boolean IncluyeAutomatico)
        //{
        //    var car2 = (from c in Personas
        //                select new
        //                {
        //                    c.Codigo,
        //                    c.idPersona,
        //                    c.Rif,
        //                    c.Nombre,
        //                    Total = c.Cuentas.Where(x => x.Activa).Sum(x => x.Monto * x.CambioLocal),
        //                    TotalDolar = c.Cuentas.Where(x => x.Activa).Sum(x => x.Monto / x.CambioDolar),
        //                    c.Paises.Iso3,
        //                    FechaUltimaGestion = c.Gestiones.Where(x => IncluyeAutomatico || x.idOperador != 1).Max(x=>(DateTime?)x.Fecha),
        //                    UltimaGestion = c.Gestiones.Where(x => IncluyeAutomatico || x.idOperador != 1).OrderByDescending(x => x.Fecha).Select(x => x.Status.Tipo).FirstOrDefault(),
        //                    StatusUltimaGestion = c.Gestiones.Where(x => IncluyeAutomatico || x.idOperador != 1).OrderByDescending(x => x.Fecha).Select(x => x.Status.Nombre).FirstOrDefault(),
        //                    Moneda = c.Paises.idMoneda,
        //                    Deuda = c.Cuentas.Where(x => x.Activa).Sum(x => x.MontoRestante * x.CambioLocal),
        //                    DeudaDolar = c.Cuentas.Where(x => x.Activa).Sum(x => x.MontoRestante / x.CambioDolar),
        //                    TieneAviso = db.Avisos.Any(x => x.FechaCancelado == null && x.idPersona == c.idPersona),
        //                    UltimoOperador = db.Cuentas_Operadores.Where(x => x.Cuentas.idPersona == c.idPersona && x.FechaFin == null).OrderByDescending(x => x.FechaInicio).Select(x => x.Operadores.Nombre).FirstOrDefault()
        //                }).Distinct().ToList();
        //    var Status = db.Status.ToList();
        //    return (from c in car2
        //            select new otCartera
        //            {
        //                Codigo = c.Codigo,
        //                idPersona = c.idPersona,
        //                Nombre = c.Nombre,
        //                Total = c.Total,
        //                Rif = c.Rif,
        //                Iso3 = c.Iso3,
        //                TipoStatus = c.UltimaGestion == null ? "Nada" : c.UltimaGestion,
        //                Moneda = c.Moneda,
        //                TotalDolar = c.TotalDolar,
        //                Deuda = c.Deuda,
        //                DeudaDolar = c.DeudaDolar,
        //                UltimaGestion = c.FechaUltimaGestion,
        //                StatusUltimaGestion = c.StatusUltimaGestion ?? "«Ninguno»",
        //                TieneAviso = c.TieneAviso,
        //                UltimoOperador = c.UltimoOperador ?? "«Ninguno»"
        //            }).ToList();
        //}
        #endregion
        #region Supervisión

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otOperadoresSimple> OperadoresSupervisadosConSupervisor_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (idOperador == 0) idOperador = Sesion.Operador.idOperador;
                    List<otOperadoresSimple> result = new List<otOperadoresSimple>();
                    List<Int32> lstOperadores = Comunes.Clones(idOperador, db);
                    foreach (Int32 Op in lstOperadores)
                    {
                        result.AddRange(db.OperadoresSupervisados(Op, "-").Select(x => new otOperadoresSimple { idOperador = x.idOperador.Value, Nombre = x.Nombre, Jerarquia = x.Jerarquia/*, Status= Conn.GetAgentStatusS(x. */}).ToList());
                    }
                    return result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otOperadoresSimple> OperadoresSupervisados_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (idOperador == 0) idOperador = Sesion.Operador.idOperador;
                    List<otOperadoresSimple> result = new List<otOperadoresSimple>();
                    List<Int32> lstOperadores = Comunes.Clones(idOperador, db);
                    foreach (Int32 Op in lstOperadores)
                    {
                        result.AddRange(db.OperadoresSupervisados(Op, "").Select(x => new otOperadoresSimple { idOperador = x.idOperador.Value, Nombre = x.Nombre, Jerarquia = x.Jerarquia/*, Status= Conn.GetAgentStatusS(x. */}).ToList());
                    }
                    return result.OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otOperadores> OperadoresSupervisadosCompleto_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (idOperador == 0) idOperador = Sesion.Operador.idOperador;
                    List<Int32> lstOperadores = Comunes.Clones(idOperador, db);
                    List<Int32> Operadores = new List<Int32>();
                    foreach (Int32 Op in lstOperadores)
                    {
                        Operadores.AddRange(db.OperadoresSupervisados(Op, "").Select(y => y.idOperador ?? 0).ToList());
                    }
                    Operadores = Operadores.Distinct().ToList();
                    return db.Operadores.Where(x => Operadores.Contains(x.idOperador)).Select(x => otOperadores.FromOperador(x)).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Correos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCorreosLst> Operador_Correos_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otCorreosLst> Result;
                    Operadores Operador = db.Operadores.Single(x => x.idOperador == idOperador);
                    Int32 IdOperador = Operador.idClon ?? idOperador;
                    IEnumerable<Entidades.Correos> subResult = (from c in db.Correos where c.idOperador == IdOperador && !c.Correos_Personas.Any() && c.TipoEspecial == null && c.FechaEliminacion == null && c.RutaEml != null select c);
                    Result = (from c in subResult
                              orderby c.FechaCreacion descending
                              select new otCorreosLst()
                              {
                                  idCorreo = c.idCorreo,
                                  //Saliente = c.RutaEml == null, 
                                  Leido = c.Leido,
                                  //Tipo = "Op", 
                                  Asunto = c.Asunto,
                                  Fecha = c.FechaCreacion,
                                  //Mensaje = c.Mensaje, 
                                  Remitente = c.Remitente
                              }).ToList();
                    return Result;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCorreosLst> Operador_CorreosClasificadosNoLeidos_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otCorreosLst> Result;
                    Operadores Operador = db.Operadores.Single(x => x.idOperador == idOperador);
                    Int32 IdOperador = Operador.idClon ?? idOperador;
                    IEnumerable<Entidades.Correos> subResult = (from c in db.Correos where c.idOperador == IdOperador && c.Correos_Personas.Any() && c.TipoEspecial == null && c.FechaEliminacion == null && c.RutaEml != null && !c.Leido select c);
                    Result = (from c in subResult
                              orderby c.FechaCreacion descending
                              select new otCorreosLst()
                              {
                                  idCorreo = c.idCorreo,
                                  //Saliente = c.RutaEml == null, 
                                  Leido = c.Leido,
                                  //Tipo = "Op", 
                                  Asunto = c.Asunto,
                                  Fecha = c.FechaCreacion,
                                  //Mensaje = c.Mensaje, 
                                  Remitente = c.Remitente
                              }).ToList();
                    return Result;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        //[OperationContract, WebGet]
        //public Boolean Operador_Correos_nov(int idOperador, int idCorreo)
        //{
        //    try
        //    {
        //        using (CobranzasDataContext db = new CobranzasDataContext())
        //        {
        //            Boolean Result = db.Correos.Any(x => x.idOperador == idOperador && x.idCorreo > idCorreo && x.RutaEml != null /*&& !x.Correos_Personas.Any()*/);
        //            return Result;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex.Informar();
        //    }
        //}
        [OperationContract, WebGet]
        public List<otCorreos> Persona_Correos_lst(int idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otCorreos> Result;
                    IEnumerable<Entidades.Correos> subResult = from c in db.Correos where c.Correos_Personas.Any(x => x.idPersona == idPersona) && c.FechaEliminacion == null select c;
                    Result = (from c in subResult select new otCorreos() { Saliente = c.RutaEml == null, Tipo = "Cl", Asunto = c.Asunto, Fecha = c.FechaCreacion, idCorreo = c.idCorreo, Mensaje = c.Mensaje, Remitente = c.Remitente }).OrderByDescending(x => x.Fecha).Take(500).ToList();
                    return Result;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Correos_Persona_ins(List<Int32> Correos, Int32 idPersona, Boolean CrearRegla)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Int32 idCorreo in Correos)
                    {
                        Correos C = db.Correos.Single(x => x.idCorreo == idCorreo);
                        Correos_Personas CP = db.Correos_Personas.SingleOrDefault(x => x.idCorreo == idCorreo && x.idPersona == idPersona);
                        if (CP == null)
                        {
                            CP = new Correos_Personas();
                            CP.idPersona = idPersona;
                            CP.idCorreo = idCorreo;
                            CP.FechaAsignado = DateTime.Now;
                            db.Correos_Personas.InsertOnSubmit(CP);
                        }
                        if (CrearRegla)
                        {
                            CorreosFiltros CF = db.CorreosFiltros.SingleOrDefault(x => x.De.ToLower() == C.Remitente.ToLower() && x.idPersona == idPersona);
                            if (CF == null)
                            {
                                CF = new CorreosFiltros();
                                CF.idPersona = idPersona;
                                CF.De = C.Remitente.ToLower();
                                CF.idOperador = Sesion.idOperador;
                                db.CorreosFiltros.InsertOnSubmit(CF);
                            }
                        }
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Correos_Persona_del(Int32 idCorreo, Int32 idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Correos_Personas CP = db.Correos_Personas.SingleOrDefault(x => x.idCorreo == idCorreo && x.idPersona == idPersona);
                    if (CP != null)
                    {
                        db.Correos_Personas.DeleteOnSubmit(CP);
                        db.SubmitChanges();
                    }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otPersona> Correos_Personas_lst(int idCorreo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Correos_Personas.Where(x => x.idCorreo == idCorreo).Select(x => new otPersona() { idPersona = x.idPersona, Nombre = x.Personas.Nombre }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCorreosFiltros> CorreosFiltros_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.CorreosFiltros./*Where(x => x.idOperador == idOperador).*/Select(x => otCorreosFiltros.FromCorreosFiltros(x)).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean CorreosFiltros_del(int idCorreoFiltro)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    CorreosFiltros CF = db.CorreosFiltros.Single(x => x.idCorreoFiltro == idCorreoFiltro);
                    db.CorreosFiltros.DeleteOnSubmit(CF);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean CorreosFiltros_sav(otCorreosFiltros CorreoFiltro)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    CorreosFiltros CF = db.CorreosFiltros.FirstOrDefault(x => x.De == CorreoFiltro.De && x.idOperador == CorreoFiltro.idOperador && x.idPersona == CorreoFiltro.idPersona);
                    if (CF == null)
                    {
                        CF = new CorreosFiltros();
                        db.CorreosFiltros.InsertOnSubmit(CF);
                    }
                    CF.De = CorreoFiltro.De;
                    CF.idOperador = CorreoFiltro.idOperador;
                    CF.idPersona = CorreoFiltro.idPersona;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptAnalisisPorClienteResult> AnalisisPorCliente_rpt(String idPais, Int32? idCliente, DateTime FechaIni, DateTime FechaFin, Int32 idOperador, Boolean Supervisados, Boolean General)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.rptAnalisisPorCliente(idPais, idCliente, FechaIni, FechaFin, idOperador, Supervisados, General).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Correo_ins(otCorreos Correo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    String TimeStamp = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                    Correos C = new Correos();
                    C.Asunto = Correo.Asunto;
                    /*                    C.Mensaje = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">" +
                                                                    "<html><head>" + Comunes.AnalisisHead() + "</head><body>" + Correo.Mensaje + Comunes.AnalisisCuerpo(Correo.idPersona.Value, Sesion.idOperador, db).ToString() + "</body></html>";*/
                    Int32 Cuentas;
                    if (Correo.Cuentas == null)
                    {
                        Cuentas = 0;
                    }
                    else
                    {
                        Cuentas = Correo.Cuentas.Count;
                        if (Cuentas == 0 && Correo.idPersona.HasValue) Cuentas = db.Cuentas.Count(x => db.CuentasOperador(Correo.idOperador, Correo.idPersona.Value).Any(y => y.idCuenta == x.idCuenta));
                    }
                    Boolean AnalisisEnCuerpo = Correo.idPersona.HasValue && Correo.Analisis && Cuentas <= 500;
                    C.Mensaje = "<div style='font-size:x-small'>";

                    C.Mensaje += (AnalisisEnCuerpo ? Comunes.AnalisisHead() : "");
                    C.Mensaje += Correo.Mensaje + "<br/>";

                    //C.Mensaje += (AnalisisEnCuerpo ? Comunes.AnalisisCuerpo(Correo.idPersona.Value, Correo.idOperador, db, Correo.Cuentas, Correo.Agrupado).ToString() : "");
                    if (AnalisisEnCuerpo && C.Mensaje.IndexOf("{analisis}") == -1) C.Mensaje += "{analisis}";
                    C.Mensaje += "<br/><br/>" + db.FirmaOperador(Correo.idOperador);
                    C.Mensaje = C.Mensaje.Replace("{analisis}", AnalisisEnCuerpo ? Comunes.AnalisisCuerpo(Correo.idPersona.Value, Correo.idOperador, db, Correo.Cuentas).ToString() : "");

                    C.Mensaje += "</div>";
                    C.Destinatarios = Correo.Destinatarios;
                    C.DestinatariosCopia = Correo.DestinatariosCopia;
                    C.DestinatariosCopiaOculta = db.Operadores.Single(x => x.idOperador == Correo.idOperador).Correo + "," + Correo.DestinatariosCopiaOculta;
                    C.FechaCreacion = DateTime.Now;
                    C.ResultadosAdjuntos = false;
                    C.Remitente = Sesion.Operador.Correo ?? "Administrador<administrador@veconinter.com.ve>";
                    String Ruta2 = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                    String DirectorioFinal = Ruta2 + TimeStamp;
                    if (Correo.Adjuntos.Trim() != "" || Correo.Analisis)
                    {
                        Directory.CreateDirectory(DirectorioFinal);
                    }
                    if (Correo.Adjuntos.Trim() != "")
                    {
                        foreach (String idAdjunto in Correo.Adjuntos.Split('|'))
                        {
                            String Adjunto = "";
                            if (idAdjunto.StartsWith("Doc")) //Doc es para facturas
                            {
                                try
                                {
                                    String Original = db.Cuentas.Single(x => x.idCuenta == Convert.ToInt32(idAdjunto.Substring(3))).Ruta;
                                    String Nombre = "Factura_" + Original.Substring(Original.LastIndexOf("/") + 1);
                                    Adjunto = DirectorioFinal + "\\" + Nombre;
                                    using (WebClient client = new WebClient())
                                    {
                                        client.DownloadFile(Original, Adjunto);
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    throw new CobranzasException("Problema Anexando factura: " + db.Cuentas.Single(x => x.idCuenta == Convert.ToInt32(idAdjunto.Substring(3))).Codigo);
                                }
                            }
                            else if (idAdjunto.StartsWith("_*")) //"_*" Adjuntos fijos
                            {
                                //                                Adjunto = Ruta2 + "\\" + idAdjunto.Substring(1);
                                String RutaFija = db.Parametros.Single(x => x.Clave == "RutaArchivos").Valor;
                                String Original = idAdjunto.Substring(2);
                                //Original = Original.Substring(Original.IndexOf("_") + 1);
                                Adjunto = RutaFija + Original;
                                //File.Copy(Ruta2 + "\\" + idAdjunto.Substring(1), Adjunto);
                            }
                            else if (idAdjunto.StartsWith("_")) //"_" es para Soportes
                            {
                                //                                Adjunto = Ruta2 + "\\" + idAdjunto.Substring(1);
                                String Original = idAdjunto.Substring(1);
                                Original = Original.Substring(Original.IndexOf("_") + 1);
                                Adjunto = DirectorioFinal + "\\" + Original;
                                File.Copy(Ruta2 + "\\" + idAdjunto.Substring(1), Adjunto);
                            }
                            else
                            {
                                Int32 Pos = idAdjunto.IndexOf("_");
                                if (Pos != -1)
                                {
                                    Adjunto = idAdjunto.Substring(Pos + 1);
                                    Adjunto = db.Soportes.Single(x => x.idSoporte == Convert.ToInt32(idAdjunto.Substring(3, Pos - 3))).Ubicacion + Adjunto;
                                }
                                else
                                {
                                    Adjunto = db.Soportes.Single(x => x.idSoporte == Convert.ToInt32(idAdjunto.Substring(3))).Ubicacion;
                                }
                            }
                            if (Adjunto.IndexOf("\\") != -1)//Ruta UNC
                            {
                                C.Adjuntos += ";" + Adjunto;
                            }
                            else//URL
                            {
                                using (WebClient client = new WebClient())
                                {
                                    String ArchivoLocal = Ruta2 + TimeStamp + "\\" + Adjunto.Substring(Adjunto.LastIndexOf("/") + 1);
                                    while (File.Exists(ArchivoLocal))
                                    {
                                        Int32 Punto = ArchivoLocal.LastIndexOf(".");
                                        ArchivoLocal = ArchivoLocal.Substring(0, Punto) + "_" + ArchivoLocal.Substring(Punto);
                                    }
                                    client.DownloadFile(Adjunto, ArchivoLocal);
                                    C.Adjuntos += ";" + ArchivoLocal;
                                }
                            }
                        }
                    }
                    if (Correo.Original.HasValue)
                    {
                        try
                        {
                            Correos Original = db.Correos.Single(x => x.idCorreo == Correo.Original.Value);
                            if (Original.RutaEml != null)
                            {
                                String Ruta = db.Parametros.Single(x => x.Clave == "RutaCorreos").Valor;
                                Ruta += Original.RutaEml + ".eml";
                                String Ruta3 = Ruta2 + "Original" + Correo.Original.Value.ToString() + ".eml";

                                File.Copy(Ruta, Ruta3, true);
                                C.Adjuntos += ";" + Ruta3;
                            }
                            else
                            {
                                if (Original.Adjuntos != "") C.Adjuntos += ";" + Original.Adjuntos;
                                String Ruta3 = Ruta2 + "Original" + Correo.Original.Value.ToString() + ".html";
                                File.WriteAllText(Ruta3, Original.Mensaje);
                                C.Adjuntos += ";" + Ruta3;
                            }
                        }
                        catch { }
                        foreach (Correos_Personas CP in C.Correos_Personas) //El correo Reenviado debe asignarse a las mismas personas
                        {
                            C.Correos_Personas.Add(new Correos_Personas() { idPersona = CP.idPersona });
                        }
                    }
                    if (Correo.idPersona.HasValue)
                    {
                        if (Correo.Analisis)
                        {
                            if (!AnalisisEnCuerpo)
                            {
                                File.Copy(Comunes.AnalisisReporte(Correo.idPersona.Value, Sesion.idOperador, Correo.Cuentas, Correo.Agrupado), DirectorioFinal + "\\Analisis.html");
                                C.Adjuntos += ";" + DirectorioFinal + "\\Analisis.html";
                            }
                            //C.Adjuntos += ";" + db.Parametros.Single(x => x.Clave == "RutaArchivos").Valor + "depositoProvincial.pdf";
                        }
                        C.Correos_Personas.Add(new Correos_Personas() { idPersona = Correo.idPersona.Value });
                    }
                    if (C.Adjuntos != null && C.Adjuntos != "")
                    {
                        C.Adjuntos = C.Adjuntos.Substring(1);
                    }
                    C.idOperador = Sesion.Operador.idOperador;
                    db.Correos.InsertOnSubmit(C);
                    C.Fecha = DateTime.Now;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public DataTable Reportes_sel(String Reporte, Int32 idTipoReporte)//Int32 id, Int32 idTipoReporte)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    TiposReporte TipoReporte = db.TiposReporte.Single(x => x.idTipoReporte == idTipoReporte);
                    String CS = TipoReporte.Origenes.ConnectionString;
                    //String Codigo = "";
                    //if (TipoReporte.Persona)
                    //{
                    //    Codigo = db.Personas.Single(x => x.idPersona == id).Codigo;
                    //}
                    //else
                    //{
                    //    Codigo = db.Cuentas.Single(x => x.idCuenta == id).Codigo;
                    //}
                    using (OleDbConnection Conn = new OleDbConnection(CS))
                    {
                        OleDbCommand Comm = Conn.CreateCommand();
                        Comm.CommandTimeout = 5 * 60;
                        //                        Comm.CommandText = TipoReporte.Origen.Replace("{0}", Codigo);
                        Comm.CommandText = Reporte;
                        OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                        DataSet Result = new DataSet();
                        DA.Fill(Result);
                        return Result.Tables[0];
                    }

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otSoporte> CuentasSoportes_lst(List<Int32> lstCuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otSoporte> Result = new List<otSoporte>();
                    if (lstCuentas == null || lstCuentas.Count == 0) return Result;
                    //Documentos de la Persona
                    Personas Persona = db.Cuentas.Single(x => x.idCuenta == lstCuentas[0]).Personas;
                    if (Persona.Soportes.Count > 0)
                    {
                        foreach (Soportes Soporte in Persona.Soportes.Where(x => x.idCuenta == null && x.Tabla == null))
                        {
                            Result.Add(new otSoporte { Nombre = Soporte.Nombre, Ubicacion = "Sop" + Soporte.idSoporte.ToString() });
                            //pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Sop{0}'>{1}<br/>", Soporte.idSoporte, Soporte.Nombre);
                        }
                    }
                    Debug.Print("Hora Inicio:{0}", DateTime.Now.ToString("hh:mm:ss.ffff"));
                    foreach (Int32 idCuenta in lstCuentas)
                    {
                        Entidades.Cuentas Cuenta = db.Cuentas.SingleOrDefault(x => x.idCuenta == Convert.ToInt32(idCuenta));
                        if (Cuenta == null) continue;
                        Result.Add(new otSoporte { Nombre = Cuenta.Codigo, Ubicacion = "" });
                        //pnlDocumentos.InnerHtml += String.Format("<h3>Cuenta {0}</h3>", Cuenta.Codigo);
                        Int16 Soportes = 0;
                        if (!String.IsNullOrWhiteSpace(Cuenta.Ruta))
                        {
                            Soportes++;
                            Result.Add(new otSoporte { Nombre = "Documento", Ubicacion = "Doc" + Cuenta.idCuenta });
                            //pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Doc{0}'>{1}<br/>", Cuenta.idCuenta, "Documento");
                        }
                        foreach (Entidades.Soportes Soporte in Cuenta.Soportes.Where(x => x.Tabla == null))
                        {
                            if (Soporte.Ubicacion.EndsWith("\\"))
                            {
                                try
                                {
                                    foreach (String Archivo in Directory.EnumerateFiles(Soporte.Ubicacion))
                                    {
                                        String NombreArchivo = Archivo.Substring(Archivo.LastIndexOf("\\") + 1);
                                        Result.Add(new otSoporte { Nombre = NombreArchivo, Ubicacion = "Sop" + Soporte.idSoporte + "_" + NombreArchivo });
                                        //pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Sop{0}_{1}'>{1}<br/>", Soporte.idSoporte, NombreArchivo);
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                if (File.Exists(Soporte.Ubicacion))
                                {
                                    Result.Add(new otSoporte { Nombre = Soporte.Nombre, Ubicacion = "Sop" + Soporte.idSoporte });
                                    //pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Sop{0}'>{1}<br/>", Soporte.idSoporte, Soporte.Nombre);
                                }
                            }
                        }
                    }
                    Debug.Print("Hora Fin:{0}", DateTime.Now.ToString("hh:mm:ss.ffff"));
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                Debug.Print(Ex.Message);
                return new List<otSoporte>();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Correos_MarcarPersonal_upd(List<Int32> Correos, Int32 TipoEspecial)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Correos Correo in db.Correos.Where(x => Correos.Contains(x.idCorreo)))
                    {
                        Correo.TipoEspecial = TipoEspecial;
                    }
                    db.SubmitChanges();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
            return true;
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptCorreosAdministradorResult> CorreosAdministrador_rpt(DateTime FechaDesde, DateTime FechaHasta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CommandTimeout = 10 * 60;
                    return db.rptCorreosAdministrador(FechaDesde, FechaHasta).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otPersonaContacto> CorreoGrupoEmpresa_lst(int idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DataTable dt = new System.Data.DataTable();
                    dt = Negocios.ListaCorreoGrupoEmpresa(idPersona);


                    List<OT.otPersonaContacto> list = new List<otPersonaContacto>();
                    otPersonaContacto cge;


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cge = new otPersonaContacto();
                        cge.idPersonaContacto = int.Parse(dt.Rows[i]["idPersona"].ToString());
                        cge.Correo = " " + dt.Rows[i]["email"].ToString();
                        list.Add(cge);


                    }
                    return list;
                }
            }


            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion




        #region Gestion
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otStatus> Status_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return (from c in db.Status where c.Tipo != "Sistema" && c.Activo && db.Operadores_Asignaciones.Any(x => x.idOperador == idOperador && x.idPais == c.idPais && x.idTipoCliente == c.idTipoCliente) && c.Activo orderby c.Nivel select new otStatus { idStatus = c.idStatus, Nombre = c.Nombre, Tipo = c.Tipo, Nivel = c.Nivel, idPais = c.idPais, idTipoCliente = c.idTipoCliente, Activo = c.Activo }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otStatus> Status_lst2(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return (from c in db.Status where c.Tipo != "Sistema" && c.Activo && db.Operadores_Asignaciones.Any(x => x.idOperador == idOperador && x.idPais == c.idPais && x.idTipoCliente == c.idTipoCliente) && c.Activo orderby c.Nivel select new otStatus { idStatus = c.idStatus, Nombre = c.Nombre + "(" + c.idPais + ")", Tipo = c.Tipo, Nivel = c.Nivel, idPais = c.idPais, idTipoCliente = c.idTipoCliente, Activo = c.Activo }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Gestion_ins(otGestion Gestion, List<String> Adjuntos)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Int32 idOperador = Gestion.idOperador;
                    try
                    {
                        List<Int32> lstOperadores = Comunes.Clones(Gestion.idOperador, db);
                        List<int> OperadoresPersona = new List<int>();
                        OperadoresPersona.AddRange(from c in db.Cuentas from d in c.Cuentas_Operadores where c.idPersona == Gestion.idPersona && d.FechaFin == null && lstOperadores.Contains(d.idOperador) select d.idOperador);
                        OperadoresPersona.AddRange(from c in db.Cuentas from e in c.Campanas_Cuentas from d in e.Campanas.Campanas_Operadores where e.FechaFin == null && e.Campanas.TipoCampana != 0 && c.idPersona == Gestion.idPersona && d.FechaFin == null && lstOperadores.Contains(d.idOperador) select d.idOperador);
                        idOperador = OperadoresPersona.First();
                    }
                    catch { }
                    if (db.Gestiones.Any(x => x.idOperador == idOperador && x.idPersona == Gestion.idPersona && x.idStatus == Gestion.idStatus && x.Fecha > DateTime.Now.AddMinutes(-10) && x.Descripcion == Gestion.Descripcion))
                    {
                        return true;
                    }

                    Entidades.Gestiones GestionNew = new Entidades.Gestiones();
                    GestionNew.idOperador = idOperador;
                    GestionNew.idPersona = Gestion.idPersona;
                    GestionNew.idStatus = Gestion.idStatus;
                    GestionNew.Descripcion = Gestion.Descripcion;
                    GestionNew.Fecha = DateTime.Now;
                    db.Gestiones.InsertOnSubmit(GestionNew);
                    db.SubmitChanges();
                    Int32 idGestion = GestionNew.idGestion;

                    foreach (Int32 Cuenta in Gestion.Cuentas)
                    {
                        Entidades.Cuentas_Gestiones CuentaNew = new Entidades.Cuentas_Gestiones();
                        CuentaNew.idGestion = idGestion;
                        CuentaNew.idCuenta = Cuenta;
                        db.Cuentas_Gestiones.InsertOnSubmit(CuentaNew);
                    }
                    db.SubmitChanges();
                    try
                    {
                        String Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                        String Ruta2 = db.Parametros.Single(x => x.Clave == "RutaSoportes").Valor;
                        String DirectorioFinal = Ruta2 + "Gestiones\\Gestion_" + GestionNew.idGestion + "\\";
                        Directory.CreateDirectory(DirectorioFinal);
                        foreach (String Adjunto in Adjuntos)
                        {
                            String Nombre = Adjunto.Split('\\').Last().Substring(20);
                            File.Copy(Ruta + Adjunto, DirectorioFinal + Nombre);
                            Soportes Soporte = new Soportes();
                            Soporte.idCliente = Gestion.idPersona;
                            Soporte.Tabla = "Gestiones";
                            Soporte.idTabla = GestionNew.idGestion;
                            Soporte.Ubicacion = DirectorioFinal + Nombre;
                            Soporte.Nombre = Nombre;
                            Soporte.TipoEspecial = 1;
                            Soporte.Codigo = "";
                            db.Soportes.InsertOnSubmit(Soporte);
                        }
                        db.SubmitChanges();
                    }
                    catch { }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return false;
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptGestionesCuentasResult> GestionesCuentas_rpt(Int32 idCliente)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.rptGestionesCuentas(idCliente).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]


        public List<OT.otCuenta> GestionGrupoEmpresa_lst(otCuenta Cuentas)
        {
            try
            {
                Int32 idCuenta = Cuentas.idCuenta;

                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    DataTable dt = new System.Data.DataTable();
                    dt = Negocios.GestionGrupoEmpresa(idCuenta);


                    List<OT.otCuenta> list = new List<otCuenta>();
                    otCuenta gge;


                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        gge = new otCuenta();
                        gge.idPersona = int.Parse(dt.Rows[i]["idPersona"].ToString());
                        list.Add(gge);


                    }

                    return list;


                }
            }

            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean GestionGupoEmpresa_ins(otGestion Gestion, List<String> Adjuntos)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    ///
                    DataTable dt = new System.Data.DataTable();
                    dt = Negocios.GestionGrupoEmpresa(Gestion.idPersona);


                    ///
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        ///
                        Gestion.idPersona = int.Parse(dt.Rows[i]["idPersona"].ToString());
                        Int32 idOperador = Gestion.idOperador;


                        try
                        {
                            List<Int32> lstOperadores = Comunes.Clones(Gestion.idOperador, db);
                            List<int> OperadoresPersona = new List<int>();
                            OperadoresPersona.AddRange(from c in db.Cuentas from d in c.Cuentas_Operadores where c.idPersona == Gestion.idPersona && d.FechaFin == null && lstOperadores.Contains(d.idOperador) select d.idOperador);
                            OperadoresPersona.AddRange(from c in db.Cuentas from e in c.Campanas_Cuentas from d in e.Campanas.Campanas_Operadores where e.FechaFin == null && e.Campanas.TipoCampana != 0 && c.idPersona == Gestion.idPersona && d.FechaFin == null && lstOperadores.Contains(d.idOperador) select d.idOperador);
                            idOperador = OperadoresPersona.First();
                        }

                        catch { }


                        if (db.Gestiones.Any(x => x.idOperador == idOperador && x.idPersona == Gestion.idPersona && x.idStatus == Gestion.idStatus && x.Fecha > DateTime.Now.AddMinutes(-10) && x.Descripcion == Gestion.Descripcion))
                        {
                            return true;
                        }


                        Entidades.Gestiones GestionNew = new Entidades.Gestiones();
                        GestionNew.idOperador = idOperador;
                        GestionNew.idPersona = Gestion.idPersona;
                        GestionNew.idStatus = Gestion.idStatus;
                        GestionNew.Descripcion = Gestion.Descripcion;
                        GestionNew.Fecha = DateTime.Now;
                        db.Gestiones.InsertOnSubmit(GestionNew);
                        db.SubmitChanges();
                        Int32 idGestion = GestionNew.idGestion;


                        foreach (Int32 Cuenta in Gestion.Cuentas)
                        {
                            Entidades.Cuentas_Gestiones CuentaNew = new Entidades.Cuentas_Gestiones();
                            CuentaNew.idGestion = idGestion;
                            CuentaNew.idCuenta = Cuenta;
                            db.Cuentas_Gestiones.InsertOnSubmit(CuentaNew);
                        }
                        db.SubmitChanges();
                        try
                        {
                            String Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                            String Ruta2 = db.Parametros.Single(x => x.Clave == "RutaSoportes").Valor;
                            String DirectorioFinal = Ruta2 + "Gestiones\\Gestion_" + GestionNew.idGestion + "\\";
                            Directory.CreateDirectory(DirectorioFinal);
                            foreach (String Adjunto in Adjuntos)
                            {
                                String Nombre = Adjunto.Split('\\').Last().Substring(20);
                                File.Copy(Ruta + Adjunto, DirectorioFinal + Nombre);
                                Soportes Soporte = new Soportes();
                                Soporte.idCliente = Gestion.idPersona;
                                Soporte.Tabla = "Gestiones";
                                Soporte.idTabla = GestionNew.idGestion;
                                Soporte.Ubicacion = DirectorioFinal + Nombre;
                                Soporte.Nombre = Nombre;
                                Soporte.TipoEspecial = 1;
                                Soporte.Codigo = "";
                                db.Soportes.InsertOnSubmit(Soporte);
                            }
                            db.SubmitChanges();
                        }
                        catch { }
                        // return true;
                    }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return false;
            }
        }

        #endregion


        #region Recursos

        [OperationContract, WebGet]
        public List<otRecursos> Recursos_lst()
        {
            try
            {
                var Result = new List<otRecursos>();
                System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Resources.Recursos", System.Reflection.Assembly.Load("App_GlobalResources"));
                ResourceSet RS = temp.GetResourceSet(CultureInfo.CurrentCulture, true, true);
                foreach (DictionaryEntry Itm in RS)
                {
                    Result.Add(new otRecursos() { Clave = Itm.Key.ToString(), Valor = Itm.Value.ToString() });
                }
                //global::
                return Result;
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Operadores

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otOperadoresSimple> Operadores_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var q = (from c in db.Operadores select new otOperadoresSimple { idOperador = c.idOperador, Nombre = c.Nombre }).ToList();
                    return q;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }

        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otSuplente> Operadores_Suplantar_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Suplentes.Where(x => x.idOperadorSuplente == Sesion.Operador.idOperador && (x.FechaFin >= DateTime.Now || x.FechaFin == null)).Select(x => new otSuplente { idOperador = x.idOperador, Operador = x.Operadores.Nombre, FechaInicio = x.FechaInicio, FechaFin = x.FechaFin, idSuplente = x.idSuplente, Cartera = x.Cartera, Correo = x.Correo, Gestion = x.Gestion, Indicadores = x.Indicadores, Reportes = x.Reportes, Supervision = x.Supervision, Distribucion = x.Distribucion }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otSuplente> Operadores_Suplantarme_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Suplentes.Where(x => x.idOperador == Sesion.Operador.idOperador && (x.FechaFin >= DateTime.Now || x.FechaFin == null)).Select(x => new otSuplente { idOperador = x.idOperador, Operador = x.Operadores1.Nombre, FechaInicio = x.FechaInicio, FechaFin = x.FechaFin, idSuplente = x.idSuplente, Cartera = x.Cartera, Correo = x.Correo, Gestion = x.Gestion, Indicadores = x.Indicadores, Reportes = x.Reportes, Supervision = x.Supervision, Distribucion = x.Distribucion }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otOperadores Operadores_sel(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Operadores Op = db.Operadores.SingleOrDefault(x => x.idOperador == idOperador);
                    return otOperadores.FromOperador(Op);
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Operadores_del(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.Operadores.SingleOrDefault(x => x.idOperador == idOperador).Activo = false;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Operadores_sav(otOperadores Operador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Operadores Op;
                    Op = db.Operadores.SingleOrDefault(x => x.idOperador == Operador.idOperador);
                    if (Op == null)
                    {
                        Op = new Operadores();
                        db.Operadores.InsertOnSubmit(Op);
                    }
                    Operadores Sup = db.Operadores.Single(x => x.idOperador == Operador.idSupervisor);
                    Op.Activo = Operador.Activo;
                    Op.Codigo = Operador.Codigo;
                    Op.Login = Operador.Login;
                    Op.FechaFin = Operador.FechaFin;
                    Op.FirmaCorreo = Operador.FirmaCorreo;
                    Op.Telefonos = Operador.Telefonos;
                    Op.Extension = Operador.Extension;
                    Op.Pais = Operador.Pais;
                    Op.Zona = Operador.Zona;

                    Op.idSupervisor = Operador.idSupervisor;
                    Op.idGrupo = Sup.idGrupo;
                    Op.Nombre = Operador.Nombre;
                    Op.Correo = Operador.Correo;
                    Op.FechaIngreso = Operador.FechaIngreso;
                    Op.Tipo = Operador.Tipo ?? Op.Tipo ?? "OP";
                    Op.Cargo = Operador.Cargo;
                    Op.idFlujo = Sup.idFlujo;
                    Op.idPaso = Sup.idPaso;
                    Op.idReglaRestriccion = Sup.idReglaRestriccion;
                    Op.Password = "12345678";
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otMetasOperador> Operador_Metas(int idOperador)
        {
            try
            {

                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Metas_Operadores.Where(x => x.idOperador == idOperador && x.FechaFin == null).Select(x => new
                    {
                        idMeta = x.idMeta,
                        Fecha = x.Metas.Metas_Operadores_Cuentas.Where(y => y.idOperador == idOperador && y.Finalizado == null && y.Activa).Select(y => y.Fecha).Distinct().OrderByDescending(y => y).FirstOrDefault(),
                        Nombre = x.Metas.Nombre,
                        Facturas = x.Metas.Metas_Operadores_Cuentas.Where(y => y.idOperador == idOperador && y.Finalizado == null && y.Activa).Count(),
                        FacturasCobradas = db.Pagos_Cuentas.Where(y => (y.Pagos.Aprobado == true) && (y.idOperador ?? y.Pagos.idOperador) == idOperador && (y.Fecha ?? y.Pagos.Fecha) >= x.Metas.FechaMetaActual && (y.Fecha ?? y.Pagos.Fecha) <= x.Metas.FechaMetaSiguiente && x.Metas.Metas_Operadores_Cuentas.Any(w => w.Finalizado == null /*&& w.idOperador == idOperador*/ && w.idMeta == x.idMeta && w.Activa && w.idCuenta == y.idCuenta)).Select(y => y.idCuenta).Distinct().Count(),
                        Meta = x.Metas.Metas_Operadores_Cuentas.Where(y => y.idOperador == idOperador && y.Finalizado == null && y.Activa).Sum(y => (decimal?)y.Meta) ?? 0,
                        Real = db.Pagos_Cuentas.Where(y => (y.Pagos.Aprobado == true) && (y.idOperador ?? y.Pagos.idOperador) == idOperador && (y.Fecha ?? y.Pagos.Fecha) >= x.Metas.FechaMetaActual && (y.Fecha ?? y.Pagos.Fecha) <= x.Metas.FechaMetaSiguiente && x.Metas.Metas_Operadores_Cuentas.Any(w => w.Finalizado == null /*&& w.idOperador == idOperador*/ && w.idMeta == x.idMeta && w.Activa && w.idCuenta == y.idCuenta)).Sum(y => (decimal?)y.Monto / y.Cuentas.CambioLocal) ?? 0
                    }).Select(x => new otMetasOperador
                    {
                        idMeta = x.idMeta,
                        Fecha = x.Fecha,
                        Nombre = x.Nombre,
                        Facturas = x.Facturas,
                        FacturasCobradas = x.FacturasCobradas,
                        Meta = x.Meta,
                        Real = x.Real,
                        Porc = x.Meta == 0 ? 0 : (x.Real / x.Meta * 100)
                    }).ToList();

#warning, tomar en cuenta la regla etc...
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean DistribucionOperadores_Ejecutar(Int32 idDistribucion)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.DistribuirOperador(idDistribucion);
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otOperadores_Asignaciones> Operadores_Asignaciones_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Operadores_Asignaciones.Where(x => x.FechaFin == null && x.idOperador == idOperador).Select(x => new otOperadores_Asignaciones { idPais = x.idPais, idTipoCliente = x.idTipoCliente }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otOperadoresSimple> Operadores_Persona_lst(int idPersona, List<Int32> Operadores)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    //Operadores de la Cuenta
                    //List<Int32> Operadores1 = db.Cuentas_Operadores.Where(x => x.Cuentas.idPersona == idPersona && x.FechaFin == null).Select(x => x.idOperador).ToList();
                    //Operadores en campañas abiertas, donde estén las cuentas
                    //List<Int32> Operadores2 =  db.Campanas_Operadores.Where(x => x.Campanas.TipoCampana == 2 && x.Campanas.Campanas_Cuentas.Any(y => y.Cuentas.idPersona == idPersona)).Select(x => x.idOperador).ToList();
                    //List<Int32> Result = Operadores1;
                    //Result.AddRange(Operadores2);
                    //Result.AddRange(Operadores);
                    List<Int32> Result = Operadores;
                    Result.AddRange(db.Operadores_Persona(idPersona).Select(x => x.idOperador).ToList());

                    Result = Result.Distinct().ToList();
                    return db.Operadores.Where(x => Result.Contains(x.idOperador)).Select(x => new otOperadoresSimple { idOperador = x.idOperador, Nombre = x.Nombre, Jerarquia = "", Status = "" }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public String Nombre(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Operadores.Single(x => x.idOperador == idOperador).Nombre;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptLoginsResult> Logins_rpt(DateTime FechaDesde, DateTime FechaHasta, Int32 idOperador, Boolean Supervisados)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.rptLogins(FechaDesde, FechaHasta, idOperador, Supervisados).ToList();//.Select(x => new otLogin {idOperador=x.idOperador, Fecha=x.Fecha.Value, Inicio=x.Inicio , Fin=x.Fin, Operador= x.Operador   })
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }



        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Salir(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (!Sesion.Impersonando)
                    {
                        Entidades.Acciones Ac = new Entidades.Acciones() { Accion = "LO", Fecha = DateTime.Now, idOperador = idOperador };
                        db.Acciones.InsertOnSubmit(Ac);
                        db.SubmitChanges();
                    }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCompromisos_rpt> Compromisos_rpt(Int32 idOperador, DateTime FechaDesde, DateTime FechaHasta, Boolean Supervisado)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    IQueryable<Compromisos_Cuentas> Resultado;
                    if (Supervisado)
                    {
                        List<Int32> Operadores = db.OperadoresSupervisados(idOperador, "").Select(x => x.idOperador ?? 0).ToList();
                        Resultado = db.Compromisos_Cuentas.Where(x => x.Fecha.Date >= FechaDesde.Date && x.Fecha.Date <= FechaHasta.Date && Operadores.Contains(x.Compromisos.idOperador));
                    }
                    else
                    {
                        Resultado = db.Compromisos_Cuentas.Where(x => x.Fecha.Date >= FechaDesde.Date && x.Fecha.Date <= FechaHasta.Date && x.Compromisos.idOperador == idOperador);
                    }
                    return Resultado.GroupBy(x => new { Fecha = x.Fecha.Date, x.Compromisos.Personas })
                            .Select(x => new otCompromisos_rpt
                            {
                                Fecha = x.Key.Fecha,
                                idPersona = x.Key.Personas.idPersona,
                                Codigo = x.Key.Personas.Codigo,
                                Persona = x.Key.Personas.Nombre,
                                MontoLocal = x.Sum(y => y.Monto * y.Cuentas.CambioLocal),
                                MontoDolar = x.Sum(y => y.Monto / y.Cuentas.CambioDolar),
                                RestanteLocal = x.Key.Personas.Cuentas.Where(y => y.Activa).Sum(y => (Decimal?)(y.MontoRestante * y.CambioLocal)) ?? 0,
                                RestanteDolar = x.Key.Personas.Cuentas.Where(y => y.Activa).Sum(y => (Decimal?)(y.MontoRestante / y.CambioDolar)) ?? 0,
                                Operador = x.First().Compromisos.Operadores.Nombre
                            }).ToList();

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Distribución
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otDistribucionCampana> DistCampana_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    List<otDistribucionCampana> Result;
                    Result = (from c in db.DistribucionCampanas
                              orderby c.Nombre
                              select new otDistribucionCampana
                              {
                                  idDistribucion = c.idDistribucion,
                                  Nombre = c.Nombre,
                                  Campana = c.Campanas.Nombre,
                                  Orden = c.Orden,
                                  Regla = c.Reglas.Nombre,
                                  Flujo = c.Flujos_Pasos.Flujos.Nombre,
                                  Paso = c.Flujos_Pasos.Pasos.Nombre,
                                  idCampana = c.idCampana,
                                  idRegla = c.idRegla,
                                  idFlujo = c.idFlujo,
                                  idPaso = c.idPaso
                              }).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otDistribucionOperador> DistOperador_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otDistribucionOperador> Result;
                    Result = (from c in db.DistribucionOperador
                              orderby c.Nombre
                              select new otDistribucionOperador
                              {
                                  idDistribucion = c.idDistribucion,
                                  Nombre = c.Nombre,
                                  Operador = c.Operadores.Nombre,
                                  Orden = c.Orden,
                                  Regla = c.Reglas.Nombre,
                                  Flujo = c.Flujos_Pasos.Flujos.Nombre,
                                  Paso = c.Flujos_Pasos.Pasos.Nombre,
                                  idOperador = c.idOperador,
                                  idRegla = c.idRegla,
                                  idFlujo = c.idFlujo,
                                  idPaso = c.idPaso
                              }).ToList();
                    return Result;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCuenta> CuentasRegla_lst(int idRegla)
        {//Modificar
#warning Sin Referencias y mal hecho este método
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var w = db.Cuentas.Where(c => !c.Campanas_Cuentas.Any(x => x.Campanas.FechaFin == null) && !c.Cuentas_Operadores.Any(x => x.FechaFin == null)).ToList()
                .Select(c => otCuenta.FromCuenta(c)).ToList();
                    return w;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCuenta> CuentasSinAsignar_lst(int idOperador, Boolean campana = true, Boolean operador = true)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    //var w = db.Cuentas.Where(c => c.Activa && !c.Campanas_Cuentas.Any(x => x.Campanas.FechaFin == null && x.Campanas.TipoCampana!=0) && !c.Cuentas_Operadores.Any(x => x.FechaFin == null)).ToList()
                    //    .Select(c => otCuenta.FromCuenta(c)).ToList();
                    var w = db.vwCuentas.Where(c => !db.Campanas_Cuentas.Any(x => x.Campanas.FechaFin == null && x.Campanas.TipoCampana != 0 && x.idCuenta == c.idCuenta) && !db.Cuentas_Operadores.Any(x => x.FechaFin == null && x.idCuenta == c.idCuenta))
                        .Select(c => otCuenta.FromvwCuenta(c)).ToList();
                    return w;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otDistribucionCampana DistribucionCampanas_sel(Int32 idDistribucion)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DistribucionCampanas DC = db.DistribucionCampanas.Single(x => x.idDistribucion == idDistribucion);
                    return new otDistribucionCampana
                        {
                            idDistribucion = DC.idDistribucion,
                            Nombre = DC.Nombre,
                            Campana = DC.Campanas.Nombre,
                            Orden = DC.Orden,
                            Regla = DC.Reglas.Nombre,
                            Flujo = DC.Flujos_Pasos.Flujos.Nombre,
                            Paso = DC.Flujos_Pasos.Pasos.Nombre,
                            idCampana = DC.idCampana,
                            idRegla = DC.idRegla,
                            idFlujo = DC.idFlujo,
                            idPaso = DC.idPaso,
                            Excluir = DC.Excluir
                        };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 DistribucionCampanas_sav(otDistribucionCampana Distribucionins)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DistribucionCampanas DC;
                    Boolean Nuevo = Distribucionins.idDistribucion == 0;
                    if (Nuevo)
                    {
                        DC = new DistribucionCampanas();
                        db.DistribucionCampanas.InsertOnSubmit(DC);
                        DC.idOperadorDueno = Sesion.idOperador;
                    }
                    else
                    {
                        DC = db.DistribucionCampanas.Single(x => x.idDistribucion == Distribucionins.idDistribucion);
                    }
                    DC.Orden = 0;
                    DC.Nombre = Distribucionins.Nombre;
                    DC.idRegla = Distribucionins.idRegla;
                    DC.idCampana = Distribucionins.idCampana;
                    DC.idFlujo = Distribucionins.idFlujo;
                    DC.idPaso = Distribucionins.idPaso;
                    DC.Excluir = Distribucionins.Excluir;
                    db.SubmitChanges();
                    db.Refresh(RefreshMode.OverwriteCurrentValues, DC);
                    return DC.idDistribucion;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean DistribucionCampanas_del(int idDistribucion)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DistribucionCampanas DC = db.DistribucionCampanas.Single(x => x.idDistribucion == idDistribucion);
                    db.DistribucionCampanas.DeleteOnSubmit(DC);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otDistribucionOperador DistribucionOperadores_sel(Int32 idDistribucion)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DistribucionOperador DC = db.DistribucionOperador.Single(x => x.idDistribucion == idDistribucion);
                    return new otDistribucionOperador
                    {
                        idDistribucion = DC.idDistribucion,
                        Nombre = DC.Nombre,
                        Operador = DC.Operadores.Nombre,
                        Orden = DC.Orden,
                        Regla = DC.Reglas.Nombre,
                        Flujo = DC.Flujos_Pasos.Flujos.Nombre,
                        Paso = DC.Flujos_Pasos.Pasos.Nombre,
                        idOperador = DC.idOperador,
                        idRegla = DC.idRegla,
                        idFlujo = DC.idFlujo,
                        idPaso = DC.idPaso
                    };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 DistribucionOperadores_sav(otDistribucionOperador Distribucionins)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DistribucionOperador DC;
                    Boolean Nuevo = Distribucionins.idDistribucion == 0;
                    if (Nuevo)
                    {
                        DC = new DistribucionOperador();
                        db.DistribucionOperador.InsertOnSubmit(DC);
                        DC.idOperadorDueno = Sesion.idOperador;
                    }
                    else
                    {
                        DC = db.DistribucionOperador.Single(x => x.idDistribucion == Distribucionins.idDistribucion);
                    }
                    DC.Orden = 0;
                    DC.Nombre = Distribucionins.Nombre;
                    DC.idRegla = Distribucionins.idRegla;
                    DC.idOperador = Distribucionins.idOperador;
                    DC.idFlujo = Distribucionins.idFlujo;
                    DC.idPaso = Distribucionins.idPaso;
                    db.SubmitChanges();
                    db.Refresh(RefreshMode.OverwriteCurrentValues, DC);
                    return DC.idDistribucion;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean DistribucionOperadores_del(int idDistribucion)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DistribucionOperador DC = db.DistribucionOperador.Single(x => x.idDistribucion == idDistribucion);
                    db.DistribucionOperador.DeleteOnSubmit(DC);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex.Informar();
            }
        }
        #endregion
        #region Pagos
        //[OperationContract, WebGet]
        //public List<otCuentasPago> Cuentas_Pagos_lst(string lstCuentas)
        //{
        //    try
        //    {
        //        using (CobranzasDataContext db = new CobranzasDataContext())
        //        {
        //            var fact = lstCuentas.Split(',').Select(x => Int32.Parse(x)).ToList();
        //            var w = db.Cuentas.Where(c => fact.Contains(c.idCuenta) && c.Cuentas_Operadores.Any(y => y.idOperador == Sesion.idOperador)).ToList()
        //                .Select(c => new otCuentasPago { idCuenta = c.idCuenta, Documento = c.Codigo, Fecha = c.FechaInicio.HasValue ? c.FechaInicio.Value.ToString("dd/MM/yyyy") : "", Total = c.Monto, TotalDolar = c.Monto / c.CambioDolar, TotalLocal = c.Monto * c.CambioLocal, Cliente = c.Clientes.Nombre, Producto = c.Productos.Nombre, Moneda = c.idMoneda, Antiguedad = (DateTime.Now - (c.FechaInicio ?? DateTime.Now)).Days, CambioDolar = c.CambioDolar, MonedaLocal = c.Personas.Paises.idMoneda, CambioLocal = c.CambioLocal, Deuda = c.MontoRestante, DeudaDolar = c.MontoRestante / c.CambioDolar, DeudaLocal = c.MontoRestante * c.CambioLocal }).ToList();
        //            return w;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex.Informar();
        //    }
        //}
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Pagos_sav(otPago Pago)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Pagos PagoNew;
                    Boolean Nuevo = Pago.idPago == 0;
                    if (Nuevo)
                    {
                        PagoNew = new Pagos();
                        db.Pagos.InsertOnSubmit(PagoNew);
                    }
                    else
                    {
                        PagoNew = db.Pagos.SingleOrDefault(x => x.idPago == Pago.idPago);
                    }

                    PagoNew.idBancoPropio = Pago.idBancoPropio;
                    PagoNew.TipoPago = Pago.TipoPago;
                    PagoNew.Fecha = DateTime.ParseExact(Pago.FechaPago, "dd/MM/yyyy", null);
                    PagoNew.Referencia = Pago.Referencia;
                    PagoNew.MontoCheque = Pago.MontoCheque;
                    PagoNew.MontoEfectivo = Pago.MontoEfectivo;
                    PagoNew.Descripcion = Pago.Descripcion;
                    PagoNew.idMoneda = Pago.idMoneda;
                    PagoNew.idPersona = Pago.idPersona;
                    PagoNew.idOperadorCrea = Sesion.Operador.idOperador;
                    PagoNew.idOperador = Pago.idOperador;
                    PagoNew.idBancoOrigen = Pago.idBancoOrigen;
                    PagoNew.Confirmado = null;
                    PagoNew.Aprobado = null;
                    PagoNew.Codigo = null;
                    PagoNew.Resultado = null;
                    PagoNew.FechaCreacion = DateTime.Now;
                    PagoNew.idStatusPago = 1;
#warning idQuemado, Buscar Referencia en El país para poder asignarlo
                    db.SubmitChanges();
                    Int32 idPago = PagoNew.idPago;

                    if (!Nuevo)
                    {
                        db.Pagos_Cuentas.DeleteAllOnSubmit(PagoNew.Pagos_Cuentas);
                        db.SubmitChanges();
                    }
                    foreach (otPagos_Cuentas Cuenta in Pago.Pagos_Cuentas)
                    {
                        Entidades.Pagos_Cuentas PagoCuentaNew = new Entidades.Pagos_Cuentas();
                        PagoCuentaNew.idPago = idPago;
                        PagoCuentaNew.idCuenta = Cuenta.idCuenta;
                        PagoCuentaNew.Monto = Cuenta.MontoCuenta;
                        PagoCuentaNew.Retencion1 = Cuenta.Retencion1;
                        PagoCuentaNew.Retencion2 = Cuenta.Retencion2;
                        PagoCuentaNew.Fecha = DateTime.Now;
                        db.Pagos_Cuentas.InsertOnSubmit(PagoCuentaNew);
                    }
                    db.SubmitChanges();
                    if (!Nuevo)
                    {
                        db.PagosDet.DeleteAllOnSubmit(PagoNew.PagosDet);
                        db.SubmitChanges();
                    }
                    foreach (otCheques Cheque in Pago.Cheques)
                    {
                        Entidades.PagosDet Det = new PagosDet();
                        Det.idBanco = Cheque.Banco;
                        Det.NroCheque = Cheque.Nro;
                        Det.Monto = Cheque.Monto;
                        PagoNew.PagosDet.Add(Det);
                    }
                    db.SubmitChanges();
                    try
                    {
                        String Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                        String Ruta2 = db.Parametros.Single(x => x.Clave == "RutaSoportes").Valor;
                        String DirectorioFinal = Ruta2 + "Pagos\\Pago_" + PagoNew.idPago + "\\";
                        Directory.CreateDirectory(DirectorioFinal);
                        foreach (String Adjunto in Pago.Adjuntos)
                        {
                            if (db.Soportes.Any(x => x.Ubicacion == Adjunto)) continue;
                            String Nombre = Adjunto.Split('\\').Last().Substring(20);
                            File.Copy(Ruta + Adjunto, DirectorioFinal + Nombre);
                            Soportes Soporte = new Soportes();
                            Soporte.idCliente = PagoNew.idPersona;
                            Soporte.Tabla = "Pagos";
                            Soporte.idTabla = PagoNew.idPago;
                            Soporte.Ubicacion = DirectorioFinal + Nombre;
                            Soporte.Nombre = Nombre;
                            Soporte.TipoEspecial = 1;
                            Soporte.Codigo = "";
                            db.Soportes.InsertOnSubmit(Soporte);
                        }
                        db.SubmitChanges();
                    }
                    catch (Exception Ex)
                    {
                        Ex.Registrar();
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
            return true;
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public DataTable PagosCobrador_rpt(Int32 idOperador, DateTime FechaIni, DateTime FechaFin)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    String CS = db.Origenes.Single(x => x.idOrigen == 1).ConnectionString;
                    using (OleDbConnection Conn = new OleDbConnection(CS))
                    {
                        OleDbCommand Comm = Conn.CreateCommand();
                        Comm.CommandTimeout = 5 * 60;
                        Comm.CommandText = "Cobranzas.rptPagosCobrador";
                        Comm.CommandType = CommandType.StoredProcedure;
                        Comm.Parameters.Add("FechaDesde", OleDbType.Date).Value = FechaIni;
                        Comm.Parameters.Add("FechaHasta", OleDbType.Date).Value = FechaFin;
                        Comm.Parameters.Add("Cobrador", OleDbType.Integer).Value = db.Operadores.Single(x => x.idOperador == idOperador).Codigo;
                        OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                        DataSet Result = new DataSet();
                        DA.Fill(Result);
                        return Result.Tables[0];
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otPago Pagos_sel(Int32 idPago)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Pagos Pago = db.Pagos.Single(x => x.idPago == idPago);
                    return new otPago
                    {
                        idPago = Pago.idPago,
                        idBancoPropio = Pago.idBancoPropio,
                        TipoPago = Pago.TipoPago,
                        FechaPago = Pago.Fecha.ToString("dd/MM/yyyy"),
                        Referencia = Pago.Referencia,
                        MontoCheque = Pago.MontoCheque,
                        MontoEfectivo = Pago.MontoEfectivo,
                        Descripcion = Pago.Descripcion,
                        idMoneda = Pago.idMoneda,
                        idPersona = Pago.idPersona,
                        idOperador = Pago.idOperador.Value,
                        idBancoOrigen = Pago.idBancoOrigen,
                        Adjuntos = db.Soportes.Where(x => x.Tabla == "Pagos" && x.idTabla == Pago.idPago).Select(x => x.Ubicacion).ToList(),
                        Cheques = Pago.PagosDet.Select(x => new otCheques { Banco = x.idBanco, Monto = x.Monto, Nro = x.NroCheque }).ToList(),
                        Pagos_Cuentas = Pago.Pagos_Cuentas.Select(x => new otPagos_Cuentas { idCuenta = x.idCuenta, MontoCuenta = x.Monto, Retencion1 = x.Retencion1 ?? 0, Retencion2 = x.Retencion2 ?? 0 }).ToList()
                    };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otPagosOperador> Pagos_Operador_Meta_lst(Int32 idOperador, Int32 idMeta, DateTime FechaIni/*, DateTime FechaFin*/)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DateTime FechaIn = FechaIni;
                    DateTime FechaFi = FechaIni.AddMonths(1);
                    return db.Pagos_Cuentas.Where(x => x.Pagos.Aprobado == true && (x.idOperador ?? x.Pagos.idOperador) == idOperador && (x.Fecha ?? x.Pagos.Fecha).Date >= FechaIn.Date && (x.Fecha ?? x.Pagos.Fecha).Date <= FechaFi.Date && db.Metas_Operadores_Cuentas.Any(y => y.idCuenta == x.idCuenta && y.idMeta == idMeta && y.Fecha == FechaIni && y.Activa)).Select(x => new OT.otPagosOperador { Cuenta = x.Cuentas.Codigo, idCuenta = x.Cuentas.idCuenta, idPago = x.idPago, idPersona = x.Pagos.idPersona, Monto = x.Monto, Moneda = x.Pagos.idMoneda, Pago = x.Pagos.Codigo, Codigo = x.Pagos.Personas.Codigo, Persona = x.Pagos.Personas.Nombre, Cliente = x.Cuentas.Clientes.Codigo, MontoDolar = x.Monto / x.Cuentas.CambioLocal, Esp = x.idOperador != null }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


        #endregion
        #region Bancos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otBancos> Bancos_lst(int idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    string idPais = db.Personas.Where(c => c.idPersona == idPersona).First().idPais;
                    var w = db.Bancos.Where(x => x.idPais == idPais).Select(c => new otBancos { idBanco = c.idBanco, Nombre = c.Nombre, Descripcion = c.Descripcion }).ToList();
                    return w;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otBancosPropios> BancosPropios_lst(int idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    string idPais = db.Personas.Where(c => c.idPersona == idPersona).First().idPais;
                    var w = db.BancosPropios.Where(x => x.Bancos.idPais == idPais).Select(c => new otBancosPropios { idBancoPropio = c.idBancoPropio, idBanco = c.idBanco, Nombre = c.Descripcion, Descripcion = c.NroCuenta, ReferenciasInfo = c.ReferenciasInfo, ReferenciasRegExp = c.ReferenciasRegExp }).ToList();
                    return w;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Indicadores
        [OperationContract, WebInvoke(Method = "POST")]
        public string ActualizarGrafico(int idOperador)
        {
            try
            {
                return Negocios.MostrarGrafico(idOperador);
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public string ActualizarGraficoSupervision(int idOperador)
        {
            try
            {
                return Negocios.MostrarGraficoSupervision(idOperador);
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public string ActualizarGraficoPorSemana(int idOperador)
        {
            try
            {
                return Negocios.MostrarGraficoPorSemana(idOperador);
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otIndicadores ActualizarIndicadores(int idOperador)
        {
            try
            {
                return Negocios.ActualizarIndicadores(idOperador);
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Reclamos
        //[OperationContract, WebInvoke(Method = "POST")]
        //public List<otCuenta> Cuentas_Reclamos_lst(List<Int32> Cuentas)
        //{
        //    try
        //    {
        //        using (CobranzasDataContext db = new CobranzasDataContext())
        //        {
        //            var w = db.Cuentas.Where(c => Cuentas.Contains(c.idCuenta) && c.Cuentas_Operadores.Any(y => y.idOperador == Sesion.idOperador)).ToList()
        //                .Select(c => otCuenta.FromCuenta(c)).ToList();
        //            return w;
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex.Informar();
        //    }
        //}
        [OperationContract, WebInvoke(Method = "POST")]
        public otReclamoMostrar Cuentas_Reclamos_sel(Int32 idReclamo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var w = db.Reclamos.Where(x => x.idReclamo == idReclamo).ToList()
                        .Select(x => new otReclamoMostrar
                        {
                            idReclamo = x.idReclamo,
                            Descripcion = x.Descripcion,
                            Codigo = x.Codigo,
                            Fecha = x.Fecha,
                            Motivo = x.ReclamosMotivos.Descripcion,
                            Status = x.idReclamoStatus.HasValue ? x.ReclamosStatus.Descripcion : "«Sin status»",
                            Departamento = x.idDepartamento.HasValue ? x.ReclamosDepartamentos.Departamento : "«Sin Departamento»",
                            idReclamoMotivo = x.idReclamoMotivo,
                            idReclamoStatus = x.idReclamoStatus,
                            idDepartamento = x.idDepartamento,
                            CuentasReclamo = x.Cuentas_Reclamos.Select(c => otCuenta.FromCuenta(c.Cuentas)).ToList(),
                            Soportes = db.Soportes.Where(y => y.TipoEspecial == 1 && y.Tabla == "Reclamos" && y.idTabla == x.idReclamo).Select(c => new otSoporte { idSoporte = c.idSoporte, Nombre = c.Nombre, Ubicacion = c.Ubicacion }).ToList()
                        }).First();
                    return w;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otReclamosMotivos> Motivos_Reclamo_lst(String idPais)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.ReclamosMotivos.Where(x => x.idPais == idPais).Select(x => new otReclamosMotivos { idReclamoMotivo = x.idReclamoMotivo, Descripcion = x.Descripcion }).OrderBy(x => x.Descripcion).ToList();
                }
            }
            catch (Exception Ex) { throw Ex.Informar(); }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public bool Reclamos_ins(otReclamo Reclamo, List<String> Adjuntos)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Reclamos ReclamoNew = new Entidades.Reclamos();
                    ReclamoNew.Descripcion = Reclamo.Descripcion;
                    ReclamoNew.idReclamoMotivo = Reclamo.idReclamoMotivo;
                    ReclamoNew.Fecha = DateTime.Now;
                    ReclamoNew.Abierto = true;
                    ReclamoNew.Procede = true;
                    ReclamoNew.Creador = Sesion.Operador.Login;
                    ReclamoNew.idOrigen = db.Cuentas.Single(x => x.idCuenta == Reclamo.Cuentas[0]).idOrigen.Value;
                    ReclamoNew.idPais = db.Personas.Single(x => x.idPersona == Reclamo.idPersona).idPais;
                    ReclamoNew.idDepartamento = 1;// Reclamo.idDepartamento;
                    ReclamoNew.idOperador = Reclamo.idOperador;
                    db.Reclamos.InsertOnSubmit(ReclamoNew);
                    db.SubmitChanges();
                    foreach (Int32 Cuenta in Reclamo.Cuentas)
                    {
                        Cuentas_Reclamos CuentaNew = new Entidades.Cuentas_Reclamos();
                        CuentaNew.idCuenta = Cuenta;
                        CuentaNew.idReclamo = ReclamoNew.idReclamo;
                        CuentaNew.idReclamoSolucion = null;
                        db.Cuentas_Reclamos.InsertOnSubmit(CuentaNew);
                    }
                    db.SubmitChanges();
                    try
                    {
                        String Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                        String Ruta2 = db.Parametros.Single(x => x.Clave == "RutaSoportes").Valor;
                        String DirectorioFinal = Ruta2 + "Reclamos\\Reclamo_" + ReclamoNew.idReclamo + "\\";
                        Directory.CreateDirectory(DirectorioFinal);
                        foreach (String Adjunto in Adjuntos)
                        {
                            String Nombre = Adjunto.Split('\\').Last().Substring(20);
                            File.Copy(Ruta + Adjunto, DirectorioFinal + Nombre);
                            Soportes Soporte = new Soportes();
                            Soporte.idCliente = Reclamo.idPersona;
                            Soporte.Tabla = "Reclamos";
                            Soporte.idTabla = ReclamoNew.idReclamo;
                            Soporte.Ubicacion = DirectorioFinal + Nombre;
                            Soporte.Nombre = Nombre;
                            Soporte.TipoEspecial = 1;
                            Soporte.Codigo = "";
                            db.Soportes.InsertOnSubmit(Soporte);
                        }
                        db.SubmitChanges();
                    }
                    catch { }

                    Int32 Status = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "_STReclamo").Valor);
                    Gestiones Gestion = new Gestiones();
                    Gestion.Descripcion = Reclamo.Descripcion;
                    Gestion.Fecha = DateTime.Now;
                    Gestion.idOperador = Reclamo.idOperador;
                    Gestion.idPersona = Reclamo.idPersona;
                    Gestion.idStatus = Status;
                    foreach (Int32 Cuenta in Reclamo.Cuentas)
                    {
                        Cuentas_Gestiones CG = new Cuentas_Gestiones();
                        CG.idCuenta = Cuenta;
                        Gestion.Cuentas_Gestiones.Add(CG);
                    }

                    db.Gestiones.InsertOnSubmit(Gestion);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex) { throw Ex.Informar(); }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otReclamosDepartamentos> ReclamosDepartamentos_lst(String idPais, Int32 idOrigen)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.ReclamosDepartamentos.Where(x => x.idPais == idPais && x.idOrigen == idOrigen).Select(x => new otReclamosDepartamentos { idDepartamento = x.idDepartamento, Departamento = x.Departamento }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Flujos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otFlujos> Flujos_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otFlujos> Result;
                    Result = (from c in db.Flujos select new otFlujos { idFlujo = c.idFlujo, Nombre = c.Nombre }).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otFlujo Flujos_sel(Int32 idFlujo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Flujos Flujo = db.Flujos.Single(x => x.idFlujo == idFlujo);
                    return new otFlujo { idFlujo = Flujo.idFlujo, Nombre = Flujo.Nombre, idReglaSalida = Flujo.idReglaSalida, idTipoCliente = Flujo.idTipoCliente, FlujoAvance = Flujo.FlujoAvance.Select(x => new otFlujoAvance { idPasoInicio = x.idPasoInicio, idPasoFinal = x.idPasoFinal, idRegla = x.idRegla, PasoInicio = x.Pasos.Nombre, PasoFinal = x.Pasos1.Nombre, Regla = x.Reglas.Nombre }).ToList(), Pasos = Flujo.Flujos_Pasos.Select(x => new otPasos { idPaso = x.idPaso, NombrePaso = x.Pasos.Nombre, Posicion = x.Posicion }).ToList() };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Flujos_del(Int32 idFlujo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Flujos Flujo = db.Flujos.Single(x => x.idFlujo == idFlujo);
                    db.Flujos.DeleteOnSubmit(Flujo);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch //(Exception Ex)
            {
                throw new Exception("No se puede Eliminar el Flujo, porque está siendo usado en otros procesos");
                // return false;
            }
        }
        [OperationContract, WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public Int32 Flujos_sav(otFlujo Flujoins)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Flujos Flujo;
                    if (Flujoins.idFlujo == 0)
                    {
                        Flujo = new Flujos();
                        db.Flujos.InsertOnSubmit(Flujo);
                        Flujo.idOperadorDueno = Sesion.idOperador;
                    }
                    else
                    {
                        Flujo = db.Flujos.Single(x => x.idFlujo == Flujoins.idFlujo);
                        db.FlujoAvance.DeleteAllOnSubmit(Flujo.FlujoAvance);
                    }
                    Flujo.Nombre = Flujoins.Nombre;
                    Flujo.idTipoCliente = Flujoins.idTipoCliente;
                    Flujo.idReglaSalida = Flujoins.idReglaSalida;

                    foreach (otPasos Paso in Flujoins.Pasos)
                    {
                        try
                        {
                            Flujo.Flujos_Pasos.Single(x => x.idPaso == Paso.idPaso).Posicion = Paso.Posicion;
                        }
                        catch
                        {
                            Flujo.Flujos_Pasos.Add(new Flujos_Pasos() { idPaso = Paso.idPaso, Posicion = Paso.Posicion });
                        }
                        //General.db.Flujos_Pasos.Single(x => x.idFlujo == Flujoins.idFlujo && x.idPaso == Paso.idPaso).Posicion = Paso.Posicion;
                    }
                    foreach (otFlujoAvance Avance in Flujoins.FlujoAvance)
                    {
                        Flujo.FlujoAvance.Add(new FlujoAvance() { idPasoInicio = Avance.idPasoInicio, idPasoFinal = Avance.idPasoFinal, idRegla = Avance.idRegla });
                    }
                    db.SubmitChanges();
                    return Flujoins.idFlujo;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Reglas
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otReglas> Reglas_lst(Int32? idOperador, Char? Tipo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    List<otReglas> Result;
                    Result = (from c in db.Reglas where (Tipo == null || c.TipoRegla == Tipo) orderby c.Nombre select new otReglas { idRegla = c.idRegla, Nombre = c.Nombre }).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otRegla Reglas_sel(Int32 idRegla)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Reglas Regla = db.Reglas.Single(x => x.idRegla == idRegla);
                    return new otRegla { idRegla = idRegla, Nombre = Regla.Nombre, Criterios = Regla.Criterios, TipoRegla = Regla.TipoRegla, ReglasDet = Regla.ReglasDet.Select(x => new otReglaDet { idCampo = x.idCampo, Campo = x.Campos.Definicion, Numero = x.Numero, Operador = x.Operador, Valor = x.Valor }).ToList() };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 Reglas_sav(otRegla Reglains)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Reglas Regla;
                    if (Reglains.idRegla != 0)
                    {
                        Regla = db.Reglas.Single(x => x.idRegla == Reglains.idRegla);
                        db.ReglasDet.DeleteAllOnSubmit(Regla.ReglasDet);
                    }
                    else
                    {
                        Regla = new Reglas();
                        db.Reglas.InsertOnSubmit(Regla);
                    }
                    Regla.Nombre = Reglains.Nombre;
                    Regla.TipoRegla = Reglains.TipoRegla;
                    Regla.Criterios = Reglains.Criterios;
                    int i = 0;
                    foreach (otReglaDet ReglaDet in Reglains.ReglasDet)
                    {
                        i++;
                        Regla.ReglasDet.Add(new Entidades.ReglasDet { idCampo = ReglaDet.idCampo, Valor = ReglaDet.Valor, Operador = ReglaDet.Operador, Numero = i });
                    }
                    db.SubmitChanges();
                    db.Refresh(RefreshMode.OverwriteCurrentValues, Regla);
                    return Regla.idRegla;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region TipoCliente
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otTipoCliente> TiposCliente_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otTipoCliente> Result;
                    Result = (from c in db.TiposCliente select new otTipoCliente { idTipoCliente = c.idTipoCliente, Nombre = c.Nombre }).ToList();
                    return Result;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Pasos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otPaso> Pasos_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otPaso> Result;
                    Result = (from c in db.Pasos select new otPaso { idPaso = c.idPaso, Nombre = c.Nombre }).ToList();
                    return Result;

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Campañas
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCampanaslst> Campanas_lst(int? idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otCampanaslst> Result = db.Campanas.Where(x => db.OperadoresSupervisados(idOperador ?? Sesion.Operador.idOperador, "").Select(y => y.idOperador).Contains(x.idOperadorDueno)).Select(c => new otCampanaslst
                    {
                        idCampana = c.idCampana,
                        Nombre = c.Nombre,
                        FechaInicio = c.FechaInicio,
                        FechaEstimadaFin = c.FechaEstimadaFin,
                        FechaFin = c.FechaFin,
                        Peso = c.Peso,
                        Activa = c.Activa
                    }).ToList();
                    //(from c in General.db.Campanas.ToList() select new otCampanaslst { idCampana = c.idCampana, Nombre = c.Nombre, FechaInicio = c.FechaInicio, FechaEstimadaFin = c.FechaEstimadaFin, FechaFin = c.FechaFin, Peso = c.Peso }).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCampanasCuentas> Campanas_Cuentas_Activas_lst(Int32 idCampana, Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas Campana = db.Campanas.SingleOrDefault(x => x.idCampana == idCampana);
                    var CC = db.Campanas_Cuentas.Where(x => db.CuentaActivaEnCampana(x.idCuenta, idCampana, idOperador).Value);
                    var Result = CC.Select(x => new otCampanasCuentas
                        {
                            idCuenta = x.idCuenta,
                            idOperador = idOperador + (x.Cuentas.Codigo != "" && x.Cuentas.Clientes.Nombre != "" ? 0 : 0),
                            FechaInicio = x.FechaInicio,
                            FechaFin = x.FechaFin,
                            Cuenta = otCuenta.FromCuenta(x.Cuentas)
                        });
                    return Result.ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCampanasCuentas> Campanas_Cuentas_Inactivas_lst(Int32 idCampana, DateTime FechaIni, DateTime FechaFin)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas Campana = db.Campanas.SingleOrDefault(x => x.idCampana == idCampana);
                    var cc = db.Campanas_Cuentas.Where(x => db.CuentaInactivaEnCampana(x.idCuenta, idCampana, FechaIni, FechaFin).Value);
                    var Result = cc.Select(x => new otCampanasCuentas
                    {
                        idCuenta = x.idCuenta,
                        idOperador = (x.Cuentas.Codigo != "" && x.Cuentas.Clientes.Nombre != "" ? 0 : 0),
                        FechaInicio = x.FechaInicio,
                        FechaFin = x.FechaFin,
                        Cuenta = otCuenta.FromCuenta(x.Cuentas)
                    });
                    return Result.ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public otCampanas Campanas_sel(Int32 idCampana)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas Campana = db.Campanas.SingleOrDefault(x => x.idCampana == idCampana);
                    if (Campana == null) throw new Exception("No se ha encontrado esa campaña");
                    if (!db.OperadoresSupervisados(Sesion.Operador.idOperador, "").Any(x => x.idOperador == Campana.idOperadorDueno))
                    {
                        throw new Exception("No tiene privilegios para ver esta campaña");
                    }
                    otCampanas Temp = new otCampanas
                    {
                        idCampana = Campana.idCampana,
                        FechaInicio = Campana.FechaInicio,
                        FechaFin = Campana.FechaFin,
                        FechaEstimadaFin = Campana.FechaEstimadaFin,
                        Nombre = Campana.Nombre,
                        idModoDistOperadores = Campana.idModoDistOperadores ?? 0,
                        idReglaCreacion = Campana.idReglaCreacion ?? 0,
                        idFlujo = Campana.idFlujo ?? 0,
                        idPaso = Campana.idPaso ?? 0,
                        Peso = Campana.Peso,
                        idOperadorDueno = Campana.idOperadorDueno,
                        Activa = Campana.Activa,
                        TipoCampana = Campana.TipoCampana,
                        idReglaSalida = Campana.idReglaSalida ?? 0,
                        MontoMeta = Campana.MontoMeta,
                        Campanas_Cuentas = new List<otCampanasCuentas>(),
                        /*Campana.Campanas_Cuentas.Select(x => new otCampanasCuentas
                        {
                            idCuenta = x.idCuenta,
                            idOperador = (x.Cuentas.Cuentas_Operadores.FirstOrDefault(y => y.FechaFin == null) ?? new Cuentas_Operadores()).idOperador,
                            FechaInicio = x.FechaInicio,
                            FechaFin = x.FechaFin,
                            Cuenta = otCuenta.FromCuenta(x.Cuentas)
                        }).ToList(),*/
                        Campanas_Operadores = Campana.Campanas_Operadores.Select(x => new otCampanasOperadores
                        {
                            FechaFin = x.FechaFin,
                            FechaInicio = x.FechaInicio,
                            idOperador = x.idOperador,
                            Operador = x.Operadores.Nombre,
                            CuentasCampana = db.Cuentas_Operadores.Count(y => y.idOperador == x.idOperador && y.FechaFin == null && db.Campanas_Cuentas.Any(z => z.idCampana == x.idCampana && z.FechaFin == null)),
                            CuentasTotales = db.Cuentas_Operadores.Count(y => y.idOperador == x.idOperador && y.FechaFin == null)
                        }).ToList()
                    };
                    return Temp;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]//()
        public List<otMoneda> Monedas_lst(Int32 idPersona)
        {
            List<otMoneda> Result = new List<otMoneda>();
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    //return db.Personas.Single(x => x.idPersona == idPersona).Paises.Paises_Monedas.Select(x=>new otMoneda{ idMoneda=x.idMoneda, Nombre=x.Monedas.Nombre }).ToList();
                    Monedas Local = db.Personas.Single(x => x.idPersona == idPersona).Paises.Monedas;
                    Result.Add(new otMoneda { idMoneda = Local.idMoneda, Nombre = Local.Nombre });
                    Result.AddRange(db.Personas.Single(x => x.idPersona == idPersona).Paises.Paises_Monedas.Select(x => new otMoneda { idMoneda = x.idMoneda, Nombre = x.Monedas.Nombre }).ToList());
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]//(BodyStyle = WebMessageBodyStyle.WrappedRequest)
        public Int32 Campanas_ins(otCampanas Campanains)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas Campana;
                    Boolean Nuevo = Campanains.idCampana == 0;
                    if (Nuevo)
                    {
                        Campana = new Campanas();
                        db.Campanas.InsertOnSubmit(Campana);
                    }
                    else
                    {

                        Campana = db.Campanas.Single(x => x.idCampana == Campanains.idCampana);
                        var Cuentas = db.Campanas_Cuentas.Where(x => !Campanains.Campanas_Cuentas.Any(y => y.idCuenta == x.idCuenta));
                        foreach (Campanas_Cuentas CCuenta in Cuentas)
                        {
                            CCuenta.FechaFin = DateTime.Now;
                        }

                        var Operadores = db.Campanas_Operadores.Where(x => !Campanains.Campanas_Operadores.Any(y => y.idOperador == x.idOperador));
                        foreach (Campanas_Operadores COperador in Operadores)
                        {
                            COperador.FechaFin = DateTime.Now;
                        }
                    }
                    Campana.Nombre = Campanains.Nombre;
                    Campana.FechaInicio = Campanains.FechaInicio == null ? DateTime.Now : Convert.ToDateTime(Campanains.FechaInicio);
                    Campana.FechaFin = Campanains.FechaFin == null ? (DateTime?)null : Convert.ToDateTime(Campanains.FechaFin);
                    Campana.FechaEstimadaFin = Campanains.FechaEstimadaFin == null ? (DateTime?)null : Convert.ToDateTime(Campanains.FechaEstimadaFin);
                    Campana.idFlujo = Campanains.idFlujo;
                    Campana.idPaso = Campanains.idPaso;
                    Campana.idOperadorDueno = Sesion.idOperador;
                    Campana.Peso = Campanains.Peso;
                    Campana.Activa = Campanains.Activa;
                    Campana.idReglaCreacion = Campanains.idReglaCreacion == 0 ? (Int32?)null : Campanains.idReglaCreacion;
                    Campana.TipoCampana = Campanains.TipoCampana;
                    Campana.idReglaSalida = Campanains.idReglaSalida == 0 ? (Int32?)null : Campanains.idReglaSalida;

                    foreach (var COperador in Campanains.Campanas_Operadores)
                    {
                        Campanas_Operadores COperadorNew = Campana.Campanas_Operadores.SingleOrDefault(x => x.idCampana == Campanains.idCampana && x.idOperador == COperador.idOperador && x.FechaFin == null);
                        if (COperadorNew == null)
                        {
                            COperadorNew = new Campanas_Operadores();
                            Campana.Campanas_Operadores.Add(COperadorNew);
                        }
                        COperadorNew.idOperador = COperador.idOperador;
                        COperadorNew.FechaInicio = DateTime.Now;
                    }
                    foreach (var CCuenta in Campanains.Campanas_Cuentas)
                    {
                        Cuentas Cuenta = db.Cuentas.Single(x => x.idCuenta == CCuenta.idCuenta);
                        Cuenta.idFlujo = Campanains.idFlujo;
                        Cuenta.idPaso = Campanains.idPaso;
                        Campanas_Cuentas CCuentaNew = Campana.Campanas_Cuentas.SingleOrDefault(x => x.idCampana == Campanains.idCampana && x.idCuenta == CCuenta.idCuenta && x.FechaFin == null);
                        if (CCuentaNew == null)
                        {
                            CCuentaNew = new Campanas_Cuentas();
                            Campana.Campanas_Cuentas.Add(CCuentaNew);
                        }
                        CCuentaNew.idCuenta = CCuenta.idCuenta;
                        CCuentaNew.FechaInicio = DateTime.Now;
                        if (CCuenta.idOperador != 0)
                        {
                            Operadores Op = db.Operadores.Single(x => x.idOperador == CCuenta.idOperador);
                            Op.Cuentas_Operadores.Add(new Cuentas_Operadores { FechaInicio = DateTime.Now, idCuenta = CCuenta.idCuenta });
                        }
                    }
                    db.SubmitChanges();
                    //General.db.Refresh(RefreshMode.OverwriteCurrentValues, Campana);
                    return Campana.idCampana;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        public Boolean Campanas_sav(otCampanas Campanains)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas Campana = db.Campanas.Single(x => x.idCampana == Campanains.idCampana);

                    Campana.Nombre = Campanains.Nombre;
                    Campana.FechaInicio = Campanains.FechaInicio;//Campanains.FechaInicio == null ? DateTime.Now : Convert.ToDateTime(Campanains.FechaInicio);
                    Campana.FechaFin = Campanains.FechaFin;//Campanains.FechaFin == "" ? (DateTime?)null : Convert.ToDateTime(Campanains.FechaFin);
                    Campana.FechaEstimadaFin = Campanains.FechaEstimadaFin;//Campanains.FechaEstimadaFin == "" ? (DateTime?)null : Convert.ToDateTime(Campanains.FechaEstimadaFin);
                    Campana.idFlujo = Campanains.idFlujo;
                    Campana.idPaso = Campanains.idPaso;
                    Campana.Peso = Campanains.Peso;
                    Campana.Activa = Campanains.Activa;
                    Campana.idOperadorDueno = Campanains.idOperadorDueno;
                    //Campana.idReglaCreacion = Campanains.idReglaCreacion==0?(Int32?)null:Campanains.idReglaCreacion;
                    Campana.idReglaSalida = Campanains.idReglaSalida == 0 ? (Int32?)null : Campanains.idReglaSalida;
                    Campana.TipoCampana = Campanains.TipoCampana;

                    foreach (otCampanasCuentas CCins in Campanains.Campanas_Cuentas)
                    {
                        var COS = db.Cuentas_Operadores.Where(x => x.idCuenta == CCins.idCuenta && x.FechaFin == null);
                        Boolean Insertar = true;
                        foreach (Cuentas_Operadores C_O in COS)
                        {
                            if (C_O.idOperador == CCins.idOperador)
                            {
                                Insertar = false;
                            }
                            else
                            {
                                C_O.FechaFin = DateTime.Now;
                            }
                        }
                        //Cuentas_Operadores CO = General.db.Cuentas_Operadores.SingleOrDefault(x => x.idCuenta == CCins.idCuenta && x.FechaFin == null);
                        //Boolean Insertar = false;
                        //if (CO == null)
                        //{
                        //    Insertar = true;
                        //}
                        //else
                        //{
                        //    if (CO.idOperador != CCins.idOperador)
                        //    {
                        //        Insertar = true;
                        //        CO.FechaFin = DateTime.Now;
                        //    }
                        //}
                        if (Insertar && CCins.idOperador.HasValue)
                        {
                            Cuentas_Operadores CO = new Cuentas_Operadores();
                            CO.idCuenta = CCins.idCuenta;
                            CO.idOperador = CCins.idOperador.Value;
                            CO.FechaInicio = DateTime.Now;
                            db.Cuentas_Operadores.InsertOnSubmit(CO);
                        }
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebGet]
        public Boolean Campanas_Cuenta_ins(Int32 idCampana, Int32 idCuenta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas_Cuentas CC = new Campanas_Cuentas();
                    CC.idCuenta = idCuenta;
                    CC.FechaInicio = DateTime.Now;
                    CC.idCampana = idCampana;
                    db.Campanas_Cuentas.InsertOnSubmit(CC);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebGet]
        public Boolean Campanas_Cuenta_del(Int32 idCampana, Int32 idCuenta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas_Cuentas CC = db.Campanas_Cuentas.Single(x => x.idCampana == idCampana && x.idCuenta == idCuenta && x.FechaFin == null);
                    CC.FechaFin = DateTime.Now;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCuenta> Campanas_SeleccionCuentas_lst(Int32 idRegla, Int32 idModoDistOperadores, List<Int32> Operadores)
        {
            try
            {
                if (idRegla == 0) { return new List<otCuenta>(); }
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    //var y = db.Cuentas.Where(c => c.Activa && (db.CumpleRegla(c.idCuenta, idRegla) ?? false)).Select(x => new { x, x.Clientes, x.Personas, x.Personas.Paises, x.Productos }).ToList();
                    //List<otCuenta> w = y.Select(c => otCuenta.FromCuenta(c.x)).ToList();
                    //Negocios.DistribuirOperadoresACuentas(ref w, idModoDistOperadores, Operadores);
                    //return w;
                    List<Int32> Cuentas = db.CuentasReglaResult(idRegla, null).Select(w => w.idCuenta ?? 0).ToList();
                    var y = db.vwCuentas.Where(x => Cuentas.Contains(x.idCuenta)).Select(x => otCuenta.FromvwCuenta(x)).ToList();
                    Negocios.DistribuirOperadoresACuentas(ref y, idModoDistOperadores, Operadores);
                    return y;
                    //var y = db.Cuentas.Where(c => c.Activa && (db.CumpleRegla(c.idCuenta, idRegla) ?? false)).Select(x => new { x, x.Clientes, x.Personas, x.Personas.Paises, x.Productos }).ToList();
                    //List<otCuenta> w = y.Select(c => otCuenta.FromCuenta(c.x)).ToList();
                    //Negocios.DistribuirOperadoresACuentas(ref w, idModoDistOperadores, Operadores);
                    //return w;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Campanas_Operadores_ins(int idCampana, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas C = db.Campanas.Single(x => x.idCampana == idCampana);
                    C.Campanas_Operadores.Add(new Campanas_Operadores { idOperador = idOperador, FechaInicio = DateTime.Now });
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Campanas_Operadores_del(int idCampana, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas C = db.Campanas.Single(x => x.idCampana == idCampana);
                    var COS = db.Cuentas_Operadores.Where(x => x.idOperador == idOperador && x.FechaFin == null && x.Cuentas.Campanas_Cuentas.Any(y => y.idCampana == idCampana));
                    foreach (Cuentas_Operadores CO in COS)
                    {
                        CO.FechaFin = DateTime.Now;
                    }
                    var CAOS = db.Campanas_Operadores.Where(x => x.idCampana == idCampana && x.idOperador == idOperador && x.FechaFin == null);
                    foreach (Campanas_Operadores CAO in CAOS)
                    {
                        CAO.FechaFin = DateTime.Now;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Campanas_Cuentas_ins(int idCampana, List<int> Cuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas C = db.Campanas.Single(x => x.idCampana == idCampana);
                    foreach (int idCuenta in Cuentas)
                    {
                        Cuentas Cuenta = db.Cuentas.Single(x => x.idCuenta == idCuenta);
                        Cuenta.idFlujo = C.idFlujo;
                        Cuenta.idPaso = C.idPaso;
                        C.Campanas_Cuentas.Add(new Campanas_Cuentas { idCuenta = idCuenta, FechaInicio = DateTime.Now });
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Campanas_Cuentas_del(int idCampana, List<int> Cuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Campanas C = db.Campanas.Single(x => x.idCampana == idCampana);
                    foreach (int idCuenta in Cuentas)
                    {
                        try
                        {
                            var CC = C.Campanas_Cuentas.Single(x => x.idCuenta == idCuenta && x.FechaFin == null);
                            CC.FechaFin = DateTime.Now;
                            var CO = db.Cuentas_Operadores.Single(x => x.idCuenta == idCuenta && x.FechaFin == null);
                            CO.FechaFin = DateTime.Now;
                        }
                        catch { }
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Operadores_Cuentas_ins(int idOperador, List<int> Cuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Operadores C = db.Operadores.Single(x => x.idOperador == idOperador);
                    foreach (int idCuenta in Cuentas)
                    {
                        Cuentas Cuenta = db.Cuentas.Single(x => x.idCuenta == idCuenta);
                        Cuenta.idFlujo = C.idFlujo;
                        Cuenta.idPaso = C.idPaso;
                        C.Cuentas_Operadores.Add(new Cuentas_Operadores { idCuenta = idCuenta, FechaInicio = DateTime.Now });
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean DistribucionCampanas_Ejecutar(Int32 idDistribucion)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.DistribuirCampana(idDistribucion);
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Combos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCombo> ModoDistOperadores_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.ModoDistOperadores.Select(x => new otCombo { id = x.idModoDistOperadores, Nombre = x.Nombre }).ToList();

                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Metas
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otMetas> Metas_lst(int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<otMetas> Result = db.Metas.Where(x => db.OperadoresSupervisados(idOperador, "").Select(y => y.idOperador).Contains(x.idOperadorDueno)).Select(c => new otMetas
                    {
                        idMeta = c.idMeta,
                        Nombre = c.Nombre,
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        Regla = c.Reglas.Nombre,
                    }).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public otMeta Metas_sel(int idMeta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Metas m = db.Metas.Single(x => x.idMeta == idMeta);
                    otMeta Result = new otMeta();
                    Result.idMeta = idMeta;
                    Result.Nombre = m.Nombre;
                    Result.FechaInicio = m.FechaInicio;
                    Result.FechaFin = m.FechaFin;
                    Result.idRegla = m.idRegla;
                    Result.Frecuencia = m.Frecuencia;
                    Result.AplicaExclusiones = m.AplicaExclusiones;
                    Result.Metas = m.Metas_Operadores_Cuentas.Select(x => x.Fecha).Distinct().OrderByDescending(x => x).Select(x => new otFecha { Fecha = x.AFechaMuyCorta() }).ToList();
                    Result.Operadores = m.Metas_Operadores.Where(x => x.FechaFin == null).Select(x => new otCombo { id = x.idOperador, Nombre = x.Operadores.Nombre }).OrderBy(x => x.Nombre).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Operadores_del(int idMeta, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.Metas_Operadores.Single(x => x.idMeta == idMeta && x.idOperador == idOperador && x.FechaFin == null).FechaFin = DateTime.Now;
                    db.SubmitChanges();
                }
                return true;
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public otMetaDetalle Metas_Detalle_sel(int idMeta, DateTime Fecha, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var Cuentas = db.Metas_Operadores_Cuentas.Where(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador);
                    otMetaDetalle Result = new otMetaDetalle();
                    //Result.Cuentas = Cuentas.Where(x => x.InclusionManual == null && x.Activa && x.idCuenta != null).Select(x => otCuenta.FromCuenta(x.Cuentas)).ToList();
                    //Result.Exclusiones = Cuentas.Where(x => !x.Activa && x.idCuenta != null).Select(x => otCuenta.FromCuenta(x.Cuentas)).ToList();
                    //Result.Inclusiones = Cuentas.Where(x => x.InclusionManual != null && x.Activa && x.idCuenta != null).Select(x => otCuenta.FromCuenta(x.Cuentas)).ToList();
                    //Result.Cuentas = db.vwCuentas.Where(y => Cuentas.Where(x => x.InclusionManual == null && x.Activa && x.idCuenta != null).Any(x => y.idCuenta == x.idCuenta)).Select(y => otCuenta.FromvwCuenta(y)).ToList();
                    //Result.Exclusiones = db.vwCuentas.Where(y => Cuentas.Where(x => !x.Activa && x.idCuenta != null).Any(x => y.idCuenta == x.idCuenta)).Select(y => otCuenta.FromvwCuenta(y)).ToList();
                    //Result.Inclusiones = db.vwCuentas.Where(y => Cuentas.Where(x => x.InclusionManual != null && x.Activa && x.idCuenta != null).Any(x => y.idCuenta == x.idCuenta)).Select(y => otCuenta.FromvwCuenta(y)).ToList();
                    Result.Cuentas = Cuentas.Where(x => x.InclusionManual == null && x.Activa && x.idCuenta != null).Select(x => new otMetaDetalleDet { Documento = x.Cuentas.Codigo, Persona = x.Cuentas.Personas.Nombre, Codigo = x.Cuentas.Personas.Codigo, idPersona = x.Cuentas.idPersona, Fecha = x.Cuentas.FechaDocumento.Value, Antiguedad = Convert.ToInt32((Fecha - x.Cuentas.FechaDocumento.Value).TotalDays), Cliente = x.Cuentas.Clientes.Codigo, Producto = x.Cuentas.Productos.Nombre, Meta = x.Meta }).ToList();
                    Result.Exclusiones = Cuentas.Where(x => !x.Activa && x.idCuenta != null).Select(x => new otMetaDetalleDet { Documento = x.Cuentas.Codigo, Persona = x.Cuentas.Personas.Nombre, Codigo = x.Cuentas.Personas.Codigo, idPersona = x.Cuentas.idPersona, Fecha = x.Cuentas.FechaDocumento.Value, Antiguedad = Convert.ToInt32((Fecha - x.Cuentas.FechaDocumento.Value).TotalDays), Cliente = x.Cuentas.Clientes.Codigo, Producto = x.Cuentas.Productos.Nombre, Meta = x.Meta }).ToList();
                    Result.Inclusiones = Cuentas.Where(x => x.InclusionManual != null && x.Activa && x.idCuenta != null).Select(x => new otMetaDetalleDet { Documento = x.Cuentas.Codigo, Persona = x.Cuentas.Personas.Nombre, Codigo = x.Cuentas.Personas.Codigo, idPersona = x.Cuentas.idPersona, Fecha = x.Cuentas.FechaDocumento.Value, Antiguedad = Convert.ToInt32((Fecha - x.Cuentas.FechaDocumento.Value).TotalDays), Cliente = x.Cuentas.Clientes.Codigo, Producto = x.Cuentas.Productos.Nombre, Meta = x.Meta }).ToList();
                    Result.Ajuste = Cuentas.Where(x => x.idCuenta == null).Sum(x => (int?)x.Meta) ?? 0;
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Ajuste_upd(int idMeta, DateTime Fecha, int idOperador, Decimal Monto)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Decimal MontoAsignar = Monto;
                    if (Monto > 0)
                    {
                        MontoAsignar = Monto - db.Metas_Operadores_Cuentas.Where(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador && x.Activa).Sum(x => (int?)x.Meta) ?? 0;
                    }

                    try
                    {
                        db.Metas_Operadores_Cuentas.Single(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador && x.idCuenta == null).Meta = MontoAsignar;

                    }
                    catch
                    {
                        db.Metas_Operadores_Cuentas.InsertOnSubmit(new Metas_Operadores_Cuentas { idMeta = idMeta, Fecha = Fecha, Activa = true, Finalizado = null, InclusionManual = DateTime.Now, idOperador = idOperador, Meta = MontoAsignar });
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Cuentas_ins(int idMeta, DateTime Fecha, int idOperador, String Codigo, Boolean EsCuenta, String idPais = "VEN")
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (EsCuenta)
                    {
                        Metas_Operadores_Cuentas MOC = new Metas_Operadores_Cuentas();
                        MOC.idMeta = idMeta;
                        MOC.Fecha = Fecha;
                        MOC.idOperador = idOperador;
                        MOC.InclusionManual = DateTime.Now;
                        MOC.Activa = true;
                        MOC.idCuenta = db.Cuentas.Single(x => x.Codigo == Codigo && x.Personas.idPais == idPais).idCuenta;
                        db.Metas_Operadores_Cuentas.InsertOnSubmit(MOC);
                    }
                    else
                    {
                        foreach (int idCuenta in db.Personas.Single(x => x.Codigo == Codigo && x.idPais == idPais).Cuentas.Select(x => x.idCuenta))
                        {
                            Metas_Operadores_Cuentas MOC = new Metas_Operadores_Cuentas();
                            MOC.idMeta = idMeta;
                            MOC.Fecha = Fecha;
                            MOC.idOperador = idOperador;
                            MOC.InclusionManual = DateTime.Now;
                            MOC.Activa = true;
                            MOC.idCuenta = idCuenta;
                            db.Metas_Operadores_Cuentas.InsertOnSubmit(MOC);
                        }
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Cuentas_del(int idMeta, DateTime Fecha, int idOperador, int idCuenta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.Metas_Operadores_Cuentas.Single(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador && x.idCuenta == idCuenta).Activa = false;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Cuentas_del2(int idMeta, DateTime Fecha, int idOperador, List<int> idCuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (int idCuenta in idCuentas)
                    {
                        db.Metas_Operadores_Cuentas.Single(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador && x.idCuenta == idCuenta).Activa = false;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Cuentas_del_undo(int idMeta, DateTime Fecha, int idOperador, int idCuenta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.Metas_Operadores_Cuentas.Single(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador && x.idCuenta == idCuenta).Activa = true;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Cuentas_del_undo2(int idMeta, DateTime Fecha, int idOperador, List<int> idCuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (int idCuenta in idCuentas)
                    {
                        db.Metas_Operadores_Cuentas.Single(x => x.idMeta == idMeta && x.Fecha == Fecha && x.idOperador == idOperador && x.idCuenta == idCuenta).Activa = true;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Metas_Operadores_ins(int idMeta, int idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.Metas_Operadores.InsertOnSubmit(new Metas_Operadores() { idMeta = idMeta, idOperador = idOperador, FechaInicio = DateTime.Now });
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 Metas_sav(otMeta Meta)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Metas MetaNew;
                    if (Meta.idMeta == 0)
                    {
                        MetaNew = new Metas();
                        db.Metas.InsertOnSubmit(MetaNew);
                        MetaNew.idOperadorDueno = Sesion.Operador.idOperador;
                    }
                    else
                    {
                        MetaNew = db.Metas.Single(x => x.idMeta == Meta.idMeta);
                    }
                    MetaNew.idRegla = Meta.idRegla;
                    MetaNew.FechaInicio = Meta.FechaInicio;
                    MetaNew.Frecuencia = Meta.Frecuencia;
                    MetaNew.Nombre = Meta.Nombre;
                    MetaNew.AplicaExclusiones = Meta.AplicaExclusiones;
                    db.SubmitChanges();
                    return MetaNew.idMeta;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCombo> MetasDet_Operadores_lst(int idMeta, DateTime Fecha)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Metas_Operadores_Cuentas.Where(x => x.idMeta == idMeta && x.Fecha == Fecha).Select(x => new otCombo { id = x.idOperador, Nombre = x.Operadores.Nombre }).Distinct().OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean EjecutarMetas()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CrearMetas(DateTime.Now);
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Exclusiones
        [OperationContract, WebInvoke(Method = "POST")]
        public String Exclusiones_ins(Int32 idOperador, Int32 idPersona, Int32 idMotivo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (!db.Exclusiones.Any(x => x.idPersona == idPersona && x.FechaFin == null))
                    {
                        db.Exclusiones.InsertOnSubmit(new Exclusiones() { FechaInicio = DateTime.Now, idOperadorInc = idOperador, idPersona = idPersona, idExclusionMotivo = idMotivo });
                    }
                    //var MOCS = db.Metas_Operadores_Cuentas.Where(x => x.Cuentas.idPersona == idPersona && x.Finalizado == null).ToList();
                    //foreach (Metas_Operadores_Cuentas MOC in MOCS)
                    //{
                    //    MOC.Activa = false;
                    //}
                    db.SubmitChanges();
                    return "La persona fue añadida a la lista de Exclusión";
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public String Exclusiones_ins_CodigoC(Int32 idOperador, String Codigo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Int32 idCuenta = db.Cuentas.Single(x => x.Codigo == Codigo).idCuenta;

                    if (!db.Exclusiones.Any(x => x.idCuenta == idCuenta && x.FechaFin == null))
                    {
                        db.Exclusiones.InsertOnSubmit(new Exclusiones() { FechaInicio = DateTime.Now, idOperadorInc = idOperador, idCuenta = idCuenta });
                    }
                    //var MOCS = db.Metas_Operadores_Cuentas.Where(x => x.idCuenta == idCuenta && x.Finalizado == null).ToList();
                    //foreach (Metas_Operadores_Cuentas MOC in MOCS)
                    //{
                    //    MOC.Activa = false;
                    //}
                    db.SubmitChanges();
                    return "La cuenta fue añadida a la lista de Exclusión";
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public String Exclusiones_ins_CodigoP(Int32 idOperador, String Codigo)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Int32 idPersona = db.Personas.Single(x => x.Codigo == Codigo).idPersona;

                    if (!db.Exclusiones.Any(x => x.idPersona == idPersona && x.FechaFin == null))
                    {
                        db.Exclusiones.InsertOnSubmit(new Exclusiones() { FechaInicio = DateTime.Now, idOperadorInc = idOperador, idPersona = idPersona });
                    }
                    //var MOCS = db.Metas_Operadores_Cuentas.Where(x => x.Cuentas.idPersona == idPersona && x.Finalizado == null).ToList();
                    //foreach (Metas_Operadores_Cuentas MOC in MOCS)
                    //{
                    //    MOC.Activa = false;
                    //}
                    db.SubmitChanges();
                    return "La persona fue añadida a la lista de Exclusión";
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


        [OperationContract, WebInvoke(Method = "POST")]
        public otExclusiones Exclusiones_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<Int32> Operadores = db.OperadoresSupervisados(idOperador, "").Select(y => y.idOperador ?? 0).ToList();
                    otExclusiones Result = new otExclusiones();
                    Result.Personas = db.Exclusiones.Where(x => x.idPersona != null && x.idOperadorExc == null && Operadores.Contains(x.idOperadorInc)).Select(x => new otExclusionesDet { idExclusion = x.idExclusion, idPersona = x.idPersona.Value, Persona = x.Personas.Nombre, Codigo = x.Personas.Codigo, FechaInicio = x.FechaInicio, Operador = x.Operadores.Nombre, Aprobado = x.Operadores1.Nombre, Motivo = x.ExclusionesMotivos.Motivo }).ToList();
                    Result.Cuentas = db.Exclusiones.Where(x => x.idCuenta != null && x.idOperadorExc == null && Operadores.Contains(x.idOperadorInc)).Select(x => new otExclusionesDet { idExclusion = x.idExclusion, idPersona = x.Cuentas.idPersona, Persona = x.Cuentas.Personas.Nombre, Codigo = x.Cuentas.Personas.Codigo, Cliente = x.Cuentas.Clientes.Codigo, Cuenta = x.Cuentas.Codigo, FechaInicio = x.FechaInicio, Operador = x.Operadores.Nombre, Aprobado = x.Operadores1.Nombre, Motivo = x.ExclusionesMotivos.Motivo }).ToList();
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Exclusiones_del(Int32 idOperador, List<Int32> Exclusiones)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Int32 idExclusion in Exclusiones)
                    {
                        Exclusiones Exclusion = db.Exclusiones.Single(x => x.idExclusion == idExclusion);
                        Exclusion.FechaFin = DateTime.Now;
                        Exclusion.idOperadorExc = idOperador;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Exclusiones_Aprobado(Int32 idOperador, List<Int32> Exclusiones)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Int32 idExclusion in Exclusiones)
                    {
                        Exclusiones Exclusion = db.Exclusiones.Single(x => x.idExclusion == idExclusion);
                        //if (Exclusion.idOperadorApr == null)
                        //{
                        Exclusion.FechaAprobacion = DateTime.Now;
                        Exclusion.idOperadorApr = idOperador;
                        //}
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public String Exclusiones_inss(Int32 idOperador, List<Int32> Cuentas, Int32 idMotivo)
        {
            Int32 Incluidas = 0;
            Int32 Excluidas = 0;
            Int32 NoIncluidas = 0;
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    foreach (Int32 idCuenta in Cuentas)
                    {
                        if (db.Exclusiones.Any(x => x.idCuenta == idCuenta && x.FechaFin == null))
                        {
                            NoIncluidas++;
                        }
                        else
                        {
                            Incluidas++;
                            db.Exclusiones.InsertOnSubmit(new Exclusiones() { FechaInicio = DateTime.Now, idOperadorInc = idOperador, idCuenta = idCuenta, idExclusionMotivo = idMotivo });
                        }
                        //var MOCS = db.Metas_Operadores_Cuentas.Where(x => x.idCuenta == idCuenta && x.Finalizado == null).ToList();
                        //foreach (Metas_Operadores_Cuentas MOC in MOCS)
                        //{
                        //    MOC.Activa = false;
                        //    Excluidas++;
                        //}
                    }
                    db.SubmitChanges();
                    return "Cuentas añadidas a la lista de Exclusión: " + Incluidas.ToString() + ". Cuentas existentes en la lista de Exclusión: " + NoIncluidas.ToString() + ". Cuentas excluidas de la meta Actual: " + Excluidas.ToString();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCombo> ExclusionesMotivos_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.ExclusionesMotivos.Where(x => x.Activo).Select(x => new otCombo { id = x.idExclusionMotivo, Nombre = x.Motivo }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Plantillas
        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otPlantillas_lst> Plantillas_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<int> Operadores = db.OperadoresSupervisados(idOperador, "").Select(y => y.idOperador ?? 0).ToList();
                    return db.Plantillas.Where(x => (x.idOperador == idOperador && x.Privado) || Operadores.Contains(x.idOperador) && !x.Privado).Select(x => new otPlantillas_lst { Fecha = x.FechaCreacion, idPlantilla = x.idPlantilla, Nombre = x.Nombre, Operador = x.Operadores.Nombre, Privado = x.Privado }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public otPlantillas Plantilla_sel(Int32 idPlantilla)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Plantillas P = db.Plantillas.Single(x => x.idPlantilla == idPlantilla);
                    return new otPlantillas
                    {
                        idPlantilla = P.idPlantilla,
                        idOperador = P.idOperador,
                        NombreOperador = P.Operadores.Nombre,
                        NombrePlantilla = P.Nombre,
                        FechaCreacion = P.FechaCreacion,
                        Asunto = P.Asunto,
                        Mensaje = P.Mensaje,
                        Adjunto = P.Adjunto,
                        DestinatariosCopia = P.DestinatariosCopia,
                        DestinatariosCopiaOculta = P.DestinatariosCopiaOculta,
                        idPais = P.idPais,
                        idTipoCliente = P.idTipoCliente,
                        Privado = P.Privado
                    };
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 Plantilla_sav(otPlantillas Plantillains)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Plantillas Plantilla;
                    Boolean Nuevo = Plantillains.idPlantilla == 0;
                    if (Nuevo)
                    {
                        Plantilla = new Plantillas();
                        db.Plantillas.InsertOnSubmit(Plantilla);
                    }
                    else
                    {
                        Plantilla = db.Plantillas.Single(x => x.idPlantilla == Plantillains.idPlantilla);
                    }
                    Plantilla.idOperador = Plantillains.idOperador;
                    Plantilla.FechaCreacion = DateTime.Now;  /*DateTime.ParseExact(Pago.FechaPago, "dd/MM/yyyy", null);*/
                    Plantilla.Nombre = Plantillains.NombrePlantilla;
                    Plantilla.Asunto = Plantillains.Asunto;
                    Plantilla.Mensaje = Plantillains.Mensaje;
                    if (Plantillains.Adjunto == "")
                    {
                        Plantilla.Adjunto = null;
                    }
                    else
                    {
                        Plantilla.Adjunto = Plantillains.Adjunto;
                    }
                    Plantilla.Privado = Plantillains.Privado;
                    Plantilla.DestinatariosCopia = Plantillains.DestinatariosCopia;
                    Plantilla.DestinatariosCopiaOculta = Plantillains.DestinatariosCopiaOculta;
                    Plantilla.idPais = Plantillains.idPais;
                    Plantilla.idTipoCliente = Plantillains.idTipoCliente;
                    db.SubmitChanges();
                    db.Refresh(RefreshMode.OverwriteCurrentValues, Plantilla);
                    return Plantilla.idPlantilla;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Plantilla_del(Int32 idPlantilla)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Plantillas P = db.Plantillas.Single(x => x.idPlantilla == idPlantilla);
                    db.Plantillas.DeleteOnSubmit(P);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otPlantillas_lst> PlantillasCorreo_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<int> Operadores = db.OperadoresSupervisados(idOperador, "").Select(y => y.idOperador ?? 0).ToList();
                    return db.Plantillas.Where(x => (x.idOperador == idOperador && x.Privado) || (Operadores.Contains(x.idOperador) && !x.Privado) || (db.OperadoresSupervisados(x.idOperador, "").Any(y => y.idOperador == idOperador) && !x.Privado)).Select(x => new otPlantillas_lst { Fecha = x.FechaCreacion, idPlantilla = x.idPlantilla, Nombre = x.Nombre, Operador = x.Operadores.Nombre, Privado = x.Privado }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public otPlantillasCorreo PlantillaCorreo_sel(Int32 idPlantilla, Int32? idPersona, Int32 idOperador)
        {
            try
            {
                return Comunes.LlenarPlantilla(idPlantilla, idPersona, idOperador);
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Telefono
        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otTelefonos_lst> TelefonosEliminar_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<Int32> Operadores = db.OperadoresSupervisados(idOperador, "").Select(y => y.idOperador ?? 0).ToList();
                    //List<otOperadoresSimple> Operadores = db.OperadoresSupervisados(idOperador, "").Select(x => new otOperadoresSimple { Nombre = x.Nombre, idOperador = x.idOperador.Value }).ToList();
                    return db.Telefonos.Where(x => Operadores.Contains(x.idOperadorEliminar.Value) && x.idOperadorConfirmadoEliminar == null).Select(x => new otTelefonos_lst { Telefono = x.Telefono, NombreOperador = x.Operadores2.Nombre, idTelefono = x.idTelefono, NombrePersona = x.Personas.Nombre ?? x.PersonasContacto.Personas.Nombre }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otTelefonos_lst> TelefonosAgregar_lst(Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    List<Int32> Operadores = db.OperadoresSupervisados(idOperador, "").Select(y => y.idOperador ?? 0).ToList();
                    //List<otOperadoresSimple> Operadores = db.OperadoresSupervisados(idOperador, "").Select(x => new otOperadoresSimple { Nombre = x.Nombre, idOperador = x.idOperador.Value }).ToList();
                    return db.Telefonos.Where(x => Operadores.Contains(x.idOperador.Value) && x.idOperadorConfirmado == null).Select(x => new otTelefonos_lst { Telefono = x.Telefono, NombreOperador = x.Operadores2.Nombre, idTelefono = x.idTelefono, NombrePersona = x.Personas.Nombre ?? x.PersonasContacto.Personas.Nombre }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Telefonos_del(Int32 idTelefono, Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Telefonos Telefono = db.Telefonos.Single(x => x.idTelefono == idTelefono);
                    Telefono.idOperadorEliminar = idOperador;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean TelefonosConfirmar(Int32 idTelefono, Int32 idOperador, Boolean Agregar, Boolean Confirmado)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Telefonos Telefono = db.Telefonos.Single(x => x.idTelefono == idTelefono);

                    if (Agregar)
                    {
                        if (Confirmado)
                        {
                            Telefono.idOperadorConfirmado = idOperador;
                        }
                        else
                        {
                            Telefono.idOperadorEliminar = idOperador;
                            Telefono.idOperadorConfirmadoEliminar = idOperador;
                        }
                    }
                    else
                    {
                        if (Confirmado)
                        {
                            Telefono.idOperadorConfirmadoEliminar = idOperador;
                        }
                        else
                        {
                            Telefono.idOperadorEliminar = null;
                        }
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<OT.otTelefonos_lst> TelefonosPersonas_lst(Int32 idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    //List<otOperadoresSimple> Operadores = db.OperadoresSupervisados(idOperador, "").Select(x => new otOperadoresSimple { Nombre = x.Nombre, idOperador = x.idOperador.Value }).ToList();
                    return db.Telefonos.Where(x => x.idPersona == idPersona && x.idOperadorEliminar == null && x.idOperadorConfirmadoEliminar == null).Select(x => new otTelefonos_lst { Telefono = x.Telefono, idTelefono = x.idTelefono, NombrePersona = x.Personas.Nombre ?? x.PersonasContacto.Personas.Nombre }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Result Llamar(String Telefono, Int32 idTelefono, Int32 idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Oficinas Oficina = db.Operadores.Single(x => x.idOperador == Sesion.Operador.idOperador).Oficinas;
                    if (Oficina.idCentralIp == null)
                    {
                        throw new Exception("La oficina en la que usted labora, no cuenta con una central Ip para realizar esta llamada");
                    }
                    CentralesIp Datos = Oficina.CentralesIp;
                    CentralIp Central = new CentralIp(Datos.Host, Datos.Port, Datos.Usuario, Datos.Contrasena, Datos.PrefijoSalida, Datos.CodigoPais, Datos.CodigoArea);
                    try
                    {

                        Llamadas Llamada = new Llamadas();
                        Llamada.Codigo = "--";
                        Llamada.Extension = Sesion.Operador.Extension;
                        Llamada.Grabacion = "";
                        Llamada.idOperador = Sesion.idOperador;
                        Llamada.idTelefono = idTelefono;
                        Llamada.Telefono = Central.Limpiar(Telefono);
                        Llamada.StatusPrimario = "";
                        Llamada.Fecha = DateTime.Now;
                        db.Llamadas.InsertOnSubmit(Llamada);
                        db.SubmitChanges();

                        Central.Llamar(Telefono, Sesion.Operador.Extension, true);
                        if (Central.UniqueId == "") throw new Exception("No se pudo establecer la llamada");
                        DateTime Inicio = DateTime.Now;
                        while ((DateTime.Now - Inicio).TotalSeconds < 15)
                        {
                            if (Central.DestChannel != "")
                            {
                                break;
                            }
                            Thread.Yield();
                        }
                        Result Result = new Result() { Canal = Central.Channel, DestChannel = Central.DestChannel, Error = "", Status = "", UniqueId = Central.UniqueId };

                        Llamada.Codigo = Result.Error != "" ? "--" : Result.UniqueId;
                        if (Llamada.Codigo == "") Llamada.Codigo = "--";
                        Llamada.StatusPrimario = Result.Error != "" ? Result.Error : Result.Status;
                        db.SubmitChanges();
                        return Result;
                    }
                    catch (Exception Ex)
                    {
                        throw Ex.Informar();
                    }
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return new Result() { Canal = "", DestChannel = "", Error = Ex.Message, Status = "", UniqueId = "" };
                //throw Ex;
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Int32 Estado()
        {
            try
            {
                CentralesIp Datos;
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Datos = db.Operadores.Single(x => x.idOperador == Sesion.Operador.idOperador).Oficinas.CentralesIp;
                }
                CentralIp Central = new CentralIp(Datos.Host, Datos.Port, Datos.Usuario, Datos.Contrasena, Datos.PrefijoSalida);
                Int32 Result = Central.StatusExtension(Sesion.Operador.Extension);
                return Result;
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return -4;
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public String Colgar(String Canal)
        {
            try
            {
                CentralesIp Datos;
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Datos = db.Operadores.Single(x => x.idOperador == Sesion.Operador.idOperador).Oficinas.CentralesIp;
                }
                CentralIp Central = new CentralIp(Datos.Host, Datos.Port, Datos.Usuario, Datos.Contrasena, Datos.PrefijoSalida);
                String Result = Central.Colgar(Canal);
                return Result;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public String Transferir(String Canal, String Extension)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    CentralesIp Datos = db.Operadores.Single(x => x.idOperador == Sesion.Operador.idOperador).Oficinas.CentralesIp;
                    CentralIp Central = new CentralIp(Datos.Host, Datos.Port, Datos.Usuario, Datos.Contrasena, Datos.PrefijoSalida);
                    String Result = Central.Transferir(Canal, Extension);
                    return Result;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptLlamadasResult> Llamadas_rpt(DateTime FechaDesde, DateTime FechaHasta, Int32 idOperador, Boolean Supervisados)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.rptLlamadas(FechaDesde, FechaHasta, idOperador, Supervisados).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Prioridad
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Agregar_Cola_Sup(Int32 idOperador, int[] Valores, int Lugar)
        {

            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    for (int i = 0; i < Valores.Length; i++)
                    {
                        db.InsertarCola(idOperador, Valores[i], Lugar);
                    }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Ranking
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otMetasOperador> Ranking_lst(Int32 idMeta, DateTime Fecha)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DateTime FechaIni = Fecha;
                    DateTime FechaFin = Fecha.AddMonths(1);

                    return db.Metas_Operadores.Where(x => x.idMeta == idMeta && x.FechaFin == null).Select(x => new
                    {
                        idOperador = x.idOperador,
                        Operador = x.Operadores.Nombre, //Operador
                        MetaFija = x.Metas.Metas_Operadores.Single(y => y.idMeta == x.idMeta && y.idOperador == x.idOperador).Ajuste ?? 0,
                        Meta = x.Metas.Metas_Operadores_Cuentas.Where(y => y.idOperador == x.idOperador && y.Fecha == Fecha && y.Activa).Sum(y => (decimal?)y.Meta) ?? 0, //Monto de todas las cuentas 
                        Real = db.Pagos_Cuentas.Where(y => (y.Pagos.Aprobado == true) && (y.idOperador ?? y.Pagos.idOperador) == x.idOperador && (y.Fecha ?? y.Pagos.Fecha) >= FechaIni && (y.Fecha ?? y.Pagos.Fecha) <= FechaFin && x.Metas.Metas_Operadores_Cuentas.Any(w => w.idMeta == x.idMeta && w.Activa && w.idCuenta == y.idCuenta && w.Fecha == Fecha)).Sum(y => (decimal?)y.Monto / y.Cuentas.CambioLocal) ?? 0 // Monto cobrado

                    }).Select(x => new otMetasOperador
                    {
                        idOperador = x.idOperador,
                        idMeta = idMeta,
                        Fecha = Fecha,
                        Operador = x.Operador,
                        MetaFija = x.MetaFija,
                        Meta = x.Meta,
                        Real = x.Real,
                        PorcMetaFija = (x.Real / x.MetaFija * 100),
                        Porc = x.Meta == 0 ? 0 : (x.Real / x.Meta * 100)
                    })
                    .ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otMetasOperador> Ranking_Operador_lst(Int32 idOperador, DateTime Fecha1, DateTime Fecha2)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    DateTime FechaIni = Fecha1;
                    DateTime FechaFin = Fecha2;

                    return db.Metas_Operadores.Where(x => x.idOperador == idOperador && x.FechaFin == null).Select(x => new
                    {
                        idOperador = x.idOperador,
                        Operador = x.Operadores.Nombre, //Operador
                        MetaFija = x.Metas.Metas_Operadores.Single(y => y.idMeta == x.idMeta && y.idOperador == x.idOperador).Ajuste ?? 0,
                        Meta = x.Metas.Metas_Operadores_Cuentas.Where(y => y.idOperador == x.idOperador && y.Fecha.Date >= FechaIni.Date && y.Fecha.Date < FechaFin.Date && y.Activa).Sum(y => (decimal?)y.Meta) ?? 0, //Monto de todas las cuentas 
                        Real = db.Pagos_Cuentas.Where(y => (y.Pagos.Aprobado == true) && (y.idOperador ?? y.Pagos.idOperador) == x.idOperador && (y.Fecha ?? y.Pagos.Fecha) >= FechaIni && (y.Fecha ?? y.Pagos.Fecha) < FechaFin && x.Metas.Metas_Operadores_Cuentas.Any(w => w.idMeta == x.idMeta && w.Activa && w.idCuenta == y.idCuenta && w.Fecha.Date >= FechaIni && w.Fecha.Date < FechaFin)).Sum(y => (decimal?)y.Monto / y.Cuentas.CambioLocal) ?? 0 // Monto cobrado

                    }).Select(x => new otMetasOperador
                    {
                        idOperador = x.idOperador,
                        idMeta = 0,
                        Fecha = Fecha1,
                        Operador = x.Operador,
                        MetaFija = x.MetaFija,
                        Meta = x.Meta,
                        Real = x.Real,
                        PorcMetaFija = (x.Real / x.MetaFija * 100),
                        Porc = x.Meta == 0 ? 0 : (x.Real / x.Meta * 100)
                    })
                    .ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        #endregion
        #region Acciones
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Acciones_ins(String Accion, Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Acciones AccionNew = new Acciones();
                    AccionNew.Accion = Accion;
                    AccionNew.Fecha = DateTime.Now;
                    AccionNew.idOperador = idOperador;
                    db.Acciones.InsertOnSubmit(AccionNew);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Suplentes
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Suplentes_ins(Int32 idOperador, Int32 idOperadorSuplente, DateTime FechaInicio, DateTime? FechaFin)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Suplentes Suplente = new Suplentes() { idOperador = idOperador, idOperadorSuplente = idOperadorSuplente, FechaInicio = FechaInicio, FechaFin = FechaFin == null ? FechaFin : FechaFin.Value.AddHours(23).AddMinutes(59), Cartera = true, Correo = true, Distribucion = true, Gestion = true, Indicadores = true, Reportes = true, Supervision = true };
                    db.Suplentes.InsertOnSubmit(Suplente);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Suplentes_del(Int32 idSuplente)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Suplentes Suplente = db.Suplentes.Single(x => x.idSuplente == idSuplente);
                    Suplente.FechaFin = DateTime.Now.AddMinutes(-1);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Ejecutar

        [OperationContract, WebInvoke(Method = "POST")]
        public DataTable EjecutarComando(String Comando)
        {
            try
            {
                String CS;
                using (CobranzasDataContext db = new CobranzasDataContext()) { CS = db.Connection.ConnectionString; }
                SqlConnection Conn = new SqlConnection(CS);
                SqlCommand Comm = Conn.CreateCommand();
                Comm.CommandText = Comando;
                Comm.CommandType = CommandType.Text;
                DbDataAdapter DAdap = new SqlDataAdapter(Comm);
                DataSet DSet = new DataSet();
                DAdap.Fill(DSet);
                return DSet.Tables[0];
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        #endregion
        #region Cuentas
        [OperationContract, WebInvoke(Method = "POST")]
        public List<rptClienteCuentasResult> ClienteCuentas_rpt(Int32 idCliente)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.rptClienteCuentas(idCliente).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCompromisos_lst> Personas_Compromisos_lst(Int32 idPersona)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Compromisos_Cuentas.Where(x => x.Cuentas.idPersona == idPersona).Select(x => new otCompromisos_lst { Documento = x.Cuentas.Codigo, Fecha = x.Fecha, Total = x.Monto, Creador = x.Compromisos.Operadores.Nombre }).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCuenta> Cuentas_lst(List<Int32> lstCuentas)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Cuentas.Where(x => lstCuentas.Contains(x.idCuenta)).Select(x => otCuenta.FromCuenta(x)).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Pruebas 
        [OperationContract, WebInvoke(Method = "POST")]
        public String PruebasTransferencia(String Data)
        {
            try
            {
                String Result = "";
                if (Data.Length == 0)
                {
                    for (int i = 0; i < 5000; i++) { Result += "**********"; }
                }
                if (Data.StartsWith("B"))
                {
                    using (CobranzasDataContext db = new CobranzasDataContext())
                    {
                        for (int i = 0; i < 500; i++) { Result += "**********"; }
                        Result = db.Responder(Result);
                        Result = "";
                    }
                }
                if (Data == "-")
                {
                    Result = Data;
                }

                return Result;
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        [OperationContract, WebInvoke(Method = "POST")]
        public String PruebasPing()
        {
            try
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted. 
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 3000;
                PingReply reply = pingSender.Send("192.168.210.8", timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("Address: {0}", reply.Address.ToString());
                    Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                    Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                    Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                    Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                }

                switch (reply.Status)
                {
                    case IPStatus.BadDestination: return "Bad Destination";
                    case IPStatus.BadHeader: return "Bad Header";
                    case IPStatus.BadOption: return "Bad Option";
                    case IPStatus.BadRoute: return "Bad Route";
                    case IPStatus.DestinationHostUnreachable: return "DestinationHostUnreachable";
                    case IPStatus.DestinationNetworkUnreachable: return "DestinationNetworkUnreachable";
                    case IPStatus.DestinationPortUnreachable: return "DestinationPortUnreachable";
                    case IPStatus.DestinationProhibited: return "DestinationProhibited";
                    //case IPStatus.DestinationProtocolUnreachable:
                    case IPStatus.DestinationScopeMismatch: return "DestinationScopeMismatch";
                    case IPStatus.DestinationUnreachable: return "DestinationUnreachable";
                    case IPStatus.HardwareError: return "HardwareError";
                    case IPStatus.IcmpError: return "IcmpError";
                    case IPStatus.NoResources: return "NoResources";
                    case IPStatus.PacketTooBig: return "PacketTooBig";
                    case IPStatus.ParameterProblem: return "ParameterProblem";
                    case IPStatus.SourceQuench: return "SourceQuench";
                    case IPStatus.Success: return reply.RoundtripTime.ToString() + " ms";
                    case IPStatus.TimeExceeded: return "TimeExceeded";
                    case IPStatus.TimedOut: return "TimedOut";
                    case IPStatus.TtlExpired: return "TtlExpired";
                    case IPStatus.TtlReassemblyTimeExceeded: return "TtlReassemblyTimeExceeded";
                    case IPStatus.Unknown: return "Unknown";
                    case IPStatus.UnrecognizedNextHeader: return "UnrecognizedNextHeader";
                    default:
                        break;
                }
                return "No se que pasó";
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        #endregion
        #region Status
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Status_sav(otStatus Status)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Status Status_new = db.Status.SingleOrDefault(x => x.idStatus == Status.idStatus);
                    if (Status_new == null)
                    {
                        Status_new = new Entidades.Status();
                        db.Status.InsertOnSubmit(Status_new);
                        Status_new.idTipoCliente = 1;
                        Status_new.idPais = "VEN";
                    }
                    Status_new.Tipo = Status.Tipo;
                    Status_new.Nivel = Status.Nivel;
                    Status_new.Nombre = Status.Nombre;
                    Status_new.Activo = Status.Activo;
                    Status_new.idTipoCliente = Status.idTipoCliente;
                    Status_new.idPais = Status.idPais;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Status_del(Int32 idStatus)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Status Status_new = db.Status.SingleOrDefault(x => x.idStatus == idStatus);
                    if (Status_new == null)
                    {
                        return false;
                    }
                    Status_new.Activo = false;
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Reporte


        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean EnviarReporte(List<otError> Errores, Int32 idOperador)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Correos Correo = new Correos();

                    Correo.Asunto = "Reporte De Errores javascript del usuario:";
                    Correo.Remitente = db.Operadores.Single(x => x.idOperador == idOperador).Correo;
                    Correo.Destinatarios = "alcampos@veconinter.com.ve";
#warning Colocar en algún lado de la configuración
                    Correo.DestinatariosCopia = "";
                    Correo.DestinatariosCopiaOculta = "";
                    Correo.Mensaje = "Errores Javascript: \n" + Errores.Aggregate<otError, String>("", (x, y) => x + y.message + "[" + y.location + ":" + y.line.ToString() + "," + y.col.ToString() + "]\n" + y.callstack + "\n\n");
                    Correo.idOperador = idOperador;
                    Correo.Fecha = DateTime.Now;
                    Correo.FechaCreacion = DateTime.Now;
                    db.Correos.InsertOnSubmit(Correo);
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        #endregion
        #region Bloqueos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<BloqueosResult> Bloqueos()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Bloqueos().ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Novedades
        [OperationContract, WebInvoke(Method = "POST")]
        public otNovedades Novedades(Int32 idOperadorLog, Int32 idOperador, Int32 idOperadorAct, DateTime? Hora)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.CommandTimeout = 10 * 60;
                    if (Sesion.Operador == null)
                    {
                        Entidades.Operadores Operador = db.Operadores.Single(x => x.idOperador == idOperadorLog);
                        Sesion.Llenar(Operador);
                    }

                    otNovedades Result = new otNovedades { Avisos = false, Cartera = false, Correos = false, Hora = DateTime.Now, Pagos = false, Reclamos = false, Reiniciar = false, Colas = false };
                    if (!Hora.HasValue) return Result;
                    Result.Avisos = db.Avisos.Any(x => x.idOperador == idOperador && x.FechaCrea > Hora);
                    if (db.Operadores.Single(x => x.idOperador == idOperador).POP3Password != null)
                    {
                        Result.Correos = db.Correos.Any(x => x.idOperador == idOperador && x.RutaEml != null && x.Fecha > Hora);
                    }
                    else
                    {
                        Result.Correos = null;
                    }
                    Result.Cartera = false;
                    /* db.Cuentas_Operadores.Any(x => x.idOperador == idOperador && (x.FechaInicio > Hora || x.FechaFin > Hora)) ||
                    db.Campanas_Operadores.Any(x => x.idOperador == idOperador && (x.FechaInicio > Hora || x.FechaFin > Hora));*/
                    //Falta identificar si las campañas a las que pertenece que sean de tipo 2 tienen nuevas cuentas, o que se hayan inactivado algunas campañas en las que estaba.
                    Result.Pagos = db.Pagos.Any(x => (x.idOperador == idOperador || x.idOperadorCrea == idOperador) && x.FechaResultado > Hora);
                    Result.Reclamos = db.Reclamos.Any(x => x.idOperador == idOperador && x.FechaResultado > Hora);
                    Result.Colas = db.Colas.Any(x => x.idOperador == idOperador && x.Fecha > Hora);
                    // aqui es para ver si se genero un nuevo eregistro en colas para poder bloquear la seleccion de las carteras
                    //



                    try
                    {
                        WindowsPrincipal Usuario = (WindowsPrincipal)HttpContext.Current.User;
                        String UsuarioWindows = Usuario.Identity.Name;
                        Result.Reiniciar = !Usuario.Identity.IsAuthenticated;
                    }
                    catch { }

                    return Result;
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                return new otNovedades { Avisos = false, Cartera = false, Correos = false, Hora = DateTime.Now, Pagos = false, Reclamos = false, Reiniciar = false };
            }
        }
        #endregion
        #region Cliente
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otClientes> Clientes_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Clientes.Where(x => db.Operadores_Asignaciones.Any(y => y.idPais == x.idPais && y.idOperador == Sesion.idOperador)).Select(x => new otClientes { Nombre = x.Codigo + " - " + x.Nombre + "(" + x.idPais + ")", id = x.idCliente, idPais = x.idPais }).OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Productos
        [OperationContract, WebInvoke(Method = "POST")]
        public List<otCombo> Productos_lst()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    return db.Productos.Where(x => db.Operadores_Asignaciones.Any(y => y.idPais == x.idPais && y.idOperador == Sesion.idOperador)).Select(x => new otCombo { Nombre = x.Nombre + "(" + x.idPais + ")", id = x.idProducto }).OrderBy(x => x.Nombre).ToList();
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Compromisos
        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Compromisos_ins(otCompromisos Compromiso)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Compromisos Compromiso_new = new Entidades.Compromisos();
                    Compromiso_new.Descripcion = Compromiso.Descripcion;
                    Compromiso_new.Fecha = DateTime.Now;
                    Compromiso_new.idPersona = db.Cuentas.Single(x => x.idCuenta == Compromiso.Compromisos_Cuentas.First().idCuenta).idPersona;
                    Compromiso_new.idOperador = Compromiso.idOperador;

                    foreach (otCompromisos_Cuentas CC in Compromiso.Compromisos_Cuentas)
                    {
                        Compromisos_Cuentas CC_New = new Compromisos_Cuentas();
                        CC_New.idCuenta = CC.idCuenta;
                        CC_New.Fecha = CC.Fecha;
                        CC_New.Monto = CC.Monto;
                        Compromiso_new.Compromisos_Cuentas.Add(CC_New);
                    }
                    db.Compromisos.InsertOnSubmit(Compromiso_new);
                    db.SubmitChanges();

                    Int32 Status = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "_STCompromiso").Valor);
                    Gestiones Gestion = new Gestiones();
                    Gestion.Descripcion = Compromiso.Descripcion;
                    Gestion.Fecha = DateTime.Now;
                    Gestion.idOperador = Compromiso.idOperador;
                    Gestion.idPersona = Compromiso_new.idPersona;
                    Gestion.idStatus = Status;
                    foreach (Int32 idCuenta in Compromiso.Compromisos_Cuentas.Select(x => x.idCuenta).Distinct())
                    {
                        Cuentas_Gestiones CG = new Cuentas_Gestiones();
                        CG.idCuenta = idCuenta;
                        Gestion.Cuentas_Gestiones.Add(CG);
                    }
                    db.Gestiones.InsertOnSubmit(Gestion);
                    db.SubmitChanges();

                    // return true;

                }
                return true;
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }
        #endregion
        #region Eliminar_Proceso

        [OperationContract, WebInvoke(Method = "POST")]
        public Boolean Matar(Int32 pid)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    db.Matar(pid);
                    return true;
                }
            }
            catch (Exception Ex)
            {
                throw Ex.Informar();
            }
        }


        #endregion
        /*
   [OperationContract, WebInvoke(Method = "POST")]
   public List<????> ???_lst(????)
   {
       try
       {
           using (CobranzasDataContext db = new CobranzasDataContext())
           { 
           }
       }
       catch (Exception Ex)
       {
           throw Ex.Informar();
       }
   }
   */

    }
}