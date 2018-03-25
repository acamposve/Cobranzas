/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Editor.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar()
{
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("input.fecha").val(new Date().ToString(Regional.FormatoFecha));
}
function Ejecutar()
{
    var Args=searchToJSON();
    LlamarServicio(_PagosCobrador_rpt, "PagosCobrador_rpt", { idOperador: Args.idOperador,FechaIni: Convert.ToDateTime($("#dtpFechaIni").val()).ToString("JSON"), FechaFin: Convert.ToDateTime($("#dtpFechaFin").val()).ToString("JSON") });
}
function _PagosCobrador_rpt(msg)
{
    TablaDataSet(msg.d,"pnlResultados");
}