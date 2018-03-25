using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas
{
    public partial class ctrlNuevoAviso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
/*            if (!IsPostBack)
            {
                try
                {
                    using (CobranzasDataContext db = new CobranzasDataContext())
                    {
                        var Usuarios = from c in db.Operadores where c.idOperador == Sesion.idOperador || c.idOperador == Sesion.idSupervisor || c.idSupervisor == Sesion.idOperador select new { c.idOperador, c.Nombre };
                        ddlUsuario.DataSource = Usuarios.ToList();
                        ddlUsuario.DataTextField = "Nombre";
                        ddlUsuario.DataValueField = "idOperador";
                        ddlUsuario.DataBind();
                        var Personas = from c in db.Personas where c.Cuentas.Any(y => y.Cuentas_Operadores.Any(x => x.idOperador == Sesion.idOperador || x.Operadores.Operadores1.idOperador == Sesion.idOperador)) select new { c.idPersona, Persona = c.Nombre + "(" + c.Rif + ") - " + c.Codigo };
                        ddlPersona.DataSource = Personas.ToList();
                        ddlPersona.DataTextField = "Persona";
                        ddlPersona.DataValueField = "idPersona";
                        ddlPersona.DataBind();

                    }
                }
                catch (Exception Ex)
                {
                    throw Ex.Informar();
                }
            }*/
        }

        /*protected void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Entidades.Avisos AvisoNew = new Entidades.Avisos();
                    AvisoNew.Aviso = txtNuevoAviso.InnerText;
                    AvisoNew.idOperadorCrea = Sesion.idOperador;
                    AvisoNew.idOperador = Convert.ToInt32(ddlUsuario.SelectedValue);
                    AvisoNew.FechaCrea = DateTime.Now;
                    AvisoNew.FechaOriginal = Convert.ToDateTime(txtFechaNuevoAviso.Value + " " + txtHoraNuevoAviso.Value);
                    AvisoNew.FechaAviso = AvisoNew.FechaOriginal;
                    Int32? idPersona;
                    var Per=db.Personas.SingleOrDefault(x=>x.Codigo==txtFlitroPersona.Text);
                    idPersona=Per!=null?Per.idPersona:(ddlPersona.SelectedValue == "0" ? (int?)null : Convert.ToInt32(ddlPersona.SelectedValue));
                    AvisoNew.idPersona = idPersona;
                    db.Avisos.InsertOnSubmit(AvisoNew);
                    db.SubmitChanges();
                    UI.Mensaje("Crear Aviso", "Mensaje Creado Exitosamente", "", Page);
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
                UI.Mensaje("Crear Aviso", Ex.Message, "", Page);
            }
        }

        protected void ddlPersona_DataBound(object sender, EventArgs e)
        {
            ddlPersona.Items.Insert(0, new ListItem("<<Ninguno>>", "0"));
        }*/
    }
}