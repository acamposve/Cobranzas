using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace Cobranzas
{
    public partial class Recursos : System.Web.UI.Page
    {
        public static String Recurso(String Clave)
        {
            try
            {
                ResourceManager temp = new ResourceManager("Resources.Recursos", Assembly.Load("App_GlobalResources"));
                ResourceSet RS = temp.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                return (from DictionaryEntry c in RS where c.Key.ToString() == Clave select c.Value.ToString()).First();
            }
            catch
            {
                return Clave;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            String Recursos = "";
            Recursos = "Recursos=Array();\n";
            ResourceManager temp = new ResourceManager("Resources.Recursos", Assembly.Load("App_GlobalResources"));
            ResourceSet RS = temp.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry Itm in RS)
            {
                Recursos += "Recursos." + Itm.Key.ToString() + "='" + HttpUtility.JavaScriptStringEncode(Itm.Value.ToString()) + "';\n";

            }
            Response.Write(Recursos);

            String Colores = "";
            Colores = "Colores={};\n";
            Colores += "Colores.Positivo='#" + Negocios.Positivo.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.Negativo='#" + Negocios.Negativo.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.Neutral='#" + Negocios.Neutral.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.Total='#" + Negocios.Total.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.Fondo='#" + Negocios.Fondo.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.Sistema='#" + Negocios.Fondo.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.LineaMayor='#" + Negocios.LineaMayor.ToArgb().ToString("X").Substring(2) + "';\n";
            Colores += "Colores.LineaMenor='#" + Negocios.LineaMenor.ToArgb().ToString("X").Substring(2) + "';\n";
            Response.Write(Colores);
            String Config = "";
            Config = "Regional={};\n";
            Config += "Regional.FormatoFecha='" + General.FormatoFecha + "';\n";
            Config += "Regional.FormatoFechaHora='" + General.FormatoFechaHora + "';\n";
            Config += "Regional.SeparadorDecimal='" + CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator + "';\n";
            Config += "Regional.SeparadorMiles='" + CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator + "';\n";
            Response.Write(Config);
            //            ScriptManager.RegisterStartupScript(Page, typeof(Page), "Inicio", Recursos, true);
        }

    }
}