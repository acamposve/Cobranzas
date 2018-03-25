using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.OT;

namespace Cobranzas
{
    public partial class ctrlExclusiones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Int32 idOperador = Convert.ToInt32(Request["idOperador"]);
            wsCobranzas ws = new wsCobranzas();
            otExclusiones Resultados = ws.Exclusiones_lst(idOperador);
            Response.Clear();
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write("Exclusiones\r\n");
            Response.Write("Personas\r\n");
            Response.Write(
                            Resources.Recursos.Person + ";" +
                            Resources.Recursos.Code + ";" +
                            Resources.Recursos.StartDate + ";" +
                            Resources.Recursos.Operator + ";" +
                            "Motivo" + ";" +
                            "Aprobado" + ";" +
                            "\r\n");

            foreach (otExclusionesDet Result in Resultados.Personas)
            {
                Response.Write(
                    Result.Persona.ToCSV() + ";" +
                    Result.Codigo.ToCSV() + ";" +
                    Result.FechaInicio.AFechaMuyCorta().ToCSV() + ";" +
                    Result.Operador.ToCSV() + ";" +
                    Result.Motivo.ToCSV() + ";" +
                    Result.Aprobado.ToCSV() + ";" +
                    "\r\n"
                    );
            }

            Response.Write("\r\nCuentas\r\n");
            Response.Write(
                            Resources.Recursos.Person  + ";" +
                            Resources.Recursos.Code + ";" +
                            Resources.Recursos.Document  + ";" +
                            Resources.Recursos.Client  + ";" +
                            Resources.Recursos.StartDate + ";" +
                            Resources.Recursos.Operator + ";" +
                            "Motivo" + ";" +
                            "Aprobado" + ";" + 
                            
                            "\r\n");

            foreach (otExclusionesDet Result in Resultados.Cuentas)
            {
                Response.Write(
                    Result.Persona.ToCSV() + ";" +
                    Result.Codigo.ToCSV() + ";" +
                    Result.Cuenta.ToCSV() + ";" +
                    Result.Cliente.ToCSV() + ";" +
                    Result.FechaInicio.AFechaMuyCorta().ToCSV() + ";" +
                    Result.Operador.ToCSV() + ";" +
                    Result.Motivo.ToCSV() + ";" +
                    Result.Aprobado.ToCSV() + ";" +
                    "\r\n"
                    );
            }
            
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Exclusiones.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();

        }

    }
}