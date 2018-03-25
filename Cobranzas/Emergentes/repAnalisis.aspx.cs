using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class repAnalisis : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Response.WriteFile(Comunes.AnalisisReporte(Convert.ToInt32(Request["idPersona"]), Convert.ToInt32(Request["idOperador"]),null));
                Response.AddHeader("Content-disposition", "attachment;FileName=\"Analisis.xls\"");
                Response.AddHeader("content-type", "text/html");
                Response.End();
            }
        }
    }
}