using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas
{
    public partial class ctrlComentario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Int32 idPersona = Convert.ToInt32(Request["idPersona"]);
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    String Comentario = db.Personas.Single(x => x.idPersona == idPersona).NotaEspecial;
                    txtComentario.Text = Comentario;
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Int32 idPersona = Convert.ToInt32(Request["idPersona"]);
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String Comentario = txtComentario.Text;
                db.Personas.Single(x => x.idPersona == idPersona).NotaEspecial= Comentario;
                db.SubmitChanges();
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "window.parent.ActualizarComentario($('#txtComentario').val());window.parent.CerrarEmergente();", true);
            }
        }
    }
}