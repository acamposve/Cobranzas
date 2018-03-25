using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptLlamadas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            List<rptLlamadasResult> Resultados = ws.Llamadas_rpt(Convert.ToDateTime(dtpFechaDesde.Value), Convert.ToDateTime(dtpFechaHasta.Value), Convert.ToInt32(idOperador.Value), Request["Supervisados"] == "1");
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());

            Response.Write( Resources.Recursos.Date + ";" +
                            Resources.Recursos.Operator + ";" +
                            Resources.Recursos.Phone + ";" +
                            Resources.Recursos.Status + ";" +
                            Resources.Recursos.Duration + ";" +
                            Resources.Recursos.EffectiveDuration + ";" +
                            Resources.Recursos.Recording + ";" +
                            Resources.Recursos.Type + ";" +
                            Resources.Recursos.Associated_Management + "\r\n");


            foreach (rptLlamadasResult Result in Resultados)
            {
                Response.Write(

                    Result.Fecha.AFechaMuyCorta() + ";" +
                    Result.Operador + ";" +
                    Result.Telefono + ";" +
                    Result.Status + ";" +
                    Result.Duracion + ";" +
                    Result.DuracionEfectiva + ";" +
                    Result.Grabacion + ";" +
                    Result.Tipo + ";" +
                    Result.Descripcion + "\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Llamadas.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}