using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.OT;

using Entidades;

namespace Cobranzas
{
    public partial class Pruebas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            wsCobranzas ws = new wsCobranzas();
            //ws.Avisos_Persona_lst(390);
            Array msg = ws.Avisos_Persona_lst(390).ToArray();
            //Expo(msg, "otPlantillas_lst");
            Expo(msg);
        }

        private void Expo(Array msg)
        {
            Response.Write(msg);
            var registros = msg.GetLength(0);
            //var columnas = msg.;
            //columnas = columnas / registros;
            //throw new NotImplementedException();
        }

        //protected void Exportar(object sender, EventArgs e)
        //{
        //    Response.Clear();

        //    wsCobranzas ws = new wsCobranzas();
        //    List<Int32> Status;
        //    if (idStatus.Value == "")
        //    {
        //        Status = new List<int>();
        //    }
        //    else
        //    {
        //        Status = idStatus.Value.Split(',').Select(x => Convert.ToInt32(x)).ToList();
        //    }

        //    List<otHistorialGestion> Gestiones = ws.Gestiones_rpt(Convert.ToDateTime(dtpFechaDesde.Value), Convert.ToDateTime(dtpFechaHasta.Value), Convert.ToInt32(idOperador.Value), Request["Supervisados"] != "0", Convert.ToInt32("0" + idCliente.Value), Convert.ToInt32("0" + idPersona.Value), Status);
        //    Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());

        //    Response.Write(Resources.Recursos.Date + ";" +
        //                   Resources.Recursos.Time + ";" +
        //                   Resources.Recursos.Status + ";" +
        //                   Resources.Recursos.Type + ";" +
        //                   Resources.Recursos.Code + ";" +
        //                   Resources.Recursos.Person + ";" +
        //                   Resources.Recursos.Description + ";" +
        //                   Resources.Recursos.Operator + "\r\n");

        //    foreach (otHistorialGestion Gestion in Gestiones)
        //    {
        //        Response.Write(
        //            "\"" + Gestion.Fecha.AFechaMuyCorta() + "\";" +
        //            "\"" + Gestion.Fecha.ToString("hh:mm tt") + "\";" +
        //            "\"" + Gestion.Status + "\";" +
        //            "\"" + Gestion.Tipo + "\";" +
        //            "\"" + Gestion.Codigo + "\";" +
        //            "\"" + Gestion.Persona + "\";" +
        //            "\"" + Gestion.Descripcion.Replace("\"", "\"\"") + "\";" +
        //            "\"" + Gestion.Operador + "\"\r\n"
        //            );
        //    }
        //    Response.AddHeader("Content-disposition", "attachment;FileName=\"Gestiones.csv\"");
        //    Response.AddHeader("content-type", "text/csv");
        //    Response.End();
        //}

        protected void Button1_Click(object sender, EventArgs e)
        {
            //new CentralIp().Llamar(TextBox1.Text, TextBox2.Text);
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("veconinter53", "V3c0n1nt3r");
                    try
                    {
                        client.DownloadFile("ftp://172.17.1.102/" + txtGrabacion.Text , Ruta + txtGrabacion.Text );
                    }
                    catch (Exception Ex)
                    {
                        Response.Write("La grabación especificada no se encuentra en el servidor");
                        return;
                    }
                }
                Response.WriteFile(Ruta + txtGrabacion.Text );
                Response.AddHeader("Content-disposition", "attachment;FileName=\"Llamada.gsm\"");
                Response.AddHeader("content-type", "audio/gsm");
            }
        }
    }
}