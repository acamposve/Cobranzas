using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class ctrlOperadores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) idOperador.Value = Sesion.Operador.idOperador.ToString();
        }
    }
}