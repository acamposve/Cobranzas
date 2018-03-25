using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.Entidades;
namespace Cobranzas
{
    public partial class Encuesta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                String Pais;
                String Codigo;
                if (Request["Pais"] != null)
                {
                    Pais = Request["Pais"];
                    Codigo = Request["Codigo"];
                }
                else
                {
                    Pais = Request["Codigo"].Substring(0, 3);
                    Codigo = Request["Codigo"].Substring(3);
                }
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    if (Pais == "VEN")
                    {
                        Gestiones Gestion = new Gestiones();
                        Gestion.idPersona = db.Personas.Single(x => x.Codigo == Codigo && x.idPais == Pais).idPersona;
                        Gestion.idOperador = 1;
                        Gestion.idStatus = 139;
                        Gestion.Descripcion = "La persona ha hecho click en el enlace de la encuesta";
                        Gestion.Fecha = DateTime.Now;

                        db.Gestiones.InsertOnSubmit(Gestion);
                        db.SubmitChanges();
                        Response.Redirect("https://docs.google.com/forms/d/1n_8ciMTiSrZyPC7gCRQX73c2R0yoOiwAjssdktl2d3Q/viewform?usp=send_form");
                    }
                    else
                    {
                        db.ExecuteCommand("INSERT INTO _Encuestas values('" + Pais + "','" + Codigo + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "')");
                        if ("COL,CRI,HND,MEX,PAN".IndexOf(Pais) != -1)
                        {
                            Response.Redirect("https://docs.google.com/forms/d/1q2PsQyktqFXUDWTIYSz-AhjErMNDYB3p9sun8oxjf50/viewform?usp=send_form");
                        }
                        else
                        {
                            Response.Redirect("https://docs.google.com/forms/d/1Pe4HO1jxNFPzRC5ohdSnYwYE3SzU8IVJ-9LpOvsCvpg/viewform?usp=send_form");
                        }

                    }

                }
            }
            catch (Exception Ex)
            {
                //File.WriteAllText(@"C:\Cobranzas\LogEncuesta.txt",Ex.Message+"\n"+Request["Pais"]+"\n"+Request["Codigo"]);
            }
        }
    }
}