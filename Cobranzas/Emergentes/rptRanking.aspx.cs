using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas.Emergentes
{
    public partial class rptRanking : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            Response.Clear();

            List<Int32> idMetas = selMetas.Value.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            DateTime Fecha = Convert.ToDateTime(dtpFecha.Value);
            //Encabezado
            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            String Encabezado = Resources.Recursos.Operator;
            String Principal = Encabezado;
            String Enc = ";" + Resources.Recursos.Goal +
                         ";" + Resources.Recursos.Amount +
                         ";" + Resources.Recursos.Effectiveness;
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                foreach (int idMeta in idMetas)
                {
                    Encabezado += Enc;
                    Principal += ";" + db.Metas.Single(x => x.idMeta == idMeta).Nombre + ";;";
                }
            }
            Encabezado += "\r\n";
            Principal += "\r\n";
            Response.Write(Principal);
            Response.Write(Encabezado);
            //Resultados
            wsCobranzas ws = new wsCobranzas();
            List<List<OT.otMetasOperador>> Rankings = new List<List<OT.otMetasOperador>>();
            List<String> Operadores = new List<string>();
            foreach (int idMeta in idMetas)
            {
                List<OT.otMetasOperador> Ranking = ws.Ranking_lst(idMeta, Fecha);
                Operadores.AddRange(Ranking.Select(x => x.Operador));
                Rankings.Add(Ranking);
            }
            Operadores = Operadores.Distinct().ToList();
            foreach (String Operador in Operadores)
            {
                Response.Write(Operador);
                foreach (List<OT.otMetasOperador> Ranking in Rankings)
                {
                    OT.otMetasOperador MO = Ranking.Single(x => x.Operador == Operador);
                    Response.Write(";" + MO.Meta.ToString() + ";" + MO.Real.ToString() + ";" + MO.Porc.ToString());
                }
                Response.Write("\r\n");
            }
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Ranking.csv\"");
            Response.AddHeader("content-type", "text/csv");
            Response.End();
        }
    }
}