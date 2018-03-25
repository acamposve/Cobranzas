/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Servicios.js"/>
function Inicializar() {
    Asig = window.parent.Asignaciones;
    if (Asig == undefined) return;
    LlenarCombo({ Combo: 'cboTipoCliente', Resultado: window.parent.TiposCliente});
    LlenarCombo({ Combo: 'cboPais', Resultado: window.parent.Paises});
    RecargarListado();
    Nuevo();
}
function RecargarListado() {
    LlamarServicio(_Status_lst, "Status_lst", { idOperador: window.parent.idOpA() });
}
function ActualizarPais() {
    return 1; //Para implementar el cambio de países
}
function ActualizarListado() {
    var idTipoCliente = $("#cboTipoCliente").val();
    var idPais = $("#cboPais").val();
    var Status = Resultados.where(function (x) { return x.idTipoCliente == idTipoCliente && x.idPais == idPais; });
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: Status,
        Campos: [
            { Titulo: "Level", Campo: "Nivel", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Type", Campo: "Tipo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Active", Campo: "Activo", Clase: "grdBooleano", Ordenacion: true, Filtro: true },
        ],
        FuncSeleccionar: "Seleccionar(«idStatus»,«Nivel»,'«Tipo»','«Nombre»',«Activo»);",
        FuncEliminar: "Inactivar(«idStatus»);"
    });
}
function _Status_lst(msg) {
    Resultados = msg.d;
    ActualizarListado();
}
function Seleccionar(idStatus, Nivel, Tipo, Nombre, Activo) {
    $("#idStatus").val(idStatus);
    $("#txtNivel").val(Nivel);
    $("#cboTipo").val(Tipo);
    $("#txtNombre").val(Nombre);
    $("#chkActivo")[0].checked = Activo;
}
function Guardar() {
    var Status = {
        idStatus: $("#idStatus").val(),
        Nivel: $("#txtNivel").val(),
        Tipo: $("#cboTipo").val(),
        Nombre: $("#txtNombre").val(),
        Activo: $("#chkActivo")[0].checked,
        idPais: $("#cboPais").val(),
        idTipoCliente: $("#cboTipoCliente").val()
    }
    LlamarServicio(_Status_sav, "Status_sav", { Status: Status });
}
function _Status_sav(msg) {
    RecargarListado();
    Nuevo()
}
function Inactivar(idStatus) {
    LlamarServicio(_Status_del, "Status_del", { idStatus: idStatus });
}
function _Status_del(msg) {
    RecargarListado();
}
function Nuevo() {
    $("#idStatus").val("0");
    $("#txtNivel").val("0");
    $("#cboTipo").val("Positivo");
    $("#txtNombre").val("");
    $("#chkActivo")[0].checked = true;
}