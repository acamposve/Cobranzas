using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.Principal;
using System.ServiceModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;
using Entidades;

namespace Cobranzas.Emergentes
{
    public partial class ctrlCuenta : System.Web.UI.Page
    {
        String Externo;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Externo = db.Parametros.Single(x => x.Clave == "Externos").Valor;
                    idCuenta = Convert.ToInt32(Request["idCuenta"]);
                    Cuenta = db.Cuentas.SingleOrDefault(x => x.idCuenta == idCuenta);
                    if (!IsPostBack)
                    {
                        if (Cuenta.Datos != null)
                        {
                            String Tabla = "<table class='TablaDatos'>";
                            foreach (var nodo in Cuenta.Datos.Elements())
                            {
                                Tabla += "<tr>";
                                Tabla += "<th>" + Recursos.Recurso(nodo.Attribute("Clave").Value) + "</th>";
                                Tabla += "<td>" + nodo.Value + "</td>";
                                Tabla += "</tr>";
                            }
                            Tabla += "</table>";
                            pnlDatos.InnerHtml = Tabla;
                        }
                        Seguridad.Ejecutar(ColocarSoportes);
                        foreach (Entidades.TiposReporte TR in db.TiposReporte.Where(x => x.Cuenta && x.idTipoCliente == Cuenta.Clientes.idTipoCliente && x.idPais==Cuenta.Personas.idPais))
                        {
                            pnlReportes.InnerHtml += @"<div class='btnReporte' onclick='location.href=""/Emergentes/ctrlReporte.aspx?idTipoReporte=" + TR.idTipoReporte + "&idCuenta=" + Cuenta.idCuenta + @"""'>" + TR.Nombre + "</div>";
                        }
                    }

                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
            }
        }
        Entidades.Cuentas Cuenta;
        Int32 idCuenta;
        public void ColocarSoportes()
        {
            if (Cuenta.Ruta != null)
            {
                String Tipo = General.Encriptar("CuentaSoporte");
                String Val = General.Encriptar(Cuenta.idPersona.ToString());
                cont.InnerHtml = "<a class='Soporte' href='" + Externo + "?Tipo=" + Tipo + "&Val=" + Val + "&Id=" + Cuenta.idCuenta.ToString() + "' id='Documento'>Documento</a>";
            }

            foreach (Entidades.Soportes Soporte in Cuenta.Soportes)
            {
                if (Soporte.Ubicacion.IndexOf("\\") != -1)//UNC
                {
                    if (Soporte.Ubicacion.EndsWith("\\")) // Es un directorio=>Mostrar Todos los archivos en el
                    {
                        String Directorio = Soporte.Ubicacion;
                        foreach (String Archivo in Directory.GetFiles(Directorio, "*.*"))
                        {
                            String Archivo2 = Archivo.Substring(Archivo.LastIndexOf("\\") + 1);
                            String Control = @"<a class='Soporte' href='/Emergentes/ctrlSoportes.aspx?idSoporte=" + Soporte.idSoporte.ToString() + "&Archivo=" + Archivo2 + "'>" + Archivo2 + "</a>";
                            cont.InnerHtml += Control;
                        }
                    }
                    else//Es un archivo directamente
                    {
                        if (File.Exists(Soporte.Ubicacion))
                        {
                            String Control = @"<a class='Soporte' href='/Emergentes/ctrlSoportes.aspx?idSoporte=" + Soporte.idSoporte.ToString() + "'>" + Soporte.Nombre + "</a>";
                            cont.InnerHtml += Control;
                        }
                    }
                }
                else //URL
                {
                    String Control = @"<a class='Soporte' href='/Emergentes/ctrlSoportes.aspx?idSoporte=" + Soporte.idSoporte.ToString() + "'>" + Soporte.Nombre + "</a>";
                    cont.InnerHtml += Control;
                }
            }

        }
    }
}