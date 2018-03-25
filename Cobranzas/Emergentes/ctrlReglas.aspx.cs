using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections;

namespace Cobranzas.Emergentes
{
    public partial class ctrlReglas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BajarCriterios();
        }
        private void BajarCriterios()
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    String Criterios = "";
                    Criterios = "Criterios=Array();\n";
                    int i = 0;
                    foreach (Entidades.Campos Criterio in db.Campos.OrderBy(x=>x.Definicion))
                    {
                        String Operadores = "[";
                        foreach (Entidades.TiposCampoDet TCD in Criterio.TiposCampo.TiposCampoDet)
                        {
                            Operadores += "{Operador:\"" + TCD.Operador + "\",idTipoCampoOtro:" + (TCD.idTipoCampoOtro ?? 0) + "}";
                        }
                        Operadores = Operadores.Replace("}{", "},{") + "]";// Reemplaza "" por ","
                        Criterios += String.Format("Criterios[{0}]={{idCampo:{1},idTipoCampo:{2},Nombre:'{3}',TipoSeleccion:'{4}',Operadores:{5}}};\n", i, Criterio.idCampo, Criterio.idTipoCampo, Criterio.Definicion, Criterio.TipoSeleccion, Operadores);
                        i++;
                    }
                    Criterios += "LlenarComboCriterios();";
                    ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", Criterios, true);
                }
            }
            catch (Exception Ex)
            {
                Ex.Registrar();
            }

        }
    }
}