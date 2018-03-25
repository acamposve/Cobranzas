/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Editor.js"/>
function Inicializar() {
    Argumentos = searchToJSON();
    LlamarServicio(_Plantillas_lst, "PlantillasCorreo_lst", { idOperador: Argumentos.idOperador });
    LlamarServicio(_TablaContacto_lst, "TablaContacto_lst", { idPersona: Argumentos.idPersona, idOperador: Argumentos.idOperador });
    if (Argumentos.idPersona)//Correo a una persona
    {
        setTimeout("TraerSoportes();", 1000);
    }
    lstAdjuntos = [];
    $("#Adjuntos").html("");
    var Texto = $("#txtMensaje").val();
    if (Argumentos.Accion == undefined) Restaurar();
    if ($("#txtMensaje").val() != "") Texto = $("#txtMensaje").val();
    new nicEditor().panelInstance('txtMensaje');
    nicEditors.findEditor('txtMensaje').setContent(Texto);

    $("#txtMensaje")[0].parentNode.onkeypress = function () {
        SalvarAlTiempo();
    };
}
var Salvar; //Indicador para el timeout de Respaldar El correo.
var _lst2;
//tabla de contacto
function _TablaContacto_lst(msg) {
    Tabla({
        Contenedor: "pnlListadoContacto",
        Resultado: msg.d,
        FuncSeleccionar: 'Contacto("«Correo»");',
        LimitarAltura: 300,
        Campos: [
           { Titulo: "Contact", Campo: "Nombre", Campo2: "Correo", Clase: "grdTexto", Ordenacion: false, Filtro: false },
        ]
    })
}
function Contacto(Correo) {
    var campo;
    if ($("#radCorreoPara")[0].checked) { campo = $("#txtCorreoPara"); }
    if ($("#radCorreoCC")[0].checked) { campo = $("#txtCorreoCC"); }
    if ($("#radCorreoCCO")[0].checked) { campo = $("#txtCorreoCCO"); }
    if (campo.val() == "") {
        campo.val(Correo);
    } else {
        campo.val(campo.val() + ";" + Correo);
    }
}
//funciones para las plantillas
function _Plantillas_lst(msg) {
    Tabla({
        Contenedor: "pnlListado",
        Resultado: msg.d,
        FuncSeleccionar: 'Seleccionar(«idPlantilla»);',
        LimitarAltura: 150,
        Campos: [
            { Titulo: "Templates", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operador", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
        ]
    })
}
function Seleccionar(idPlantilla) {
    LlamarServicio(_PlantillaCorreo_sel, "PlantillaCorreo_sel", { idPlantilla: idPlantilla, idPersona: Argumentos.idPersona, idOperador: Argumentos.idOperador });
}
function _PlantillaCorreo_sel(msg) {
    Plantilla = msg.d;
    $("#Adjuntos").html("");
    $("#txtCorreoCC").val(Plantilla.DestinatariosCopia);
    $("#txtCorreoCCO").val(Plantilla.DestinatariosCopiaOculta);
    $("#txtCorreoAsunto").val(Plantilla.Asunto);
    $("#txtMensaje").val(Plantilla.Mensaje);
    nicEditors.findEditor('txtMensaje').setContent(Plantilla.Mensaje);
    var adjuntos = Plantilla.Adjunto.split(";");
    for (var i = 0; i < adjuntos.length; i++) {
        AgregarAdjunto("*" + adjuntos[i]);
    }
}
//Funciones Originales
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
            AgregarAdjunto(_lstAdjuntos[i].substr(1));
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
    if ($("#txtCorreoAsunto").val() == "") {
        Mensaje({ mensaje: "Debe Escribir un asunto para su correo" });
        return;
    }
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
    Emergente({ url: "ctrlSubirSoportes.aspx", width: 300, height: 200 });
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
