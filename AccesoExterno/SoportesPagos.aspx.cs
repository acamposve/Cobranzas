using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cobranzas
{
    public partial class SoportesPagos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Url.Host.ToUpper() != "VECCSVS008" && Request.Url.Host.ToUpper() != "USMIAVS021" && Request.Url.Host.ToUpper() != "LOCALHOST")
            {
                Response.Clear();
                Response.Write(Request.Url.Host);
                Response.Write(": No se puede consultar desde fuera de Veconinter");
                Response.Flush();
                Response.End();
                return;
            }
            if (!IsPostBack)
            {
                if (Request["idPais"] != null)
                {
                    cboPais.SelectedValue = Request["idPais"];
                }
                if (Request["idPago"] != null)
                {
                    txtCodigo.Text = Request["idPago"];
                }
                if (Request["Codigo"] != null)
                {
                    txtCodigo.Text = Request["Codigo"];
                }
                btnBuscar_Click(null, null);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Pagos Pago = cboPais.SelectedValue == "VEN" ? db.Pagos.SingleOrDefault(x => x.Codigo == txtCodigo.Text && x.Personas.idPais == cboPais.SelectedValue) : db.Pagos.SingleOrDefault(x => x.idPago == Convert.ToInt32(txtCodigo.Text));
                if (Pago == null)
                {
                    pnlResultados.InnerHtml = "No se ha encontrado el Pago";
                    idPago.Value = "";
                    return;
                }
                idPago.Value = Pago.idPago.ToString();
                //Datos
                lblTipoPago.Text = Pago.TipoPago == 1 ? "Depósito" : "Tranferencias";
                lblFechaNuevoPago.Text = Pago.Fecha.AFechaCorta();
                lblReferencia.Text = Pago.Referencia;
                lblBancoDestino.Text = Pago.BancosPropios.Descripcion;
                lblMoneda.Text = Pago.idMoneda;
                lblEfectivo.Text = Pago.MontoEfectivo.ToString("N2");
                lblOperador.Text = Pago.Operadores.Nombre;
                trCheques.Visible = (Pago.TipoPago == 1);
                trTransferencia.Visible = (Pago.TipoPago == 2);
                lblBancoOrigen.Text = Pago.Bancos != null ? Pago.Bancos.Descripcion : "";
                lblDescripcion.Text = Pago.Descripcion;
                Decimal MontoTotal = (Pago.MontoTotal ?? 0);
                Decimal MontoAplicacion = Pago.Pagos_Cuentas.Sum(x => (Decimal?)x.Monto) ?? 0;
                Decimal Retenciones = Pago.Pagos_Cuentas.Sum(x => ((x.Cuentas.MontoIva ?? 0) * (x.Retencion1 ?? 0) + (x.Cuentas.MontoBase ?? 0) * (x.Retencion2 ?? 0)) / 100);
                Decimal Restante = MontoTotal - MontoAplicacion - Retenciones;
                lblTotalPago.Text = MontoTotal.ToString("N2");
                lblTotalAplicacion.Text = MontoAplicacion.ToString("N2");
                lblTotalRetenciones.Text = Retenciones.ToString("N2");
                lblMontoRestante.Text = Restante.ToString("N2");
                //Cheques
                pnlCheques.InnerHtml += "<table class='TablaDatos'><tr><th>Nro Cheque</th><th>Banco Cheque</th><th>Monto Cheque</th>";
                foreach (Entidades.PagosDet Cheque in Pago.PagosDet)
                {
                    pnlCheques.InnerHtml += "<tr><td>" + Cheque.NroCheque + "</td><td>" + Cheque.Bancos.Descripcion + "</td><td>" + Cheque.Monto.ToString("N2") + "</td></tr>";
                }
                pnlCheques.InnerHtml += "</table>";
                //Soportes
                IEnumerable<Entidades.Soportes> Soportes = db.Soportes.Where(x => x.idTabla == Pago.idPago && x.Tabla == "Pagos");
                if (Soportes.Count() == 0)
                {
                    pnlResultados.InnerHtml = "No se han Encontrado Soportes para el Pago";
                    return;
                }
                pnlResultados.InnerHtml = "";
                foreach (Entidades.Soportes Soporte in Soportes)
                {
                    pnlResultados.InnerHtml += "<a href='Default.aspx?Tipo=" + General.Encriptar("PagoSoporte") + "&Id=" + Soporte.idSoporte.ToString() + "&Val=" + General.Encriptar(Soporte.idTabla.ToString()) + "' target='_blank' class='Telefono'>" + Soporte.Nombre + "</a><br/>";
                }
                //Facturas
                pnlFacturasSelecionadas.InnerHtml = "<table class='TablaDatos'><tr><th>Factura</th><th>Monto</th><th>Retencion1</th><th>Retencion2</th></tr>";
                foreach (Entidades.Pagos_Cuentas PC in Pago.Pagos_Cuentas)
                {
                    pnlFacturasSelecionadas.InnerHtml += "<tr>" + "<td>" + PC.Cuentas.Codigo + "</td>" + "<td>" + PC.Monto.ToString() + "</td>" + "<td>" + (PC.Retencion1 ?? 0).ToString() + "</td>" + "<td>" + (PC.Retencion2 ?? 0).ToString() + "</td>" + "</tr>";
                }
                pnlFacturasSelecionadas.InnerHtml += "</table>";

                //Acciones
                SCI.Visible = cboPais.SelectedValue != "VEN" && String.IsNullOrEmpty(Pago.Codigo);

            }
        }

        protected void btnRechazarPago_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Pagos Pago;
                try
                {
                    Pago = db.Pagos.Single(x => x.idPago == Convert.ToInt32(this.idPago.Value));
                    Pago.idStatusPago = 7;//Rechazado Procesamiento
                    Pago.Resultado = txtRechazo.Text;
                    Pago.FechaResultado = DateTime.Now;
                    Pago.Confirmado = false;
                    Pago.Aprobado = false;
                    Entidades.Avisos Aviso = new Entidades.Avisos();
                    Aviso.Aviso = "El Pago del cliente(" + Pago.Personas.idPais + "): " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha sido rechazado por el SCI por el siguiente motivo: " + txtRechazo.Text;
                    Aviso.FechaAviso = DateTime.Now.AddMinutes(5);
                    Aviso.FechaCancelado = null;
                    Aviso.FechaOriginal = DateTime.Now.AddMinutes(5);
                    Aviso.FechaCrea = DateTime.Now;
                    Aviso.idOperador = Pago.idOperadorCrea ?? Pago.idOperador ?? 1;
                    Aviso.idOperadorCrea = 1;
                    Aviso.idPersona = Pago.idPersona;
                    Aviso.VecesMostrada = 0;
                    db.Avisos.InsertOnSubmit(Aviso);
                    db.SubmitChanges();

                }
                catch (Exception Ex)
                {
                    Response.Write("Error Rechazando el Pago");
                    return;
                }
            }
        }
        protected void btnLlevarPago_Click(object sender, EventArgs e)
        {
            using (CobranzasDataContext db = new CobranzasDataContext())
            {
                Entidades.Pagos Pago;
                try
                {
                    Pago = db.Pagos.Single(x => x.idPago == Convert.ToInt32(this.idPago.Value));
                }
                catch (Exception Ex)
                {
                    Response.Write("Error Llevando el Pago a SCI");
                    return;
                }
                int? idOrigen = Pago.Pagos_Cuentas.First().Cuentas.idOrigen;
                Entidades.Origenes Origen = db.Origenes.Single(x => x.idOrigen == idOrigen);
                //OleDbConnection Conn = new OleDbConnection("Provider=SQLOLEDB.1;Persist Security Info=False;User ID=mberroteran;Password=mberroteran;Initial Catalog=SCI;Data Source=VECCSVS020");
                OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString);//"Provider=SQLOLEDB.1;Persist Security Info=False;User ID=mberroteran;Password=mberroteran;Initial Catalog=milleniumv2;Data Source=VECCSVS020");
                Conn.Open();

                OleDbCommand Comm = Conn.CreateCommand();
                Comm.CommandTimeout = 10 * 60;
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.CommandText = "transact_Cobros";

                Comm.Parameters.Add("transactId", OleDbType.Integer);
                Comm.Parameters.Add("cobroPaisId", OleDbType.Integer).Direction = ParameterDirection.Output;
                Comm.Parameters.Add("fechaCobro", OleDbType.Date).Direction = ParameterDirection.Output;
                Comm.Parameters.Add("empresaPaisId", OleDbType.Integer);
                Comm.Parameters.Add("descripcion", OleDbType.VarChar);
                Comm.Parameters.Add("ptocreacion", OleDbType.VarChar);
                Comm.Parameters.Add("moneda", OleDbType.Boolean);
                Comm.Parameters.Add("cobradorPaisId", OleDbType.Integer);
                Comm.Parameters.Add("clientePaisId", OleDbType.Integer);

                OleDbCommand Comm2 = Conn.CreateCommand();
                Comm2.CommandTimeout = 10 * 60;
                Comm2.CommandType = CommandType.StoredProcedure;
                Comm2.CommandText = "transact_entradasbanco";

                Comm2.Parameters.Add("transactId", OleDbType.Integer);
                Comm2.Parameters.Add("entradaID", OleDbType.Integer).Direction = ParameterDirection.Output;
                Comm2.Parameters.Add("empresaPaisId", OleDbType.Integer);
                Comm2.Parameters.Add("cuentaID", OleDbType.Integer);
                Comm2.Parameters.Add("fecharecepcion", OleDbType.Date);
                Comm2.Parameters.Add("transferencia", OleDbType.Boolean);
                Comm2.Parameters.Add("numdeposito", OleDbType.Integer);
                Comm2.Parameters.Add("cobroPaisId", OleDbType.Integer);
                Comm2.Parameters.Add("montoefectivo", OleDbType.Decimal);


                OleDbCommand Comm3 = Conn.CreateCommand();
                Comm3.CommandTimeout = 10 * 60;
                Comm3.CommandType = CommandType.StoredProcedure;
                Comm3.CommandText = "transact_cheques";

                Comm3.Parameters.Add("transactId", OleDbType.Integer);
                Comm3.Parameters.Add("IDcheque", OleDbType.Integer).Direction = ParameterDirection.Output;
                Comm3.Parameters.Add("entradaID", OleDbType.Integer);
                Comm3.Parameters.Add("chequeId", OleDbType.VarChar);
                Comm3.Parameters.Add("chequebco", OleDbType.VarChar);
                Comm3.Parameters.Add("montocheque", OleDbType.Decimal);

                //Comm3.Parameters.Add("IDcheque", OleDbType.Numeric).Direction = ParameterDirection.Output;
                //Comm3.Parameters.Add("entradaID", OleDbType.Numeric);
                //Comm3.Parameters.Add("chequeid", OleDbType.Char);
                //Comm3.Parameters.Add("chequebco", OleDbType.Char);
                //Comm3.Parameters.Add("montocheque", OleDbType.Numeric);
                //Comm3.Parameters.Add("cobroid", OleDbType.Numeric);
                //Comm3.Parameters.Add("bancoid", OleDbType.Char);
                //Comm3.Parameters.Add("numdeposito", OleDbType.Char);
                //Comm3.Parameters.Add("chequefecha", OleDbType.Char);
                //Comm3.Parameters.Add("bolivar", OleDbType.Boolean);

                OleDbCommand Comm4 = Conn.CreateCommand();
                Comm4.CommandTimeout = 10 * 60;
                Comm4.CommandType = CommandType.StoredProcedure;
                Comm4.CommandText = "Cobranzas.Insertar_Pagos_Cuentas";
                Comm4.Parameters.Add("CodigoPago", OleDbType.VarChar);
                Comm4.Parameters.Add("CodigoCuenta", OleDbType.VarChar);
                Comm4.Parameters.Add("Monto", OleDbType.Decimal);
                Comm4.Parameters.Add("Retencion1", OleDbType.Decimal);
                Comm4.Parameters.Add("Retencion2", OleDbType.Decimal);
                Comm4.Parameters.Add("Pais", OleDbType.Char, 3);

                OleDbCommand Comm5 = Conn.CreateCommand();
                Comm5.CommandTimeout = 10 * 60;
                Comm5.CommandType = CommandType.Text;
                Comm5.CommandText = "SELECT Empresapaisid FROM OEmpresapais as ep inner join opais as p on ep.pais=p.pais where p.iso3='" + Pago.Personas.idPais + "'";
                Int32 EmpresaPaisId = Convert.ToInt32(Comm5.ExecuteScalar());

                Comm5.CommandText = "select cuentaid from tbancocuenta as bc inner join tbanco as  b on bc.bancoid=b.bancoid where b.bancopaisid='" + Pago.BancosPropios.Bancos.Codigo + "' and b.empresapaisid=" + EmpresaPaisId.ToString() + " and bc.numcuenta='" + Pago.BancosPropios.NroCuenta + "'";
                Int32 CuentaId = Convert.ToInt32(Comm5.ExecuteScalar());

                Comm5.CommandText = "SELECT Top 1 OS.puertoid FROM dstEstacion AS ES INNER JOIN SubRedSerie AS SS ON SS.subrID=ES.subrID INNER JOIN oOficSerie AS OS ON OS.serie=SS.serieDMDG AND OS.empresaPaisID=SS.empresaPaisID INNER JOIN oEmpresapais as EP on SS.Empresapaisid=Ep.empresapaisid INNER JOIN oPais as P on Ep.pais=p.pais WHERE ES.usuario=SUSER_NAME()and p.iso3='" + Pago.Personas.idPais + "'";
                String PuertoCreacion = Convert.ToString(Comm5.ExecuteScalar());

                OleDbTransaction Trans = Conn.BeginTransaction();
                Boolean PagoListo = false;
                try
                {
                    Comm.Transaction = Trans;
                    Comm2.Transaction = Trans;
                    Comm3.Transaction = Trans;
                    Comm4.Transaction = Trans;


                    //    String PuertoCreacion = Pago.Pagos_Cuentas.First().Cuentas.Datos.Descendants("Dato").Where(x => x.Attribute("Clave").Value == "Port").First().Value;
                    Comm.Parameters["transactId"].Value = 1;
                    Comm.Parameters["empresaPaisId"].Value = EmpresaPaisId;
                    Comm.Parameters["descripcion"].Value = Pago.Descripcion;
                    Comm.Parameters["ptocreacion"].Value = PuertoCreacion;
                    Comm.Parameters["moneda"].Value = Pago.idMoneda == "USD" ? 0 : 1;
                    Comm.Parameters["cobradorPaisId"].Value = db.Operadores_Asignaciones.Single(x => x.idOperador == Pago.idOperador && x.idPais == Pago.Personas.idPais).Codigo;
                    Comm.Parameters["clientePaisId"].Value = Pago.Personas.Codigo;
                    Comm.ExecuteNonQuery();
                    Pago.Codigo = Comm.Parameters["cobroPaisId"].Value.ToString();
                    //Comm.Parameters["fechaCobro", OleDbType.Date).Direction = ParameterDirection.Output;

                    Comm2.Parameters["transactId"].Value = 1;
                    Comm2.Parameters["empresaPaisId"].Value = EmpresaPaisId;
                    Comm2.Parameters["cuentaID"].Value = CuentaId;
                    Comm2.Parameters["fecharecepcion"].Value = Pago.Fecha;
                    Comm2.Parameters["transferencia"].Value = Pago.TipoPago == 2;
                    Comm2.Parameters["numdeposito"].Value = Pago.Referencia;
                    Comm2.Parameters["cobroPaisId"].Value = Pago.Codigo;
                    Comm2.Parameters["montoefectivo"].Value = Pago.MontoEfectivo;
                    Comm2.ExecuteNonQuery();
                    Int32 Entrada = Convert.ToInt32(Comm2.Parameters["entradaID"].Value);

                    foreach (Entidades.Pagos_Cuentas PC in Pago.Pagos_Cuentas)
                    {
                        Comm4.Parameters["CodigoPago"].Value = Pago.Codigo;
                        Comm4.Parameters["CodigoCuenta"].Value = PC.Cuentas.Codigo;
                        Comm4.Parameters["Monto"].Value = PC.Monto;
                        Comm4.Parameters["Retencion1"].Value = PC.Retencion1;
                        Comm4.Parameters["Retencion2"].Value = PC.Retencion2;
                        Comm4.Parameters["Pais"].Value = Pago.Personas.idPais;
                        Comm4.ExecuteNonQuery();
                    }

                    foreach (Entidades.PagosDet Cheque in Pago.PagosDet)
                    {
                        Comm3.Parameters["transactId"].Value = 1;
                        Comm3.Parameters["entradaID"].Value = Entrada;
                        Comm3.Parameters["chequeId"].Value = Cheque.NroCheque;
                        Comm3.Parameters["chequebco"].Value = Cheque.Bancos.Codigo;
                        Comm3.Parameters["montocheque"].Value = Cheque.Monto;
                        Comm3.ExecuteNonQuery();
                        Cheque.Codigo = Comm3.Parameters["IDcheque"].Value.ToString();

                    }
                    Trans.Commit();
                    PagoListo = true;
                    Pago.idStatusPago = 6;//Aprobado Procesamiento
                    ClientScript.RegisterStartupScript(typeof(Page), "Aviso", "alert('El código del Pago Generado es:" + Pago.Codigo + "')");

                    db.SubmitChanges();

                    try//Crear Gestión
                    {
                        Int32 Status = Convert.ToInt32(db.Parametros.Single(x => x.Clave == "_STPago" + Pago.Personas.idPais).Valor);
                        Entidades.Gestiones Gestion = new Entidades.Gestiones();
                        Gestion.Descripcion = Pago.Descripcion;
                        db.Gestiones.InsertOnSubmit(Gestion);
                        Gestion.Fecha = DateTime.Now;
                        Gestion.idOperador = Pago.idOperadorCrea.Value;
                        Gestion.idPersona = Pago.idPersona;
                        Gestion.idStatus = Status;
                        foreach (Int32 idCuenta in Pago.Pagos_Cuentas.Select(x => x.idCuenta))
                        {
                            Entidades.Cuentas_Gestiones CG = new Entidades.Cuentas_Gestiones();
                            CG.idCuenta = idCuenta;
                            Gestion.Cuentas_Gestiones.Add(CG);
                        }
                        db.SubmitChanges();
                    }
                    catch (Exception Ex)
                    {
                    }

                }
                catch (Exception Ex)
                {
                    if (!PagoListo)
                    {
                        Response.Write("Error Llevando Pago " + Pago.idPago + "Mensaje: " + Ex.Message);
                        Trans.Rollback();
                        db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, Pago);
                        Pago.Codigo = null;
                        Pago.Resultado = Ex.Message;
                        Pago.FechaResultado = DateTime.Now;
                        Pago.Confirmado = false;
                        Pago.Aprobado = false;
                        Pago.idStatusPago = 2;
                        db.SubmitChanges();
                        Entidades.Avisos Aviso = new Entidades.Avisos();

                        Aviso.Aviso = "El Pago del cliente(" + Pago.Personas.idPais + "): " + Pago.Personas.Codigo + ", Referencia: " + Pago.Referencia + ", Ha sido rechazado por el SCI por el siguiente motivo: " + Ex.Message;
                        Aviso.FechaAviso = DateTime.Now.AddMinutes(5);
                        Aviso.FechaCancelado = null;
                        Aviso.FechaOriginal = DateTime.Now.AddMinutes(5);
                        Aviso.FechaCrea = DateTime.Now;
                        Aviso.idOperador = Pago.idOperadorCrea ?? Pago.idOperador ?? 1;
                        Aviso.idOperadorCrea = 1;
                        Aviso.idPersona = Pago.idPersona;
                        Aviso.VecesMostrada = 0;
                        db.Avisos.InsertOnSubmit(Aviso);
                        db.SubmitChanges();
                    }

                }

            }
        }
    }
}