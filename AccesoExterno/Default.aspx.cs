using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
namespace Cobranzas
{
    public partial class Default : System.Web.UI.Page
    {
        String Tipo;
        String Id;
        String Val;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["Tipo"] == null || Request["Id"] == null || Request["Val"] == null)
            {
                Response.Write("La dirección es incorrecta, no ha seleccionado ningún Archivo para visualizar");
                Response.Flush();
                return;
            }
            Tipo = General.Desencriptar(Request["Tipo"].Replace(" ", "+"));
            Id = Request["Id"];
            Val = General.Desencriptar(Request["Val"].Replace(" ", "+"));
            switch (Tipo)
            {
                case "CuentaSoporte":
                    try
                    {
                        Seguridad.Ejecutar(CuentaSoporte);
                    }
                    catch { }
                    break;
                case "PagoSoporte":
                    try
                    {
                        Seguridad.Ejecutar(PagoSoporte);
                    }
                    catch { }
                    break;
                default:
                    Response.Write("No ha seleccionado un Archivo correcto");
                    break;
            }
        }
        protected void CuentaSoporte()
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Cuentas Cuenta = db.Cuentas.Single(x => x.idCuenta == Convert.ToInt32(Id));
                if (Convert.ToInt32(Val) != Cuenta.idPersona)
                {
                    Response.Write("No está autorizado para ver este Soporte");
                    return;
                }
                if (Cuenta.Ruta == null)
                {
                    Response.Write("El archivo que ha seleccionado no está disponible en este momento, favor intente más tarde.");
                    return;
                }
                if (Cuenta.Personas.idPais == "USA" && Cuenta.Soportes.Any(x => x.Codigo == "") && Request["Original"] == null)
                {
                    Response.Write("<html><head></head><body><table style='width:100%; height:100%;'><tr>"+
                        "<td style='width:50%'><iframe height='100%' width='100%' seamless src='" + Request.Url + "&Original=1'>Outstanding Debt Notice</iframe></td>" +
                        "<td style='width:50%'><iframe height='100%' width='100%' seamless src='" + Request.Url + "&Original=2'>Invoice</iframe></td>" +
                        "</tr></table></body></html>");
                    return;
                }
                String Ruta = "";
                if (Request["Original"] == "2")
                {
                    Ruta = Cuenta.Soportes.First(x => x.Codigo == "").Ubicacion;
                }
                else
                {
                    Ruta = Cuenta.Ruta;
                }
                try //trata de guardar la gestión
                {
                    Entidades.Gestiones Gestion = new Entidades.Gestiones();
                    Gestion.idOperador = 1;
                    Gestion.Fecha = DateTime.Now;
                    Gestion.Descripcion = "La persona ha visto el soporte de la factura";
                    Gestion.idPersona = Cuenta.idPersona;
                    Gestion.idStatus = 29;
                    Gestion.Cuentas_Gestiones.Add(new Entidades.Cuentas_Gestiones { idCuenta = Cuenta.idCuenta });
                    db.Gestiones.InsertOnSubmit(Gestion);
                    db.SubmitChanges();
                }
                catch { }
                BajarArchivo(Ruta, Response);
                return;
            }
        }
        protected void PagoSoporte()
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Soportes Soporte = db.Soportes.Single(x => x.idSoporte == Convert.ToInt32(Id));
                if (Convert.ToInt32(Val) != Soporte.idTabla)
                {
                    Response.Write("No está autorizado para ver este Soporte");
                    return;
                }
                if (Soporte.Ubicacion == null)
                {
                    Response.Write("El archivo que ha seleccionado no está disponible en este momento, favor intente más tarde.");
                    return;
                }
                String Ruta = Soporte.Ubicacion;
                BajarArchivo(Ruta, Response);
                return;
            }
        }
        public static void BajarArchivo(String Ruta, HttpResponse Response)
        {
            String Disposition = "attachment";
            if (Ruta.ToLower().EndsWith(".pdf")) { Response.ContentType = "application/pdf"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".jpg")) { Response.ContentType = "image/jpeg"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".gif")) { Response.ContentType = "image/gif"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".bmp")) { Response.ContentType = "image/bmp"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".png")) { Response.ContentType = "image/png"; Disposition = "inline"; }
            if (Ruta.ToLower().EndsWith(".msg")) { Response.ContentType = "application/vnd.ms-outlook"; Disposition = "attachment"; }

            String NombreArchivo = "";
            String Archivo = "";
            if (Ruta.IndexOf("\\") != -1)//Ruta UNC
            {
                Archivo = Ruta;
                NombreArchivo = Ruta.Substring(Ruta.LastIndexOf("\\") + 1);
            }
            else//URL
            {
                NombreArchivo = Ruta.Substring(Ruta.LastIndexOf("/") + 1);
                String Ruta2 = "";
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Ruta2 = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                }
                Archivo = Ruta2 + DateTime.Now.ToString("yyyyMMddHHmmssfff") + NombreArchivo;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(Ruta, Archivo);
                }
            }
            Response.Clear();
            Response.AddHeader("content-disposition", Disposition + ";filename=\"" + NombreArchivo + "\"");
            Response.WriteFile(Archivo);
            Response.Flush();
            Response.End();
        }
    }
}