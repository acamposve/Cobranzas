/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    GestionAutomatica = false;
    PausaGestion = true;
    Errores = [];
    Avisos = [];
    Asignaciones = {};
    window.onerror = OnError;

    FastidiarPorCorreos = true;

    $(".EncSeccion").click(function () {
        $('#' + this.id.substr(1)).slideToggle();
        OcultarAvisos();
    })
    PonerHora();
    $('#Avisos').click(function () {
        if ($('#Avisos').css('left') == "-340px") { desp = '0px'; } else { desp = '-340px'; }
        $('#Avisos').animate({
            left: desp
        }, 1000, function () {

        });
    });

    LlamarServicio(_Avisos_lst2, "Avisos_lst", { idAviso: $("#idAviso").val() });
    setInterval("PonerHora()", 60000);
    TipoOperador = $("#TipoOperador").val();

    $("input.fecha").datepicker($.datepicker.regional["es"]);
    LlamarServicio(_ActualizarGrafico, "ActualizarGrafico", { idOperador: idOpA() });//warning se cambió de idOp a idOpA
    LlamarServicio(_ActualizarGraficoSupervision, "ActualizarGraficoSupervision", { idOperador: idOpA() });
    LlamarServicio(_ActualizarGraficoPorSemana, "ActualizarGraficoPorSemana", { idOperador: idOpA() });
    LlamarServicio(_ActualizarIndicadores, "ActualizarIndicadores", { idOperador: idOpA() });//warning se cambió de idOp a idOpA
    LlamarServicio(_Operadores_Asignaciones_lst, "Operadores_Asignaciones_lst", { idOperador: idOpA() });
    $("#pnlGestion").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            if (ui.newPanel[0].id == "tabIndicadores") {
                LlamarServicio(_ActualizarGrafico, "ActualizarGrafico", { idOperador: idOp() });
                LlamarServicio(_ActualizarIndicadores, "ActualizarIndicadores", { idOperador: idOp() });
            }
            if (ui.newPanel[0].id == "tabGestion" && $('#idPersona').val() == "") {
                Mensaje({ mensaje: Recursos["msgNoPerSel"] }); //no ha seleccionado a una persona para gestionar
                return false;
            }
            if (ui.newPanel[0].id == "tabSupervision") {
                LlamarServicio(_ActualizarGraficoSupervision, "ActualizarGraficoSupervision", { idOperador: idOpA() });
                LlamarServicio(_ActualizarGraficoPorSemana, "ActualizarGraficoPorSemana", { idOperador: idOpA() });
            }
        }
    });

    $("#tabsOtrasActividades").tabs({ show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 } });


    LlamarServicio(_Status_lst, "Status_lst", { idOperador: idOpA() });

    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisadosConSupervisor_lst", { idOperador: idOpA() });
    ActualizarSuplentes();

    setInterval('Novedades();', 45000);
    window.onbeforeunload = function () {
        Salir(idOpL());
    }
    setTimeout(InicioRetrasado, 5000);
    LimpiarAdjuntos();

}
function OnError(message, location, line, colno, error) {
    $("#pnlErrores").show();
    var Err = $("#nroErrores").text();
    $("#nroErrores").text(Convert.ToInt32("0" + Err) + 1);
    Errores.push({ message: message, location: location, line: line, col: colno, callstack: error.stack });
    return false;
}
function EnviarReporte() {
    LlamarServicio(_EnviarReporte, "EnviarReporte", { Errores: Errores, idOperador: idOpL() });
}
function _EnviarReporte(msg) {
    $("#pnlErrores").hide();
    Errores = [];
}
function InicioRetrasado() {
    RefrescarCorreos();
    LlamarServicio(_Personas_Tocadas_lst, "Personas_Tocadas_lst", { idOperador: idOp() });
    if ($("#idPersona").val() != "") {
        Persona_Mostrar($("#idPersona").val());
    }
    else {
        //        $("#pstIndicadores").click();
        // $("#pstIndicadores").click();
    }
    LlamarServicio(_Operador_Metas, "Operador_Metas", { idOperador: idOpA() });
    if (!OperadorEs('SU', 'CO', 'GE', 'AD')) ActualizarCartera();
    LlamarServicio(_ExclusionesMotivos_lst, "ExclusionesMotivos_lst", {});
}
function _ExclusionesMotivos_lst(msg) {
    LlenarCombo({ Combo: "cboExclusionMotivo", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Seleccione»", ValorNull: "" });
}
function _Operadores_Asignaciones_lst(msg) {
    TiposCliente = msg.d.select("idTipoCliente").distinct();
    Paises = msg.d.select("idPais").distinct();
}

function _Operador_Metas(msg) {
    Tabla({
        Contenedor: "pnlMetas",
        Resultado: msg.d,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Facturas", Campo: "Facturas", Clase: "grdEntero", Ordenacion: false, Filtro: false, Resumen: 'TOTAL' },
            { Titulo: "Collected", Campo: "FacturasCobradas", Clase: "grdEntero", Ordenacion: false, Filtro: false, Resumen: 'TOTAL' },
            { Titulo: "Goal", Campo: "Meta", Post: "USD", Clase: "grdDecimal", Ordenacion: false, Filtro: false, Resumen: 'TOTAL' },
            { Titulo: "Collected", Campo: "Real", Post: "USD", Clase: "grdDecimal", Ordenacion: false, Filtro: false, Resumen: 'TOTAL' },
            { Titulo: "Effectiveness", Campo: "Porc", Clase: "grdDecimal", Post: "%", Ordenacion: false, Filtro: false }
        //            { Titulo: "Adj", Campo: function(a){return a.TieneAdjuntos}, Campo2: "", Clase: "", Pre: "", Post: "", Ordenacion: true, Filtro: true, Combo: _combo_ }
        ],
        FuncSeleccionar: "AbrirMetas(«idMeta»,'«Fecha»'," + idOp() + ");"
    });

}
function AbrirMetas(idMeta, Fecha, idOperador) {
    Emergente({ url: "/Emergentes/ctrlMetaDetalle.aspx?idMeta=" + idMeta + "&Fecha=" + Fecha + "&idOperador=" + idOperador });
}
function ActualizarContrasena(CambiarAJuro) {
    setTimeout('Emergente({ url: "/Emergentes/ctrlOperador.aspx' + (CambiarAJuro ? "?Cambiar=1" : "") + '", width:300, height:300 })', 2000);
}
//----------------------------------------------------------------------------------------------------------Bandeja de Entrada
function _Personas_Tocadas_lst(msg) {
    PersonasTocadas = msg.d;
    LlenarCombo({ Combo: "cboaPersona", Resultado: msg.d, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
}
function AsignarCorreosAPersona(CrearRegla) {
    var idPersona = $("#cboaPersona").val();
    if (idPersona == "" || idPersona == null) { Mensaje({ mensaje: Recursos["msgMustSelectPerson"] }); return; }
    var CorreosSel = ObtenerSeleccionados("pnlCorreos").select("idCorreo");
    if (CorreosSel.length == 0) {
        Mensaje({ mensaje: "Debe Seleccionar el/los correos Primero" });
        return;
    }
    //Correos_Persona_ins(_Correos_Persona_ins, CorreosSel, idPersona, CrearRegla);RA
    LlamarServicio(_Correos_Persona_ins, "Correos_Persona_ins", { Correos: CorreosSel, idPersona: idPersona, CrearRegla: CrearRegla });
}
function _Correos_Persona_ins(msg) {
    Mensaje({ mensaje: Recursos["msgMailAssigned"] });
    RefrescarCorreos();
}
function MarcarComoPersonal() {
    var Correos = ObtenerSeleccionados("pnlCorreos").select("idCorreo");
    if (Correos.length == 0) {
        Mensaje({ mensaje: "Debe Seleccionar el/los correos Primero" }); return;
    }
    LlamarServicio(_Correos_MarcarPersonal_upd, "Correos_MarcarPersonal_upd", { Correos: Correos, TipoEspecial: 2 });

}
function _Correos_MarcarPersonal_upd() {
    RefrescarCorreos();
}
function idP() //Persona Actual
{
    return $("#idPersona").val();
}
function idOp() //Este es el Operador Supervisado
{
    return $("#idOperador").val();
}
function idOpA() // Este es el Operador Suplantado
{
    return $("#idOperadorAct").val();
}
function idOpL() //Este es el Operador Logueado
{
    return $("#idOperadorLog").val();
}
function AdministrarFiltros() {
    Emergente({ url: "/Emergentes/ctrlFiltros.aspx?idOperador=" + idOp() });
}
//function ActualizarCorreos()
//{
//   Operador_Correos_nov(_Operador_Correos_nov, idOp(), $("#idCorreo").val());
//}
function RefrescarCorreos() {
    //Operador_Correos_lst(_Operador_Correos_lst, idOp());RA
    LlamarServicio(_Operador_Correos_lst, "Operador_Correos_lst", { idOperador: idOp() });
    //Operador_CorreosClasificadosNoLeidos_lst(_Operador_CorreosClasificadosNoLeidos_lst, idOp());RA
    LlamarServicio(_Operador_CorreosClasificadosNoLeidos_lst, "Operador_CorreosClasificadosNoLeidos_lst", { idOperador: idOp() });
}
//function _Operador_Correos_nov(msg)
//{
//    if (msg.d === true)
//    {
//        RefrescarCorreos();
//        //Mensaje({ mensaje: "Hay Correo Nuevo" });
//    }
//}
function _Operador_Correos_lst(msg) {
    $("#idCorreo").val(msg.d.length == 0 ? 0 : msg.d[0].idCorreo);
    if (FastidiarPorCorreos) {
        if (msg.d.length > 10) Mensaje({ mensaje: "Tiene " + msg.d.length + " Correos sin Asignar, Recuerde que debe asignar estos correos." });
        FastidiarPorCorreos = false;
    }
    $("#Cantidad").text(msg.d.length);
    Tabla({
        Contenedor: "pnlCorreos",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idCorreo",
        LimitarAltura: 200,
        Campos: [
        { Titulo: "MailRead", Campo: function (a) { return "/Img/" + (a.Leido ? "CorreoLeido16.png" : "CorreoEntrante16.png"); }, Clase: "grdTexto", Ordenacion: false, Filtro: false, Imagen: true },
            { Titulo: "MailFrom", Campo: "Remitente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Subject", Campo: "Asunto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFechaHora", Ordenacion: true, Filtro: true }
        //            { Titulo: "Adj", Campo: function(a){return a.TieneAdjuntos}, Campo2: "", Clase: "", Pre: "", Post: "", Ordenacion: true, Filtro: true, Combo: _combo_ }
        ],
        FuncSeleccionar: "AbrirCorreo(«idCorreo»,'Correo',1);"
    });
}
function _Operador_CorreosClasificadosNoLeidos_lst(msg) {
    $("#idCorreo").val(msg.d.length == 0 ? 0 : msg.d[0].idCorreo);
    if (FastidiarPorCorreos) {
        if (msg.d.length > 10) Mensaje({ mensaje: "Tiene " + msg.d.length + " Correos clasificados pero no leídos" });
        FastidiarPorCorreos = false;
    }
    $("#Cantidad2").text(msg.d.length);
    Tabla({
        Contenedor: "pnlCorreosClasificadosNoLeidos",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idCorreo",
        LimitarAltura: 200,
        Campos: [
        { Titulo: "MailRead", Campo: function (a) { return "\\Img\\" + (a.Leido ? "CorreoLeido16.png" : "CorreoEntrante16.png"); }, clase: "grdTexto", Ordenacion: false, Filtro: false, Imagen: true },
            { Titulo: "MailFrom", Campo: "Remitente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Subject", Campo: "Asunto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true }
        //            { Titulo: "Adj", Campo: function(a){return a.TieneAdjuntos}, Campo2: "", Clase: "", Pre: "", Post: "", Ordenacion: true, Filtro: true, Combo: _combo_ }
        ],
        FuncSeleccionar: "AbrirCorreo(«idCorreo»,'Correo',2);"
        //Filtros: FiltrosCNL,
        //OrdenadoPor: OrdenadoPorCNL
    });
}
//----------------------------------------------------------------------------------------------------------Supervisión
function Supervisar(idOperador) {
    LlamarServicio(Supervisar_Nombre, "Nombre", { idOperador: idOperador });
    $("#idOperador").val(idOperador);
    $("#pnlCartera").html("Espere por favor...");
    //Cartera_lst(_Cartera_lst, idOperador, idOpL() != idOp());RA
    ActualizarCartera();
    //LlamarServicio(_Cartera_lst, "Cartera_lst", { idOperador: idOperador, Supervisado: idOpL() != idOp() });
    //Personas_Tocadas_lst(_Personas_Tocadas_lst, idOperador);RA
    LlamarServicio(_Personas_Tocadas_lst, "Personas_Tocadas_lst", { idOperador: idOperador });
    LlamarServicio(_Operador_Metas, "Operador_Metas", { idOperador: idOperador });
    RefrescarCorreos();
    $("#pstCartera").click();
    $("#idPersona").val("");
    $("#minipnlSupervision")[0].style.display = (idOpL() == idOperador) ? 'none' : '';

}
function Supervisar_Nombre(msg) {
    $("#lblSupervisando").text(msg.d);
}
function Suplantar_Nombre(msg) {
    $("#lblSuplantando").text(msg.d);
}

function _ActualizarGraficoSupervision(msg) {
    var hora = new Date().getTime();
    var Ruta = msg.d + "?hora=" + hora;
    $("#chrtGestionesSupervision").attr("src", Ruta);
}
function _ActualizarGraficoPorSemana(msg) {
    var hora = new Date().getTime();
    var Ruta = msg.d + "?hora=" + hora;
    $("#chrtGestionesPorSemana").attr("src", Ruta);
}
//----------------------------------------------------------------------------------------------------------Gestion
function btnEnviarNuevoCorreo_Click() {
    lstCuentas = window.parent.ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    Emergente({ url: "/Emergentes/ctrlEnvioCorreo.aspx?idPersona=" + $("#idPersona").val() + "&idOperador=" + idOpA(), modal: false });
}
function _Status_lst(msg) {
    //Ver _Status_lst en ctrlReglas.aspx.js
    lstStatus = msg.d;
}

function SelecionGestion() {
    var Status = $("#cboGestStatus option:selected").html();
    Mensaje({ mensaje: "Usted esta seguro de cerrar una gestion con el siguiente motivo: " + Status });
}

function ActualizarComboStatus(idPais, idTipoCliente) {
    var Status = lstStatus.where(function (x) { return x.idPais == idPais && x.idTipoCliente == idTipoCliente });
    $("#cboGestStatus").html("");
    var papa = $("#cboGestStatus")[0];
    var opcion = document.createElement("OPTION");
    $(opcion).text("<<Seleccione>>");
    $(opcion).val(0);
    papa.appendChild(opcion);
    var i;
    for (i = 0; i < Status.length; i++) {
        var opcion = document.createElement("OPTION");
        $(opcion).text(Status[i].Nombre);
        $(opcion).val(Status[i].idStatus);
        switch (Status[i].Tipo) {
            case "Positivo":
                opcion.style.backgroundColor = Colores.Positivo;
                break;
            case "Neutral":
                opcion.style.backgroundColor = Colores.Neutral;
                break;
            case "Negativo":
                opcion.style.backgroundColor = Colores.Negativo;
                opcion.style.color = "#FFFFFF";
                break;
        }
        papa.appendChild(opcion);
    }
}
function _Cuentas_Pagos_lst(msg) {
    var fecha = new Date();
    var FechaT = Dos(fecha.getDate()) + "/" + Dos(fecha.getMonth() + 1) + "/" + fecha.getFullYear().toString();
    $('#txtFechaNuevoPago')[0].value = FechaT;

    var FacturasPago = msg.d;
    $("#pnlPeFacturasPago").html("");
    var pnl = $("#pnlPeFacturasPago")[0];
    var tabla = CrearTabla("Tabla");
    tabla.setAttribute("id", "tblPeFacturasPago");
    pnl.appendChild(tabla);
    var fila = AgregarFila(tabla);
    AgregarHeader(fila, "Documento", "grdTexto", true, true).setAttribute("campo", "idCuenta");
    AgregarHeader(fila, "Moneda", "grdTexto", true, true).setAttribute("campo", "Moneda");
    AgregarHeader(fila, "Deuda USD", "grdDecimal", true, true);
    AgregarHeader(fila, "Tasa a USD", "grdTexto", true, true);
    AgregarHeader(fila, "Deuda Origen", "grdDecimal", false, false);
    AgregarHeader(fila, "Pago Origen", "grdDecimal", false, false).setAttribute("campo", "MontoPago");
    AgregarHeader(fila, "Tasa a Local", "grdTexto", true, true);
    AgregarHeader(fila, "Deuda Local", "grdDecimal", true, true);

    var MontoPago = 0;
    var Monedas = Array();
    for (i = 0; i < FacturasPago.length; i++) {
        if ($.inArray(FacturasPago[i].Moneda, Monedas) == -1) {
            Monedas.push(FacturasPago[i].Moneda);
        }

        fila = AgregarFila(tabla, "itmDeuda");
        AgregarCelda(fila, FacturasPago[i].Documento, "grdTexto").setAttribute("idCuenta", FacturasPago[i].idCuenta);
        AgregarCelda(fila, FacturasPago[i].Moneda, "grdTexto")
        AgregarCelda(fila, FormatDecimal(FacturasPago[i].DeudaDolar) + "USD", "grdDecimal");
        AgregarCelda(fila, FormatDecimal(FacturasPago[i].CambioDolar), "grdDecimal");
        AgregarCelda(fila, FormatDecimal(FacturasPago[i].Deuda) + FacturasPago[i].Moneda, "grdDecimal");
        //MontoPago = MontoPago + FacturasPago[i].Deuda;
        var txtPago = CrearInputBox(FormatDecimal(0), "text", "grdDecimal");
        txtPago.setAttribute("MontoDeuda", FacturasPago[i].Deuda);
        txtPago.setAttribute("onblur", "ValidarSumaPagos(this);");
        txtPago.setAttribute("onfocus", "return focusDec(this);");
        txtPago.setAttribute("onkeydown", "return kdDec(event);");
        txtPago.setAttribute("onkeypress", "return kpDec(event, this);");
        AgregarCelda(fila, "", "grdDecimal").appendChild(txtPago);
        AgregarCelda(fila, FormatDecimal(FacturasPago[i].CambioLocal), "grdDecimal");
        AgregarCelda(fila, FormatDecimal(FacturasPago[i].DeudaLocal) + FacturasPago[i].MonedaLocal, "grdDecimal");
    }
    RedefinirAlternativo(tabla);
    $("#txtMontoPago").val(FormatDecimal(MontoPago));
    $("#txtMontoRestante").val(FormatDecimal(0));

    var cboMonedasPago = $("#cboMonedasPago");
    for (i = 0; i < Monedas.length; i++) {
        cboMonedasPago.append($('<option>', { value: Monedas[i], text: Monedas[i] }));
    }
    ValidarMonedaCuentasPago();
}
function _Gestion_ins(msg) {
    var resp = msg.d;
    if (resp == false) {
        Mensaje({ mensaje: Recursos["ErrorManagement"] });
    }
    else {
        if (AvisosPropios.length > 0) {
            LlamarServicio(_Avisos_del, "Avisos_del", { Avisos: AvisosPropios, Comentario: $("#txtGestDescripcion").val() });
        }
        //RefrescarPersona();
        $("#txtGestDescripcion").val("");
        $("#cboGestStatus option:first").attr('selected', 'selected');
        //        Mensaje({ mensaje: "La gestión se guardó correctamente" });
        Preguntar({ mensaje: "La gestión se guardó correctamente, ¿Desea gestionar a otra persona?", funcion: FinalizarGestion });
        //Cartera_lst(_Cartera_lst, idOp(), idOpL() != idOp()); /*Warning*/RA
        //LlamarServicio(_Cartera_lst, "Cartera_lst", { idOperador: idOp(), Supervisado: idOpL() != idOp() });
        RefrescarGestiones()

        try {
            for (var i = 0; i < Cartera.length; i++) {
                var ItemCartera = Cartera[i].Personas.where(function (a) { return a.idPersona == idP(); })[0];
                if (ItemCartera == undefined) break;
                ItemCartera.FechaUltimaGestion = (new Date()).ToString("JSON");
                var Status = lstStatus.where(function (a) { return a.idStatus == idUltimoStatus; });
                ItemCartera.StatusUltimaGestion = Status[0].Nombre;
                ItemCartera.TipoStatus = Status[0].Tipo;
            }
            _Cartera_lst({ d: Cartera });
        } catch (e) {
            ActualizarCartera();
        }
        LimpiarAdjuntos();
    }
}
function _Avisos_del(msg) {
    Avisos_Actualizar();
}
function RefrescarPersona() {
    var idPersona = $("#idPersona").val();
    if (idPersona == "") return;
    Persona_Mostrar(idPersona);
}
function RefrescarGestiones() {
    var idPersona = $("#idPersona").val();
    if (idPersona == "") return;
    LlamarServicio(_Personas_Gestiones_lst, "Personas_Gestiones_lst", { idPersona: idPersona, idOperador: idOp() });
}
function FinalizarGestion() {
    LlamarServicio(_Personas_sel, "Personas_sel", { idPersona: 0, idOperador: idOp() });
}
function _Personas_sel(msg) {
    var Persona = msg.d;
    if (Persona == null) {
        Mensaje({ mensaje: "Ya no hay más clientes para gestionar" });
        $("#idPersona").val("");
        $("#pstIndicadores").click();
        return;
    }

    $("#idPersona").val(Persona.idPersona);
    $("#lblPeCodigo").text(Persona.Codigo);
    $("#lblPeRif").text(Persona.Rif);
    $("#lnkPeURL").attr("href", Persona.URL);
    $("#lnkPeURL").text(Persona.URL);
    $("#lblPeEmail").text(Persona.Email);
    $("#lblPeNombre").text(Persona.Nombre);
    $("#lblPeDireccion").text(Persona.DireccionFiscal);

    $("#lblPeDireccionEntrega").text(Persona.DireccionEntrega);
    $("#lblPeContacto").text(Persona.Contacto);

    $("#divPeDatos").html("");
    if (Persona.Datos != null) {
        XMLDatos = XMLParse(Persona.Datos);
        var Nodos = XMLDatos.getElementsByTagName("Dato");

        var TablaDatos = CrearTabla("Tabla");
        $("#divPeDatos")[0].appendChild(TablaDatos);

        var tr = AgregarFila(TablaDatos);
        AgregarHeader(tr, "Dato", "grdTexto", false, false, "");
        AgregarHeader(tr, "Valor", "grdTexto", false, false, "");
        for (var i = 0; i < Nodos.length; i++) {
            var Clave = Nodos[i].getAttribute("Clave");
            var Valor = Nodos[i].textContent;
            var tr = AgregarFila(TablaDatos);
            AgregarCelda(tr, Clave, "grdTexto");
            AgregarCelda(tr, Valor, "grdTexto");
        }
    }

    $("#idPais").val(Persona.ISO3);
    var pais = $("#lblPePais");
    var papa = pais[0].parentNode;
    $(papa).html("");
    var img = document.createElement("img");
    img.src = "Img/Banderas/" + Persona.ISO3 + "24.png";
    papa.appendChild(img);
    pais.text(Persona.Pais);
    papa.appendChild(pais[0]);
    ActualizarComboStatus(Persona.ISO3, Persona.idTipoCliente);
    //Avisos Propios
    AvisosPropios = [];
    if (Persona.AvisosPropios.length > 0) {
        Preguntar({ mensaje: "Hay avisos propios asociados a esta persona, ¿Desea Cancelar los avisos propios al Guardar una Gestión?", funcion: function () { AvisosPropios = Persona.AvisosPropios; } });
    }
    //---Teléfonos
    var i;
    var lstTel = $("#lstPeTelefonos");
    lstTel.html("");
    lstTelefonos = [];
    indiceColaTelefonos = -1;
    for (i = 0; i < Persona.Telefonos.length; i++) {
        var Telefono = Persona.Telefonos[i];
        if (Telefono == undefined) break;
        if (Telefono.idTelefono == undefined) break;
        InsertarTelefono(Telefono, lstTel[0]);
    }

    //---Soportes
    var i;
    var pnlSop = $("#pnlPeSoportes");
    pnlSop.html("");
    for (i = 0; i < Persona.Soportes.length; i++) {
        var a = document.createElement("a");

        $(a).addClass("Soporte");
        $(a).text(Persona.Soportes[i].Nombre);
        a.onclick = new Function('Emergente({url:"/Emergentes/ctrlSoportes.aspx?idSoporte=' + Persona.Soportes[i].idSoporte + '"});'); //Persona.Soportes[i].Ubicacion;
        a.target = "_blank";
        pnlSop[0].appendChild(a);
    }
    //---Contactos    
    var j;
    var lstPC = $("#lstPeContactos");
    lstPC.html("");
    for (i = 0; i < Persona.PersonasContacto.length; i++) {
        MostrarPersonaContacto(Persona.PersonasContacto[i]);
        //var div = document.createElement("div");

        //$(div).addClass("Contacto");

        //var div1 = document.createElement("div");
        //$(div1).addClass("Nombre");
        //$(div1).text(Persona.PersonasContacto[i].Nombre);
        //div.appendChild(div1);

        //var div2 = document.createElement("div");
        //$(div2).addClass("Cargo");
        //$(div2).text(Persona.PersonasContacto[i].Cargo);
        //div.appendChild(div2);

        //var div3 = document.createElement("div");
        //$(div3).addClass("Correo");
        //$(div3).text(Persona.PersonasContacto[i].Correo);
        //div.appendChild(div3);

        //var div4 = document.createElement("div");
        //$(div4).addClass("Telefonos");
        //for (j = 0; j < Persona.PersonasContacto[i].Telefonos.length; j++) {

        //    var Telefono = Persona.PersonasContacto[i].Telefonos[j];
        //    if (Telefono == undefined) break;
        //    InsertarTelefono(Telefono, div4);

        //    //            var div5 = document.createElement("div");
        //    //            div5.className = "Telefono";
        //    //            div5.title = (Telefono.CodigoArea || "") + Telefono.Telefono;
        //    //            div5.onclick = LlamarNumero;
        //    //            $(div5).text(formatear(Telefono));
        //    //            div5.id = Telefono.idTelefono;
        //    //            div4.appendChild(div5);

        //    //            //            var a = document.createElement("a");
        //    //            $(a).addClass("Telefono");
        //    //            $(a).text(Persona.PersonasContacto[i].Telefonos[j]);
        //    //            a.href = "sip:" + ObtenerNumeros(Persona.PersonasContacto[i].Telefonos[j]);
        //    //            div4.appendChild(a);
        //}
        //div.appendChild(div4);

        //var btn = document.createElement("input");
        //btn.type = "Button";
        //$(btn).addClass("btnEliminar32");
        //btn.onclick = new Function("EliminarPersonasContacto(" + Persona.PersonasContacto[i].idPersonaContacto + ");");
        //btn.title = "Eliminar Persona Contacto";
        //div.appendChild(btn);
        //lstPC[0].appendChild(div);
        //lstPC[0].appendChild(document.createElement("HR"));
    }
    $("#pnlPeCuentas").html("Espere Por Favor...");
    //Personas_Cuentas_lst(_Personas_Cuentas_lst, Persona.idPersona, idOp());RA
    LlamarServicio(_Personas_Cuentas_lst, "Personas_Cuentas_lst", { idPersona: Persona.idPersona, idOperador: idOp() });
    $("#pnlPeGestiones").html("Espere Por Favor...");
    //Personas_Gestiones_lst(_Personas_Gestiones_lst, Persona.idPersona, idOp());RA
    LlamarServicio(_Personas_Gestiones_lst, "Personas_Gestiones_lst", { idPersona: Persona.idPersona, idOperador: idOp() });
    _Persona_Pagos_lst(Persona.Pagos)
    _Persona_Reclamos_lst(Persona.Reclamos)
    LlamarServicio(_Personas_Compromisos_lst, "Personas_Compromisos_lst", { idPersona: Persona.idPersona });
    LlamarServicio(_Avisos_Persona_lst, "Avisos_Persona_lst", { idPersona: Persona.idPersona });

    //Observaciones
    Tabla({
        Contenedor: "pnlPeObservaciones",
        Resultado: Persona.Observaciones,
        Campos: [
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Description", Campo: "Descripcion", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Severity", Campo: "Severidad", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });

    //Comentarios
    ActualizarComentario(Persona.Comentarios);
    //Reportes
    $("#pnlPeReportes").empty();

    for (var i = 0; i < Persona.Reportes.length; i++) {
        var div = document.createElement("div");
        div.className = "btnReporte";
        $("#pnlPeReportes")[0].appendChild(div);
        $(div).text(Persona.Reportes[i].Nombre);
        div.onclick = new Function('Emergente({ url: "/Emergentes/ctrlReporte.aspx?idTipoReporte=' + Persona.Reportes[i].idTipoReporte + '&idPersona=' + $("#idPersona").val() + '"});');
    }

    $("#pstGestion")[0].click();
    $("#datosPersona").slideDown();
    if (GestionAutomatica) {
        PlayGestion("Retrasada");
    }
}
function EliminarPersonasContacto(idPersonaContacto) {
    Preguntar({ mensaje: "¿Está seguro de que desea eliminar este contacto?", funcion: new Function("LlamarServicio(_PersonasContacto_del,'PersonasContacto_del',{idPersonaContacto:" + idPersonaContacto + "});") });
}
function _PersonasContacto_del(msg) {
    if (msg.d) {
        Mensaje({ mensaje: "La persona contacto ha sido eliminada satisfactoriamente" });
        Persona_Mostrar($("#idPersona").val());
    }
}
function EditarComentario() {
    Emergente({ url: "/Emergentes/ctrlComentario.aspx?idPersona=" + $("#idPersona").val(), width: 400, height: 300 });
}

function ActualizarComentario(Comentario) {
    $("#lblPeComentarios").html((Comentario || "").replace(/\n/g, '<br/>'));
}
function RefrescarCompromisos() {
    LlamarServicio(_Personas_Compromisos_lst, "Personas_Compromisos_lst", { idPersona: $("#idPersona").val() });
}
function _Personas_Compromisos_lst(msg) {
    Tabla({
        Contenedor: "pnlPeCompromisos",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFechaHora", Ordenacion: false, Filtro: false },
            { Titulo: "Total", Campo: "Total", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "Creator", Campo: "Creador", Clase: "grdTexto", Ordenacion: false, Filtro: false }
        ]
    });
}
function _Avisos_Persona_lst(msg) {
    Tabla({
        Contenedor: "pnlPeAvisos",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Reminder", Campo: "Aviso", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Date", Campo: "FechaAviso", Clase: "grdFechaHora", Ordenacion: false, Filtro: false },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Creator", Campo: "OperadorCrea", Clase: "grdTexto", Ordenacion: false, Filtro: false }
        ],
        FuncSeleccionar: "AbrirAviso(«idAviso»);"
    });

}
function _Personas_Gestiones_lst(msg) {
    //---Gestiones

    Tabla({
        Contenedor: "pnlPeGestiones",
        Resultado: msg.d,
        LimitarAltura: 400,
        Campos: [
            { Titulo: " ", Campo: "Img", Clase: "grdImagen", Imagen: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
//            { Titulo: "Description", Campo: "Descripcion", Clase: "grdTexto", Tooltip: function (a) { return "Cuentas:\n" + a.Cuentas.join("\n"); }, Ordenacion: true, Filtro: true },
            { Titulo: "Description", Campo: "Descripcion", Clase: "grdTexto", Tooltip: "Cuentas", Ordenacion: true, Filtro: true },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true, Color: "Tipo" }
        ],
        FuncSeleccionar: "AbrirCorreo(«id»,'«Img»',0);"
    });
}
function AbrirDocumento(idCuenta) {
    Emergente({ url: "/Emergentes/ctrlSoportes.aspx?idCuenta=" + idCuenta });
}
function _Personas_Cuentas_lst(msg) {
    //---Cuentas
    if (msg.d.length > 0 && msg.d[0].idPersona != idP()) return; //Si se van a cargar cuentas que no son de la persona actual... no la cargues.
    Tabla({
        Contenedor: "pnlPeCuentas",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idCuenta",
        LimitarAltura: 400,
        Campos: [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true, OnClick: "AbrirDocumento(«idCuenta»);", Resumen: "CUENTA" },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "DeliveryDate", Campo: "FechaEntrega", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true, Resumen: "NINGUNO" },
            { Titulo: "Client", Campo: "CodigoCliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Campo2: "Moneda", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Remaining", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalUSD", Campo: "TotalDolar", ToolTip: "CambioDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "DeudaDolar", ToolTip: "CambioDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Claim", Campo: "EnReclamo", Clase: "grdBooleano" },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Goal", Campo: "EsMeta", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "AbrirCuenta(«idCuenta»);"
    });

}
function AbrirCuenta(idCuenta) {
    Emergente({ url: "/Emergentes/ctrlCuenta.aspx?idCuenta=" + idCuenta });
}
function _Persona_Pagos_lst(Pagos) {
    Tabla({
        Contenedor: "pnlPePagos",
        Resultado: Pagos,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Code", Campo: "Codigo", Campo2: "", Clase: "grdTexto" },
            { Titulo: "Reference", Campo: "Referencia", Campo2: "", Clase: "grdTexto" },
            { Titulo: "Type", Campo: "Tipo", Campo2: "", Clase: "grdTexto" },
            { Titulo: "Date", Campo: "Fecha", Campo2: "", Clase: "grdFecha" },
            { Titulo: "Amount", Campo: "Monto", Campo2: "Moneda", Clase: "grdDecimal" },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto" },
            { Titulo: "Result", Campo: "Resultado", Clase: "grdTexto" }
        ],
        FuncSeleccionar: "AbrirPago(«idPago»,«Aprobado»,«Confirmado»);"
    });
}
function _Persona_Reclamos_lst(Reclamos) {

    Tabla({
        Contenedor: "pnlPeReclamos",
        Resultado: Reclamos,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Reason", Campo: "Motivo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Amount", Campo: "MontoLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Department", Campo: "Departamento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Result", Campo: "Resultado", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "AbrirReclamo(«idReclamo»);"
    });

}
function Persona_Mostrar(idPersona, propio) {
    if (propio == undefined) propio = false;
    $("#idPersona").val("");
    LlamarServicio(_Personas_sel, "Personas_sel", { idPersona: idPersona, idOperador: propio ? idOpA() : idOp() });
}
function ConfirmarGestion() {

    var Status = $("#cboGestStatus option:selected").html();
    Preguntar({ mensaje: "La gestión se guardara con el Status: " + Status + ", ¿Esta seguro?", funcion: GuardarGestion });
}
function GuardarGestion() {
    var tablaCuenta = $("#pnlPeCuentas")[0].getElementsByTagName("table")[0];
    var campos = obtenerCampos(tablaCuenta);
    var colIdCuenta = $.inArray("idCuenta", campos);

    var lstCuentas = Array();
    elem = tablaCuenta.getElementsByTagName("tr");
    for (i = 1; i < elem.length; i++) {
        try {
            var checkBox = elem[i].getElementsByTagName("td")[colIdCuenta].getElementsByTagName("input")[0];
            if (checkBox.checked) {
                lstCuentas.push($(checkBox).val());
            }
        }
        catch (ex) { }
    }

    var rtGestion = {};
    rtGestion.idOperador = idOpL();
    rtGestion.idPersona = $("#idPersona").val();
    rtGestion.idStatus = $('#cboGestStatus').find(":selected").val();
    idUltimoStatus = rtGestion.idStatus;
    rtGestion.Descripcion = $("#txtGestDescripcion").val();
    rtGestion.Cuentas = lstCuentas;
    if (rtGestion.idStatus == 0) {
        Mensaje({ mensaje: "No ha seleccionado un status" });
        return;
    }
    lstCuentas2 = ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    if (lstCuentas.length == 0 && elem.length > 2) {
        //        Mensaje({ mensaje: "No hay facturas seleccionadas, ¿Desea continuar?", titulo: "Guardando gestión", buttons: { Aceptar: function () { Gestion_ins(_Gestion_ins, rtGestion); $(this).dialog("close"); }, Cancelar: function () { $(this).dialog("close"); } } });
        Mensaje({ mensaje: "No ha seleccionado ninguna factura, No Puede Guardar la gestión", titulo: "Guardando gestión" });
        return
    }
    //Gestion_ins(_Gestion_ins, rtGestion);RA
    LlamarServicio(_Gestion_ins, "Gestion_ins", { Gestion: rtGestion, Adjuntos: lstAdjuntos });
}

function obtenerCuentasTabla(tablaCuenta) {
    var campos = obtenerCampos(tablaCuenta);
    var colIdCuenta = $.inArray("idCuenta", campos);
    var facturas = "";

    elem = tablaCuenta.getElementsByTagName("tr");
    for (i = 1; i < elem.length; i++) {
        try {
            var checkBox = elem[i].getElementsByTagName("td")[colIdCuenta].getElementsByTagName("input")[0];
            if (checkBox.checked) {
                if (facturas == "") {
                    facturas = $(checkBox).val();
                } else {
                    facturas = facturas + "," + $(checkBox).val();
                }
            }
        }
        catch (ex) { }
    }

    return facturas;
}
function NuevoPago() {
    var tablaCuenta = $("#pnlPeCuentas")[0].getElementsByTagName("table")[0];
    //Cuentas = obtenerCuentasTabla(tablaCuenta);
    Cuentas = ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    idPersona = $("#idPersona").val();
    idOperadorSup = idOp();
    idOperadorAct = idOpA();
    idOperadorLog = idOpL();
    //    if (Cuentas == "")
    //    if (Cuentas.length == 0)
    //    {
    //        Mensaje({ mensaje: "Debe seleccionar al menos una cuenta" });
    //    }
    //    else
    //    {
    //        Emergente({ url: "/Emergentes/ctrlNuevoPago.aspx?cuentas=" + cuentas });
    Emergente({ url: "/Emergentes/ctrlNuevoPago.aspx", modal: false });
    //    }
}
function NuevoCompromiso() {
    Cuentas = ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    idPersona = $("#idPersona").val();
    idOperadorSup = idOp();
    idOperadorAct = idOpA();
    idOperadorLog = idOpL();
    if (Cuentas.length == 0) {
        Mensaje({ mensaje: "Debe seleccionar al menos una cuenta" });
    }
    else {
        Emergente({ url: "/Emergentes/ctrlCompromisos.aspx" });
    }
}
function NuevoReclamo() {
    Cuentas = ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    idPersona = $("#idPersona").val();
    idOperadorSup = idOp();
    idOperadorAct = idOpA();
    idOperadorLog = idOpL();
    if (Cuentas.length == 0) {
        Mensaje({ mensaje: "Debe seleccionar al menos una cuenta" });
    }
    else {
        Emergente({ url: "/Emergentes/ctrlNuevoReclamo.aspx?oper=Ins" });
    }
}
function Pais() {
    return $("#idPais").val();
}
//------------------------------------------------------------------------------------------------------------Generales;
function AbrirCorreo(id, Tipo1, Tipo) {
    if (id == 0 || id == undefined) return;
    if (Tipo1.indexOf("Telefono") != -1) {
        location.href = "/Emergentes/Grabaciones.aspx?Grabacion=" + id + "&Tipo=id";
    }
    else if (Tipo1.indexOf("Correo") != -1) {
        Emergente({ url: "/Emergentes/ctrlCorreo.aspx?idCorreo=" + id + "&idOperador=" + idOpA() + "&Tipo=" + Tipo, modal: false, close: function (event, ui) { RefrescarCorreos(); } });
    } else if (Tipo1.indexOf("UsuarioAdjunto") != -1) {
        Emergente({ url: "/Emergentes/ctrlSoportes.aspx?idGestion=" + id, modal: false });
    }
}
function AbrirPago(idPago, Aprobado, Confirmado) {
    //    if (Confirmado == null || Confirmado == false)
    if (Aprobado == false || Confirmado == false) {
        idPersona = $("#idPersona").val();
        idOperadorSup = idOp();
        idOperadorAct = idOpA();
        idOperadorLog = idOpL();
        Emergente({ url: "/Emergentes/ctrlNuevoPago.aspx?idPago=" + idPago.toString() });
    }
    else {
        Mensaje({ mensaje: "El Pago ya ha sido confirmado, no puede editarlo" });
    }
}
function AbrirCompromiso(idCompromiso) {
    //Mensaje({ mensaje: "Todavía no implementado" });
}
function AbrirReclamo(idReclamo) {
    Emergente({ url: "/Emergentes/ctrlReclamos.aspx?idReclamo=" + idReclamo.toString() });
    //Mensaje({ mensaje: "Todavía no implementado" });
}
//function Llamar()
//{
//    Emergente({ url: "/Emergentes/ctrlLlamada.aspx" });
//}
//------------------------------------------------------------------------------------------------------------Cartera
function _Cartera_lst(msg) {
    Cartera = msg.d;
    $("#pnlCartera").empty();
    bandera = 0;
    var Panel = $("#pnlCartera")[0];
    for (var i = 0; i < msg.d.length; i++) {
        Grupo = msg.d[i];
        
        
        if (bandera == 0)
        {
        
            //        var h3 = document.createElement("h3");
            //        $(h3).text(Grupo.Nombre);
            //        Panel.appendChild(h3);
            var enc = document.createElement("div");
            var img = document.createElement("img");
            img.src = "Img/Soporte_16.png";
            //img.onclick = function (e) { Cartera_Descargar(Grupo.idCampana); e.cancelBubble=true;};
            img.onclick = new Function("Cartera_Descargar(" + Grupo.idCampana + "); event.cancelBubble=true;")
            var spn = document.createElement("span");
            $(spn).text(Grupo.Nombre + "[" + Grupo.Personas.length + " Personas]");
            enc.appendChild(spn);
            enc.appendChild(img);


        enc.id = "_pnlCartera" + i.toString();
        enc.className = "EncSeccion";
        Panel.appendChild(enc);
        $(enc).click(function () {
            $('#' + this.id.substr(1)).slideToggle();
            OcultarAvisos();
        })

        var div = document.createElement("div");
        div.id = "pnlCartera" + i.toString();
        div.className = "Seccion";
        Panel.appendChild(div);

            Tabla({
                Contenedor: div.id,
                Resultado: Grupo.Personas,
                idSeleccion: (OperadorEs('SU', 'CO', 'GE', 'AD') ? "idPersona" : undefined),
                Campos: [
                { Titulo: "Status", Campo: function (a) { return "/Img/" + a.TipoStatus + "24.png"; }, Clase: "grdTexto", Ordenacion: "TipoStatus", Tooltip: "StatusUltimaGestion", Filtro: false, Imagen: true },
                { Titulo: "R", Campo: "TieneAviso", Clase: "grdBooleano", Ordenacion: true, Filtro: false },
                { Titulo: "LastManagement", Campo: "FechaUltimaGestion", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
                { Titulo: "LastOperator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
                { Titulo: "AssignedOperator", Campo: "UltimoOperador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
                { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
                { Titulo: "Rif", Campo: "Rif", Clase: "grdTexto", Ordenacion: true, Filtro: true },
                { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
                { Titulo: "TotalUSD", Campo: "TotalDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
                { Titulo: "RemainingUSD", Campo: "RestanteDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
                { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
                { Titulo: "RemainingLocal", Campo: "RestanteLocal", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
                { Titulo: "Country", Campo: function (a) { return "/Img/Banderas/" + a.idPais + "24.png"; }, Clase: "grdTexto", Ordenacion: "Iso3", Filtro: false, Imagen: true },
                ],
                FuncSeleccionar: "if (OperadorEs('SO','BO','SU','CO','GE','AD','SI')) Persona_Mostrar(«idPersona»);"
            });
            if (Grupo.idCampana == -2) {
                bandera = 1;
            }
                

        }



    }

}
function _OperadoresSupervisados_lst(msg) {
    if (msg.d.length == 0) return;
    LlenarCombo({ Combo: "cboOperadorAviso", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    $("#cboOperadorAviso").val(idOp());
    var Op = msg.d;
    var papa = $("#pnlSupervision");
    papa.html("");
    papa = papa[0];
    var jerarquia = "";
    var tabla;
    var celdaAnterior = papa;
    for (var i = 0; i < Op.length; i++) {
        if (Op[i].Jerarquia == "-") continue;
        var JA = Op[i].Jerarquia.length; //Actual
        var JP = jerarquia.length; //Pasada
        if (JA > JP) {
            papa = celdaAnterior;
            tabla = CrearTabla();
            papa.appendChild(tabla);
        }
        if (JA < JP) {
            for (var j = 0; j < (JP - JA) / 3; j++) {
                while ((papa = papa.parentNode).tagName.toLowerCase() != "td") { };
            }
            tabla = papa.getElementsByTagName("table")[0];
        }
        var fila = AgregarFila(tabla, "itmSeleccionable");
        fila.onclick = new Function("Supervisar(" + Op[i].idOperador + ");");
        var celda = AgregarHeader(fila, "");
        var img = document.createElement("img");
        img.src = "Img/usuario24.png";
        celda.appendChild(img);
        AgregarHeader(fila, Op[i].Nombre).style.textAlign = "left";
        fila = AgregarFila(tabla);
        AgregarCelda(fila, "   ");
        celdaAnterior = AgregarCelda(fila, "");
        jerarquia = Op[i].Jerarquia;
    }
}
//------------------------------------------------------------------------------------------------------------Indicadores
function _ActualizarGrafico(msg) {
    var hora = new Date().getTime();
    var Ruta = msg.d + "?hora=" + hora;
    $("#chrtGestiones").attr("src", Ruta);
}

function _ActualizarIndicadores(msg) {
    $("#lblMetaGestiones").text((msg.d.MetaGestiones));
    $("#lblRealGestiones").text((msg.d.RealGestiones));
    Colorear("barGestiones", "lblRealGestiones", msg.d.MetaGestiones, msg.d.RealGestiones);

    $("#lblMetaGestionesPersonas").text((msg.d.MetaGestionesPersonas));
    $("#lblRealGestionesPersonas").text((msg.d.RealGestionesPersonas));
    Colorear("barGestionesPersonas", "lblRealGestionesPersonas", msg.d.MetaGestionesPersonas, msg.d.RealGestionesPersonas);

    $("#lblMetaMonto").text(FormatDecimal(msg.d.MetaMonto));
    $("#lblRealMonto").text(FormatDecimal(msg.d.RealMonto));
    Colorear("barMonto", "lblRealMonto", msg.d.MetaMonto, msg.d.RealMonto);

    $("#lblMetaTiempo").text(Convert.ToTimeSpan(msg.d.MetaTiempo * 60));
    $("#lblRealTiempo").text(Convert.ToTimeSpan(msg.d.RealTiempo * 60));
    Colorear("barTiempo", "lblRealTiempo", msg.d.MetaTiempo, msg.d.RealTiempo);

    $("#lblMetaTiempoInactivo").text(Convert.ToTimeSpan(msg.d.MetaTiempoInactivo * 60));
    $("#lblRealTiempoInactivo").text(Convert.ToTimeSpan(msg.d.RealTiempoInactivo * 60));
    Colorear("barInactivo", "lblRealTiempoInactivo", msg.d.MetaTiempoInactivo, msg.d.RealTiempoInactivo, true);

    $("#lblMetaTMO").text(Convert.ToTimeSpan(Convert.ToInt32(msg.d.MetaTMO * 60)));
    $("#lblRealTMO").text(Convert.ToTimeSpan(Convert.ToInt32(msg.d.RealTMO * 60)));
    Colorear("barTMO", "lblRealTMO", msg.d.MetaTMO, msg.d.RealTMO, true);

    $("#lblCuentasAsignadas").text(msg.d.CuentasAsignadas);
    $("#lblPersonasAsignadas").text(msg.d.PersonasAsignadas);

    $("#lblTiempoEnLlamada").text(Convert.ToTimeSpan(msg.d.TiempoEnLlamada));
    $("#lblLlamadasRealizadas").text(msg.d.LlamadasRealizadas);
    $("#lblLlamadasContestadas").text(msg.d.LlamadasContestadas);
    $("#lblLlamadasNoContestadas").text(msg.d.LlamadasNoContestadas);
}
function Colorear(barra, id, meta, real, maximo) {
    var Control = $("#" + id);
    var Barra = $("#" + barra);
    var Color;
    var Tamano;
    Barra.text(meta == 0 ? "0%" : FormatDecimal(real * 100.0 / meta) + "%");
    if (maximo !== true) {
        Tamano = (meta == 0 || real / meta > 1.2 ? 120 : real * 100.0 / meta);
        if (real >= meta)//Excelente
        {
            Color = Colores.Positivo;
        }
        else if (real >= meta * .8)//Bastante Bien, casi cumpliendo la meta
        {
            Color = Colores.Neutral;
        }
        else // mal, lejos de la meta
        {
            Color = Colores.Negativo;
        }
    }
    else {
        Tamano = (meta == 0 || real / meta > 1.2 ? 120 : real * 100.0 / meta);
        if (real <= meta) //Excelente
        {
            Color = Colores.Positivo;
        }
        else if (real >= meta * 1.2) //mal, lejos de la meta
        {
            Color = Colores.Negativo;
        }
        else //Bastante Bien, casi cumpliendo la meta
        {
            Color = Colores.Neutral;
        }
    }
    Control.css("color", Color);
    Barra.css("background-color", Color);
    Barra.css("width", Tamano);
    if (Tamano == 120) {
        Barra.css("border-right", "1px dotted white");
    }
}

//Warning
function BuscarRNC() {
    var rif = $("#lblPeRif").text();
    rif = rif.replace(/[-]/g, "");
    Emergente({ url: "http://rncenlinea.snc.gob.ve/reportes/resultado_busqueda?p=1&rif=" + rif + "&search=RIF" });
}

function BuscarGoogle() {
    var Nombre = $("#lblPeNombre").text();
    Nombre = Nombre.replace(/[\s]/g, "+");
    Nombre = Nombre.replace(/&/g, "%26");
    window.open("https://www.google.co.ve/#q=" + Nombre, "_blank");
}
function BuscarPAC() {
    var Nombre = $("#lblPeNombre").text();
    Nombre = Nombre.replace(/[\s]/g, "+");
    Nombre = Nombre.replace(/&/g, "%26");
    window.open("http://www.pac.com.ve/index.php?option=com_jumi&fileid=9&Itemid=119&keyword=" + Nombre, "_blank");
}
function CrearAviso() {
    var Aviso = {};
    Aviso.idOperador = $("#cboOperadorAviso").val();
    Aviso.idOperadorCrea = idOpL();
    Aviso.idPersona = $("#idPersona").val();
    Aviso.Prioritario = $("#chkAvisoPrioritario")[0].checked;

    if ($("#dtpFechaAviso").val() == "") {
        Mensaje({ mensaje: "Debe seleccionar una fecha válida para este aviso" });
        return;
    }
    if ($("#txtNuevoAviso").val() == "") {
        Mensaje({ mensaje: "Debe seleccionar un Texto para este aviso" });
        return;
    }
    Aviso.FechaAviso = Convert.ToDateTime($("#dtpFechaAviso").val() + " " + $("#cboHoraAviso").val() + ":" + $("#cboMinutoAviso").val()).ToString("JSON");
    Aviso.Aviso = $("#txtNuevoAviso").val();
    //Avisos_ins(_Avisos_ins, Aviso);RA
    LlamarServicio(_Avisos_ins, "Avisos_ins", { Aviso: Aviso });
}
function _Avisos_ins(msg) {
    if (msg.d === true) {
        Mensaje({ mensaje: "Tu aviso ha sido creado exitosamente" });
        $("#txtNuevoAviso").val("");
    }
}
function Seniat() {
    Emergente({ url: "http://contribuyente.seniat.gob.ve/BuscaRif/BuscaRif.jsp" });
}
function AgregarTelefono() {
    Emergente({ url: "/Emergentes/ctrlNuevoTelefono.aspx?idPersona=" + $("#idPersona").val(), width: 300, height: 200 });
}
function EliminarTelefono() {
    Emergente({ url: "/Emergentes/ctrlEliminarTelefono.aspx?idPersona=" + $("#idPersona").val() + "&idOperador=" + idOp() });
}
function AgregarContacto() {
    Emergente({ url: "/Emergentes/ctrlNuevoContacto.aspx?idPersona=" + $("#idPersona").val(), width: 300, height: 300 });
}
function FiltrarPersona() {
    var filtro = $("#txtFiltroPersona").val().toUpperCase();
    if (filtro == "") {
        LlenarCombo({ Combo: "cboaPersona", Resultado: PersonasTocadas, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
    } else {
        LlenarCombo({ Combo: "cboaPersona", Resultado: PersonasTocadas.where(function (a) { return a.Nombre.toUpperCase().indexOf(filtro) != -1; }), CampoId: "idPersona", CampoValor: "Nombre" });
    }
}
function Cartera_Descargar(idCampana) {
    if (idCampana == undefined) {
        location.href = "/Descargas/Descargas.aspx?Reporte=Cartera&idOperador=" + idOp() + "&IncluyeAutomatico=" + ($("#chkIncluyeAutomatico")[0].checked ? "true" : "false" + "&Supervisor=" + (idOpL() != idOp() ? "true" : "false"));
    } else {
        location.href = "/Descargas/Descargas.aspx?Reporte=Cartera&idOperador=" + idOp() + "&IncluyeAutomatico=" + ($("#chkIncluyeAutomatico")[0].checked ? "true" : "false" + "&Supervisor=" + (idOpL() != idOp() ? "true" : "false") + "&idCampana=" + idCampana);
    }
}

function OcultarAvisos() {
    if ($('#Avisos').css('left') != "-340px") {
        $('#Avisos').animate({
            left: "-340px"
        }, 1000, function () {

        });
    }
}
function OperadorEs() {
    for (var i = 0; i < arguments.length; i++) {
        if (TipoOperador.indexOf(arguments[i]) != -1) return true;
    }
    return false;
}
function _Avisos_lst(msg) {
    Vencidos = 0;
    var i;
    for (i = 0; i < msg.d.length; i++) {
        Avisos_Cargar(msg.d[i]);
    }
}
function _Avisos_lst2(msg) {
    _Avisos_lst(msg)
    if (Vencidos > 0) {
        Mensaje({ mensaje: Recursos["msgExpiredReminders"].replace("#", Vencidos) });
    }
}
function Avisos_Cargar(Aviso) {
    $("#idAviso").val(Aviso.idAviso);
    var tabla = document.getElementById("tblAvisos");
    var tbody = tabla.getElementsByTagName("tbody")[0];
    var fila = document.createElement("TR");
    Aviso.FechaAviso = Convert.ToDateTime(Aviso.FechaAviso);
    AgregarCelda(fila, Aviso.FechaAviso.ToString(Regional.FormatoFechaHora));
    fila.setAttribute("class", "itmAviso");
    fila.onclick = new Function("AbrirAviso(" + Aviso.idAviso + ");");
    AgregarCelda(fila, Aviso.Aviso);
    AgregarCelda(fila, Aviso.OperadorCrea);
    AgregarCelda(fila, Aviso.CodigoPersona);
    var filas = tbody.getElementsByTagName("TR");
    var referencia = null;
    var i;
    for (i = 1; i < filas.length; i++) {
        var celdas = filas[i].getElementsByTagName("TD");
        if (Convert.ToDateTime($(celdas[0]).text()) > Aviso.FechaAviso) {
            referencia = filas[i];
            break;
        }
    }
    if (referencia == null) {
        tbody.appendChild(fila);
    } else {
        tbody.insertBefore(fila, referencia);
    }
    var tiempo = Aviso.FechaAviso - (new Date());
    if (tiempo > 0) {
        if (tiempo < 24 * 60 * 60 * 1000) // si el tiempo es menor a 1 día.
        {
            Avisos.push({ Aviso: Aviso.idAviso, Handle: setTimeout("AbrirAviso(" + Aviso.idAviso + ");", tiempo) });
        }
    } else {
        fila.setAttribute("class", "itmAvisoVencido");
        Vencidos++;
    }
}
function Avisos_Actualizar() {
    var tabla = document.getElementById("tblAvisos");
    var tbody = tabla.getElementsByTagName("tbody")[0];
    var filas = tbody.getElementsByTagName("TR");
    var i;
    for (i = filas.length - 1; i > 0; i--) {
        tbody.removeChild(filas[i]);
    }
    for (i = 0; i < Avisos.length; i++) {
        clearTimeout(Avisos[i].Handle);
    }
    Avisos = [];

    //    $("#idAviso").val("0");
    //Avisos_lst(_Avisos_lst, 0);RA
    LlamarServicio(_Avisos_lst, "Avisos_lst", { idAviso: 0 });
}
function AbrirAviso(idAviso) {
    var iframe = $("#aviso iframe");
    iframe[0].src = "/Emergentes/ctrlAviso.aspx?idAviso=" + idAviso;
    var argumentos = { modal: false, width: 450, height: 400, minWidth: 450, minHeight: 400 }
    argumentos.close = function (event, ui) { $("#aviso iframe")[0].src = "about:blank"; };
    argumentos.hide = { effect: "fade", duration: 500 };
    $("#aviso").dialog(argumentos);
}
function CerrarAviso() {
    $("#aviso").dialog("close");
}
function _Operadores_Suplantar_lst(msg) {

    Tabla({
        Contenedor: "pnlOperadoresYoSuplantar",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto" },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha" },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha" },
            { Titulo: "Dashboard", Campo: "Indicadores", Clase: "grdBooleano" },
            { Titulo: "InBox", Campo: "Correo", Clase: "grdBooleano" },
            { Titulo: "Portfolio", Campo: "Cartera", Clase: "grdBooleano" },
            { Titulo: "Management", Campo: "Gestion", Clase: "grdBooleano" },
            { Titulo: "Supervision", Campo: "Supervision", Clase: "grdBooleano" },
            { Titulo: "Distribution", Campo: "Distribucion", Clase: "grdBooleano" },
            { Titulo: "Reports", Campo: "Reportes", Clase: "grdBooleano" },
        ],
        FuncSeleccionar: "Suplantar(«idOperador»);"
    });

}
function _Operadores_Suplantarme_lst(msg) {

    Tabla({
        Contenedor: "pnlOperadoresSuplantarme",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Suplente", Campo: "Operador", Clase: "grdTexto" },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha" },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha" },
            { Titulo: "Dashboard", Campo: "Indicadores", Clase: "grdBooleano" },
            { Titulo: "InBox", Campo: "Correo", Clase: "grdBooleano" },
            { Titulo: "Portfolio", Campo: "Cartera", Clase: "grdBooleano" },
            { Titulo: "Management", Campo: "Gestion", Clase: "grdBooleano" },
            { Titulo: "Supervision", Campo: "Supervision", Clase: "grdBooleano" },
            { Titulo: "Distribution", Campo: "Distribucion", Clase: "grdBooleano" },
            { Titulo: "Reports", Campo: "Reportes", Clase: "grdBooleano" },
        ],
        FuncEliminar: "EliminarSuplantacion(«idSuplente»);"
    });

}
function Suplantar(idOperador) {
    $("#pnlCartera").html("Espere por favor...");
    $("#idPersona").val("");

    $("#minipnlSuplantacion")[0].style.display = (idOpL() == idOperador) ? 'none' : '';
    LlamarServicio(Suplantar_Nombre, "Nombre", { idOperador: idOperador });
    $("#idOperador").val(idOperador);
    $("#idOperadorAct").val(idOperador);
    LlamarServicio(_Personas_Tocadas_lst, "Personas_Tocadas_lst", { idOperador: idOp() });
    ActualizarCartera();
    RefrescarCorreos();
    $("#pstCartera").click();
    LlamarServicio(_ActualizarGraficoSupervision, "ActualizarGraficoSupervision", { idOperador: idOperador });
    LlamarServicio(_ActualizarGraficoPorSemana, "ActualizarGraficoPorSemana", { idOperador: idOperador });
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisadosConSupervisor_lst", { idOperador: idOperador });
    LlamarServicio(_ActualizarIndicadores, "ActualizarIndicadores", { idOperador: idOperador });
}
function Revertir() {
    Suplantar(idOpL()); //Suplantar al Logueado
}
function Novedades() {
    if (llamadas != 0) return;
    var Hora = $("#Hora").val();
    if (Hora == "") Hora = null;
    LlamarServicio(_Novedades, "Novedades", { idOperadorLog: idOpL(), idOperador: idOp(), idOperadorAct: idOpA(), Hora: Hora });
}
function _Novedades(msg) {
    var Result = msg.d;
    if (Result.Reiniciar) {
        Emergente({ url: "/Default.aspx?RL=1" });
    }
    if (Result.Avisos) {
        Avisos_Actualizar();
    }
    if (Result.Correos == null) {
        $("#NoRecibeCorreos").show();
        $("#pstBandejaEntrada").css({ color: "Red" });
    } else {
        $("#NoRecibeCorreos").hide();
        $("#pstBandejaEntrada").css({ color: "" });
    }
    if (Result.Correos) {
        RefrescarCorreos();
    }
    if (Result.Cartera) {
        //Cartera_lst(_Cartera_lst, idOp(), idOpL() != idOp());RA
        //LlamarServicio(_Cartera_lst, "Cartera_lst", { idOperador: idOp(), Supervisado: idOpL() != idOp() });
        ActualizarCartera();
    }


    if (Result.Colas) {
        ActualizarCartera();
    }

    $("#Hora").val(Result.Hora);
}

function Importar(CodigoPersona, idOrigen, idPais) {
    LlamarServicio(_ImportarPersona_sel, "ImportarPersona_sel", { CodigoPersona: CodigoPersona, idOrigen: idOrigen, idPais: idPais });
}
function _ImportarPersona_sel(msg) {
    if (msg.d == 0) {
        Mensaje({ mensaje: "No se pudo importar el cliente" });
        return;
    }
    Persona_Mostrar(msg.d);
}
function ActualizarCartera() {
    LlamarServicio(_Cartera_lst, 'Cartera_lst', { idOperador: idOp(), Supervisado: idOpL() != idOp(), IncluyeAutomatico: $("#chkIncluyeAutomatico")[0].checked });
}


function InsertarColas(posicion) {
    //modificar para crear un arreglo y enviar un solo objeto al servidor
    var x = document.getElementsByTagName("div").pnlCartera.children.length / 2;
    var valores = [];
    for (var i = 0; i < x; i++) {
        var nombre = "pnlCartera" + i;
        valores = valores.concat(ObtenerSeleccionados(nombre).select("idPersona"));
    }

    LlamarServicio(_Agregar_Cola_Sup, 'Agregar_Cola_Sup', { Valores: valores, Lugar: posicion });
    //sacar este mensaje para que se reciba en el mensaje del servicio
    ActualizarCartera();
}

function _Agregar_Cola_Sup(msg) {
    if (msg.d == 0) {
        Mensaje({ mensaje: "Cuenta procesada a la cola del operador correctamente" });
        return;
    }

}

function setCookieCulture() {
    document.cookie = 'Culture=' + $('#ddlIdioma').val();
}
//--------------------
function formatear(Telefono) {
    if (Telefono.CodigoArea == null) {
        var Tel = Telefono.Telefono.replace(/[-]/g, "").replace(/[\/]/g, "");
        return "(" + Tel.substring(0, 4) + ") " + Tel.substring(4);
    } else {
        return "(" + Telefono.CodigoArea + ") " + Telefono.Telefono;
    }
}

function LlamarNumero() {
    Llamada({ url: '/Emergentes/ctrlLlamada.aspx?Numero=' + this.title + "&idTelefono=" + this.id + "&idPersona=" + idP() });
}
function FinalizarLlamada() {
    try {
        CerrarLlamada();
    } catch (e) { }
    LlamarServicio(_Personas_Gestiones_lst, "Personas_Gestiones_lst", { idPersona: idP() });
    if (GestionAutomatica && !PausaGestion) {
        Preguntar({ mensaje: "Llamada Finalizada, ¿Intentar el siguiente Teléfono?", funcion: ColaDeLlamadas });
    }
}
function InsertarTelefono(Telefono, contenedor) {
    if (contenedor == undefined) contenedor = $('#lstPeTelefonos')[0];
    var div = document.createElement("div");
    div.className = "Telefono";
    div.title = (Telefono.CodigoArea || "") + (Telefono.Telefono || "");
    div.onclick = LlamarNumero;
    $(div).text(formatear(Telefono));
    div.id = Telefono.idTelefono;
    contenedor.appendChild(div);
    lstTelefonos.push(Telefono);
}
function ColaDeLlamadas() {
    try {
        if (PausaGestion) return;
    } catch (e) { }
    if (indiceColaTelefonos != -1) {
        try {
            $("#" + lstTelefonos[indiceColaTelefonos].idTelefono + ".Telefono").removeClass("TelefonoActivo");
        } catch (e) { }
    }
    indiceColaTelefonos++;

    if (indiceColaTelefonos >= lstTelefonos.length) {
        Mensaje({ mensaje: "Ya no hay más teléfonos para llamar, por favor haga una gestión para continuar." });
    }
    else {
        var Telefono = lstTelefonos[indiceColaTelefonos];
        //setTimeout('FinalizarLlamada();', 5000);
        $("#" + lstTelefonos[indiceColaTelefonos].idTelefono).addClass("TelefonoActivo");
        //Mensaje({ mensaje: "Llamando a :" + Telefono.Telefono });
        Llamada({ url: '/Emergentes/ctrlLlamada.aspx?Numero=' + (Telefono.CodigoArea || "") + Telefono.Telefono + "&idTelefono=" + Telefono.idTelefono + "&idPersona=" + idP() });
    }
}
function PlayGestion(Retrasada) {
    LlamarServicio(_Acciones_ins, "Acciones_ins", { Accion: "G>", idOperador: idOp() });
    PausaGestion = false;
    GestionAutomatica = true;
    if (idP() > 0) {
        if (Retrasada == "Retrasada") {
            setTimeout("Retraso(20);", 1000);
        } else {
            ColaDeLlamadas();
        }
    } else {
        FinalizarGestion();
    }
    $("#btnPlay").hide();
    $("#btnPause").show();
    $("#btnStop").show();
}
function PauseGestion() {
    LlamarServicio(_Acciones_ins, "Acciones_ins", { Accion: "G|", idOperador: idOp() });
    PausaGestion = true;
    $("#btnPlay").show();
    $("#btnPause").hide();
    $("#btnStop").show();
}
function StopGestion() {
    LlamarServicio(_Acciones_ins, "Acciones_ins", { Accion: "G*", idOperador: idOp() });
    PausaGestion = true;
    GestionAutomatica = false;
    $("#btnPlay").show();
    $("#btnPause").hide();
    $("#btnStop").hide();
}
function _Acciones_ins() {
    //Cuerpo.
}
function Retraso(n) {
    $("#Contador").text(n);
    if (n <= 0) {
        $("#Contador").text("");
        ColaDeLlamadas();
    } else {
        setTimeout("Retraso(" + (n - 1).toString() + ");", 1000);
    }
}
function EjecutarPruebas() {
    indicePrueba = 0;
    Prueba();
}
function Prueba() {
    switch (indicePrueba) {
        case 0: //Tiempo de Respuesta
            tiempo = new Date();
            LlamarServicio(_Prueba, "PruebasTransferencia", { Data: "-" });
            break;
        case 1: //Prueba Subida
            tiempo = new Date();
            var Data = "";
            for (var i = 0; i < 5000; i++) { Data += "**********"; }
            LlamarServicio(_Prueba, "PruebasTransferencia", { Data: Data });
            break;
        case 2:
            tiempo = new Date();
            LlamarServicio(_Prueba, "PruebasTransferencia", { Data: "" });
            break;
        case 3:
            tiempo = new Date();
            LlamarServicio(_Prueba, "PruebasTransferencia", { Data: "B" });
            break;
        case 4:
            LlamarServicio(_Prueba, "PruebasPing", {});
            break;
        case 5:
            LlamarServicio(_Prueba, "Bloqueos");
            break;
        default: break;
    }
}
function _Prueba(msg) {
    switch (indicePrueba) {
        case 0: //Tiempo de Respuesta
            var tiempo2 = new Date();
            $("#SWTiempoRespuesta").text(((tiempo2 - tiempo) / 1000).toString() + " s");
            break;
        case 1: //Prueba Subida
            var tiempo2 = new Date();
            $("#SWTiempoSubida").text(((tiempo2 - tiempo) / 1000).toString() + " s");
            break;
        case 2: //Prueba Bajada
            var tiempo2 = new Date();
            $("#SWTiempoBajada").text(((tiempo2 - tiempo) / 1000).toString() + " s");
            break;
        case 3: //Prueba Base de Datos
            var tiempo2 = new Date();
            $("#BDTiempoTransporte").text(((tiempo2 - tiempo) / 1000).toString() + " s");
            break;
        case 4: //Prueba Ping
            $("#BDPing").text(msg.d);
            break;
        case 5: //Bloqueos
            _Bloqueos(msg);
            break;
        default: break;
    }
    indicePrueba++;
    Prueba()
}
function EliminarSuplantacion(idSuplente) {
    LlamarServicio(_Suplentes_del, "Suplentes_del", { idSuplente: idSuplente });
}
function _Suplentes_del(msg) {
    ActualizarSuplentes();
}
function ActualizarSuplentes() {
    LlamarServicio(_Operadores_Suplantarme_lst, "Operadores_Suplantarme_lst");
    LlamarServicio(_Operadores_Suplantar_lst, "Operadores_Suplantar_lst");
}
function ConsultarBloqueos() {
    LlamarServicio(_Bloqueos, "Bloqueos");
}
function _Bloqueos(msg) {
    Tabla({
        Contenedor: "BDBloqueos",
        Resultado: msg.d,
        Campos: [
            { Titulo: "P1", Campo: "p1", Clase: "grdEntero", Ordenacion: false, Filtro: false },
            { Titulo: "P2", Campo: "p2", Clase: "grdEntero", Ordenacion: false, Filtro: false },
            { Titulo: "Usuario Bloqueado", Campo: "UsuarioBloqueado", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Usuario Bloqueador", Campo: "UsuarioBloqueador", Clase: "grdTexto", Ordenacion: false, Filtro: false }
        ],
        FuncEliminar: "Matar(«p1»);"
    });
}
function Matar(id) {
    LlamarServicio(_Matar, "Matar", { spid: id });
}
function _Matar(msd) {
    ConsultarBloqueos();
    Mensaje({ mensaje: "El proceso ha sido eliminado" });
}
function EjecutarComando() {
    LlamarServicio(_EjecutarComando, "EjecutarComando", { Comando: $("#txtBDComando").val() });
}
function _EjecutarComando(msg) {
    TablaDataSet(msg.d, 'pnlResultadoBDComando');
}
function ExcluirMetasCuentas() {
    var Motivo = $("#cboExclusionMotivo").val();
    if (Motivo == "") { Mensaje({ mensaje: "Debe Seleccionar un Motivo para Excluir" }); return; }
    Cuentas = ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    if (Cuentas.length == 0) { Mensaje({ mensaje: "Debe Seleccionar Cuentas Excluir" }); return; }
    LlamarServicio(_Exclusiones_ins, "Exclusiones_inss", { idOperador: idOp(), Cuentas: Cuentas, idMotivo: Motivo });
}
function ExcluirMetasPersona() {
    var Motivo = $("#cboExclusionMotivo").val();
    if (Motivo == "") { Mensaje({ mensaje: "Debe Seleccionar un Motivo para Excluir" }); return; }
    LlamarServicio(_Exclusiones_ins, "Exclusiones_ins", { idOperador: idOp(), idPersona: idP(), idMotivo: Motivo });
}
function _Exclusiones_ins(msg) {
    Mensaje({ mensaje: msg.d });
}

function CargarPersonaContacto(idPersonaContacto) {
    if (idPersonaContacto != 0) {
        LlamarServicio(_PersonaContacto_sel, "PersonaContacto_sel", { idPersonaContacto: idPersonaContacto })
    }
}
function _PersonaContacto_sel(msg) {
    MostrarPersonaContacto(msg.d);
}
function MostrarPersonaContacto(PersonaContacto) {

    var lstPC = $("#lstPeContactos");

    var div = document.createElement("div");

    $(div).addClass("Contacto");

    var div1 = document.createElement("div");
    $(div1).addClass("Nombre");
    $(div1).text(PersonaContacto.Nombre);
    div.appendChild(div1);

    var div2 = document.createElement("div");
    $(div2).addClass("Cargo");
    $(div2).text(PersonaContacto.Cargo);
    div.appendChild(div2);

    var div3 = document.createElement("div");
    $(div3).addClass("Correo");
    $(div3).text(PersonaContacto.Correo);
    div.appendChild(div3);

    var div4 = document.createElement("div");
    $(div4).addClass("Telefonos");
    for (j = 0; j < PersonaContacto.Telefonos.length; j++) {

        var Telefono = PersonaContacto.Telefonos[j];
        if (Telefono == undefined) break;
        InsertarTelefono(Telefono, div4);

    }
    div.appendChild(div4);

    var btn = document.createElement("input");
    btn.type = "Button";
    $(btn).addClass("btnEliminar32");
    btn.onclick = new Function("EliminarPersonasContacto(" + PersonaContacto.idPersonaContacto + ");");
    btn.title = "Eliminar Persona Contacto";
    div.appendChild(btn);
    lstPC[0].appendChild(div);
    lstPC[0].appendChild(document.createElement("HR"));

}
//Para actualizar la lista de telefenos una vez eliminado
function RemoverTelefono(idTelefono) {
    var div = $("#" + idTelefono)[0];
    div.parentNode.removeChild(div);
    var i = lstTelefonos.indexOf(lstTelefonos.where(function (a) { return a.idTelefono == idTelefono })[0]);
    lstTelefonos.splice(i, 1);
}


function Adjuntar() {
    Emergente({ url: "/Emergentes/ctrlSubirSoportes.aspx", width: 300, height: 200 });
}
function AgregarAdjunto(Archivo) {
    var div = document.createElement("DIV");
    div.onclick = function () {
        var Archivo = $(this).text();
        this.parentNode.removeChild(this);
        for (var i = 0; i < lstAdjuntos.length; i++) {
            var Parte = lstAdjuntos[i].substr(pos + 1);
            if (Parte == Archivo) {
                a.splice(i); break;
            }
        }
    };
    var pos = Archivo.indexOf("_");
    $(div).text(Archivo.substr(pos + 1));
    div.className = "Telefono";
    lstAdjuntos.push(Archivo);
    $("#Adjuntos")[0].appendChild(div);
}
function LimpiarAdjuntos() {
    lstAdjuntos = [];
    $("#Adjuntos").html("");
}

function Seleccionar(idPersona) {
    window.parent.Persona_Mostrar(idPersona);
}











function ConfirmarGestionGrupoEmpresa() {

    var Status = $("#cboGestStatus option:selected").html();
    Preguntar({ mensaje: "La gestión se guardara con el Status: " + Status + ", ¿Esta seguro?", funcion: GuardarGestionGrupoEmpresa });
}



function GuardarGestionGrupoEmpresa() {



    var tablaCuenta = $("#pnlPeCuentas")[0].getElementsByTagName("table")[0];
    var campos = obtenerCampos(tablaCuenta);
    var colIdCuenta = $.inArray("idCuenta", campos);

    var lstCuentas = Array();
    elem = tablaCuenta.getElementsByTagName("tr");
    for (i = 1; i < elem.length; i++) {
        try {
            var checkBox = elem[i].getElementsByTagName("td")[colIdCuenta].getElementsByTagName("input")[0];
            if (checkBox.checked) {
                lstCuentas.push($(checkBox).val());
            }
        }
        catch (ex) { }
    }

    var rtGestion = {};
    rtGestion.idOperador = idOpL();
    rtGestion.idPersona = $("#idPersona").val();
    rtGestion.idStatus = $('#cboGestStatus').find(":selected").val();
    idUltimoStatus = rtGestion.idStatus;
    rtGestion.Descripcion = $("#txtGestDescripcion").val();
    rtGestion.Cuentas = lstCuentas;
    if (rtGestion.idStatus == 0) {
        Mensaje({ mensaje: "No ha seleccionado un status" });
        return;
    }
    lstCuentas2 = ObtenerSeleccionados("pnlPeCuentas").select("idCuenta");
    if (lstCuentas.length == 0 && elem.length > 2) {
        //        Mensaje({ mensaje: "No hay facturas seleccionadas, ¿Desea continuar?", titulo: "Guardando gestión", buttons: { Aceptar: function () { Gestion_ins(_Gestion_ins, rtGestion); $(this).dialog("close"); }, Cancelar: function () { $(this).dialog("close"); } } });
        Mensaje({ mensaje: "No ha seleccionado ninguna factura, No Puede Guardar la gestión", titulo: "Guardando gestión" });
        return
    }
    //Gestion_ins(_Gestion_ins, rtGestion);RA
   

    
           

    LlamarServicio(_GestionGupoEmpresa_ins, "GestionGupoEmpresa_ins", { Gestion: rtGestion, Adjuntos: lstAdjuntos });

}

function _GestionGupoEmpresa_ins(msg) {
    var resp = msg.d;
    if (resp == false) {
        Mensaje({ mensaje: Recursos["ErrorManagement"] });
    }
    else {
        if (AvisosPropios.length > 0) {
            LlamarServicio(_Avisos_del, "Avisos_del", { Avisos: AvisosPropios, Comentario: $("#txtGestDescripcion").val() });
        }
        //RefrescarPersona();
        $("#txtGestDescripcion").val("");
        $("#cboGestStatus option:first").attr('selected', 'selected');
        //        Mensaje({ mensaje: "La gestión se guardó correctamente" });
        Preguntar({ mensaje: "La gestión se guardó correctamente, ¿Desea gestionar a otra persona?", funcion: FinalizarGestion });
        //Cartera_lst(_Cartera_lst, idOp(), idOpL() != idOp()); /*Warning*/RA
        //LlamarServicio(_Cartera_lst, "Cartera_lst", { idOperador: idOp(), Supervisado: idOpL() != idOp() });
        RefrescarGestiones()

        try {
            for (var i = 0; i < Cartera.length; i++) {
                var ItemCartera = Cartera[i].Personas.where(function (a) { return a.idPersona == idP(); })[0];
                if (ItemCartera == undefined) break;
                ItemCartera.FechaUltimaGestion = (new Date()).ToString("JSON");
                var Status = lstStatus.where(function (a) { return a.idStatus == idUltimoStatus; });
                ItemCartera.StatusUltimaGestion = Status[0].Nombre;
                ItemCartera.TipoStatus = Status[0].Tipo;
            }
            _Cartera_lst({ d: Cartera });
        } catch (e) {
            ActualizarCartera();
        }
        LimpiarAdjuntos();
    }
}