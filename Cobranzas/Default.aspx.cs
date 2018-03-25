using System;
using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Security.Principal;
using System.Linq;
using System.Web.Security;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using Cobranzas.Entidades;
namespace Cobranzas
{
    public partial class _Default : Pagina
    {
        private bool ValidateUser(string userName, string passWord)
        { return true; }
        protected void Page_Load(object sender, EventArgs e)
        {
            //SqlConnection Conn = new SqlConnection("Data Source=SERV005;User ID=sa;Password=avila;Initial Catalog=milleniumv2");
            //Conn.Open();
            //Conn.Close();

            //CentralIp.VoIpConexion Conn = new CentralIp.VoIpConexion("172.17.1.102");
            //Conn.Conectar();
            //CentralIp.Respuesta Resp;

            //Resp = Conn.Login("Mberroteran", ".m4n8bds0as7");
            //Resp = Conn.GetAgentStatus("Agent/4401");
            //Resp = Conn.LoginAgent("Agent/4401", "1234", "502");
            //Conn.LLamar("04241798184");
            //Resp = Conn.LogoutAgent("Agent/4401","1234");
            //Resp = Conn.Logout();
            //Conn.Desconectar();
            //Correos Correo = new Correos();
            //Correo.Asunto = "Hola!";
            //String Prueba = General.LeerValorPropiedad(Correo, "Asunto").ToString();

            if (!IsPostBack)
            {
                if (Request["RL"] == "1")
                {
                    ClientScript.RegisterStartupScript(typeof(PageAction), "Ini", "window.parent.CerrarEmergente();", true);
                }
                if (Request["salir"] == "1")
                {
                    try
                    {
                        if (!Sesion.Impersonando) Sesion.Actividad("LO");
                        Session.Abandon();
                    }
                    catch (Exception Ex)
                    {
                        UI.Mensaje("Sistema de Cobranzas", "No se pudo cerrar la sesión", "", Page);
                    }
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                    //Response.StatusCode = 401;
                }
                try
                {
                    WindowsIdentity Mi_Iden = (WindowsIdentity)User.Identity;
                    txtUsuario.Text = Mi_Iden.Name.Split('\\')[1];
                    try
                    {
                        using (CobranzasDataContext db = new CobranzasDataContext())
                        {
                            txtUsuario.Enabled = db.Operadores.Single(x => x.Login == txtUsuario.Text).Tipo.IndexOf("AD") != -1;
                        }
                    }
                    catch
                    {
                        txtUsuario.Enabled = txtUsuario.Text == "mberroteran";
                    }
                }
                catch (Exception Ex)
                {
                    Ex.Registrar();
                }
            }
            //General.Mensaje("Prueba", "Prueba", "", Page);
            //Sesion.idOperador = 4;
            //Response.Write(Mi_Iden.AuthenticationType); Response.Write("<br/>");
            //Response.Write(Mi_Iden.Groups); Response.Write("<br/>");
            //Response.Write(Mi_Iden.ImpersonationLevel); Response.Write("<br/>");
            //Response.Write(Mi_Iden.IsAnonymous); Response.Write("<br/>");
            //Response.Write(Mi_Iden.IsAuthenticated); Response.Write("<br/>");
            //Response.Write(Mi_Iden.IsGuest); Response.Write("<br/>");
            //Response.Write(Mi_Iden.IsSystem); Response.Write("<br/>");
            //Response.Write(Mi_Iden.Name); Response.Write("<br/>");
            //Response.Write(Mi_Iden.Owner); Response.Write("<br/>");
            //Response.Write(Mi_Iden.Token); Response.Write("<br/>");
            //Response.Write(Mi_Iden.User); Response.Write("<br/>");

            //System.Collections.IList visibleTables = Global.DefaultModel.VisibleTables;
            //if (visibleTables.Count == 0)
            //{
            //    throw new InvalidOperationException("No hay tablas accesibles. Asegúrese de que hay al menos un modelo de datos registrado en Global.asax y de que está habilitada la técnica scaffolding, o bien implemente páginas personalizadas.");
            //}
            //Menu1.DataSource = visibleTables;
            //Menu1.DataBind();
            /*
    <asp:GridView ID="Menu1" runat="server" AutoGenerateColumns="false"
        CssClass="DDGridView" RowStyle-CssClass="td" HeaderStyle-CssClass="th" CellPadding="6">
        <Columns>
            <asp:TemplateField HeaderText="Nombre de la tabla" SortExpression="TableName">
                <ItemTemplate>
                    <asp:DynamicHyperLink ID="HyperLink1" runat="server"><%# Eval("DisplayName") %></asp:DynamicHyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
             */
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (ValidateUser(txtUsuario.Text, txtContraseña.Text))
            {
                try
                {
                    using (CobranzasDataContext db = new CobranzasDataContext())
                    {
                        //if (txtUsuario.Text != ((WindowsIdentity)User.Identity).Name.Split('\\').Last()) { Sesion.Impersonando = true; }
                        Entidades.Operadores Op = db.Operadores.Single(x => x.Login == txtUsuario.Text);
                        if (!Op.Activo)
                        {
                            UI.Mensaje("Sistema de Cobranzas", "Usuario Inactivo, diríjase a su Supervisor", "", Page);
                            return;
                        }

                        Sesion.Llenar(Op);
                        if (!Sesion.Impersonando)
                        {
                            Sesion.Actividad("LI");
                        }
                    }
                    Response.Redirect("Gestion.aspx", true);
                }
                catch (Exception Ex)
                {
                    //Response.Write(Ex.Message);
                    //Ex.Registrar();
                    UI.Mensaje("Sistema de Cobranzas", "No se pudo iniciar sesión, usuario incorrecto o inexistente", "", Page);
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }

    }
}
