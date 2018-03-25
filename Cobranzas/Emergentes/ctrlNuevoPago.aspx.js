/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar()
{
    ReferenciasRegExp = {};
    Pago = {};
    Argumentos = searchToJSON();
    Cargar = Argumentos.idPago != undefined;
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    var idPersona = window.parent.idPersona;
    if (idPersona == undefined) return;
    var Operadores = [];
    Operadores.push(window.parent.idOperadorSup); //Supervisado
    Operadores.push(window.parent.idOperadorAct); //Suplantado
    Operadores.push(window.parent.idOperadorLog); //Logueado
    LlamarServicio(_Bancos_lst, "Bancos_lst", { idPersona: idPersona }, false);
    LlamarServicio(_BancosPropios_lst, "BancosPropios_lst", { idPersona: idPersona });
    $("#pnlPeFacturasPago").html("Cargando...");
    if (!Cargar && window.parent.Cuentas != undefined)
    {
        LlamarServicio(_Cuentas_lst, "Cuentas_lst", { lstCuentas: window.parent.Cuentas });
    }

    LlamarServicio(_Monedas_lst, "Monedas_lst", { idPersona: idPersona });
    LlamarServicio(_Operadores_Persona_lst, "Operadores_Persona_lst", { idPersona: idPersona, Operadores: Operadores });
    lstAdjuntos = [];
    CambiarModo();

    if (Cargar)
    {
        LlamarServicio(_Pagos_sel, "Pagos_sel", { idPago: Argumentos.idPago });
    }
}
function _Pagos_sel(msg)
{
    Pago = msg.d;
    Cuentas2 = Pago.Pagos_Cuentas;
    LlamarServicio(_Cuentas_lst, "Cuentas_lst", { lstCuentas: Pago.Pagos_Cuentas.select("idCuenta") });

    $("#txtFechaNuevoPago").val(Pago.FechaPago);
    $("#txtNuevoPago").val(Pago.Descripcion);
    $("#cboTipoPago").val(Pago.TipoPago);
    $("#cboTipoPago")[0].disabled = true;
    $("#cboBancos").val(Pago.idBancoPropio);
    $("#txtReferenciaPago").val(Pago.Referencia);
    $("#txtTotalCheques").val(Pago.MontoCheque);
    $("#txtMontoEfectivo").val(Pago.MontoEfectivo.ToString());
    $("#cboMonedasPago").val(Pago.idMoneda);
    $("#cboOperadorPago").val(Pago.idOperador);
    $("#cboBancoOrigen").val(Pago.idBancoOrigen);

    var Cheques = Pago.Cheques;
    for (var i = 0; i < Cheques.length; i++)
    {
        /*Cheque*/
        Cheque = Cheques[i];
        var filaCheque = AgregarCheque();
        var Celdas = filaCheque.getElementsByTagName("td");
        $(Celdas[0].getElementsByTagName("input")[0]).val(Cheque.Nro);
        $(Celdas[1].getElementsByTagName("select")[0]).val(Cheque.Banco);
        $(Celdas[2].getElementsByTagName("input")[0]).val(Cheque.Monto.ToString());
    }
    var Adjuntos = Pago.Adjuntos;
    for (var i = 0; i < Adjuntos.length; i++)
    {
        AgregarAdjunto(Adjuntos[i]);
    }

    //ValidarSumaPagos();
}
function _Operadores_Persona_lst(msg)
{
    LlenarCombo({ Combo: "cboOperadorPago", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    $("#cboOperadorPago").val(window.parent.idOperadorSup);
    if (Pago.idOperadorPago != undefined)
    {
        $("#cboOperadorPago").val(Pago.idOperadorPago);
    }
}
function _Monedas_lst(msg)
{
    MonedaLocal = msg.d[0].idMoneda;
    LlenarCombo({ Combo: "cboMonedasPago", Resultado: msg.d, CampoId: "idMoneda", CampoValor: "idMoneda" });
    if (Pago.idMoneda != undefined)
    {
        $("#cboMonedasPago").val(Pago.idMoneda);
    }

    try
    {
        if (Cuentas != undefined) RefrescarCuentas();
    } catch (ex)
    {
    }
}
function _Cuentas_lst(msg)
{
    Cuentas = msg.d;
    try
    {
        for (var i = 0; i < Cuentas2.length; i++)
        {
            for (var j = 0; j < Cuentas.length; j++)
            {
                if (Cuentas[j].idCuenta == Cuentas2[i].idCuenta)
                {
                    Cuentas[j].MontoNuevo = Cuentas2[i].MontoCuenta;
                    Cuentas[j].RetIva = Cuentas2[i].Retencion1;
                    Cuentas[j].RetISLR = Cuentas2[i].Retencion2;
                    //Cuentas[j].MontoRetISLR = Cuentas2[i].Retencion2;
                }
            }
        }
    } catch (e) { }
    var Moneda = $("#cboMonedasPago").val();
    if (Moneda != null) RefrescarCuentas();
}
function RefrescarCuentas()
{
    var Moneda = $("#cboMonedasPago").val();
    if (Moneda == "USD") Sufijo = "Dolar";
    if (Moneda == MonedaLocal) Sufijo = "Local";
    var CampoT = "Total" + Sufijo;
    var CampoD = "Deuda" + Sufijo;
    try
    {
        if (Cuentas2 != undefined)
        {
            CampoN = "MontoNuevo";
        } else
        {
            CampoN = CampoD;
        }
    } catch (e)
    {
        CampoN = CampoD;
    }
    //var CampoN = "MontoNuevo";
    var CampoB = "MontoBase" + Sufijo;
    var CampoI = "MontoIva" + Sufijo;
    Tabla({
        Contenedor: "pnlPeFacturasPago",
        Resultado: Cuentas/*.where(function (a) { return a.Moneda == Moneda; })*/,
        idSeleccion: "idCuenta",
        TodosSeleccionados: true,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto" },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha" },
            { Titulo: "Total", Campo: CampoT, Clase: "grdDecimal" },
            { Titulo: "Base", Campo: CampoB, Clase: "grdDecimal" },
            { Titulo: "Tax", Campo: CampoI, Clase: "grdDecimal" },
            { Titulo: "Remaining", Campo: CampoD, Clase: "grdDecimal" },
            { Titulo: "Amount", Campo: CampoN, Clase: "grdDecimal", Input: true, Alias: "MontoNuevo" },
            { Titulo: "%RetIva", Campo: "RetIva", Clase: "grdEntero", Combo: $("#porcRetIva")[0], Alias: "RetIva" },
            { Titulo: "Amount", Campo: "MontoRetIva", Clase: "grdDecimal", Alias: "MontoRetIva" },
            { Titulo: "%RetISLR", Campo: "RetISLR", Clase: "grdEntero", Combo: $("#porcRetISLR")[0], Alias: "RetISLR" },
            { Titulo: "Amount", Campo: "MontoRetISLR", Clase: "grdDecimal", Alias: "MontoRetISLR" }
        ]
    });
    var Selects = $("#pnlPeFacturasPago")[0].getElementsByTagName("select");
    for (i = 0; i < Selects.length; i++) Calcular(Selects[i]);
    $("#txtMontoPago").val(Cuentas.where(function (a) { return a.Moneda == Moneda; }).sum(function (a) { return a[Campo]; }).ToString());
    ValidarSumaPagos();
    $("#pnlPeFacturasPago input").blur(new Function("return ValidarSumaPagos(this);"));
    $("#pnlPeFacturasPago select").blur(new Function("Calcular(this); return ValidarSumaPagos(this);"));
}
function Calcular(t)
{
    var Control = t;
    while (t.tagName.toUpperCase() != "TR")
    {
        t = t.parentNode;
    }
    var Celdas = t.getElementsByTagName("TD");

    var MontoIva = Convert.ToDecimal($(Celdas[5]).text());
    var MontoBase = Convert.ToDecimal($(Celdas[4]).text());
    var RetIva = Celdas[8].getElementsByTagName("select")[0];
    var MontoRetIva = Celdas[9]; //.getElementsByTagName("input")[0];
    var RetISLR = Celdas[10].getElementsByTagName("select")[0];
    var MontoRetISLR = Celdas[11]; //.getElementsByTagName("input")[0];

    if (Control == RetIva) //Cambié el Porcentaje
    {
        $(MontoRetIva).text((Convert.ToDecimal($(RetIva).val()) * MontoIva / 100.0).ToString());
    }
    if (MontoIva == 0)
    {
        $(MontoRetIva).text((0.0).ToString());
    }
    if (Control == RetISLR) //Cambié el Porcentaje
    {
        var porc = Convert.ToDecimal($(RetISLR).val());
        $(MontoRetISLR).text((porc * MontoBase / 100.0).ToString());
    }
    return;
}
function ValidarSumaPagos(t)
{
    $("#lblError").text("");
    var Cuentas = ObtenerSeleccionados("pnlPeFacturasPago");
    for (var i = 0; i < Cuentas.length; i++)
    {
        if (Cuentas[i].MontoNuevo /*+ Cuentas[i].MontoRetIva + Cuentas[i].MontoRetISLR*/ > Cuentas[i].Deuda)
        {
            $("#lblError").append("* " + Recursos["msgPaymentExceedDebt"] + "<br/>");
            break;
        }
    }
    var Pagos = Cuentas.select("MontoNuevo").sum();
    var Retenciones = Cuentas.select("MontoRetIva").sum() + Cuentas.select("MontoRetISLR").sum();
    $("#txtTotalAplicacion").val(Pagos.ToString());
    $("#txtTotalRetenciones").val(Retenciones.ToString());

    var Tabla = $('#Cheques')[0];
    var Filas = Tabla.getElementsByTagName("tr");
    var MontoCheque = 0;
    for (var i = 1; i < Filas.length; i++) //Se salta el 0 que es un modelo...
    {
        Celdas = Filas[i].getElementsByTagName("td");
        MontoCheque += Convert.ToDecimal($(Celdas[2].getElementsByTagName("input")).val());
    }
    $("#txtTotalCheques").val(MontoCheque.ToString());

    var Total = MontoCheque + Convert.ToDecimal($("#txtMontoEfectivo").val());
    $("#txtTotalPago").val(Total.ToString());
    var Restante = Math.round((Total - Pagos + Retenciones) * 100) / 100;
    $("#txtMontoRestante").val(Restante.ToString());
    if (Restante < 0.00)
    {
        $("#lblError").append("* " + Recursos["msgPaymentsDiffersTotal"] + "<br/>");
    }
    return;
}

function _Pagos_sav(msg)
{
    var resp = msg.d;
    if (resp == false)
    {
        Mensaje({ mensaje: Recursos["msgPaymentWasntSaved"] });
    }
    else
    {
        window.parent.RefrescarPersona();
        window.parent.Mensaje({ mensaje: Recursos["msgPaymentSaved"] });
        window.parent.CerrarEmergente();
    }
}
function InsertarPago()
{
    var rtPago = {};
    rtPago.idPago = Cargar ? Argumentos.idPago : 0;
    rtPago.FechaPago = $("#txtFechaNuevoPago").val();

    if (rtPago.FechaPago == "")
    {
        Mensaje({ mensaje: "Debe especificar una fecha para el pago" });
        return;
    }
    try
    {
        var Fecha = Convert.ToDateTime(rtPago.FechaPago);
        if (Fecha > (new Date()))
        {
            Mensaje({ mensaje: "La fecha del Pago es superior a la fecha actual" });
            return;
        }
    } catch (e)
    {
        Mensaje({ mensaje: "La fecha del Pago no tiene el formato correcto (dd/mm/aaaa): " + e });
        return;
    }

    rtPago.idBancoPropio = $("#cboBancos").val();
    rtPago.Referencia = $("#txtReferenciaPago").val();
    if (rtPago.Referencia == "")
    {
        Mensaje({ mensaje: Recursos["msgMustEnterReference"] });
        return;
    }
    var Validacion = ReferenciasRegExp.where(function (a) { return a.idBancoPropio == rtPago.idBancoPropio })[0];
    var valid = new RegExp(Validacion.ReferenciasRegExp);
    if (!valid.test(rtPago.Referencia))
    {
        Mensaje({ mensaje: "El Número de referencia no es válido, debe tener este formato: " + Validacion.ReferenciasInfo });
        return;
    }

    rtPago.Descripcion = $("#txtNuevoPago").val();
    if (rtPago.Descripcion == "")
    {
        Mensaje({ mensaje: Recursos["msgMustEnterDescription"] });
        return;
    }
    if (lstAdjuntos.length == 0)
    {
        Mensaje({ mensaje: "Debe adjuntar soportes para este pago" });
        return;
    }
    var tablaFact = $("#pnlPeFacturasPago")[0];

    elem = tablaFact.getElementsByTagName("tr");
    var campos = obtenerCampos(tablaFact);
    var colIdCuenta = $.inArray("idCuenta", campos);
    var colMontoPago = $.inArray("MontoNuevo", campos);

    rtPago.TipoPago = $("#cboTipoPago").val();

    rtPago.MontoCheque = Convert.ToDecimal($("#txtTotalCheques").val());
    if (rtPago.MontoCheque == undefined) rtPago.MontoCheque = 0;
    rtPago.MontoEfectivo = Convert.ToDecimal($("#txtMontoEfectivo").val());
    var MontoTotal = rtPago.MontoCheque + rtPago.MontoEfectivo;
    if (MontoTotal <= 0)
    {
        Mensaje({ mensaje: "Debe seleccionar un monto para este pago" });
        return;
    }
    rtPago.idMoneda = $("#cboMonedasPago").val();
    rtPago.idPersona = window.parent.idPersona;
    rtPago.idOperador = $("#cboOperadorPago").val();
    rtPago.idBancoOrigen = rtPago.TipoPago == 1 ? null : $("#cboBancoOrigen").val();
    var FactSel = ObtenerSeleccionados("pnlPeFacturasPago");
    //    if (FactSel.length == 0)
    //    {
    //        Mensaje({ mensaje: "Debe Seleccionar al menos una cuenta para continuar" });
    //        return;
    //    }


    var MontoCuentas = FactSel.select("MontoNuevo").sum() - FactSel.select("MontoRetIva").sum() - FactSel.select("MontoRetISLR").sum();
    var Restante = Math.round((MontoTotal - MontoCuentas) * 100) / 100;
    if (Restante < 0)
    {
        Mensaje({ mensaje: Recursos["msgPaymentsDiffersTotal"] });
        return;
    }
    rtPago.Pagos_Cuentas = [];

    for (var i = 0; i < FactSel.length; i++)
    {
        try
        {
            var rtFacturasPago = {};
            rtFacturasPago.idCuenta = Convert.ToInt32(FactSel[i]["idCuenta"]);
            rtFacturasPago.MontoCuenta = Convert.ToDecimal(FactSel[i]["MontoNuevo"]);
            rtFacturasPago.Retencion1 = Convert.ToDecimal(FactSel[i]["RetIva"]);
            rtFacturasPago.Retencion2 = Convert.ToDecimal(FactSel[i]["RetISLR"]);

            if (rtFacturasPago.MontoCuenta > 0)
            {
                rtPago.Pagos_Cuentas.push(rtFacturasPago);
            }
        } catch (ex) { }
    }

    var tablaCheques = $("#Cheques")[0];

    elem = tablaCheques.getElementsByTagName("tr");
    rtPago.Cheques = [];
    for (var i = 2; i < elem.length; i++)
    {
        try
        {
            var Celdas = elem[i].getElementsByTagName("td");
            var rtCheque = {};
            rtCheque.Nro = $(Celdas[0].getElementsByTagName("input")[0]).val();
            rtCheque.Banco = Convert.ToInt32($(Celdas[1].getElementsByTagName("select")[0]).val());
            rtCheque.Monto = Convert.ToDecimal($(Celdas[2].getElementsByTagName("input")[0]).val());
            rtPago.Cheques.push(rtCheque);
        } catch (ex) { }
    }
    rtPago.Adjuntos = lstAdjuntos;
    LlamarServicio(_Pagos_sav, "Pagos_sav", { Pago: rtPago })
}

function _BancosPropios_lst(msg)
{
    var Bancos = msg.d;
    ReferenciasRegExp = Bancos;
    var cboBancos = $("#cboBancos");
    if (Bancos.length == 0)
    {
        window.parent.CerrarEmergente();
        window.parent.Mensaje({ mensaje: Recursos["msgNoBanks"] });
    }
    for (i = 0; i < Bancos.length; i++)
    {
        cboBancos.append($('<option>', { value: Bancos[i].idBancoPropio, text: Bancos[i].Nombre + '-' + Bancos[i].Descripcion }));
    }
    if (Pago.idBancoPropio != undefined)
    {
        $("#cboBancos").val(Pago.idBancoPropio);
    }
}
function _Bancos_lst(msg)
{
    var Bancos = msg.d;
    if (Bancos.length == 0)
    {
        window.parent.CerrarEmergente();
        window.parent.Mensaje({ mensaje: Recursos["msgNoBanks"] });
    }
    LlenarCombo({ Combo: "cboBancoCheque", Resultado: Bancos, CampoId: 'idBanco', CampoValor: 'Nombre' });
    LlenarCombo({ Combo: "cboBancoOrigen", Resultado: Bancos, CampoId: 'idBanco', CampoValor: 'Nombre' });
    if (Pago.idBancoOrigen != undefined)
    {
        $("#cboBancoOrigen").val(Pago.idBancoOrigen);
    }
    //    var cboBancos = $("#cboBancoCheque");
    //    for (i = 0; i < Bancos.length; i++)
    //    {
    //        cboBancos.append($('<option>', { value: Bancos[i].idBanco, text: Bancos[i].Nombre + '-' + Bancos[i].Descripcion }));
    //    }
}
function AgregarCheque()
{
    var Modelo = $("#modeloCheque")[0];
    var Clon = Modelo.cloneNode(true);
    Clon.style.display = "";
    $("#Cheques")[0].appendChild(Clon);
    return Clon;
}
function EliminarCheque(t)
{
    t.parentNode.parentNode.parentNode.removeChild(t.parentNode.parentNode);
    ValidarSumaPagos();
}
function CambiarModo()
{
    var opc = $("#cboTipoPago").val();
    $("#trCheques")[0].style.display = opc == 1 ? "" : "none";
    $("#trTransferencia")[0].style.display = opc == 2 ? "" : "none";
}

function Adjuntar()
{
    Emergente({ url: "ctrlSubirSoportes.aspx", width: 300, height: 200 });
}
function AgregarAdjunto(Archivo)
{
    var div = document.createElement("DIV");
    div.onclick = function ()
    {
        var Control = this;
        Preguntar({ mensaje: "¿Está Seguro de querer eliminar el soporte?",
            funcion: function ()
            {
                var Archivo2 = $(Control).text();
                Control.parentNode.removeChild(Control);
                for (var i = 0; i < lstAdjuntos.length; i++)
                {
                    var pos = lstAdjuntos[i].indexOf("_");
                    var Parte = lstAdjuntos[i].substr(pos + 1);
                    if (Parte == Archivo2)
                    {
                        lstAdjuntos.splice(i); break;
                    }
                }
            }
        })
    }
    var pos = Archivo.indexOf("_");
    $(div).text(Archivo.substr(pos + 1));
    div.className = "Adjunto";
    lstAdjuntos.push(Archivo);
    $("#Adjuntos")[0].appendChild(div);
}
