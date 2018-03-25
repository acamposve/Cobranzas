/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Editor.js"/>
function Inicializar() {
    Argumentos = searchToJSON();
    //$("#idOperador").val(searchToJSON().idOperador);
    //idOperador = $_GET("idOperador");
    LlamarServicio(_TelefonosPersonas_lst, "TelefonosPersonas_lst", { idPersona: Argumentos.idPersona });
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            if (ui.newPanel[0].id == "tabListadoAgregar" && $("#idPlantilla").val() == "") {
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

function _TelefonosPersonas_lst(msg) {
    Tabla({
        Contenedor: "pnlListado",
        Resultado: msg.d,
        FuncSeleccionar: 'Seleccionar(«idTelefono»);',
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Phone", Campo: "Telefono", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Nombre", Campo: "NombrePersona", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    })
}

function Seleccionar(idTelefono) {
    //idOperador = $("#idOperador").val();
    LlamarServicio(_Telefonos_del, "Telefonos_del", { idTelefono: idTelefono, idOperador: Argumentos.idOperador });
    window.parent.RemoverTelefono(idTelefono);
}
function _Telefonos_del(msg) {
    Mensaje({ mensaje: Recursos.ActionCompleted });
    LlamarServicio(_TelefonosPersonas_lst, "TelefonosPersonas_lst", { idPersona: Argumentos.idPersona });
}