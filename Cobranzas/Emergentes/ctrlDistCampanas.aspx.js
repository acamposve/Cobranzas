/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    //DistCampana_lst(_DistCampana_lst);RA
    LlamarServicio(_DistCampana_lst, "DistCampana_lst");
    //Flujos_lst(_Flujos_lst);RA
    LlamarServicio(_Flujos_lst, "Flujos_lst");
    //Pasos_lst(_Pasos_lst);RA
    LlamarServicio(_Pasos_lst, "Pasos_lst");
    //Campanas_lst(_Campanas_lst);RA
    LlamarServicio(_Campanas_lst, "Campanas_lst");
    //Reglas_lst(_Reglas_lst);RA
    LlamarServicio(_Reglas_lst, "Reglas_lst");
    $("#tabs").tabs({ show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui)
        {
            if (ui.newPanel[0].id == "tabDetalles" && $('#idDistribucion').val() == "")
            {
                Mensaje({ mensaje: "No ha seleccionado ninguna regla para ver" });
                return false;
            }
        }
    });
}
function _Flujos_lst(msg)
{
    LlenarCombo({ Combo: "cboFlujo", Resultado: msg.d, CampoId: "idFlujo", CampoValor: "Nombre" });
}
function _Pasos_lst(msg)
{
    LlenarCombo({ Combo: "cboPaso", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" });
}
function _Campanas_lst(msg)
{
    LlenarCombo({ Combo: "cboCampana", Resultado: msg.d, CampoId: "idCampana", CampoValor: "Nombre" });
}
function _Reglas_lst(msg)
{
    LlenarCombo({ Combo: "cboRegla", Resultado: msg.d, CampoId: "idRegla", CampoValor: "Nombre" });
}
function Nuevo()
{
    $("#txtNombre").val("");
    $("#cboRegla").val(0);
    $("#cboCampana").val(0);
    $("#cboFlujo").val(0);
    $("#cboPaso").val(0);
    $("#idDistribucion").val(0);
    $("#pstDetalles").click();
}
function Abrir(idDistribucion)
{
    //DistribucionCampanas_sel(_DistribucionCampanas_sel, idDistribucion);RA
    LlamarServicio(_DistribucionCampanas_sel, "DistribucionCampanas_sel", { idDistribucion: idDistribucion });
}
function _DistribucionCampanas_sel(msg)
{
    Distribucion = msg.d;
    $("#idDistribucion").val(Distribucion.idDistribucion);
    $("#txtNombre").val(Distribucion.Nombre);
    $("#cboRegla").val(Distribucion.idRegla);
    $("#cboCampana").val(Distribucion.idCampana);
    $("#cboFlujo").val(Distribucion.idFlujo);
    $("#cboPaso").val(Distribucion.idPaso);
    $("#chkExcluir")[0].checked=Distribucion.Excluir;
    $("#pstDetalles").click();
}
function Eliminar(idDistribucion)
{
    //DistribucionCampanas_del(_DistribucionCampanas_del, idDistribucion);RA
    LlamarServicio(_DistribucionCampanas_del, "DistribucionCampanas_del", { idDistribucion: idDistribucion });
}
function _DistribucionCampanas_del(msg)
{
    //DistCampana_lst(_DistCampana_lst);RA
    LlamarServicio(_DistCampana_lst, "DistCampana_lst");
    $("#pstListado").click();
    $("#idDistribucion").val(0);
}
function Guardar()
{
    Distribucion = {};
    Distribucion.Nombre=$("#txtNombre").val();
    Distribucion.idRegla=$("#cboRegla").val();
    Distribucion.idCampana=$("#cboCampana").val();
    Distribucion.idFlujo=$("#cboFlujo").val();
    Distribucion.idPaso=$("#cboPaso").val();
    Distribucion.idDistribucion = $("#idDistribucion").val();
    Distribucion.Excluir = $("#chkExcluir")[0].checked;
    //DistribucionCampanas_sav(_DistribucionCampanas_sav, Distribucion);RA
    LlamarServicio(_DistribucionCampanas_sav, "DistribucionCampanas_sav", { Distribucionins: Distribucion });
}
function Forzar() {
    LlamarServicio(_DistribucionCampanas_Ejecutar, "DistribucionCampanas_Ejecutar", { idDistribucion: $("#idDistribucion").val() });
}
function _DistribucionCampanas_Ejecutar(msg) {
    Mensaje({ mensaje: "Ejecutado Correctamente" })
}

function _DistribucionCampanas_sav(msg)
{
    $("idDistribucion").val(msg.d.idDistribucion);
    //DistCampana_lst(_DistCampana_lst);RA
    LlamarServicio(_DistCampana_lst, "DistCampana_lst");
    Mensaje({mensaje:"Guardado Correctamente"})
}
function _DistCampana_lst(msg)
{

    Tabla({
        Contenedor: "pnlDistCuentasCampanas",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idDistribucion",
        LimitarAltura: 300,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true},
            { Titulo: "Orden", Campo: "Orden", Clase: "grdEntero", Ordenacion: true, Filtro: true},
            { Titulo: "Regla", Campo: "Regla", Clase: "grdTexto", Ordenacion: true, Filtro: true},
            { Titulo: "Campaña", Campo: "Campana", Clase: "grdTexto", Ordenacion: true, Filtro: true},
            { Titulo: "Flujo", Campo: "Flujo", Clase: "grdTexto", Ordenacion: true, Filtro: true},
            { Titulo: "Paso", Campo: "Paso", Clase: "grdTexto", Ordenacion: true, Filtro: true},
        ],
        FuncSeleccionar: "Abrir(«idDistribucion»);",
        FuncEliminar: "Eliminar(«idDistribucion»);"
    });
}