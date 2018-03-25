using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class ctrlNuevoContacto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Int32 idPersona = Convert.ToInt32(Request["idPersona"]);
            Int32 idPersonaContacto = 0;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                try
                {
                    Entidades.PersonasContacto Cont = new Entidades.PersonasContacto();
                    Cont.Cargo = txtCargo.Text;
                    Cont.Nombre = txtNombre.Text;
                    Cont.Email = txtEmail.Text;
                    Cont.idPersona = idPersona;
                    Cont.Activa = true;
                    foreach (String TelefonoCrudo in txtTelefonos.Text.Replace(";", ",").Split(','))
                    {
                        String Telefono = General.ObtenerNumeros(TelefonoCrudo);
                        if (!new System.Text.RegularExpressions.Regex(@"^\d{11}$").IsMatch(Telefono.Trim()))
                        //if ((Telefono.Trim().Length != 11 && Telefono.Trim().Length != 7) || !new System.Text.RegularExpressions.Regex(@"^\d{7}(\d{4})?$").IsMatch(Telefono.Trim()))
                        {
                            UI.Mensaje("", "El teléfono: \"" + TelefonoCrudo + "\" no tiene el formato correcto", "", Page);
                            //ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", "", true);
                            return;
                        }
                        Cont.Telefonos.Add(new Entidades.Telefonos { CodigoPais = "+58", CodigoArea = Telefono.Trim().Substring(0, 4), Telefono = Telefono.Trim().Substring(4), Confirmado = false, Importado = false, idOperador = Sesion.idOperador });
                    }
                    db.PersonasContacto.InsertOnSubmit(Cont);
                    db.SubmitChanges();
                    idPersonaContacto = Cont.idPersonaContacto;
                }
                catch (Exception Ex)
                {

                }
            }

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", "var p=window.parent;p.Mensaje({mensaje:'Contacto Registrado satisfactoriamente'});p.CargarPersonaContacto(" + idPersonaContacto.ToString() + ");p.CerrarEmergente();", true);

        }
    }
}