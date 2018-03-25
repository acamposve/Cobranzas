using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Entidades;

namespace Cobranzas
{
    public partial class rptAnalisisPorCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Int32 idOperador = Request["ctl00$body$cboOperador"] == null ? 0 : Convert.ToInt32(Request["ctl00$body$cboOperador"]);
            Boolean Supervisados = Request["Supervisados"] == "1";
            Boolean General = Request["General"] == "1";
            wsCobranzas ws = new wsCobranzas();

            List<rptAnalisisPorClienteResult> Resultado = ws.AnalisisPorCliente_rpt(Request["ctl00$body$idPais"], Convert.ToInt32(Request["ctl00$body$cboCliente"]), Convert.ToDateTime(Request["ctl00$body$dtpFechaIni"]), Convert.ToDateTime(Request["ctl00$body$dtpFechaFin"]), idOperador, Supervisados, General);
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());

            Response.Write(
            Resources.Recursos.Code + ";" +
            Resources.Recursos.Person + ";" +
            Resources.Recursos.Country + ";" +
            "TipoPersona" + ";" +
            Resources.Recursos.Client + ";" +
            Resources.Recursos.Total + ";" +
            "0-30" + ";" +
            "30-45" + ";" +
            "45-60" + ";" +
            "60-90" + ";" +
            "90-120" + ";" +
            "120-180" + ";" +
            ">180" + ";" +
            Resources.Recursos.LastManagement + ";" +
            Resources.Recursos.LastManagement + "\r\n");

            foreach (rptAnalisisPorClienteResult Fila in Resultado)
            {
                Response.Write(
                    "\"" + Fila.Codigo + "\";" +
                    "\"" + Fila.Persona + "\";" +
                    "\"" + Fila.Pais + "\";" +
                    "\"" + Fila.TipoPersona + "\";" +
                    "\"" + Fila.Cliente + "\";" +
                    "\"" + Fila.Total.Value.ToString() + "\";" +
                    "\"" + Fila.M0a30.Value.ToString() + "\";" +
                    "\"" + Fila.M30a45.Value.ToString() + "\";" +
                    "\"" + Fila.M45a60.Value.ToString() + "\";" +
                    "\"" + Fila.M60a90.Value.ToString() + "\";" +
                    "\"" + Fila.M90a120.Value.ToString() + "\";" +
                    "\"" + Fila.M120a180.Value.ToString() + "\";" +
                    "\"" + Fila.M180ainf.Value.ToString() + "\";" +
                    "\"" + Fila.UltimaGestion + "\";" +
                    "\"" + Fila.FechaUltimaGestion + "\"\r\n"
                );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"AnalisisDeAntiguedad.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}