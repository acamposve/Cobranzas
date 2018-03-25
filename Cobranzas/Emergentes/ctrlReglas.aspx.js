/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    Campos = Array();
    Arg = searchToJSON();
    if (document.location.search.indexOf("sel") == -1) {
        $("#pnlSeleccion").hide();
    }
    if (Arg.idRegla != undefined) {
        LlamarServicio(_Reglas_sel, "Reglas_sel", { idRegla: Arg.idRegla });
    } else {
        $("#btnEliminar").hide();
    }
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            if (ui.newPanel[0].id == "tabDetalles" && $('#idRegla').val() == "") {
                Mensaje({ mensaje: "No ha seleccionado ninguna regla para ver" });
                return false;
            }
        }
    });
    if (Arg.Tipo != undefined) {
        $("#cboTipo").val(Arg.Tipo);
        $("#cboTipo")[0].disabled = true;
        LlamarServicio(_Reglas_lst, "Reglas_lst", { Tipo: Arg.Tipo });
    } else {
        LlamarServicio(_Reglas_lst, "Reglas_lst");
    }
    LlamarServicio(_Status_lst, "Status_lst", { idOperador: Arg.idOperador });
    LlamarServicio(_Pasos_lst, "Pasos_lst");
    LlamarServicio(_Personas_lst, "Personas_lst", { idOperador: Arg.idOperador });
    LlamarServicio(_Clientes_lst, "Clientes_lst", { idOperador: Arg.idOperador });
    LlamarServicio(_Productos_lst, "Productos_lst", { idOperador: Arg.idOperador });
}
function _Status_lst(msg) {
    //Ver _Status_lst en gestiones.aspx.js
    lstStatus = msg.d;
    var Status = lstStatus;
    var papa = $("#cboStatus")[0];

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
function _Pasos_lst(msg) {
    LlenarCombo({ Combo: "cboPasos", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" });
}
function _Personas_lst(msg) {
    LlenarCombo({ Combo: "cboPersonas", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre" });
}
function _Clientes_lst(msg) {
    LlenarCombo({ Combo: "cboClientes", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre" });
}
function _Productos_lst(msg) {
    LlenarCombo({ Combo: "cboProductos", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre" });
}
function Seleccionar() {
    var idRegla = $('#idRegla').val();
    if (idRegla == "" || idRegla == "0") {
        Mensaje({ mensaje: "No se puede seleccionar" });
        return false;
    }
    window.parent.CrearRegla(idRegla, $("#txtNombreRegla").val());
    window.parent.CerrarEmergente();
}
function Cancelar() {
    window.parent.CerrarEmergente();
}
function Eliminar() {
    window.parent.CrearRegla(0, "");
    window.parent.CerrarEmergente();
}
function Nuevo() {
    $('#idRegla').val("0");
    $("#nroCampo").val("0");
    Campos = [];
    $("#txtNombreRegla").val("");
    ActualizarCampos();
    $('#pstDetalles').click();
}
function AgregarCriterio() {
    var idCampo = $("#cboCampo").val();
    var Campo = $("#cboCampo option:selected").text();
    var Operador = $("#cboOperador option:selected").text();
    var ValorCampo = SeleccionTipoValor();
    var itm = { idCampo: idCampo, Campo: Campo, Operador: Operador, Valor: ValorCampo, Numero: 0 };
    var indice = $("#nroCampo").val();
    if (indice == "") {
        Campos[Campos.length] = itm;
        AgregarCampo(itm);
    }
    else {
        Campos[indice] = itm;
        ActualizarCampos();
    }
    $("#nroCampo").val("");
}
function EliminarCriterio() {
    var indice = $("#nroCampo").val();
    if (indice == "") {
        return;
    } else {
        Campos.pop(indice);
        ActualizarCampos();
    }
    $("#nroCampo").val("");
}
function ActualizarCampos() {
    var pnl = $("#pnlReglaCriterios");
    pnl.html("");
    pnl = pnl[0];

    var tabla = CrearTabla("Tabla");
    pnl.appendChild(tabla);
    var fila = AgregarFila(tabla);

    AgregarHeader(fila, "Campo", "grdTexto", false, false);
    AgregarHeader(fila, "Operador", "grdTexto", false, false);
    AgregarHeader(fila, "Valor", "grdTexto", false, false);
    var combo = $("#cboOperadores")[0];
    var i;
    for (i = 0; i < Campos.length; i++) {
        AgregarCampo(Campos[i], tabla).onclick = new Function("CargarCampo(" + i + ");");
    }

    RedefinirAlternativo(tabla);
}
function CargarCampo(i) {
    $("#cboCampo").val(Campos[i].idCampo);
    LlenarComboOperadores()
    $("#cboOperador").val(Campos[i].Operador);
    SeleccionTipoValor(Campos[i].Valor);
    $("#nroCampo").val(i);
};
function AgregarCampo(Campo, tabla) {
    if (tabla == undefined) { tabla = $("#pnlReglaCriterios table")[0]; var redef = true; }
    var fila = AgregarFila(tabla, "itmSeleccionable");
    AgregarCelda(fila, Campo.Campo, "grdTexto");
    AgregarCelda(fila, Campo.Operador, "grdTexto");
    var Valor = Campo.Valor;
    var bc = "white";
    try {
        var TipoSel = Criterios.where(function (a) { return a.idCampo == Campo.idCampo }).select("TipoSeleccion")[0];
        var cbo = undefined;
        switch (TipoSel) {
            case "cboPasos":
                cbo = $("#cboPasos")[0];
                break;
            case "cboStatus":
                cbo = $("#cboStatus")[0];
                break;
            case "cboPersonas":
                cbo = $("#cboPersonas")[0];
                break;
            case "cboClientes":
                cbo = $("#cboClientes")[0];
                break;
            case "cboProductos":
                cbo = $("#cboProductos")[0];
                break;
            case "checkBox":
                cbo = $("#chkValor")[0];
                break;
        }
        if (cbo != undefined) {
            var opciones = cbo.getElementsByTagName("OPTION");
            for (var i = 0; i < opciones.length; i++) {
                if ($(opciones[i]).val() == Valor) {
                    Valor = $(opciones[i]).text();
                    bc = opciones[i].style.backgroundColor;
                    break;
                }
            }
        }
    } catch (e) { }
    var celda = AgregarCelda(fila, Valor, "grdTexto");
    celda.style.backgroundColor = bc;
    if (redef) RedefinirAlternativo(tabla);
    return fila;
}
function LlenarComboCriterios() {
    var i;
    var combo = $("#cboCampo")[0];
    for (i = 0; i < Criterios.length; i++) {
        var option = document.createElement("option");
        $(option).val(Criterios[i].idCampo);
        $(option).text(Criterios[i].Nombre);
        combo.appendChild(option);
    }
}
function LlenarComboOperadores() {
    var idCampo = $("#cboCampo").val();
    var Criterio = Criterios.where(function (x) { return x.idCampo == idCampo })[0];
    LlenarCombo({ Combo: "cboOperador", Resultado: Criterio.Operadores, CampoId: "Operador" });
    SeleccionTipoValor();
}
function _Reglas_lst(msg) {
    var Deudas = msg.d;

    Tabla({
        Contenedor: "pnlDistReglas",
        Resultado: msg.d,
        LimitarAltura: 300,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "SeleccionarRegla(«idRegla»);"
    });

    $("#nroCampo").val("");
}
function SeleccionarRegla(idRegla) {
    LlamarServicio(_Reglas_sel, "Reglas_sel", { idRegla: idRegla });
}
function GuardarReglas() {
    var Regla = { idRegla: $("#idRegla").val(), Nombre: $("#txtNombreRegla").val(), Criterios: "", TipoRegla: $("#cboTipo").val(), ReglasDet: Campos };
    LlamarServicio(Reglas_sav_Respuesta, "Reglas_sav", { Reglains: Regla });
}
function Reglas_sav_Respuesta(msg) {
    Mensaje({ mensaje: "Regla Guardada Satisfactoriamente" });
    LlamarServicio(_Reglas_lst, "Reglas_lst");
    LlamarServicio(_Reglas_sel, "Reglas_sel", { idRegla: msg.d });
}
function _Reglas_sel(msg) {
    var Regla = msg.d;
    $("#idRegla").val(Regla.idRegla);
    Campos = Regla.ReglasDet;
    $("#txtNombreRegla").val(Regla.Nombre);
    ActualizarCampos();
    $('#pstDetalles').click();
    $("#nroCampo").val("");
    $("#cboTipo").val(Regla.TipoRegla);
}
function SeleccionTipoValor(valor) {
    var idCampo = $("#cboCampo").val();
    var Criterio = Criterios.where(function (x) { return x.idCampo == idCampo })[0];
    var Operador = $("#cboOperador").val();
    var TipoC = Criterio.idTipoCampo;
    var TipoO = Criterio.Operadores.where(function (x) { return x.Operador == Operador })[0].idTipoCampoOtro;
    var Tipo = TipoO == 0 ? TipoC : TipoO;
    var TipoSel = Criterio.TipoSeleccion;
    if (TipoO != 0) { TipoSel = "TextBox"; }
    $("#txtValor").hide();
    $("#cboStatus").hide();
    $("#cboPersonas").hide();
    $("#cboClientes").hide();
    $("#cboPasos").hide();
    $("#chkValor").hide();
    $("#cboProductos").hide();
    var txt;
    switch (TipoSel) {
        case "TextBox":
            txt = $("#txtValor");
            txt.datepicker("destroy");
            break;
        case "DatePicker":
            txt = $("#txtValor");
            txt.datepicker($.datepicker.regional["es"]);
            break;
        case "cboPasos":
            txt = $("#cboPasos");
            break;
        case "cboStatus":
            txt = $("#cboStatus");
            break;
        case "cboPersonas":
            txt = $("#cboPersonas");
            break;
        case "cboClientes":
            txt = $("#cboClientes");
            break;
        case "cboProductos":
            txt = $("#cboProductos");
            break;
        case "checkBox":
            txt = $("#chkValor");
            break;
    }
    txt.show();
    if (valor != undefined) {
        if (txt[0].id == "chkValor") {
            txt[0].checked = valor == 1;
        }
        else {
            txt.val(valor);
        }
    }
    else {
        if (txt[0].id == "chkValor") {
            return txt[0].checked ? 1 : 0;
        }
        else {
            return txt.val();
        }
    }

}