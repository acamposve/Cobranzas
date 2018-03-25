using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Cobranzas.Emergentes
{
    public partial class ctrlReporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {
                    Int32 idTipoReporte = Convert.ToInt32(Request["idTipoReporte"]);
                    Entidades.TiposReporte TipoReporte = db.TiposReporte.Single(x => x.idTipoReporte == idTipoReporte);
                    lbltitulo.InnerText = TipoReporte.Nombre;
                    Int32 idCuenta = Convert.ToInt32(Request["idCuenta"] ?? "0");
                    Int32 idPersona = Convert.ToInt32(Request["idPersona"] ?? "0");
                    String Ruta = TipoReporte.Origen;
                    Regex ER = new Regex(@"{(?<Campo>\w+)(,(?<Inicio>\d+)(,(?<Longitud>\d+))?)?}");
                    PideParametros.Value = "0";
                    pnlResultado.InnerHtml = "";
                    foreach (Match Match in ER.Matches(Ruta))
                    {
                        String Campo = Match.Groups["Campo"].Value;
                        String Valor = "";
                        switch (Campo.ToLower())
                        {
                            case "codigocuenta":
                                Valor = db.Cuentas.Single(x => x.idCuenta == idCuenta).Codigo;
                                break;
                            case "idcuenta":
                                Valor = idCuenta.ToString();
                                break;
                            case "idpersona":
                                Valor = idPersona.ToString();
                                break;
                            case "codigopersona":
                                Valor = db.Personas.Single(x => x.idPersona == idPersona).Codigo;
                                break;
                            case "idcliente":
                                if (Request[Campo] != null)
                                {
                                    Valor = Request[Campo];
                                }
                                else
                                {
                                    PideParametros.Value = "1";
                                    pnlResultado.InnerHtml += "<br/>Cliente: <select id='idCliente' name='idCliente'> <script type='text/javascript'>LlenarClientes();</script>";
                                    continue;
                                }
                                break;
                            default:
                                if (Request[Campo] != null)
                                {
                                    Valor = Request[Campo];
                                }
                                else
                                {
                                    PideParametros.Value = "1";
                                    pnlResultado.InnerHtml += String.Format("<br/>{0}:<input type='Text' id='{0}' name='{0}'>", Campo);
                                    continue;
                                }
                                break;
                        }

                        if (Valor == "")
                        {
                            Ruta = Ruta.Replace(Match.Value, "");
                            continue;
                        }
                        Int32 Inicio = Convert.ToInt32(Match.Groups["Inicio"].Success ? Match.Groups["Inicio"].Value : "0");
                        Int32 Longitud = Convert.ToInt32(Match.Groups["Longitud"].Success ? Match.Groups["Longitud"].Value : "0");
                        if (Longitud < 0)
                        {
                            Inicio += Longitud; Longitud = -Longitud;
                        }
                        while (Inicio < 0)
                        {
                            Inicio = Inicio + Valor.Length;
                        }
                        if (Inicio == 0 && Longitud == 0)
                        {
                            Ruta = Ruta.Replace(Match.Value, Valor);
                            continue;
                        }
                        if (Longitud == 0)
                        {
                            Longitud = Valor.Length - Inicio;
                        }
                        Ruta = Ruta.Replace(Match.Value, Valor.Substring(Inicio, Longitud));
                    }
                    if (PideParametros.Value == "1")
                    {
                        btnEjecutar.Visible = true;
                        return;
                    }
                    if (TipoReporte.TipoOrigen == 2)
                    {
                        try
                        {
                            Response.Clear();
                            Response.Redirect(Ruta, true);
                            Response.Flush();
                            Response.End();
                        }
                        catch { }
                    }
                    else
                    {
                        Origen.Value = Ruta;
                    }
                }
            }
            catch (Exception Ex)
            {
                UI.Mensaje("Error", Ex.Informar().Message, "", Page);
            }
        }
        protected void btnEjecutar_Click(object sender, EventArgs e)
        { }

    }
}