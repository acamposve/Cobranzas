using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptGestionesCuentas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            Int32 idCl = Convert.ToInt32(idCliente.Value);
            List<rptGestionesCuentasResult> Resultado = ws.GestionesCuentas_rpt(idCl);
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write("Gestiones por Cuenta \r\n");
            Response.Write(
            Resources.Recursos.Code + ";" +
            Resources.Recursos.Person + ";" +
            Resources.Recursos.Country + ";" +
            Resources.Recursos.Account + ";" +
            Resources.Recursos.Type + ";" +
            Resources.Recursos.Date + ";" +
            Resources.Recursos.ExchangeRate + ";" +
            Resources.Recursos.Category + ";" +
            Resources.Recursos.Current + ";" +
            Resources.Recursos.M30 + ";" +
            Resources.Recursos.M45 + ";" +
            Resources.Recursos.M60 + ";" +
            Resources.Recursos.M90 + ";" +
            Resources.Recursos.M120 + ";" +
            Resources.Recursos.M180 + "\r\n");

            foreach (rptGestionesCuentasResult Fila in Resultado)
            {
                Response.Write(
                    "\"" + Fila.Codigo + "\";" +
                    "\"" + Fila.Cliente + "\";" +
                    "\"" + Fila.Pais + "\";" +
                    "\"" + Fila.Factura + "\";" +
                    "\"" + Fila.TipoFactura + "\";" +
                    "\"" + Fila.FechaDocumento + "\";" +
                    "\"" + Fila.Cambio + "\";" +
                    "\"" + Fila.Categoria + "\";" +
                    "\"" + Fila.Corriente + "\";" +
                    "\"" + Fila.M30 + "\";" +
                    "\"" + Fila.M45 + "\";" +
                    "\"" + Fila.M60 + "\";" +
                    "\"" + Fila.M90 + "\";" +
                    "\"" + Fila.M120 + "\";" +
                    "\"" + Fila.M180 + "\";" +
                    "\"" + Fila.Status + "\";" +
                    "\"" + (Fila.UltimaGestion??"").Replace("\"","\"\"").Replace("\n"," ") + "\";" +
                    "\"" + Fila.FechaUltimaGestion + "\";" +
                    "\"" + Fila.UltimoOperador + "\"\r\n"
                );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"GestionesCuentas.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}