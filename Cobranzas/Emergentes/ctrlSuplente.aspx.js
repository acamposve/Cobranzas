/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    var Args = searchToJSON();
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Args.idOperador });
}
function _OperadoresSupervisados_lst(msg)
{
    LlenarCombo({ Combo: "cboSuplantado", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    LlenarCombo({ Combo: "cboSuplente", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
}
function Agregar()
{
    var idOperador = Convert.ToInt32($("#cboSuplantado").val());
    var idOperadorSuplente = Convert.ToInt32($("#cboSuplente").val());
    var FechaInicio = Convert.ToDateTime($("#dtpFechaInicio").val()).ToString("JSON");
    var FechaFin = Convert.ToDateTime($("#dtpFechaFin").val());
    FechaFin = FechaFin == null ? "null" : FechaFin.ToString("JSON");
    LlamarServicio(_Suplentes_ins, "Suplentes_ins", { idOperador: idOperador, idOperadorSuplente: idOperadorSuplente, FechaInicio: FechaInicio, FechaFin: FechaFin });
}
function _Suplentes_ins(msg)
{
    if (msg.d)
    {
        window.parent.ActualizarSuplentes();
        window.parent.CerrarEmergente();
    }
    else
    {
        Mensaje({ mensaje: "No se pudo crear el suplente, revise sus datos y vuelva a intentar" });
    }
}