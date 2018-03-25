using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptClienteCuentas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            Int32 idCl = Convert.ToInt32(idCliente.Value);
            List<rptClienteCuentasResult> Resultado = ws.ClienteCuentas_rpt(idCl);
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write("Cuentas del Cliente \r\n");
            Response.Write(
            Resources.Recursos.Code + ";" +
            Resources.Recursos.Person + ";" +
            Resources.Recursos.Country + ";" +
            Resources.Recursos.Account + ";" +
            Resources.Recursos.Type + ";" +
            Resources.Recursos.Date + ";" +
            "Bl;" +
            "Buque;" +
            Resources.Recursos.Currency + ";" +
            Resources.Recursos.Remaining + ";" +
            Resources.Recursos.Currency + ";" +
            Resources.Recursos.RemainingLocal + ";" +
            Resources.Recursos.ExchangeRate + ";" +
            "Viaje;" +
            "Puerto Carga;" +
            "Puerto Descarga;" +
            "Tipo Persona" + ";" +
            "Categoría" + "\r\n");

            foreach (rptClienteCuentasResult Fila in Resultado)
            {
                Response.Write(
                    "\"" + Fila.Codigo + "\";" +
                    "\"" + Fila.Persona + "\";" +
                    "\"" + Fila.Pais + "\";" +
                    "\"" + Fila.Factura + "\";" +
                    "\"" + Fila.Producto + "\";" +
                    "\"" + Fila.BL + "\";" +
                    "\"" + Fila.Buque + "\";" +
                    "\"" + Fila.FechaDocumento + "\";" +
                    "\"" + Fila.Moneda + "\";" +
                    "\"" + Fila.MontoRestante + "\";" +
                    "\"" + Fila.MonedaLocal + "\";" +
                    "\"" + Fila.MontoLocal + "\";" +
                    "\"" + Fila.CambioLocal + "\";" +
                    "\"" + Fila.Viaje + "\";" +
                    "\"" + Fila.PuertoCarga + "\";" +
                    "\"" + Fila.PuertoDescarga + "\";" +
                    "\"" + Fila.TipoPersona + "\";" +
                    "\"" + Fila.Categoria + "\"\r\n"
                );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"ClienteCuentas.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}