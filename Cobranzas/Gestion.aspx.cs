using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.Entidades;
using System.Web.UI.DataVisualization.Charting;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Threading;

namespace Cobranzas
{
    public partial class Gestion : System.Web.UI.Page
    {
        #region Para ViewState en servidor
        protected override void SavePageStateToPersistenceMedium(object viewState)
        {
            string str = "VIEWSTATE#" + Request.UserHostAddress + "#" + DateTime.Now.Ticks.ToString();// Generar una clave única para este viewstate
            Cache.Add(str, viewState, null, DateTime.Now.AddMinutes(Session.Timeout), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);// Guardar el viewstate en el caché.
            ClientScript.RegisterHiddenField("__VIEWSTATE_CLAVE", str);
        }
        protected override object LoadPageStateFromPersistenceMedium()
        {
            string str = Request.Form["__VIEWSTATE_CLAVE"];// Lee la Clave para cargar el ViewState
            if (!str.StartsWith("VIEWSTATE#"))// Revisa si es una clave válida...
            {
                throw new Exception("Clave de ViewState Inválida:" + str);
            }
            return Cache[str];
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                //if (Request["Reintentos"] != null)
                //{
                //    Thread.Sleep(5000);
                //}
                if (Sesion.Operador == null) Response.Redirect("Default.aspx", true);

                lblUser.Text = Sesion.Operador.Nombre;
                if (Sesion.Operador.POP3Password == null)
                {
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "actcont", "ActualizarContrasena()", true);
                }

                if (!IsPostBack)
                {
                    idOperadorLog.Value = Sesion.Operador.idOperador.ToString(); // Logueado
                    idOperadorAct.Value = Sesion.Operador.idOperador.ToString(); // Suplantado
                    idOperador.Value = Sesion.Operador.idOperador.ToString(); //Supervisado
                }
                TipoOperador.Value = Sesion.Operador.Tipo;

                if (Sesion.idPersona != null) idPersona.Value = Sesion.idPersona.ToString();
                if (Sesion.idOperador == 0) { Response.Redirect("Default.aspx"); }
                chrtGestiones.ImageUrl = Negocios.MostrarGrafico(Sesion.Operador.idOperador);
                chrtGestionesSupervision.ImageUrl = Negocios.MostrarGraficoSupervision(Sesion.Operador.idOperador);
                chrtGestionesPorSemana.ImageUrl = Negocios.MostrarGraficoPorSemana(Sesion.Operador.idOperador);
                if (!(Sesion.Operador.Tipo.Contains("SU") ||
                    Sesion.Operador.Tipo.Contains("AD") ||
                    Sesion.Operador.Tipo.Contains("GE") ||
                    Sesion.Operador.Tipo.Contains("CO") ||
                    Sesion.Operador.Tipo.Contains("DI")))
                {
                    liDistribucion.Visible = false;  //.Style.Add(HtmlTextWriterStyle.Display, "none");
                    liSupervision.Visible = false;//.Style.Add(HtmlTextWriterStyle.Display, "none");
                    idFiltro.Visible = false;
                }

                //Es de sistemas
                liSistema.Visible = Sesion.Operador.idGrupo == 1;
                tabSistema.Visible = Sesion.Operador.idGrupo == 1;


                /*if (Request["Reintentos"]!="4") throw new Exception("Prueba");*/
            }
            catch (Exception Ex)
            {
                try { Ex.Registrar(); }
                catch { }
                Int32 Reintentos = Convert.ToInt32(Request["Reintentos"] ?? "0") + 1;
                if (Reintentos == 11)
                {
                    Response.Clear();
                    Response.Write("Actualmente hay algunos inconvenientes con el sistema, por favor notifique al dpto. de Sistemas. ");
                    Response.Flush();
                    Response.End();
                    return;
                }
                Response.Clear();
                Response.AddHeader("Location", "gestion.aspx?Reintentos=" + Reintentos);
                Response.Write("Haciendo Reintento #" + Reintentos + " de 10 en " + Reintentos * 10 + " segundos... Por favor Espere...");
                Response.Write("<meta http-equiv='Refresh' Content='" + Reintentos * 10 + ";URL=Gestion.aspx?Reintentos=" + Reintentos + "'>");
                Response.Flush();
                Response.End();
                return;
                //Response.Redirect("Gestion.aspx?Reintentos=" + Reintentos, true);
            }
        }
    }
}
