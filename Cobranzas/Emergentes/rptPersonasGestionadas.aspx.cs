using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptPersonasGestionadas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            Int32? idCliente = Convert.ToInt32(Request["ctl00$body$cboCliente"]);
            if (idCliente == 0) idCliente = null;
            List<rptPersonasGestionadasResult> Resultado = ws.PersonasGestionadas_rpt(Convert.ToDateTime(Request["ctl00$body$dtpFechaDesde"]), Convert.ToDateTime(Request["ctl00$body$dtpFechaHasta"]), Convert.ToDateTime(Request["ctl00$body$dtpFechaFin"]), idCliente, Convert.ToInt32(Request["ctl00$body$cboOperador"]), Request["Supervisados"] == "1" && Request["idOperador"] == Request["ctl00$body$cboOperador"], Request["ctl00$body$cboTipoReporte"] == "1", idPais.Value);
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write("Personas " + (Request["ctl00$body$cboTipoReporte"] == "1" ? "" : "No ") + "Gestionadas desde " + Request["ctl00$body$dtpFechaDesde"] + " hasta " + Request["ctl00$body$dtpFechaHasta"]+"\r\n");
            Response.Write(
            Resources.Recursos.Code + ";" +
            Resources.Recursos.Person + ";" +
            Resources.Recursos.Country + ";" +
            Resources.Recursos.Client + ";" +
            Resources.Recursos.Operator + ";" +
            Resources.Recursos.AssignmentDate + ";" +
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

            foreach (rptPersonasGestionadasResult Fila in Resultado)
            {
                Response.Write(
                    "\"" + Fila.Codigo + "\";" +
                    "\"" + Fila.Persona + "\";" +
                    "\"" + Fila.Pais + "\";" +
                    "\"" + (Fila.Cliente??"") + "\";" +
                    "\"" + (Fila.Operador ?? "") + "\";" +
                    "\"" + (Fila.FechaAsignacion.AFechaMuyCorta()) + "\";" +
                    "\"" + (Fila.Total ?? 0).ToString() + "\";" +
                    "\"" + (Fila.M0a30??0).ToString() + "\";" +
                    "\"" + (Fila.M30a45??0).ToString() + "\";" +
                    "\"" + (Fila.M45a60??0).ToString() + "\";" +
                    "\"" + (Fila.M60a90??0).ToString() + "\";" +
                    "\"" + (Fila.M90a120??0).ToString() + "\";" +
                    "\"" + (Fila.M120a180??0).ToString() + "\";" +
                    "\"" + (Fila.M180ainf??0).ToString() + "\";" +
                    "\"" + (Fila.UltimaGestion) + "\";" +
                    "\"" + Fila.StatusUltimaGestion + "\"\r\n"
                );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"PersonasGestionadas.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}