using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.OleDb;
using Entidades;

namespace Cobranzas
{
    public partial class PerfilCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            pnlExportar.Visible = true;
            //            Response.BinaryWrite(System.Text.UTF8Encoding.UTF8.GetPreamble());
            try
            {
                using (CobranzasDataContext db = new CobranzasDataContext())
                {

                    Int32 idPersona = Convert.ToInt32(Request["idPersona"]);
                    DateTime Desde = Convert.ToDateTime(Request["FechaDesde"]);
                    DateTime Hasta = Convert.ToDateTime(Request["FechaHasta"]);
                    Int32 idCliente = Convert.ToInt32(Request["idCliente"] ?? "0");
                    Boolean IncluyeComentario = Request["Comentario"] != null;

                    Personas Persona = db.Personas.Single(x => x.idPersona == idPersona);
                    General.FormatoFecha = "dd/MM/yyyy";
                    General.FormatoFechaW = "ddd dd/MM/yyyy";
                    lblFechaDesde.Text = Desde.AFechaMuyCorta();
                    lblFechaHasta.Text = Hasta.AFechaMuyCorta();
                    lblNaviera.Text = idCliente == 0 ? "Todas" : db.Clientes.Single(x => x.idCliente == idCliente).Nombre;
                    lblNombre.Text = Persona.Nombre;
                    lblRif.Text = Persona.Rif;
                    lblCodigo.Text = Persona.Codigo;
                    lblPais.Text = Persona.Paises.Nombre;
                    lblDireccionFiscal.Text = Persona.DireccionFiscal;
                    lblDireccionEntrega.Text = Persona.DireccionEntrega;
                    lblDeudaLocal.Text = Persona.Cuentas.Where(x => x.Activa && (idCliente == 0 || x.idCliente == idCliente)).Sum(x => x.MontoRestante * x.CambioLocal).ToString("N") + Persona.Paises.idMoneda;
                    lblDeudaDolar.Text = Persona.Cuentas.Where(x => x.Activa && (idCliente == 0 || x.idCliente == idCliente)).Sum(x => x.MontoRestante / x.CambioDolar).ToString("N") + "USD";
                    lblCantidadFacturas.Text = Persona.Cuentas.Count(x => x.Activa && (idCliente == 0 || x.idCliente == idCliente)).ToString();
                    var Pagos = db.Pagos.Where(x => x.idPersona == idPersona && x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta).OrderByDescending(x => x.Fecha);
                    Pagos Pago = Pagos.FirstOrDefault();
                    lblUltimoPago.Text = Pago == null ? "Sin Pagos en el Período" : Pago.Fecha.AFechaCorta();
                    lblCantidadPagos.Text = Pagos.Count().ToString();
                    var Gestiones = Persona.Gestiones.Where(x => (idCliente == 0 || x.Cuentas_Gestiones.Any(y => y.Cuentas.idCliente == idCliente)) && x.Status.Tipo != "Sistema" & x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta).OrderByDescending(x => x.Fecha);
                    var Llamadas = db.Llamadas.Where(x => (x.Telefonos.idPersona == idPersona || x.Telefonos.PersonasContacto.idPersona == idPersona) && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta));
                    Gestiones Gestion = Gestiones.First();
                    lblStatus.Text = Gestion.Status.Nombre;
                    lblGestion.Text = Gestion.Descripcion;
                    lblGestionesTotales.Text = Gestiones.Count().ToString();
                    lblLlamadasContestadas.Text = Llamadas.Where(x => x.StatusPrimario == "Contestado" && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).Count().ToString();
                    lblLlamadasNoContestadas.Text = Llamadas.Where(x => x.StatusPrimario != "Contestado" && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).Count().ToString();
                    lblEnviosAutomaticos.Text = Persona.Gestiones.Count(x => x.idStatus == 18).ToString();
                    lblCorreos.Text = db.Correos.Count(x => x.RutaEml == null && x.Correos_Personas.Any(y => y.idPersona == idPersona) && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).ToString();
                    lblCorreosAsignados.Text = db.Correos.Count(x => x.RutaEml != null && x.Correos_Personas.Any(y => y.idPersona == idPersona) && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).ToString();
                    lblPagosProcesados.Text = db.Pagos.Count(x => x.idPersona == idPersona && x.idOperador != null && x.Confirmado == true && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).ToString();
                    lblReclamos.Text = db.Reclamos.Count(x => x.Codigo != null && x.idOperador != null && x.Cuentas_Reclamos.Any(y => y.Cuentas.idPersona == idPersona) && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).ToString();
                    lblCompromisos.Text = db.Compromisos.Count(x => x.idPersona == idPersona && (x.Fecha.Date >= Desde && x.Fecha.Date <= Hasta)).ToString();
                    lblAvisos.Text = db.Avisos.Count(x => x.idPersona == idPersona && (x.FechaAviso.Date >= Desde && x.FechaAviso.Date <= Hasta)).ToString();
                    lblVisitas.Text = Gestiones.Count(x => x.idStatus == 12).ToString();
                    Origenes Origen = Persona.Cuentas.First().Origenes;
                    using (OleDbConnection Conn = new OleDbConnection(Origen.ConnectionString))
                    {
                        OleDbCommand Comm = Conn.CreateCommand();
                        Comm.CommandType = CommandType.StoredProcedure;
                        Comm.CommandText = "Cobranzas.TiempoPago";
                        Comm.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = Persona.Codigo;
                        Comm.Parameters.Add("Pais", OleDbType.Char, 3).Value = Persona.idPais;
                        Comm.Parameters.Add("Modo", OleDbType.Integer).Value = 0;
                        DataSet DS = new DataSet();
                        OleDbDataAdapter DA = new OleDbDataAdapter(Comm);
                        DA.Fill(DS);
                        DataRow Fila = DS.Tables[0].Rows[0];
                        lblTF1.Text = Convert.ToInt32(Fila["C00_15"]).ToString();
                        lblTF2.Text = Convert.ToInt32(Fila["C16_30"]).ToString();
                        lblTF3.Text = Convert.ToInt32(Fila["C31_45"]).ToString();
                        lblTF4.Text = Convert.ToInt32(Fila["C46_60"]).ToString();
                        lblTF5.Text = Convert.ToInt32(Fila["C61_90"]).ToString();
                        lblTF6.Text = Convert.ToInt32(Fila["C91_"]).ToString();
                        lblTM1.Text = Convert.ToDecimal(Fila["M00_15"]).ToString();
                        lblTM2.Text = Convert.ToDecimal(Fila["M16_30"]).ToString();
                        lblTM3.Text = Convert.ToDecimal(Fila["M31_45"]).ToString();
                        lblTM4.Text = Convert.ToDecimal(Fila["M46_60"]).ToString();
                        lblTM5.Text = Convert.ToDecimal(Fila["M61_90"]).ToString();
                        lblTM6.Text = Convert.ToDecimal(Fila["M91_"]).ToString();

                        Comm.Parameters["Modo"].Value = 1;
                        DS = new DataSet();
                        DA = new OleDbDataAdapter(Comm);
                        DA.Fill(DS);
                        Fila = DS.Tables[0].Rows[0];
                        lblCF1.Text = Convert.ToInt32(Fila["C00_15"]).ToString();
                        lblCF2.Text = Convert.ToInt32(Fila["C16_30"]).ToString();
                        lblCF3.Text = Convert.ToInt32(Fila["C31_45"]).ToString();
                        lblCF4.Text = Convert.ToInt32(Fila["C46_60"]).ToString();
                        lblCF5.Text = Convert.ToInt32(Fila["C61_90"]).ToString();
                        lblCF6.Text = Convert.ToInt32(Fila["C91_"]).ToString();
                        lblCM1.Text = Convert.ToDecimal(Fila["M00_15"]).ToString();
                        lblCM2.Text = Convert.ToDecimal(Fila["M16_30"]).ToString();
                        lblCM3.Text = Convert.ToDecimal(Fila["M31_45"]).ToString();
                        lblCM4.Text = Convert.ToDecimal(Fila["M46_60"]).ToString();
                        lblCM5.Text = Convert.ToDecimal(Fila["M61_90"]).ToString();
                        lblCM6.Text = Convert.ToDecimal(Fila["M91_"]).ToString();

                        OleDbCommand Comm2 = Conn.CreateCommand();
                        Comm2.CommandType = CommandType.StoredProcedure;
                        Comm2.CommandText = "Cobranzas.PersonaCondiciones";
                        Comm2.Parameters.Add("CodigoPersona", OleDbType.VarChar).Value = Persona.Codigo;
                        Comm2.Parameters.Add("Pais", OleDbType.Char, 3).Value = Persona.idPais;
                        Comm2.Parameters.Add("FechaDesde", OleDbType.Date).Value = Desde.Date;
                        Comm2.Parameters.Add("FechaHasta", OleDbType.Date).Value = Hasta.Date;

                        DS = new DataSet();
                        DA = new OleDbDataAdapter(Comm2);
                        DA.Fill(DS);
                        Fila = DS.Tables[0].Rows[0];

                        lblPagaDG.Text = Convert.ToBoolean(Fila["PagaDG"]) ? "Sí" : "No";
                        lblPagaSeguro.Text = Convert.ToBoolean(Fila["PagaSeguro"]) ? "Sí" : "No"; ;
                        lblTaquilla.Text = Convert.ToBoolean(Fila["Taquilla"]) ? "Sí" : "No";
                        lblChequesDevueltos.Text = Convert.ToBoolean(Fila["ChequesDevueltos"]) ? "Sí" : "No";
                    }
                }
            }
            catch (Exception Ex)
            {

            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            pnlExportar.Visible = false;
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Perfil.xls\"");
            Response.AddHeader("content-type", "text/html");//application/xls
        }

        protected void btnExportarWord_Click(object sender, EventArgs e)
        {
            pnlExportar.Visible = false;
            Response.AddHeader("Content-disposition", "attachment;FileName=\"Perfil.doc\"");
            Response.AddHeader("content-type", "text/html");//application/msword
        }
    }
}