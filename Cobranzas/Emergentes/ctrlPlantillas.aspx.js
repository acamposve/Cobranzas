/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Editor.js"/>
function Inicializar() {
    Argumentos = searchToJSON();
    LlamarServicio(_Plantillas_lst, "Plantillas_lst", { idOperador: Argumentos.idOperador });
    LlenarCombo({ Combo: "cboPais", Resultado: window.parent.Paises });
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Argumentos.idOperador });
    LlenarCombo({ Combo: "cboTipoCliente", Resultado: window.parent.TiposCliente });
    if (Argumentos.idPersona)//Correo a una persona
    {
        setTimeout("TraerSoportes();", 1000);
    }
    lstAdjuntos = [];
    $("#Adjuntos").html("");
    var Texto = $("#txtMensaje").val();
    //if (Argumentos.Accion == undefined) Restaurar();
    if ($("#txtMensaje").val() != "") Texto = $("#txtMensaje").val();
    new nicEditor().panelInstance('txtMensaje');
    nicEditors.findEditor('txtMensaje').setContent(Texto);

    $("#txtMensaje")[0].parentNode.onkeypress = function () {
        SalvarAlTiempo();
    };
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            if (ui.newPanel[0].id == "tabDetalles" && $("#idPlantilla").val() == "") {
                Mensaje({ mensaje: "No ha seleccionado ningún item para ver" });
                return false;
            }
        }
    });
    $("#tabs2").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
        }
    });
}
function _OperadoresSupervisados_lst(msg) {
    LlenarCombo({ Combo: "cboOperador", Resultado: msg.d, CampoValor: "Nombre", CampoId: "idOperador" });
}

function _Plantillas_lst(msg) {
    Tabla({
        Contenedor: "pnlListado",
        Resultado: msg.d,
        FuncSeleccionar: 'Seleccionar(«idPlantilla»);',
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Seguridad", Campo: "Privado", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Nombre", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operador", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Fecha de Creacion", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
        ]
    })
}
function Seleccionar(idPlantilla) {
    LlamarServicio(_Plantilla_sel, "Plantilla_sel", { idPlantilla: idPlantilla });
}
function _Plantilla_sel(msg) {
    Plantilla = msg.d;
    $("#cboOperador").val(Plantilla.idOperador);
    $("#lblFecha").text(Convert.ToDateTime(Plantilla.FechaCreacion).ToString(Regional.FormatoFecha));
    $("#cboPais").val(Plantilla.idPais);
    $("#txtNombre").val(Plantilla.NombrePlantilla)
    $("#cboTipoCliente").val(Plantilla.idTipoCliente);
    $("#txtCorreoCC").val(Plantilla.DestinatariosCopia);
    $("#txtCorreoCCO").val(Plantilla.DestinatariosCopiaOculta);
    $("#txtCorreoAsunto").val(Plantilla.Asunto);
    $("#txtMensaje").val(Plantilla.Mensaje);
    $("#idPlantilla").val(Plantilla.idPlantilla);
    nicEditors.findEditor('txtMensaje').setContent(Plantilla.Mensaje);
    $("#chkPrivate")[0].checked = Plantilla.Privado;
    lstAdjuntos = [];
    $("#Adjuntos").html("");
    if (Plantilla.Adjunto) {
        var adjuntos = Plantilla.Adjunto.split(";");
        for (var i = 0; i < adjuntos.length; i++) {
            AgregarAdjunto(adjuntos[i]);
        }
    }

    $("#pstDetalles").click();
}
function Eliminar() {
    var idPlantilla = $("#idPlantilla").val();
    LlamarServicio(_Plantilla_del, "Plantilla_del", { idPlantilla: idPlantilla });
}

function _Plantilla_del(msg) {
    Mensaje({ mensaje: "La plantilla ha sido eliminado satisfactoriamente" });
    LlamarServicio(_Plantillas_lst, "Plantillas_lst", { idOperador: Argumentos.idOperador });
    $("#pstListado").click();
}
function Nuevo() {
    lstAdjuntos = [];
    $("#Adjuntos").html("");
    $("#cboOperador").val(Argumentos.idOperador);
    $("#lblFecha").text((new Date()).ToString(Regional.FormatoFecha));
    $("#cboPais").val("");
    $("#txtNombre").val("");
    $("#cboTipoCliente").val("");
    $("#txtCorreoCC").val("");
    $("#txtCorreoCCO").val("");
    $("#idPlantilla").val(0);
    $("#txtCorreoAsunto").val("");
    $("#txtMensaje").val("");
    nicEditors.findEditor('txtMensaje').setContent("");
    $("#pstDetalles").click();
}

function Guardar() {
    if ($("#cboPais").val() == "") {
        Mensaje({ mensaje: "Debe selecionar un pais." });
        return false;
    }
    if ($("#cboTipoCliente").val() == "") {
        Mensaje({ mensaje: "Debe selecionar un tipo de cliente." });
        return false;
    }
    if ($("#txtMensaje").val() == "") {
        Mensaje({ mensaje: "Debe ingresar un mensaje." });
        return false;
    }

    var Result = {};
    Result.idOperador = $("#cboOperador").val();
    Result.FechaCreacion = Convert.ToDateTime($("#lblFecha").text()).ToString("JSON");
    Result.idPais = $("#cboPais").val();
    Result.idTipoCliente = $("#cboTipoCliente").val();
    Result.DestinatariosCopia = $("#txtCorreoCC").val();
    Result.DestinatariosCopiaOculta = $("#txtCorreoCCO").val();
    Result.NombrePlantilla = $("#txtNombre").val();
    Result.Asunto = $("#txtCorreoAsunto").val();
    nicEditors.findEditor('txtMensaje').saveContent();
    Result.Mensaje = $("#txtMensaje").val();
    Result.idPlantilla = $("#idPlantilla").val();
    Result.Adjunto = lstAdjuntos.join(";");
    Result.Privado = $("#chkPrivate")[0].checked;
    LlamarServicio(_Plantillas_sav, "Plantilla_sav", { Plantillains: Result });
    LlamarServicio(_Plantillas_lst, "Plantillas_lst", { idOperador: Argumentos.idOperador });
    $("#pstListado").click();
}

function _Plantillas_sav(msg) {
    Mensaje({ mensaje: "La Plantilla ha sido Guardado Satisfactoriamente" });
    LlamarServicio(_Plantillas_lst, "Plantillas_lst", { idOperador: Argumentos.idOperador });
    $("#pstListado").click();
}

//funciones de correo
var Salvar; //Indicador para el timeout de Respaldar El correo.
var _lst2;
function SalvarAlTiempo() {
    clearTimeout(Salvar);
    Salvar = setTimeout(Respaldar, 500);
}
function TraerSoportes() {
    var lstCuentas = window.parent.lstCuentas;
    if (lstCuentas == undefined) {
        return;
    }
    LlamarServicio(_CuentasSoportes_lst, "CuentasSoportes_lst", { lstCuentas: lstCuentas });
}
function _CuentasSoportes_lst(msg) {
    var Contenedor = $("#pnlDocumentos")[0];
    for (var i = 0; i < msg.d.length; i++) {
        var sop = msg.d[i];
        if (sop.Ubicacion == "") {
            var h3 = document.createElement("h3");
            $(h3).text("Cuenta " + sop.Nombre + ": ");
            Contenedor.appendChild(h3);
        }
        else {
            var inp = document.createElement("input");
            inp.type = "checkbox";
            inp.id = sop.Ubicacion;
            if ($.inArray(inp.id, _lst2) != -1) {
                inp.checked = true;
            }
            inp.className = "adjunto";
            Contenedor.appendChild(inp);
            var span = document.createElement("span");
            $(span).text(sop.Nombre);
            Contenedor.appendChild(span);
        }
    }
}
function Respaldar() {
    window.parent.EnvioCorreo = Correo();
}
function Restaurar() {
    var _Correo = window.parent.EnvioCorreo;
    if (_Correo == undefined) { return; }
    if (_Correo.idPersona != Argumentos.idPersona) { return; }
    $("#txtCorreoAsunto").val(_Correo.Asunto);
    $("#txtMensaje").val(_Correo.Mensaje);

    $("#txtCorreoPara").val(_Correo.Destinatarios);
    $("#txtCorreoCC").val(_Correo.DestinatariosCopia);
    $("#txtCorreoCCO").val(_Correo.DestinatariosCopiaOculta);
    try { $("#chkAnalisisAntiguedad")[0].checked = _Correo.Analisis; } catch (e) { }
    try { $("#chkAgrupado")[0].checked = _Correo.Agrupado; } catch (e) { }
    _lstAdjuntos = _Correo.Adjuntos.split("|");
    _lst2 = [];
    for (var i = 0; i < _lstAdjuntos.length; i++) {
        if (_lstAdjuntos[i].substr(0, 1) == "_") {
            AgregarAdjunto(_lstAdjuntos[i].subst(1));
        }
        else {
            _lst2.push(_lstAdjuntos[i]);
        }
    }
}
function Correo() {
    nicEditors.findEditor('txtMensaje').saveContent();
    var _Correo = {};
    _Correo.Asunto = $("#txtCorreoAsunto").val();
    _Correo.Mensaje = $("#txtMensaje").val();
    _Correo.Destinatarios = $("#txtCorreoPara").val();
    _Correo.DestinatariosCopia = $("#txtCorreoCC").val();
    _Correo.DestinatariosCopiaOculta = $("#txtCorreoCCO").val();
    try {
        _Correo.Analisis = $("#chkAnalisisAntiguedad")[0].checked;
    } catch (e) {
        _Correo.Analisis = false;
    }
    try {
        _Correo.Agrupado = $("#chkAgrupado")[0].checked;
    } catch (e) {
        _Correo.Agrupado = false;
    }
    _Correo.idPersona = Argumentos.idPersona;
    _Correo.Cuentas = window.parent.lstCuentas;
    _Correo.idOperador = Argumentos.idOperador;
    if (Argumentos.idCorreo) {
        _Correo.Original = $("#chkOriginal")[0].checked ? Argumentos.idCorreo : null;
    }
    _Correo.Adjuntos = Adjuntos();
    return _Correo;
}

function btnCorreoEnviar_Click() {
    var _Correo = Correo();
    if (!ValidarDireccionesEnCorreo(_Correo)) return;
    LlamarServicio(_Correos_ins, "Correo_ins", { Correo: _Correo });
}
function _Correos_ins(msg) {
    window.parent.RefrescarGestiones();
    window.parent.CerrarEmergente();
    window.parent.Mensaje({ mensaje: "El Correo fue enviado Satisfactoriamente" });
}
function Adjuntos() {
    var result = "";
    $("input.adjunto").each(function () { if (this.checked) result += "|" + this.id });
    for (var i = 0; i < lstAdjuntos.length; i++) {
        result += "|" + "_" + lstAdjuntos[i];
    }
    if (result != "") result = result.substr(1);
    return result;
}
function Adjuntar() {
    Emergente({ url: "ctrlSubirSoportes.aspx?Permanente=1", width: 300, height: 200 });
}
function AgregarAdjunto(Archivo) {
    var div = document.createElement("DIV");
    div.onclick = function () {
        var Archivo = $(this).text();
        this.parentNode.removeChild(this);
        for (var i = 0; i < lstAdjuntos.length; i++) {
            var pos = lstAdjuntos[i].indexOf("_");
            var Parte = lstAdjuntos[i].substr(pos + 1);
            //var Parte = lstAdjuntos[i];
            if (Parte == Archivo) {
                lstAdjuntos.splice(i, 1); break;

            }
        }
    };
    var pos = Archivo.indexOf("_");
    $(div).text(Archivo.substr(pos + 1));
    div.className = "Telefono";
    lstAdjuntos.push(Archivo);
    $("#Adjuntos")[0].appendChild(div);
    SalvarAlTiempo();
}
function ValidarDireccionesEnCorreo(_Correo) {
    var Errores = ValidarDirecciones(_Correo.Destinatarios) +
                ValidarDirecciones(_Correo.DestinatariosCopia) +
                ValidarDirecciones(_Correo.DestinatariosCopiaOculta);
    if (Errores == "") return true;
    Mensaje({ mensaje: Errores });
    return false;

}
function ValidarDirecciones(Direcciones) {
    var Errores = "";
    if (Direcciones.trim() == "") return "";
    Direcciones = Direcciones.replace(/[,]/g, ";");
    var Correos = Direcciones.split(";");
    for (var i = 0; i < Correos.length; i++) {
        var Correo = Correos[i].trim();
        if (!ValidarDireccion(Correo)) Errores += "La dirección: '" + Correo + "' es Incorrecta <br/>";
    }
    return Errores;
}
function ValidarDireccion(Direccion) {
    if (Direccion.trim() == "") return true;
    var ini = Direccion.indexOf("<");
    var fin = Direccion.indexOf(">");
    var Correo = Direccion;
    if (ini != -1 && fin != -1 && ini < fin) Correo = Direccion.substr(ini + 1, fin - ini - 1);
    var regex = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    var result = regex.test(Correo);
    return result;
}