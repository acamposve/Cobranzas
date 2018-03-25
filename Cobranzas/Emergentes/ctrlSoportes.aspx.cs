using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class ctrlSoportes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Seguridad.Ejecutar(BajarArchivo);
        }
        public void BajarArchivo()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    //String RutaTemporales = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                    String Archivo = "";
                    //String NombreArchivo = "";
                    if (Request["idSoporte"] != null)
                    {
                        Entidades.Soportes Soporte = db.Soportes.Single(x => x.idSoporte == Convert.ToInt32(Request["idSoporte"]));
                        String Ubicacion = Soporte.Ubicacion;
                        Archivo = Ubicacion + Request["Archivo"] ?? "";
                    }
                    if (Request["idCuenta"] != null)
                    {
                        Entidades.Cuentas Cuenta = db.Cuentas.Single(x => x.idCuenta == Convert.ToInt32(Request["idCuenta"]));
                        Archivo = Cuenta.Ruta;
                    }
                    if (Request["idGestion"] != null)
                    {
                        Response.Write("<!DOCTYPE html><html><head><link href='/Estilos/Estilos.css?ver=9' rel='stylesheet' type='text/css' /></head><body><h1>Soportes para la Gestión:</h1>");
                        foreach (Entidades.Soportes Soporte in db.Soportes.Where(x => x.idTabla == Convert.ToInt32(Request["idGestion"]) && x.Tabla == "Gestiones"))
                        {
                            Response.Write("<a class='Telefono' href='/Emergentes/ctrlSoportes.aspx?idSoporte=" + Soporte.idSoporte + "'>" + Soporte.Nombre + "</a><br>");
                        }

                        Response.Write("</body></html>");
                        Response.Flush();
                        Response.End();
                        return;
                    }

                    try
                    {
                        Negocios.BajarArchivo(Archivo, Response);
                    }
                    catch { }
                    //if (Archivo.IndexOf("\\") == -1)// URL
                    //{
                    //    NombreArchivo = Archivo.Substring(Archivo.LastIndexOf("//") + 1);
                    //    String Ruta2 = RutaTemporales + DateTime.Now.ToString("yyyyMMddHHmmss") + NombreArchivo;

                    //    Response.Redirect(Archivo, true);
                    //    return;
                    //}
                    //else { //UNC
                    //    NombreArchivo = Archivo.Substring(Archivo.LastIndexOf("\\") + 1);
                    //}
                    //if (Archivo.ToLower().EndsWith(".pdf")) Response.ContentType = "application/pdf";
                    //if (Archivo.ToLower().EndsWith(".jpg")) Response.ContentType = "image/jpeg";
                    //if (Archivo.ToLower().EndsWith(".gif")) Response.ContentType = "image/gif";
                    //Response.AddHeader("content-disposition", "inline;filename='" + NombreArchivo + "'");
                    ////application/octet-stream
                    //Response.WriteFile(Archivo, true);

                    //Response.End();
                }
            }
            catch (Exception Ex)
            {
                //Ex.Registrar();
            }
        }
    }
}