using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Entidades;

namespace Cobranzas.Emergentes
{
    public partial class ctrlEnvioCorreo : System.Web.UI.Page
    {
        int idPersona;
        Entidades.Personas Persona;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    pnlOriginal.Visible = false; 
                    //try
                    //{
                    //    txtMensaje.InnerText = "<br/><br/><analisis>" + db.FirmaOperador(Convert.ToInt32(Request["idOperador"]));
                    //}
                    //catch { }
                    if (Request["idPersona"] != null)
                    {
                        idPersona = Convert.ToInt32(Request["idPersona"]);
                        Persona = db.Personas.Single(x => x.idPersona == idPersona);
                        //txtCorreoPara.Value = Persona.Email;
                        //txtCorreoCC.Value = String.Join(",", Persona.PersonasContacto.Select(x => x.Email));
                        //txtCorreoAsunto.Value = "Análisis de Antigüedad al " + DateTime.Now.AFechaMuyCorta() + " - Veconinter - (" + Persona.Codigo + ")" + Persona.Nombre;
                        chkAnalisisAntiguedad.Checked = true;
                        //Seguridad.Ejecutar(CrearAdjuntos); 
                    }
                    else {
                        chkAnalisisAntiguedad.Visible = false;
                    }
                    if (Request["idCorreo"] != null)
                    {
                        chkAnalisisAntiguedad.Checked = false;
                        int idCorreo = Convert.ToInt32(Request["idCorreo"]);
                        Entidades.Correos Correo = db.Correos.Single(x => x.idCorreo == idCorreo);
                        String Encabezado = "<hr/>";
                        Encabezado += "Correo Original Enviado el:" + Correo.FechaCreacion + "<br/>";
                        Encabezado += "De: " + Correo.Remitente + "<br/>";
                        Encabezado += "Para: " + Correo.Destinatarios + "<br/>";
                        Encabezado += "CC: " + Correo.DestinatariosCopia + "<br/>";
                        Encabezado += "Asunto: " + Correo.Asunto + "<br/>";
                        Encabezado += "<br/>";
                        txtMensaje.InnerText += Encabezado;
                        if (Request["Accion"] != "Reenviar")
                        {
                            txtCorreoPara.Value = Correo.Remitente;
                            txtCorreoAsunto.Value = Correo.Asunto.StartsWith("Re:", StringComparison.InvariantCultureIgnoreCase) ? Correo.Asunto : "Re:" + Correo.Asunto;
                        }
                        else
                        {
                            txtCorreoAsunto.Value = Correo.Asunto.StartsWith("Rv:", StringComparison.InvariantCultureIgnoreCase) ? Correo.Asunto : "Rv:" + Correo.Asunto;
                        }
                        if (Correo.RutaEml == null)
                        {
                            //txtMensaje.InnerText += Correo.Mensaje;
                        }
                        if (Request["Accion"] == "ResponderTodos")
                        {
                            txtCorreoCC.Value = Correo.Destinatarios + ";" + Correo.DestinatariosCopia;
                            while (txtCorreoCC.Value.StartsWith(";")) { txtCorreoCC.Value = txtCorreoCC.Value.Substring(1); }
                        }
                        String RutaCorreo = (@"/Temp/Correo" + idCorreo + (true ? ".html" : ".txt"));
                        iframe.Attributes.Add("src", RutaCorreo);
                        pnlOriginal.Visible = true;
                    }

                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
            }
        }
        /*
        void CrearAdjuntos()
        {
            //Documentos de la Persona
            if (Persona.Soportes.Count > 0)
            {
                foreach (Entidades.Soportes Soporte in Persona.Soportes)
                {
                    pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Sop{0}'>{1}<br/>", Soporte.idSoporte, Soporte.Nombre);
                }
            }
            if (Request["Cuentas"] != null)
            {
                foreach (String idCuenta in Request["Cuentas"].Split(','))
                {
                    Entidades.Cuentas Cuenta = General.db.Cuentas.SingleOrDefault(x => x.idCuenta == Convert.ToInt32(idCuenta));
                    if (Cuenta == null) continue;
                    pnlDocumentos.InnerHtml += String.Format("<h3>Cuenta {0}</h3>", Cuenta.Codigo);
                    Int16 Soportes = 0;
                    if (Cuenta.Ruta != null)
                    {
                        Soportes++;
                        pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Doc{0}'>{1}<br/>", Cuenta.idCuenta, "Documento");
                    }
                    foreach (Entidades.Soportes Soporte in Cuenta.Soportes)
                    {
                        if (Soporte.Ubicacion.EndsWith("\\"))
                        {
                            foreach (String Archivo in Directory.EnumerateFiles(Soporte.Ubicacion))
                            {
                                String NombreArchivo = Archivo.Substring(Archivo.LastIndexOf("\\") + 1);
                                pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Sop{0}_{1}'>{1}<br/>", Soporte.idSoporte, NombreArchivo);
                            }
                        }
                        else
                        {
                            if (File.Exists(Soporte.Ubicacion))
                            {
                                pnlDocumentos.InnerHtml += String.Format("<input type='checkbox' class='adjunto' id='Sop{0}'>{1}<br/>", Soporte.idSoporte, Soporte.Nombre);
                            }
                        }
                    }
                }
            }

        }
        //*/
    }
}