using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.OT;

namespace Cobranzas.Emergentes
{
    public partial class rptCompromisos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            List<otCompromisos_rpt> Compromisos = ws.Compromisos_rpt(Convert.ToInt32(idOperador.Value), Convert.ToDateTime(dtpFechaDesde.Value), Convert.ToDateTime(dtpFechaHasta.Value), Supervisado.Value == "1");
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write(Resources.Recursos.Date + ";" +
                            Resources.Recursos.Code + ";" +
                            Resources.Recursos.Person + ";" +
                            Resources.Recursos.TotalLocal + ";" +
                            Resources.Recursos.TotalUSD + ";" +
                            Resources.Recursos.RemainingLocal + ";" +
                            Resources.Recursos.RemainingUSD + ";" +
                            Resources.Recursos.Operator + "\r\n");

            foreach (otCompromisos_rpt Compromiso in Compromisos)
            {
                Response.Write(
                    Compromiso.Fecha + ";" +
                    Compromiso.Codigo + ";" +
                    Compromiso.Persona + ";" +
                    Compromiso.MontoLocal + ";" +
                    Compromiso.MontoDolar + ";" +
                    Compromiso.RestanteLocal + ";" +
                    Compromiso.RestanteDolar + ";" +
                    Compromiso.Operador + "\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Compromisos.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}