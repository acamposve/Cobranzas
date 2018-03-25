using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class ctrlNuevoTelefono : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            Int32 idPersona = Convert.ToInt32(Request["idPersona"]);
            Int32? idPersonaContacto = Request["idPersonaContacto"] != null ? (Convert.ToInt32(Request["idPersonaContacto"])) : (Int32?)null;
            Int32 idTelefono = 0;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Telefonos Telefono = new Entidades.Telefonos { idOperador = Sesion.idOperador, idPersona = idPersona, idPersonaContacto = idPersonaContacto, CodigoArea = txtCodigoArea.Text, Reputacion = null, Telefono = txtTelefono.Text, CodigoPais = "+58" };
                db.Telefonos.InsertOnSubmit(Telefono);
                db.SubmitChanges();
                idTelefono = Telefono.idTelefono;

            }

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", "window.parent.InsertarTelefono({idTelefono:" + idTelefono.ToString() + ",CodigoArea:'" + txtCodigoArea.Text + "',Telefono:'" + txtTelefono.Text + "'});window.parent.Mensaje({mensaje:'Teléfono Registrado satisfactoriamente'});window.parent.CerrarEmergente();", true);
        }
    }
}