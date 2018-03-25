/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Editor.js"/>
function Inicializar() {
    Argumentos = searchToJSON();
    LlamarServicio(_TelefonosEliminar_lst, "TelefonosEliminar_lst", { idOperador: Argumentos.idOperador });
    LlamarServicio(_TelefonosAgregar_lst, "TelefonosAgregar_lst", { idOperador: Argumentos.idOperador });
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

function _TelefonosEliminar_lst(msg) {
    Tabla({
        Contenedor: "pnlListadoEliminar",
        Resultado: msg.d,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Name", Campo: "NombrePersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Phone", Campo: "Telefono", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "NombreOperador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Delete", Pre: "Eliminar definitivamente", Campo: function (a) { return "";}, Clase: "grdTexto", OnClick: "GestionarTelefono(«idTelefono»,false,true);", Filtro: false },
            { Titulo: "Cancel", Pre: "Cancelar eliminación", Campo: function (a) { return "";}, Clase: "grdTexto", OnClick: "GestionarTelefono(«idTelefono»,false,false);", Filtro: false}
            //{ Titulo: "Agregar", Campo: function (a) { return "/Img/Aceptar24.png"; }, Clase: "grdTexto", OnClick: "AgregarTelefono(«idTelefono»);", Filtro: false, Imagen: true }
        ]
    })
}

function _TelefonosAgregar_lst(msg) {
    Tabla({
        Contenedor: "pnlListadoAgregar",
        Resultado: msg.d,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Name", Campo: "NombrePersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Phone", Campo: "Telefono", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "NombreOperador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Confirmar", Pre: "Confirmar inclusón", Campo: function (a) { return ""; }, Clase: "grdTexto", OnClick: "GestionarTelefono(«idTelefono»,true,true);", Filtro: false },
            { Titulo: "Cancel", Pre: "Cancelar eliminación", Campo: function (a) { return ""; }, Clase: "grdTexto", OnClick: "GestionarTelefono(«idTelefono»,true,false);", Filtro: false }
            //{ Titulo: "Agregar", Campo: function (a) { return "/Img/Aceptar24.png"; }, Clase: "grdTexto", OnClick: "AgregarTelefono(«idTelefono»);", Filtro: false, Imagen: true }
        ]
    })
}

function GestionarTelefono(idTelefono, Agregar, Confirmado) {
    LlamarServicio(_TelefonosConfirmar, "TelefonosConfirmar", { idTelefono: idTelefono, idOperador: Argumentos.idOperador, Agregar: Agregar, Confirmado: Confirmado });
}

function _TelefonosConfirmar(msg) {
    Mensaje({ mensaje: Recursos.ActionCompleted });
    LlamarServicio(_TelefonosEliminar_lst, "TelefonosEliminar_lst", { idOperador: Argumentos.idOperador });
    LlamarServicio(_TelefonosAgregar_lst, "TelefonosAgregar_lst", { idOperador: Argumentos.idOperador });
}
