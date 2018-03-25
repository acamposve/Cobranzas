using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.OT;

namespace Cobranzas.Emergentes
{
    public partial class rptAvisos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            List<otAviso> Avisos = ws.Avisos_Creados_lst(Convert.ToInt32(idOperador.Value), Convert.ToDateTime(dtpFechaDesde.Value), Convert.ToDateTime(dtpFechaHasta.Value));
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write(Resources.Recursos.Reminder + ";" +
                            Resources.Recursos.Creator + ";" +
                            Resources.Recursos.Operator + ";" +
                            Resources.Recursos.CreationDate + ";" +
                            Resources.Recursos.OriginalDate + ";" +
                            Resources.Recursos.Date + ";" +
                            Resources.Recursos.CancelationDate + ";" +
                            Resources.Recursos.Person + ";" +
                            Resources.Recursos.Code + ";" +
                            Resources.Recursos.Comment + "\r\n");

            foreach (otAviso Aviso in Avisos)
            {
                Response.Write(
                    "\"" + Aviso.Aviso.Replace("\"", "\"\"") + "\";" +
                    Aviso.OperadorCrea + ";" +
                    Aviso.Operador + ";" +
                    Aviso.FechaCrea + ";" +
                    Aviso.FechaOriginal + ";" +
                    Aviso.FechaAviso + ";" +
                    Aviso.FechaCancelado + ";" +
                    Aviso.NombrePersona + ";" +
                    Aviso.CodigoPersona + ";" +
                    "\"" + (Aviso.Comentario??"").Replace("\"", "\"\"") + "\"\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Avisos.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}