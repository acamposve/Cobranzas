using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Cobranzas.OT;
using Entidades;

namespace Cobranzas
{
    public partial class ctrlMetas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Exportar(object sender, EventArgs e)
        {
            Int32 idMeta = Convert.ToInt32(this.idMeta.Value);
            DateTime Fecha = Convert.ToDateTime(this.Fecha.Value);
            Int32 idOperador = Convert.ToInt32(this.idOperador.Value);
            wsCobranzas ws = new wsCobranzas();
            otMetaDetalle Resultados = ws.Metas_Detalle_sel(idMeta, Fecha, idOperador);
            String Operador;
            String Meta;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Operador = db.Operadores.Single(x => x.idOperador == idOperador).Nombre;
                Meta = db.Metas.Single(x => x.idMeta == idMeta).Nombre;
            }
            Response.Clear();
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write(Meta + " del Operador: " + Operador + " Desde:" + Fecha + "\r\n");
            Response.Write(Resources.Recursos.Type + ";" +
                            Resources.Recursos.Person + ";" +
                            Resources.Recursos.Code + ";" +
                            Resources.Recursos.Document + ";" +
                            Resources.Recursos.Date + ";" +
                            Resources.Recursos.Overdue + ";" +
                            Resources.Recursos.Client + ";" +
                            Resources.Recursos.Product + ";" +
                            Resources.Recursos.Goal
                            + "\r\n");

            foreach (otMetaDetalleDet Result in Resultados.Cuentas)
            {
                Response.Write(
                    "Metas;" +
                    Result.Persona.ToCSV() + ";" +
                    Result.Codigo.ToCSV() + ";" +
                    Result.Documento.ToCSV() + ";" +
                    Result.Fecha.AFechaMuyCorta().ToCSV() + ";" +
                    Result.Antiguedad + ";" +
                    Result.Cliente.ToCSV() + ";" +
                    Result.Producto.ToCSV() + ";" +
                    Result.Meta + "\r\n"
                    );
            }
            foreach (otMetaDetalleDet Result in Resultados.Exclusiones)
            {
                Response.Write(
                    "Exclusiones;" +
                    Result.Persona.ToCSV() + ";" +
                    Result.Codigo.ToCSV() + ";" +
                    Result.Documento.ToCSV() + ";" +
                    Result.Fecha.AFechaMuyCorta().ToCSV() + ";" +
                    Result.Antiguedad + ";" +
                    Result.Cliente.ToCSV() + ";" +
                    Result.Producto.ToCSV() + ";" +
                    Result.Meta + "\r\n"
                    );
            }
            foreach (otMetaDetalleDet Result in Resultados.Inclusiones)
            {
                Response.Write(
                    "Inclusiones;" +
                    Result.Persona.ToCSV() + ";" +
                    Result.Codigo.ToCSV() + ";" +
                    Result.Documento.ToCSV() + ";" +
                    Result.Fecha.AFechaMuyCorta().ToCSV() + ";" +
                    Result.Antiguedad + ";" +
                    Result.Cliente.ToCSV() + ";" +
                    Result.Producto.ToCSV() + ";" +
                    Result.Meta + "\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Metas.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();

        }

        protected void ExportarRanking(object sender, EventArgs e)
        {
            Int32 idMeta = Convert.ToInt32(this.idMeta.Value);
            DateTime Fecha = Convert.ToDateTime(this.FechaRanking.Value);
            wsCobranzas ws = new wsCobranzas();
            List<otMetasOperador> Resultados = ws.Ranking_lst(idMeta, Fecha);
            String Meta;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Meta = db.Metas.Single(x => x.idMeta == idMeta).Nombre;
            }
            Response.Clear();
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            Response.Write("Ranking de " + Meta + " Desde:" + Fecha + "\r\n");
            Response.Write(Resources.Recursos.Operator + ";" +
                            Resources.Recursos.Goal + ";" +
                            Resources.Recursos.Real + ";" +
                            Resources.Recursos.Percent + "\r\n");

            foreach (otMetasOperador Result in Resultados)
            {
                Response.Write(
                    Result.Operador.ToCSV() + ";" +
                    Result.Meta + ";" +
                    Result.Real + ";" +
                    Result.Porc + "\r\n"
                    );
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Ranking.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();

        }
    }
}