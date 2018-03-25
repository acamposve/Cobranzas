/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    //DistOperador_lst(_DistOperador_lst);RA
    LlamarServicio(_DistOperador_lst, "DistOperador_lst");
}
function _DistOperador_lst(msg) {
    var Deudas = msg.d;
    $("#pnlDistCuentasOperadores").html("");
    var pnl = $("#pnlDistCuentasOperadores")[0];
    var tabla = CrearTabla("Tabla");
    pnl.appendChild(tabla);
    var fila = AgregarFila(tabla);
    AgregarHeader(fila, "Nombre", "grdTexto", true, true);
    AgregarHeader(fila, "Orden", "grdEntero", true, true);
    AgregarHeader(fila, "Regla", "grdTexto", true, true);
    AgregarHeader(fila, "Operador", "grdTexto", true, true);

    for (i = 0; i < Deudas.length; i++) {
        fila = AgregarFila(tabla, "itmSeleccionable");
        AgregarCelda(fila, Deudas[i].Nombre, "grdTexto");
        AgregarCelda(fila, Deudas[i].Orden, "grdEntero");
        AgregarCelda(fila, Deudas[i].Regla, "grdTexto");
        AgregarCelda(fila, Deudas[i].Operador, "grdTexto");
    }
    RedefinirAlternativo(tabla);

}



function Inicializar() {
    //DistOperador_lst(_DistOperador_lst);RA
    LlamarServicio(_DistOperador_lst, "DistOperador_lst");
    //Flujos_lst(_Flujos_lst);RA
    LlamarServicio(_Flujos_lst, "Flujos_lst");
    //Pasos_lst(_Pasos_lst);RA
    LlamarServicio(_Pasos_lst, "Pasos_lst");
    //Operadores_lst(_Operadores_lst);RA
    LlamarServicio(_Operadores_lst, "Operadores_lst");
    //Reglas_lst(_Reglas_lst);RA
    LlamarServicio(_Reglas_lst, "Reglas_lst");
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            if (ui.newPanel[0].id == "tabDetalles" && $('#idDistribucion').val() == "") {
                Mensaje({ mensaje: "No ha seleccionado ninguna regla para ver" });
                return false;
            }
        }
    });
}
function _Flujos_lst(msg) {
    LlenarCombo({ Combo: "cboFlujo", Resultado: msg.d, CampoId: "idFlujo", CampoValor: "Nombre" });
}
function _Pasos_lst(msg) {
    LlenarCombo({ Combo: "cboPaso", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" });
}
function _Operadores_lst(msg) {
    LlenarCombo({ Combo: "cboOperador", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
}
function _Reglas_lst(msg) {
    LlenarCombo({ Combo: "cboRegla", Resultado: msg.d, CampoId: "idRegla", CampoValor: "Nombre" });
}
function Nuevo() {
    $("#txtNombre").val("");
    $("#cboRegla").val(0);
    $("#cboOperador").val(0);
    $("#cboFlujo").val(0);
    $("#cboPaso").val(0);
    $("#idDistribucion").val(0);
    $("#pstDetalles").click();
}
function Abrir(idDistribucion) {
    //DistribucionOperadores_sel(_DistribucionOperadores_sel, idDistribucion);RA
    LlamarServicio(_DistribucionOperadores_sel, "DistribucionOperadores_sel", { idDistribucion: idDistribucion });
}
function _DistribucionOperadores_sel(msg) {
    Distribucion = msg.d;
    $("#idDistribucion").val(Distribucion.idDistribucion);
    $("#txtNombre").val(Distribucion.Nombre);
    $("#cboRegla").val(Distribucion.idRegla);
    $("#cboOperador").val(Distribucion.idOperador);
    $("#cboFlujo").val(Distribucion.idFlujo);
    $("#cboPaso").val(Distribucion.idPaso);
    $("#pstDetalles").click();
}
function Eliminar(idDistribucion) {
    //DistribucionOperadores_del(_DistribucionOperadores_del, idDistribucion);RA
    LlamarServicio(_DistribucionOperadores_del, "DistribucionOperadores_del", { idDistribucion: idDistribucion });
}
function _DistribucionOperadores_del(msg) {
    //DistOperador_lst(_DistOperador_lst);RA
    LlamarServicio(_DistOperador_lst, "DistOperador_lst");
    $("#pstListado").click();
    $("#idDistribucion").val(0);
}
function Guardar() {
    Distribucion = {};
    Distribucion.Nombre = $("#txtNombre").val();
    Distribucion.idRegla = $("#cboRegla").val();
    Distribucion.idOperador = $("#cboOperador").val();
    Distribucion.idFlujo = $("#cboFlujo").val();
    Distribucion.idPaso = $("#cboPaso").val();
    Distribucion.idDistribucion = $("#idDistribucion").val();
    //DistribucionOperadores_sav(_DistribucionOperadores_sav, Distribucion);RA
    //LlamarServicio(_DistribucionOperadores_sav, "DistribucionOperadores_sav", { Distribucion: Distribucion });
    LlamarServicio(_DistribucionOperadores_sav, "DistribucionOperadores_sav", { Distribucionins: Distribucion });
}
function Forzar() {
    LlamarServicio(_DistribucionOperadores_Ejecutar, "DistribucionOperadores_Ejecutar", { idDistribucion: $("#idDistribucion").val() });
}
function _DistribucionOperadores_Ejecutar(msg)
{
    Mensaje({ mensaje: "Ejecutado Correctamente" })
}
function _DistribucionOperadores_sav(msg) {
    $("idDistribucion").val(msg.d.idDistribucion);
    //DistOperador_lst(_DistOperador_lst);RA
    LlamarServicio(_DistOperador_lst, "DistOperador_lst");
    Mensaje({ mensaje: "Guardado Correctamente" })
}
function _DistOperador_lst(msg) {

    Tabla({
        Contenedor: "pnlDistCuentasOperadores",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idDistribucion",
        LimitarAltura: 300,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Orden", Campo: "Orden", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Regla", Campo: "Regla", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operador", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Flujo", Campo: "Flujo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Paso", Campo: "Paso", Clase: "grdTexto", Ordenacion: true, Filtro: true },
        ],
        FuncSeleccionar: "Abrir(«idDistribucion»);",
        FuncEliminar: "Eliminar(«idDistribucion»);"
    });
}