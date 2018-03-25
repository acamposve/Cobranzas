using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.OT;

namespace Cobranzas.Emergentes
{
    public partial class rptGestiones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            wsCobranzas ws = new wsCobranzas();
            List<Int32> Status;
            //List<String> Paises;
            if (idStatus.Value == "")
            {
                Status = new List<int>();
            }
            else
            {
                Status = idStatus.Value.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            }

            //if (idPais.Value == "")
            //{
            //    Paises = new List<String>();
            //}
            //else
            //{
            //    Paises = idPais.Value.Split(',').ToList();
            //}

            List<otHistorialGestion> Gestiones = ws.Gestiones_rpt(Convert.ToDateTime(dtpFechaDesde.Value), Convert.ToDateTime(dtpFechaHasta.Value), idPais.Value, Convert.ToInt32(idOperador.Value), Request["Supervisados"] != "0", Convert.ToInt32("0" + idCliente.Value), Convert.ToInt32("0" + idPersona.Value), Status);
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());

            Response.Write(Resources.Recursos.Date + ";" +
                           Resources.Recursos.Time + ";" +
                           Resources.Recursos.Status + ";" +
                           Resources.Recursos.Type + ";" +
                           Resources.Recursos.Code + ";" +
                           Resources.Recursos.Person + ";" +
                           Resources.Recursos.Description + ";" +
                           Resources.Recursos.Operator + ";" +
                           Resources.Recursos.Country + "\r\n");

            foreach (otHistorialGestion Gestion in Gestiones)
            {
                Response.Write(
                    "\"" + Gestion.Fecha.AFechaMuyCorta() + "\";" +
                    "\"" + Gestion.Fecha.ToString("hh:mm tt") + "\";" +
                    "\"" + Gestion.Status + "\";" +
                    "\"" + Gestion.Tipo + "\";" +
                    "\"" + Gestion.Codigo + "\";" +
                    "\"" + Gestion.Persona + "\";" +
                    "\"" + Gestion.Descripcion.Replace("\"", "\"\"") + "\";" +
                    "\"" + Gestion.Operador + "\";" +
                    "\"" + Gestion.idPais + "\"\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Gestiones.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}