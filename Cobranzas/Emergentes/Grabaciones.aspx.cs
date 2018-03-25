using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using Entidades;

namespace Cobranzas.Emergentes
{
    public partial class Grabaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String Ruta = "";
                String Grabacion = "";
                Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                Grabacion = Request["Grabacion"];
                Int32 idLlamada = Convert.ToInt32(Request["idLlamada"]);
                if (Request["Grabacion"] != null) {
                    Grabacion = db.Llamadas.SingleOrDefault(x => x.idLlamada == Convert.ToInt32(Grabacion)).Grabacion;
                }
                if (Request["idLlamada"] != null)
                {
                    Grabacion = db.Llamadas.SingleOrDefault(x => x.idLlamada == idLlamada).Grabacion;
                }

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("veconinter53", "V3c0n1nt3r");
                    try
                    {
                        client.DownloadFile("ftp://172.17.1.102/" + Grabacion + ".gsm", Ruta + Grabacion + ".gsm");
                    }
                    catch (Exception Ex)
                    {
                        Response.Write("La grabación especificada no se encuentra en el servidor");
                    }
                }
                try
                {
                    Response.WriteFile(Ruta + Grabacion + ".gsm");
                    Response.AddHeader("Content-disposition", "attachment;FileName=\"Llamada.gsm\"");
                    Response.AddHeader("content-type", "audio/gsm");
                }
                catch { 
                
                }
                Response.End();
            }
        }
    }
}