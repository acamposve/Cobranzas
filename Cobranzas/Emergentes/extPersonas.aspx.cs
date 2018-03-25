using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
namespace Cobranzas
{
    public partial class extPersonas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text == "" && txtNombre.Text == "") return;
            String CS = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=Cobranzas;Password=&&admin%%abc;Initial Catalog=milleniumv2;Data Source=SERV005";
            try
            {
                using (OleDbConnection Conn = new OleDbConnection(CS))
                {
                    Conn.Open();
                    OleDbCommand Comm = Conn.CreateCommand();
                    Comm.CommandType = CommandType.Text;
                    if (txtCodigo.Text != "")
                    {
                        Comm.CommandText = "SELECT * FROM oCliente WHERE ClienteId=?";
                        Comm.Parameters.Add("clienteid", OleDbType.Integer).Value = Convert.ToInt32(txtCodigo.Text);
                    }
                    else
                    {
                        Comm.CommandText = "SELECT * FROM oCliente WHERE clinombre like '%'+ ? +'%'";
                        Comm.Parameters.Add("clinombre", OleDbType.VarChar).Value = txtNombre.Text;
                    }
                    OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                    DataSet DS = new DataSet();
                    DA.Fill(DS);
                    LlenarTablaResultados(DS);
                }
            }
            catch (Exception Ex) {
                UI.Mensaje(Resources.Recursos.titSystem, "Ocurrió un error al procesar su petición: Error " + Ex.Registrar().ToString(), "", Page);
            }
        }

        private void LlenarTablaResultados(DataSet DS)
        {
            String Result = "<h2>Resultados</h2>";
            Result += "<table class='Tabla' ><tr><th>Codigo</th><th>Nombre</th><th>Escoger</th></tr>";
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