using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
namespace SCIInterfaz
{
    public partial class Reportes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                String Pais = Request["Paisid"];
                String CS = Pais == "VE" ?
                    "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=webReportes;Password=web123456;Initial Catalog=ReportsAPP;Data Source=SERV005" :
                    "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=webReportes;Password=web123456;Initial Catalog=ReportsAPP;Data Source=USMIAVS016";
                OleDbConnection Conn = new OleDbConnection(CS);
                Conn.Open();
                OleDbCommand Comm = Conn.CreateCommand();
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.CommandText = "insertar_ticketV2";

                Comm.Parameters.Add("ticketid", OleDbType.Numeric).Direction = ParameterDirection.Output;
                Comm.Parameters.Add("Reporte", OleDbType.VarChar).Value = Request["Reporte"];
                Comm.Parameters.Add("Parametros", OleDbType.Boolean).Value = true;
                Comm.Parameters.Add("SubReporte", OleDbType.Boolean).Value = true;
                Comm.Parameters.Add("UID", OleDbType.VarChar).Value = "webReportes";
                Comm.Parameters.Add("PWD", OleDbType.VarChar).Value = "web123456";
                Comm.Parameters.Add("paisid", OleDbType.Char, 3).Value = Pais;
                Comm.Parameters.Add("zona", OleDbType.Char, 10).Value = Request["Zona"];
                Comm.ExecuteNonQuery();
                Int32 Ticket = Convert.ToInt32(Comm.Parameters["ticketid"].Value);

                foreach (String Clave in Request.QueryString.AllKeys)
                {
                    if (Clave.ToUpper() != "REPORTE" && Clave.ToUpper() != "ZONA" && Clave.ToUpper() != "PAISID")
                    {
                        OleDbCommand Comm2 = Conn.CreateCommand();
                        Comm2.CommandType = CommandType.StoredProcedure;
                        Comm2.CommandText = "insertar_ticketDetalle";
                        Comm2.Parameters.Add("ticketId", OleDbType.Numeric).Value = Ticket;
                        Comm2.Parameters.Add("NombreParametro", OleDbType.VarChar, 30).Value = Clave;
                        Comm2.Parameters.Add("ValorParametro", OleDbType.VarChar, 120).Value = Request.QueryString[Clave];
                        Comm2.Parameters.Add("TipoParametro", OleDbType.Char, 10).Value = "x";
                        Comm2.ExecuteNonQuery();
                    }
                }
                if (Pais == "VE")
                {
                    Response.Redirect("http://reportesve.veconinter.com/WCR2/viewer.aspx?ticket=" + Ticket.ToString(), true);
                }
                else
                {
                    Response.Redirect("http://reportesus.veconinter.com/WCR2/viewer.aspx?ticket=" + Ticket.ToString(), true);
                }
            }
            catch (Exception Ex)
            {
                Response.Write("Ocurrió un error al tratar de visualizar el reporte: " + Ex.Message);
            }
        }
    }
}