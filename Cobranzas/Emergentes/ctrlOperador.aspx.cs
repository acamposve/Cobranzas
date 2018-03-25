using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class ctrlOperador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["Cambiar"] == null)
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Operadores Operador = db.Operadores.Single(x => x.idOperador == Sesion.Operador.idOperador);
                    if (Operador.POP3Password != null)
                    {
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", "window.parent.CerrarEmergente();", true);
                    }
                }
            }
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (txtContrasena.Text.Trim() == "")
            {
                UI.Mensaje("Sistema de Cobranzas", "Debe especificar una Contraseña", "", Page);
                return;
            }
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Operadores Operador = db.Operadores.Single(x => x.idOperador == Sesion.Operador.idOperador);
                Operador.POP3Password = txtContrasena.Text;
                Operador.POP3Login = Operador.Login;
                Operador.UltimaFechaCorreoEntrante = Convert.ToDateTime("2014-04-08");
                db.SubmitChanges();
            }
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", "window.parent.CerrarEmergente();", true);
        }
    }
}