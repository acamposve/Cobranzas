using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas
{
    public partial class ctrlAviso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnAceptar.Visible = false;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                int idAviso = Convert.ToInt32(Request["idAviso"]);
                Page.Title = idAviso.ToString();

                Entidades.Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                try
                {
                    Aviso.VecesMostrada = Aviso.VecesMostrada + 1;
                    db.SubmitChanges();
                }
                catch (Exception Ex)
                {
                    Ex.Registrar();
                }
                lblAviso.Text = Aviso.Aviso;
                if (Aviso.Personas == null)
                {
                    btnCuenta.Text = "«Sin Persona Asignada»";
                    idPersona.Value = "";
                }
                else
                {
                    btnCuenta.Text = Aviso.Personas.Nombre;
                    idPersona.Value = Aviso.idPersona.ToString();
                }

                if (Aviso.Prioritario) {

                    btnAsignarPrimero.Visible = false;
                    btnAsignarUltimo.Visible = false;
                    btnCancelar.Visible = false;
                    btnAceptar.Visible = true;
                }
                lblHora.Text = Aviso.FechaAviso.AFechaHora();
                lblCreacion.Text = Aviso.FechaCrea.AFechaHora();
                lblOriginal.Text = Aviso.FechaOriginal.AFechaHora();
                lblUsuario.Text = Aviso.Operadores1.Nombre;
                //var b=(new wsCobranzas()).Avisos_lst(0);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (txtComentario.Text.Trim() == "")
            {
                UI.Mensaje("Avisos", "Debe especificar un comentario para cancelar el Aviso.", "", Page);
                return;
            }
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                int idAviso = Convert.ToInt32(Request["idAviso"]);
                Entidades.Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                Aviso.FechaCancelado = DateTime.Now;
                Aviso.Comentario = txtComentario.Text;
                try
                {
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "window.parent.CerrarAviso();window.parent.Avisos_Actualizar();", true);
                }
                catch (Exception Ex)
                {
                    UI.Mensaje("Avisos", Ex.Message, "", Page);
                }
            }
        }

        protected void btnPosponer_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                int idAviso = Convert.ToInt32(Request["idAviso"]);
                Entidades.Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                Aviso.FechaAviso = DateTime.Now.AddMinutes(10);
                try
                {
                    db.SubmitChanges();
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "window.parent.CerrarAviso();window.parent.Avisos_Actualizar();", true);
                }
                catch (Exception Ex)
                {
                    UI.Mensaje("Avisos", Ex.Message, "", Page);
                }
            }
        }

        protected void btnIgnorar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "window.parent.CerrarAviso();window.parent.Avisos_Actualizar();", true);
        }

        protected void btnAsignarPrimero_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                int idAviso = Convert.ToInt32(Request["idAviso"]);
                //Entidades.Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                //Aviso.FechaCancelado = DateTime.Now;
                Int32 Lugar = (db.Colas.Where(x => x.idOperador == Sesion.Operador.idOperador /*&& x.idPersona == Convert.ToInt32(idPersona.Value)*/).Min(x => (int?)x.Lugar) ?? 1) - 1;
                Entidades.Colas Cola = db.Colas.SingleOrDefault(x => x.idOperador == Sesion.Operador.idOperador && x.idPersona == Convert.ToInt32(idPersona.Value));
                if (Cola == null)
                {
                    Cola = new Entidades.Colas { idOperador = Sesion.Operador.idOperador, idPersona = Convert.ToInt32(idPersona.Value), Lugar = Lugar , Fecha = DateTime.Now};
                    db.Colas.InsertOnSubmit(Cola);
                }
                else
                {
                    Cola.Lugar = Lugar;
                }
                db.SubmitChanges();

                ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "window.parent.CerrarAviso();window.parent.Avisos_Actualizar();", true);
            }
        }

        protected void btnAsignarUltimo_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                int idAviso = Convert.ToInt32(Request["idAviso"]);
                //Entidades.Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                //Aviso.FechaCancelado = DateTime.Now;
                Int32 Lugar = (db.Colas.Where(x => x.idOperador == Sesion.Operador.idOperador /*&& x.idPersona == Convert.ToInt32(idPersona.Value)*/).Max(x => (int?)x.Lugar) ?? -1) + 1;
                Entidades.Colas Cola = db.Colas.SingleOrDefault(x => x.idOperador == Sesion.Operador.idOperador && x.idPersona == Convert.ToInt32(idPersona.Value));
                if (Cola == null)
                {
                    Cola = new Entidades.Colas { idOperador = Sesion.Operador.idOperador, idPersona = Convert.ToInt32(idPersona.Value), Lugar = Lugar };
                    db.Colas.InsertOnSubmit(Cola);
                }
                else
                {
                    Cola.Lugar = Lugar;
                }
                db.SubmitChanges();
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "window.parent.CerrarAviso();window.parent.Avisos_Actualizar();", true);
            }
        }
        protected void btnCuenta_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                //int idAviso = Convert.ToInt32(Request["idAviso"]);
                //Entidades.Avisos Aviso = db.Avisos.Single(x => x.idAviso == idAviso);
                //Aviso.FechaCancelado = DateTime.Now;
                //Aviso.Comentario = "(Sistema) La persona fue cargada mediante este mensaje";
                //db.SubmitChanges();
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "ini", "AbrirPersona();window.parent.CerrarAviso();window.parent.Avisos_Actualizar();", true);
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {

        }
    }
}