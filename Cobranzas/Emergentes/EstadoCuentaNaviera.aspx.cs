using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class EstadoCeuentaNaviera : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            DataTable dt = new System.Data.DataTable();

            dt = Negocios.ListarClientes();

            TableRow trContenido = new TableRow();
            DataTable tdContenidoidCliente = new DataTable();
            TableCell tdContenidoCodigo = new TableCell();
            TableCell tdContenidoNombre = new TableCell();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idCliente = int.Parse(dt.Rows[i]["idCliente"].ToString());
                string Codigo = dt.Rows[i]["Codigo"].ToString();
                string Nombre = dt.Rows[i]["Nombre"].ToString();






            }





        }
    }
}