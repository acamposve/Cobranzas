using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.Reflection;

namespace Cobranzas
{
    public class Campos
    {
        public String Campo;
        public String Campo2;
        public String Prefijo;
        public String Sufijo;
        public String Titulo;
        public String ValorNull;
    };
    public static class UI
    {
        public static void Mensaje(string titulo, string msg, string url, System.Web.UI.Page page)
        {
            ScriptManager.RegisterStartupScript(page, typeof(Page), "Inicial", "$(document).ready(function () {Mensaje({titulo:'" + titulo + "',mensaje:'" + msg + "',url:'" + url + "'});})", true);
        }
        public static Exception Informar(this Exception Ex)
        {
            if (Ex is CobranzasException) return Ex;
            return new Exception("Error #" + Ex.Registrar().ToString() + ", Se registraron automáticamente los detalles de este error para poderlo solventar mejor.", new Exception(Ex.Serializar()));
        }
        public static int Registrar(this Exception Ex)
        {
            //if (!EventLog.SourceExists("Cobranzas"))
            //EventLog.CreateEventSource("Cobranzas", "Application");
            String Evento = Ex.Serializar();
            try
            {
                EventLog.WriteEntry("Application", Evento, EventLogEntryType.Error);
            }
            catch { }
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    var Error = new Cobranzas.Entidades.Errores { Error = Evento, Fecha = DateTime.Now, Usuario = Cobranzas.Sesion.Operador.Login };
                    db.Errores.InsertOnSubmit(Error);
                    db.SubmitChanges(System.Data.Linq.ConflictMode.ContinueOnConflict);
                    return Error.idError;
                }
            }
            catch { }
            return 0;
        }
        public static string Serializar(this Exception Ex)
        {
            if (Ex == null) return "";
            String Evento = "";
            Evento = String.Format("Mensaje: {0}\nDatos: {1}\nError interno: [{2}]\nOrigen:{3}\nSeguimiento de Pila: [{4}]\nDestino:{5}\nUsuario:{6}", Ex.Message, Ex.Data, Ex.InnerException.Serializar(), Ex.Source, Ex.StackTrace, Ex.TargetSite, Cobranzas.Sesion.NombreOperador);
            return Evento;
        }

        public static string DescargarLista<type>(List<type> ObjetoClase)
        {
            string Resultado = "<table>";
            try
            {
                string Objeto = typeof(type).Name;

                FieldInfo[] info = typeof(type).GetFields();

                //foreach (FieldInfo item in info)
                //{
                //    resultado += item.Name + " " + item.FieldType+"<br/>";
                //}

                Resultado += "<tr>";
                foreach (FieldInfo item in info)
                {
                    Resultado += "<th>" + item.Name + "</th>";
                }
                Resultado += "</tr>";

                foreach (type valores in ObjetoClase)
                {
                    Resultado += "<tr>";
                    foreach (FieldInfo item in info)
                    {
                        try
                        {
                            Resultado += "<td>" + General.LeerValorCampo(valores, item.Name).ToString() + "</td>";
                        }
                        catch (Exception x)
                        {
                            Resultado += "<td> &nbsp; </td>";

                        }
                    }
                    Resultado += "</tr>";
                }
                Resultado += "</table>";
            }
            catch (Exception ex)
            {
                throw;
            }
            return Resultado;
        }

        public static string DescargaFormateado<type>(String Titulo, List<type> ObjetoClase, List<Campos> Atributos)
        {
            //Ejemplo de uso
            //wsCobranzas ws = new wsCobranzas();
            //List<Campos> Datos = new List<Campos>();
            //Datos.Add(new Campos { Campo = "Aviso", Titulo = "Avisos", Prefijo ="prueba"});
            //Datos.Add(new Campos { Campo = "FechaAviso", Titulo = "Fecha de Aviso", Sufijo ="201555"});
            //Datos.Add(new Campos { Campo = "FechaCancelado", Titulo = "Fecha de Cancelacion", ValorNull = "nada" });
            //Response.Clear();
            //Response.Write(UI.DescargaFormateado("Avisos",ws.Avisos_Persona_lst(390), Datos));
            //Response.AddHeader("Content-disposition", "attachment;FileName=\"ClienteCuentas.xls\"");
            //Response.AddHeader("content-type", "application/xls");
            //Response.End();

            string Resultado = @"<!DOCTYPE html>" +
                "<html><head>" +
                "<style>" +
                ".enc {background-color: #000040;color:White;}" +
                ".norm{ background-color: #FFFFFF;}" +
                ".alt{background-color: #CCCCFF;}" +
                ".celda{border: 1px solid #000040;}" +
                ".monto{text-align:rigth;}" +
                "body{font-family: Verdana,Arial; font-size:small !important}" +
                "</style>" +
                "</head><body>" +
                "<h1>"+Titulo+"</h1>" +
                "<table class='tabla'>";
            try
            {
                string Objeto = typeof(type).Name;

                FieldInfo[] info = typeof(type).GetFields();

                Resultado += "<tr>";
                foreach (Campos item in Atributos)
                {
                    Resultado += "<th>" + Recursos.Recurso(item.Titulo) + "</th>";
                }
                Resultado += "</tr>";

                foreach (type valores in ObjetoClase)
                {
                    Resultado += "<tr>";
                    foreach (Campos item in Atributos)
                    {
                        try
                        {
                            if (String.IsNullOrEmpty(item.Campo2))
                            {
                                Resultado += "<td>" + item.Prefijo + General.LeerValorCampo(valores, item.Campo).ToString() + item.Sufijo + "</td>";
                            }
                            else
                            {
                                Resultado += "<td>" + item.Prefijo + General.LeerValorCampo(valores, item.Campo).ToString() + " " + General.LeerValorCampo(valores, item.Campo2).ToString() + item.Sufijo + "</td>";
                            }

                        }
                        catch (Exception x)
                        {
                            if (String.IsNullOrEmpty(item.ValorNull))
                            {
                                Resultado += "<td> &nbsp; </td>";
                            }
                            else
                            {
                                Resultado += "<td>" + item.ValorNull + "</td>";
                            }

                        }
                    }
                    Resultado += "</tr>";
                }
                Resultado += "</table></body></html>";
            }
            catch (Exception ex)
            {
                throw;
            }
            return Resultado;
        }
    }
}