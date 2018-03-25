/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar() {
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            //            if (ui.newPanel[0].id == "tabDetalles" && $('#idCampana').val() == "")
            //            {
            //                Mensaje({ mensaje: "No ha seleccionado ninguna campana para ver" });
            //                return false;
            //            }
        }
    });
    Args = searchToJSON();
    Actualizar();
}
function Actualizar() {
    LlamarServicio(_Exclusiones_lst, "Exclusiones_lst", { idOperador: Args.idOperador });
}
function _Exclusiones_lst(msg) {
    Tabla({
        Contenedor: "pnlPersonas",
        Resultado: msg.d.Personas,
        TodosSeleccionados: false,
        idSeleccion: "idExclusion",
        Campos: [
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Motivo", Campo: "Motivo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Aprobado", Campo: "Aprobado", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "Abrir(«idPersona»);"
    });

    Tabla({
        Contenedor: "pnlCuentas",
        Resultado: msg.d.Cuentas,
        TodosSeleccionados: false,
        idSeleccion: "idExclusion",
        Campos: [
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Account", Campo: "Cuenta", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Motivo", Campo: "Motivo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Aprobado", Campo: "Aprobado", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "Abrir(«idPersona»);"
    });

}
function Desincorporar_Personas() {
    var Exclusiones = ObtenerSeleccionados("pnlPersonas").select("idExclusion");
    LlamarServicio(_Exclusiones_del, "Exclusiones_del", { idOperador: Args.idOperador, Exclusiones: Exclusiones });

}
function Desincorporar_Cuentas() {
    var Exclusiones = ObtenerSeleccionados("pnlCuentas").select("idExclusion");
    LlamarServicio(_Exclusiones_del, "Exclusiones_del", { idOperador: Args.idOperador, Exclusiones: Exclusiones });
}
function Aprobar_Personas() {
    var Exclusiones = ObtenerSeleccionados("pnlPersonas").select("idExclusion");
    LlamarServicio(_Exclusiones_Aprobado, "Exclusiones_Aprobado", { idOperador: Args.idOperador, Exclusiones: Exclusiones });

}
function Aprobar_Cuentas() {
    var Exclusiones = ObtenerSeleccionados("pnlCuentas").select("idExclusion");
    LlamarServicio(_Exclusiones_Aprobado, "Exclusiones_Aprobado", { idOperador: Args.idOperador, Exclusiones: Exclusiones });
}
function _Exclusiones_del(msg) {
    Mensaje({ mensaje: "Las cuentas seleccionadas han sido desincorporadas de la lista de exclusiones satisfactoriamente" });
    Actualizar();
}
function _Exclusiones_Aprobado(msg) {
    Mensaje({ mensaje: "Las cuentas seleccionadas han sido aprobadas satisfactoriamente" });
    Actualizar();
}
function Incorporar_Persona() {
    LlamarServicio(_Exclusiones_ins, "Exclusiones_ins_CodigoP", { idOperador: Args.idOperador, Codigo: $("#txtPersona").val() });
}
function Incorporar_Cuenta() {
    LlamarServicio(_Exclusiones_ins, "Exclusiones_ins_CodigoC", { idOperador: Args.idOperador, Codigo: $("#txtCuenta").val() });
}
function _Exclusiones_ins(msg) {
    Mensaje({ mensaje: "Las cuentas seleccionadas han sido agregadas satisfactoriamente en la lista de exclusiones" });
    Actualizar();
}
function Abrir(idPersona) {
    window.parent.Persona_Mostrar(idPersona);
}