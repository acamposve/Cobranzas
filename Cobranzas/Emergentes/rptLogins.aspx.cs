using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptLogins : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            List<rptLoginsResult> Resultados = ws.Logins_rpt(Convert.ToDateTime(dtpFechaDesde.Value), Convert.ToDateTime(dtpFechaHasta.Value), Convert.ToInt32(idOperador.Value), Request["Supervisados"]=="1");
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write(Resources.Recursos.Date + ";" +
                            Resources.Recursos.Start + ";" +
                            Resources.Recursos.End + ";" +
                            Resources.Recursos.Operator + "\r\n");

            foreach (rptLoginsResult Result in Resultados)
            {
                Response.Write(
                    Result.Fecha.AFechaMuyCorta()  + ";" +
                    Result.Inicio.Value.ToString("HH:mm") + ";" +
                    Result.Fin.Value.ToString("HH:mm") + ";" +
                    Result.Operador + "\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Logins.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}