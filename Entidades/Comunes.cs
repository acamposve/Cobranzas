using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cobranzas.OT;
using System.IO;
using System.Resources;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Globalization;
using Entidades;

namespace Cobranzas
{
    public static class Comunes
    {
        public static List<String> ObtenerTelefonos(String Telefonos)
        {
            List<String> Result = new List<String>();
            foreach (String Telefono in Telefonos.Split(','))
            {
                if (Telefono.Trim() == "") continue;
                String[] ColTelf = Telefono.Split('/');
                foreach (String TelefonoReal in ColTelf)
                {
                    try
                    {
                        if (TelefonoReal.Trim() == "") continue;
                        String TelefonoIns = (TelefonoReal.Length < 5) ? ColTelf[0].Substring(0, ColTelf[0].Length - TelefonoReal.Length) + TelefonoReal : TelefonoReal;
                        //if (!Persona.Telefonos.Any(x => x.Telefono == TelefonoIns))
                        //{
                        Result.Add(TelefonoIns);
                        //}
                    }
                    catch
                    {
                        Result.Add(TelefonoReal);
                    }
                }
            }
            return Result;
        }

        public static string AnalisisHead()
        {
            return
            "<style>" +
            ".enc {background-color: #000040;color:White;}" +
            ".norm{ background-color: #FFFFFF;}" +
            ".alt{background-color: #CCCCFF;}" +
            ".celda{border: 1px solid #000040;}" +
            ".monto{text-align:rigth;}" +
            "body{font-family: Verdana,Arial; font-size:small !important}" +
            "</style>";
        }
        public static string AnalisisReporte(Int32 idPersona, Int32 idOperador, List<Int32> Cuentas, Boolean Agrupado = false)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                String Ruta = db.Parametros.Single(x => x.Clave == "RutaTemporales").Valor;
                Ruta += @"AA" + idPersona.ToString() + "_" + idOperador.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
                StringBuilder Result = new StringBuilder();
                Result.Append(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                Result.Append("<html><head>");
                Result.Append(AnalisisHead());
                Result.Append("</head><body>");
                File.AppendAllText(Ruta, Result.ToString(), System.Text.Encoding.UTF8);
                Result = AnalisisCuerpo(idPersona, idOperador, db, Cuentas, Agrupado);
                Result.Append("</body></html>");
                File.AppendAllText(Ruta, Result.ToString(), System.Text.Encoding.UTF8);
                return Ruta;
            }
        }

        public static StringBuilder AnalisisCuerpo(Int32 idPersona, Int32 idOperador, CobranzasDataContext db, List<Int32> Cuentas, Boolean Agrupado = false, String Cultura = "es-VE")
        {
            StringBuilder Result = new StringBuilder();
            StringBuilder Encabezado = new StringBuilder();
            Debug.Print("Inicio:" + DateTime.Now.ToString("HH:mm:ss"));
            Entidades.Personas Persona = db.Personas.Single(x => x.idPersona == idPersona);
            //var Deudas = Persona.Cuentas.Where(x => x.Activa&&(x.Campanas_Cuentas.Any(y=>y.Campanas.TipoCampana==2)||x.Cuentas_Operadores.Any(y => y.idOperador == idOperador))).Select(x => otCuenta.FromCuenta(x)).ToList();
            List<otCuenta> Deudas;
            if (Cuentas == null || Cuentas.Count == 0)
            {
                Deudas = db.vwCuentas.Where(x => db.CuentasOperador(idOperador, idPersona).Any(y => y.idCuenta == x.idCuenta)).Select(x => otCuenta.FromvwCuenta(x)).ToList();
            }
            else
            {
                Deudas = db.vwCuentas.Where(x => Cuentas.Contains(x.idCuenta)).Select(x => otCuenta.FromvwCuenta(x)).ToList();
            }

            if (Agrupado)
            {
                Deudas = Deudas.OrderBy(x => x.Cliente).ToList();
            }
            Debug.Print("Despues de la Consulta:" + DateTime.Now.ToString("HH:mm:ss"));

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Cultura);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Cultura);

            if (Cultura.StartsWith("es"))
            {
                Result.Append("<b>Análisis de Antigüedad al " + DateTime.Now.Date.ToString("dd/MM/yyyy") + " Cliente: (" + Persona.Codigo + ") " + Persona.Nombre + "</b>");
                Encabezado.Append(@"<table style=""width:100%; border-collapse:collapse;font-size:x-small""><tr class=""enc"">");
                Encabezado.Append(@"<th class=""celda"">Documento" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Fecha" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Antigüedad" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Cliente" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Producto" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Total" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Restante" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Total(USD)" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Restante(USD)" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Total(Local)" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Restante(Local)" + "</th>");
#warning BL quemado
                Encabezado.Append(@"<th class=""celda"">BL" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Status" + "</th>");
            }
            else if (Cultura.StartsWith("en"))
            {
                Result.Append("<b>Outstanding Accounts " + DateTime.Now.Date.ToString("MM/dd/yyyy") + " Client: (" + Persona.Codigo + ") " + Persona.Nombre + "</b>");
                Encabezado.Append(@"<table style=""width:100%; border-collapse:collapse;font-size:x-small""><tr class=""enc"">");
                Encabezado.Append(@"<th class=""celda"">Document" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Date" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Overdue" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Client" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Product" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Total" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Remaining" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Total(USD)" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Remaining(USD)" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Total(Local)" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Remainimg(Local)" + "</th>");
#warning BL quemado
                Encabezado.Append(@"<th class=""celda"">BL" + "</th>");
                Encabezado.Append(@"<th class=""celda"">Status" + "</th>");
            }
            Encabezado.Append("</tr>");
            if (!Agrupado) Result.Append(Encabezado);
            Boolean Alt = true;
            Decimal TotalDolar = 0;
            Decimal DeudaDolar = 0;
            Decimal TotalLocal = 0;
            Decimal DeudaLocal = 0;
            Decimal TotalDolar2 = 0;
            Decimal DeudaDolar2 = 0;
            Decimal TotalLocal2 = 0;
            Decimal DeudaLocal2 = 0;
            String Tipo = General.Encriptar("CuentaSoporte");
            String Val = General.Encriptar(idPersona.ToString());
            int Total = Deudas.Count;
            String Externo = db.Parametros.Single(x => x.Clave == "Externos").Valor;
            String Cliente = "";
            Boolean Empezando = true;
            foreach (var Deuda in Deudas)
            {
                if (Cliente != Deuda.Cliente && Agrupado)
                {
                    if (!Empezando)
                    {
                        Result.Append(@"<tr class=""enc"">");
                        Result.Append(@"<td colspan=""7"">&nbsp;</td>");
                        Result.Append(@"<td style=""text-align:right"">" + TotalDolar2.ToString("N2") + "</td>");
                        Result.Append(@"<td style=""text-align:right"">" + DeudaDolar2.ToString("N2") + "</td>");
                        Result.Append(@"<td style=""text-align:right"">" + TotalLocal2.ToString("N2") + "</td>");
                        Result.Append(@"<td style=""text-align:right"">" + DeudaLocal2.ToString("N2") + "</td>");
                        Result.Append(@"<td colspan=""2"">&nbsp;</td>");
                        Result.Append("</tr>");
                        Result.Append("</table>");
                    }
                    TotalDolar2 = 0;
                    DeudaDolar2 = 0;
                    TotalLocal2 = 0;
                    DeudaLocal2 = 0;
                    Result.Append("<br><b>" + Deuda.Cliente + "</b>");
                    Result.Append(Encabezado);
                    Alt = true;
                    Empezando = false;
                }
                Alt = !Alt;
                Result.Append(@"<tr class=""" + (Alt ? "alt" : "norm") + @""">");
                Result.Append(@"<td class=""celda"" style=""text-align:left""><a href=""" + Externo + "?Tipo=" + Tipo + "&Val=" + Val + "&Id=" + Deuda.idCuenta.ToString() + "\">" + Deuda.Documento + "</a></td>");
                Result.Append(@"<td class=""celda"" style=""text-align:left"">" + Deuda.Fecha.AFechaMuyCorta() + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:center"">" + Deuda.Antiguedad.ToString() + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:left"">" + Deuda.CodigoCliente + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:left"">" + Deuda.Producto + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:right"">" + Deuda.Total.ToString("N2") + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:right"">" + Deuda.Deuda.ToString("N2") + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:right"">" + Deuda.TotalDolar.ToString("N2") + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:right"">" + Deuda.DeudaDolar.ToString("N2") + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:right"">" + Deuda.TotalLocal.ToString("N2") + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:right"">" + Deuda.DeudaLocal.ToString("N2") + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:left"">" + Deuda.CampoExtra + "</td>");
                Result.Append(@"<td class=""celda"" style=""text-align:left"">" + Deuda.Status + "</td>");
                Result.Append("</tr>");
                TotalDolar += Deuda.TotalDolar;
                DeudaDolar += Deuda.DeudaDolar;
                TotalLocal += Deuda.TotalLocal;
                DeudaLocal += Deuda.DeudaLocal;

                TotalDolar2 += Deuda.TotalDolar;
                DeudaDolar2 += Deuda.DeudaDolar;
                TotalLocal2 += Deuda.TotalLocal;
                DeudaLocal2 += Deuda.DeudaLocal;
                Cliente = Deuda.Cliente;
            }
            if (Agrupado)
            {
                Result.Append(@"<tr class=""enc"">");
                Result.Append(@"<td colspan=""7"">&nbsp;</td>");
                Result.Append(@"<td style=""text-align:right"">" + TotalDolar2.ToString("N2") + "</td>");
                Result.Append(@"<td style=""text-align:right"">" + DeudaDolar2.ToString("N2") + "</td>");
                Result.Append(@"<td style=""text-align:right"">" + TotalLocal2.ToString("N2") + "</td>");
                Result.Append(@"<td style=""text-align:right"">" + DeudaLocal2.ToString("N2") + "</td>");
                Result.Append(@"<td colspan=""2"">&nbsp;</td>");
                Result.Append("</tr>");
                //Result.Append("</table>");

            }

            Result.Append(@"<tr class=""enc"">");
            Result.Append(@"<td colspan=""7"">Total General</td>");
            Result.Append(@"<td style=""text-align:right"">" + TotalDolar.ToString("N2") + "</td>");
            Result.Append(@"<td style=""text-align:right"">" + DeudaDolar.ToString("N2") + "</td>");
            Result.Append(@"<td style=""text-align:right"">" + TotalLocal.ToString("N2") + "</td>");
            Result.Append(@"<td style=""text-align:right"">" + DeudaLocal.ToString("N2") + "</td>");
            Result.Append(@"<td colspan=""2"">&nbsp;</td>");
            Result.Append("</tr>");
            Result.Append("</table>");

            //Result.Append("Estimado Cliente: Lo invitamos a realizar su depósito o pago a través de nuestra PLANILLA PERSONALIZADA que ha puesto a disposición el banco BBVA Provincial para todos nuestros clientes y consignatarios, la cual encontrará adjunto a este correo.");
            Result.Append("<br/><br/>");
            return Result;
        }

        /*public static string GenerarFirma(int idOperador)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Operadores Operador = db.Operadores.Single(x => x.idOperador == idOperador);
                if (Operador.FirmaCorreo != "") return Operador.FirmaCorreo;
                String Firma = db.Parametros.Single(x => x.Clave == "FirmaCorreo").Valor;
                Firma = Firma.Replace("{Nombre}", Operador.Nombre);
                Firma = Firma.Replace("{Correo}", Operador.Correo);
                Firma = Firma.Replace("{Login}", Operador.Login);
                Firma = Firma.Replace("{Cargo}", Operador.Cargo);
                Firma = Firma.Replace("{Codigo}", Operador.Codigo);
                Firma = Firma.Replace("{Grupo}", Operador.Grupos.Nombre);
                Firma = Firma.Replace("{Telefono}", Operador.Telefonos);
                Firma = Firma.Replace("{TelefonoOficina}", Operador.Oficinas.Telefonos);
                Firma = Firma.Replace("{FaxOficina}", Operador.Oficinas.Fax);
                Firma = Firma.Replace("{Extension}", Operador.Extension);
                Firma = Firma.Replace("{Zona}", Operador.Zona);
                Firma = Firma.Replace("{Pais}", Operador.Pais);
                Firma = Firma.Replace("{CiudadOficina}", Operador.Oficinas.Ciudad);
                Firma = Firma.Replace("{PaisOficina}", Operador.Oficinas.Pais);
                return Firma;
            }
        }*/
        public static List<Int32> Clones(Int32 idOperador, CobranzasDataContext db)
        {
            List<Int32> result = db.Operadores.Where(x => x.idClon == idOperador).Select(x => x.idOperador).ToList();
            result.Add(idOperador);
            return result.Distinct().ToList();
        }
        public static List<Int32> OperadoresSupervisados(Int32 idOperador, CobranzasDataContext db)
        {
            List<Int32> result = (from c in Clones(idOperador, db) from d in db.OperadoresSupervisados(c, "").ToList() select d.idOperador ?? 0).ToList();
            return result.Distinct().ToList();
        }
        public static otPlantillasCorreo LlenarPlantilla(Int32 idPlantilla, Int32? idPersona, Int32 idOperador)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Plantillas Plantilla = db.Plantillas.Single(x => x.idPlantilla == idPlantilla);

                String Mensaje = Plantilla.Mensaje;
                String Asunto = Plantilla.Asunto;
                if (idPersona.HasValue)
                {
                    var Cuentas = db.CuentasOperador(idOperador, idPersona);
                    Entidades.Personas Persona = db.Personas.Single(x => x.idPersona == idPersona);
                    //otPersona Persona = Personas_sel(idPersona.Value, idOperador);
                    String DeudaLocal = Cuentas.Sum(x => x.MontoRestante * x.CambioLocal).ToString("N2");
                    String DeudaDolar = Cuentas.Sum(x => x.MontoRestante / x.CambioDolar).ToString("N2") + "USD";
                    String MinimaAntiguedad = (DateTime.Now.Date - Cuentas.OrderByDescending(x => x.FechaDocumento).First().FechaDocumento.Value.Date).TotalDays.ToString();
                    String MaximaAntiguedad = (DateTime.Now.Date - Cuentas.OrderBy(x => x.FechaDocumento).First().FechaDocumento.Value.Date).TotalDays.ToString();
                    try
                    {
                        DeudaLocal += db.Personas.Single(x=>x.idPersona==Cuentas.First().idPersona).Paises.idMoneda;
                    }
                    catch { }
                    Int32 Cantidad = Cuentas.Count();
                    Mensaje = Mensaje.Replace("{fecha}", DateTime.Now.AFechaCorta());
                    Mensaje = Mensaje.Replace("{persona}", Persona.Nombre);
                    Mensaje = Mensaje.Replace("{rif}", Persona.Rif);
                    Mensaje = Mensaje.Replace("{codpersona}", Persona.Codigo);
                    Mensaje = Mensaje.Replace("{contacto}", Persona.Contacto);
                    Mensaje = Mensaje.Replace("{deudalocal}", DeudaLocal);
                    Mensaje = Mensaje.Replace("{deudadolar}", DeudaDolar);
                    Mensaje = Mensaje.Replace("{minimaantiguedad}", MinimaAntiguedad);
                    Mensaje = Mensaje.Replace("{maximaantiguedad}", MaximaAntiguedad);
                    Mensaje = Mensaje.Replace("{facturas}", Cantidad.ToString());
                    Mensaje = Mensaje.Replace("{pais}", Persona.Paises.Nombre);
                    Mensaje = Mensaje.Replace("{codpais}", Persona.idPais);

                    Asunto = Asunto.Replace("{fecha}", DateTime.Now.AFechaCorta());
                    Asunto = Asunto.Replace("{persona}", Persona.Nombre);
                    Asunto = Asunto.Replace("{rif}", Persona.Rif);
                    Asunto = Asunto.Replace("{codpersona}", Persona.Codigo);
                    Asunto = Asunto.Replace("{contacto}", Persona.Contacto);
                    Asunto = Asunto.Replace("{deudalocal}", DeudaLocal);
                    Asunto = Asunto.Replace("{deudadolar}", DeudaDolar);
                    Asunto = Asunto.Replace("{minimaantiguedad}", MinimaAntiguedad);
                    Asunto = Asunto.Replace("{maximaantiguedad}", MaximaAntiguedad);
                    Asunto = Asunto.Replace("{facturas}", Cantidad.ToString());
                    Asunto = Asunto.Replace("{pais}", Persona.Paises.Nombre);
                    Asunto = Asunto.Replace("{codpais}", Persona.idPais);
                }
                return new otPlantillasCorreo
                {
                    Asunto = Asunto,
                    Mensaje = Mensaje,
                    Adjunto = Plantilla.Adjunto,
                    DestinatariosCopia = Plantilla.DestinatariosCopia,
                    DestinatariosCopiaOculta = Plantilla.DestinatariosCopiaOculta,
                    idPais = Plantilla.idPais,
                    idTipoCliente = Plantilla.idTipoCliente
                };
            }
        }
    }
}
