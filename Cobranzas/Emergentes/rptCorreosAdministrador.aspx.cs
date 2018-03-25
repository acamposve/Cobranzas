using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{

    


    public partial class rptCorreosAdministrador : System.Web.UI.Page
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
            List<rptCorreosAdministradorResult> Resultado = ws.CorreosAdministrador_rpt(Convert.ToDateTime(Request["ctl00$body$dtpFechaDesde"]), Convert.ToDateTime(Request["ctl00$body$dtpFechaHasta"]));
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write("Correos de Administrador Enviados desde " + Request["ctl00$body$dtpFechaDesde"] + " hasta " + Request["ctl00$body$dtpFechaHasta"]+ "\r\n");
            Response.Write(
            Resources.Recursos.Code + ";" +
            Resources.Recursos.Person + ";" +
            Resources.Recursos.Email + ";" +
            Resources.Recursos.Operator + ";" +
            Resources.Recursos.Date + ";" +
            Resources.Recursos.Category + ";" +
            Resources.Recursos.Reason + "\r\n");

            foreach (rptCorreosAdministradorResult Fila in Resultado)
            {
                Response.Write(
                    "\"" + Fila.Codigo + "\";" +
                    "\"" + Fila.Persona + "\";" +
                    "\"" + Fila.Email + "\";" +
                    "\"" + Fila.Ejecutivo + "\";" +
                    "\"" + Fila.FechaCreacion.AFechaHora() + "\";" +
                    "\"" + Fila.Categoria + "\";" +
                    "\"" + Fila.MotivoRechazo + "\"\r\n"
                );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"CorreosAdministrador.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}