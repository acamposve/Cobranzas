using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cobranzas.OT;

namespace Cobranzas
{
    public partial class Descargas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String Reporte = Request["Reporte"];
            switch (Reporte)
            {
                case "Cartera":
                    Int32 idOperador = Convert.ToInt32(Request["idOperador"]);
                    Boolean IncluyeAutomatico = Request["IncluyeAutomatico"] == "true";
                    Boolean Supervisado = Request["Supervisado"] == "true";
                    Response.Clear();
                    Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
                    Response.AddHeader("Content-disposition", "attachment;FileName=\"Cartera.csv\"");
                    Response.AddHeader("content-type", "text/csv");
                    using (CobranzasDataContext db = new CobranzasDataContext())
                    {
                        Response.Write("Cartera de " + db.Operadores.Single(x => x.idOperador == idOperador).Nombre + "\r\n");
                    }

                    if (Request["idCampana"] == null)
                    {
                        List<OT.otCarteraGrupo> Cartera = (new wsCobranzas()).Cartera_lst(idOperador, Supervisado, IncluyeAutomatico);
                        foreach (otCarteraGrupo Grupo in Cartera)
                        {
                            Response.Write(Grupo.Nombre + "\r\n");
                            EscribirCartera(Grupo.Personas);
                        }
                    }
                    else
                    {
                        Int32 idCampana = Convert.ToInt32(Request["idCampana"]);
                        List<Entidades.Operador_CuentasResult> Campana;
                        using (CobranzasDataContext db = new CobranzasDataContext())
                        {
                            Campana = db.Operador_Cuentas(idOperador, idCampana, IncluyeAutomatico, Supervisado).ToList();
                            Response.Write("Grupo " + (idCampana==-2 ? "Cola de Gestión":(idCampana==-1?"Supervisados":(idCampana==0?"Asignados Directamente":db.Campanas.Single(x => x.idCampana == idCampana).Nombre))) + "\r\n");
                        }
                        EscribirCartera(Campana);

                    }

                    Response.End();
                    break;
                default:
                    break;
            }
        }

        private void EscribirCartera(List<Entidades.Operador_CuentasResult> Personas)
        {
            Response.Write(
            Resources.Recursos.Status + ";" +
            Resources.Recursos.LastManagement + ";" +
            Resources.Recursos.LastManagement + ";" +
            Resources.Recursos.Operator + ";" +
            Resources.Recursos.LastOperator + ";" +
            Resources.Recursos.Code + ";" +
            Resources.Recursos.Rif + ";" +
            Resources.Recursos.Name + ";" +
            Resources.Recursos.TotalUSD + ";" +
            Resources.Recursos.RemainingUSD + ";" +
            Resources.Recursos.TotalLocal + ";" +
            Resources.Recursos.RemainingLocal + ";" +
            Resources.Recursos.Country + "\r\n");

            foreach (Entidades.Operador_CuentasResult Persona in Personas)
            {
                Response.Write(
                @"""" + Persona.TipoStatus + @""";" +
                @"""" + Persona.StatusUltimaGestion + @""";" +
                @"""" + Persona.FechaUltimaGestion + @""";" +
                @"""" + Persona.Operador + @""";" +
                @"""" + Persona.UltimoOperador + @""";" +
                @"""" + Persona.Codigo + @""";" +
                @"""" + Persona.Rif + @""";" +
                @"""" + Persona.Nombre + @""";" +
                @"" + Persona.TotalDolar + @";" +
                @"" + Persona.RestanteDolar + @";" +
                @"" + Persona.TotalLocal + @";" +
                @"" + Persona.RestanteLocal + @";" +
                @"""" + Persona.idPais + @"""" + "\r\n");
            }
        }
    }
}