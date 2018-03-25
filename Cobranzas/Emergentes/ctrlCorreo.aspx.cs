using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Cobranzas
{
    public partial class ctrlCorreo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Seguridad.Ejecutar(PrepararCorreo);
            }
        }
        protected void PrepararCorreo()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    idOperador.Value = Request["idOperador"];//Sesion.idOperador.ToString();
                    int id = Convert.ToInt32(Request["idCorreo"]);
                    int Tipo = Convert.ToInt32(Request["Tipo"]);
                    idCorreo.Value = id.ToString();
                    Entidades.Correos Correo = db.Correos.Single(x => x.idCorreo == id);
                    try
                    {
                        List<Entidades.Correos> CorreoAnterior;
                        List<Entidades.Correos> CorreoSiguiente;
                        if (Tipo == 1)
                        {
                            CorreoSiguiente = db.Correos.Where(x => x.idOperador == Correo.idOperador && x.FechaCreacion < Correo.FechaCreacion && x.TipoEspecial == null && !x.Correos_Personas.Any() && x.RutaEml != null).OrderByDescending(x => x.FechaCreacion).Take(1).ToList();
                            CorreoAnterior = db.Correos.Where(x => x.idOperador == Correo.idOperador && x.FechaCreacion > Correo.FechaCreacion && x.TipoEspecial == null && !x.Correos_Personas.Any() && x.RutaEml != null).OrderBy(x => x.FechaCreacion).Take(1).ToList();
                        }
                        else if (Tipo == 2)
                        {
                            CorreoSiguiente = db.Correos.Where(x => x.idOperador == Correo.idOperador && x.FechaCreacion < Correo.FechaCreacion && x.Correos_Personas.Any() && x.RutaEml != null && !x.Leido).OrderByDescending(x => x.FechaCreacion).Take(1).ToList();
                            CorreoAnterior = db.Correos.Where(x => x.idOperador == Correo.idOperador && x.FechaCreacion > Correo.FechaCreacion && x.Correos_Personas.Any() && x.RutaEml != null && !x.Leido).OrderBy(x => x.FechaCreacion).Take(1).ToList();
                        }
                        else
                        {
                            CorreoSiguiente = new List<Entidades.Correos>();
                            CorreoAnterior = new List<Entidades.Correos>();
                        }

                        if (CorreoSiguiente.Count == 0)
                        {
                            lnkSiguiente.Visible = false;
                        }
                        else
                        {
                            lnkSiguiente.HRef = "ctrlCorreo.aspx?idCorreo=" + CorreoSiguiente.First().idCorreo + "&idOperador=" + idOperador.Value + "&Tipo=" + Tipo;
                        }
                        if (CorreoAnterior.Count == 0)
                        {
                            lnkAnterior.Visible = false;
                        }
                        else
                        {
                            lnkAnterior.HRef = "ctrlCorreo.aspx?idCorreo=" + CorreoAnterior.First().idCorreo + "&idOperador=" + idOperador.Value + "&Tipo=" + Tipo;
                        }
                    }
                    catch { }
                    Correo.Leido = true;
                    db.SubmitChanges();
                    lblAsunto.Text = Correo.Asunto;
                    lblDestinatarios.Text = Server.HtmlEncode(Correo.Destinatarios);
                    lblDestinatariosCopia.Text = Server.HtmlEncode(Correo.DestinatariosCopia);
                    lblDestinatariosCopiaOculta.Text = Server.HtmlEncode(Correo.DestinatariosCopiaOculta);
                    lblFecha.Text = Correo.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss tt");
                    String Mensaje = "";
                    //Boolean isHTML = true;
                    if (Correo.Mensaje != null)
                    {
                        Mensaje = Correo.Mensaje;

                    }
                    if (Correo.RutaEml != null)
                    {
                        List<String> NoAdjuntos = new List<string>();
                        String Ruta = db.Parametros.Single(x => x.Clave == "RutaCorreos").Valor;
                        OpenPop.Mime.Message m = OpenPop.Mime.Message.Load(new System.IO.FileInfo(Ruta + Correo.RutaEml + ".eml"));
                        OpenPop.Mime.MessagePart Parte = m.FindFirstHtmlVersion();
                        if (Parte == null) Parte = m.FindFirstPlainTextVersion();
                        Mensaje = Parte.GetBodyAsText();
                        //isHTML = !m.MessagePart.IsText;
                        if (Parte.ContentType.MediaType.ToLower() == "text/plain") Mensaje = "<pre>" + Mensaje + "</pre>";
                        try
                        {
                            foreach (Match Match in Regex.Matches(Mensaje, "\"(cid:(\\S+))\""))
                            {
                                //Debug.Print(Match.ToString());
                                String ID = Match.Groups[2].Value;
                                NoAdjuntos.Add(ID);
                                try
                                {
                                    OpenPop.Mime.MessagePart Parte2 = BuscarParte(ID, m.MessagePart);
                                    String Ext = Parte2.FileName;
                                    try
                                    {
                                        Ext = Ext.Substring(Ext.LastIndexOf("."));
                                    }
                                    catch
                                    {
                                        Ext = "";
                                    }
                                    String Ruta2 = Guardar(Parte2, id.ToString());
                                    //Parte2.Save(new FileInfo(MapPath(Ruta2)));
                                    Mensaje = Mensaje.Replace(Match.Groups[0].Value, "\"" + Ruta2 + "\"");
                                }
                                catch { }

                            }
                            foreach (OpenPop.Mime.MessagePart Adjunto in m.FindAllAttachments())
                            {
                                if (NoAdjuntos.Contains(Adjunto.ContentId)) continue;
                                String Ruta2 = Guardar(Adjunto, id.ToString());
                                lblAdjuntos.Text += String.Format(@"</span><a href=""{0}"" title=""{2}"" target=""_blank"" class=""AdjuntoSL"">{1}</a><span>", Ruta2, Adjunto.ContentDisposition.FileName??"Adjunto", Adjunto.ContentDescription);
                            }
                        }
                        catch { }
                    }
                    //lblAdjuntos.Text=@"</span><a href=""http://www.google.com"" target=""_blank"" class=""Adjunto"">aquí</a><span>";
                    lblRemitente.Text = Correo.Remitente;
                    try
                    {
                        String RutaCorreo = (@"/Temp/Correo" + id.ToString() + (true ? ".html" : ".txt"));
                        iframe.Attributes["src"] = RutaCorreo;
                        File.WriteAllText(MapPath(RutaCorreo), Mensaje, System.Text.Encoding.UTF8);
                    }
                    catch { }
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
            }
        }

        protected String Guardar(OpenPop.Mime.MessagePart Parte, String Id)
        {
            String Dir = @"/Temp/Correo" + Id + "/";
            String Ruta2;
            try
            {
                Ruta2 = Dir + General.ArreglarNombre(Parte.ContentDisposition.FileName);
            }
            catch
            {
                Ruta2 = Dir + "Temp" + Parte.ContentId;
            }
            if (!Directory.Exists(MapPath(Dir)))
            {
                Directory.CreateDirectory(MapPath(Dir));
            }
            Parte.Save(new FileInfo(MapPath(Ruta2)));
            return Ruta2;
        }
        protected OpenPop.Mime.MessagePart BuscarParte(string Id, OpenPop.Mime.MessagePart M)
        {
            if (M.ContentId == Id) return M;
            if (M.MessageParts != null)
            {
                foreach (OpenPop.Mime.MessagePart Parte in M.MessageParts)
                {
                    OpenPop.Mime.MessagePart Result = BuscarParte(Id, Parte);
                    if (Result != null) return Result;
                }
            }
            return null;
        }
    }
}
