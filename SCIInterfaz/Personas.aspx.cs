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
    public partial class Personas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBuscarPorCódigo_Click(object sender, EventArgs e)
        {
            String CS = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=Cobranzas;Password=&&admin%%abc;Initial Catalog=milleniumv2;Data Source=SERV005";
            using (OleDbConnection Conn = new OleDbConnection(CS))
            {
                Conn.Open();
                OleDbCommand Comm = Conn.CreateCommand();
                Comm.CommandType = CommandType.Text;
                Comm.CommandText = "SELECT * FROM oCliente WHERE ClienteId=?";
                Comm.Parameters.Add("clienteid", OleDbType.Integer).Value = Convert.ToInt32(txtCodigo.Text);
                OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                DataSet DS = new DataSet();
                DA.Fill(DS);
                LlenarTablaResultados(DS);
            }
        }

        protected void btnBuscarPorNombre_Click(object sender, EventArgs e)
        {
            String CS = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=Cobranzas;Password=&&admin%%abc;Initial Catalog=milleniumv2;Data Source=SERV005";
            using (OleDbConnection Conn = new OleDbConnection(CS))
            {
                Conn.Open();
                OleDbCommand Comm = Conn.CreateCommand();
                Comm.CommandType = CommandType.Text;
                Comm.CommandText = "SELECT * FROM oCliente WHERE clinombre like '%'+ ? +'%'";
                Comm.Parameters.Add("clinombre", OleDbType.VarChar).Value = txtCodigo.Text ;
                OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                DataSet DS = new DataSet();
                DA.Fill(DS);
                LlenarTablaResultados(DS);
            }
        }
        private void LlenarTablaResultados(DataSet DS)
        {
            String Result = "<h2>Resultados</h2>";
            Result += "<table class='TablaDatos' ><tr><th>Codigo</th><th>Nombre</th><th>Escoger</th></tr>";
            foreach (DataRow Fila in DS.Tables[0].Rows)
            {
                Int32 ID = Convert.ToInt32(Fila["ClienteId"]);
                String Nombre = Convert.ToString(Fila["CliNombre"]);
                Result += "<tr><td>" + ID.ToString() + "</td><td>" + Nombre + "</td><td><input type='button' onclick='Importar(" + ID.ToString() + ")' value='Escoger'/></td></tr>";
            }
            Result += "</table>";
            divContenido.InnerHtml = Result;
        }
    }
}