/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js" />
/// <reference path="/Scripts/ordenacion.js" />
/// <reference path="/Scripts/validaciones.js" />
$(document).ready(Inicializar);
function Inicializar()
{
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("input.fecha").val(new Date().ToString(Regional.FormatoFecha));
    Argumentos = searchToJSON();
    $("#idOperador").val(Argumentos.idOperador);
}
function Ejecutar()
{

    var idCliente = Convert.ToInt32($("#cboCliente").val());
    if (idCliente == 0) idCliente = null;
    var TipoRep= $("#cboTipoReporte").val();
    if (TipoRep=="null") TipoRep=null;
    LlamarServicio(_CorreosAdministrador_rpt, "CorreosAdministrador_rpt",
    {
        FechaDesde: Convert.ToDateTime($("#dtpFechaDesde").val()).ToString("JSON"),
        FechaHasta: Convert.ToDateTime($("#dtpFechaHasta").val()).ToString("JSON")
    });
}
function _CorreosAdministrador_rpt(msg)
{
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Email", Campo: "Email", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Ejecutivo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "FechaCreacion", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Category", Campo: "Categoria", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Reason", Campo: "MotivoRechazo", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });
}
