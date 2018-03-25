using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using Entidades;

namespace Cobranzas.Emergentes
{
    public partial class rptPagosCobrador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    String CS = db.Origenes.Single(x => x.idOrigen == 1).ConnectionString;

                    Response.Clear();
                    Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
                    Response.AddHeader("Content-disposition", "attachment;FileName=\"PagosOperador.csv\"");
                    Response.AddHeader("content-type", "text/csv");
                    Response.Write("Pagos de " + db.Operadores.Single(x => x.idOperador == Convert.ToInt32(Request["idOperador"])).Nombre + "\r\n");

                    using (OleDbConnection Conn = new OleDbConnection(CS))
                    {
                        OleDbCommand Comm = Conn.CreateCommand();
                        Comm.CommandTimeout = 5 * 60;
                        Comm.CommandText = "Cobranzas.rptPagosCobrador";
                        Comm.CommandType = CommandType.StoredProcedure;
                        Comm.Parameters.Add("FechaDesde", OleDbType.Date).Value = Convert.ToDateTime(Request["ctl00$body$dtpFechaIni"]);
                        Comm.Parameters.Add("FechaHasta", OleDbType.Date).Value = Convert.ToDateTime(Request["ctl00$body$dtpFechaFin"]);
                        Comm.Parameters.Add("Cobrador", OleDbType.Integer).Value = db.Operadores.Single(x => x.idOperador == Convert.ToInt32(Request["idOperador"])).Codigo;
                        OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                        DataSet Result = new DataSet();
                        DA.Fill(Result);
                        Response.Write(General.DataTableToCSV(Result.Tables[0]));
                        Response.Flush();
                    }
                    Response.End();
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
            }
        }
    }
}