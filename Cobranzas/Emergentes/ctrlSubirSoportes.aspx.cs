using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Configuration;
using System.IO;
using Entidades;

namespace Cobranzas.Emergentes
{
    public partial class ctrlSubirSoportes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    Tamano.Value = GetMaxRequestLength().ToString();
                }
                catch { }
            }
            if (Request.Files.Count > 0)
            {
                Seguridad.Ejecutar(Guardar);
            }
        }

        private void Guardar()
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String Ruta;
                String NombreTemp;
                String Nombre = Request.Files[0].FileName.Split('\\').Last().Split('/').Last().Replace(";", "").Replace(",", "");
                if (Request["Permanente"] != "1")
                {
                    Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                    NombreTemp = "adj" + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + Nombre;
                }
                else
                {
                    Ruta = db.Parametros.Single(x => x.Clave == "RutaArchivos").Valor;
                    Int32 Count = 0;
                    NombreTemp = Nombre;
                    while (File.Exists(Ruta + NombreTemp))
                    {
                        Count++;
                        NombreTemp = Nombre.Replace(".", "(" + Count.ToString() + ").");
                        if (Count == 100)
                        {
                            break;
                        }
                    }
                }
                //Nombre = "adj" + DateTime.Now.ToString("yyyyMMddHHmmssff") + "_" + Request.Files[0].FileName.Split('\\').Last().Split('/').Last().Replace(";", "").Replace(",", "");
                Ruta += NombreTemp;
                Request.Files[0].SaveAs(Ruta);
                ArchivoSubido.Value = NombreTemp;
            }
        }
        private int GetMaxRequestLength()
        {
            // presume that the section is not defined in the web.config
            int result = 4096;

            HttpRuntimeSection section =
            ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
            if (section != null) result = section.MaxRequestLength;

            return result;
        }
    }
}