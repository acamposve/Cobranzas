/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("input.fecha").val(AFechaMuyCorta(new Date()));
    LlamarServicio(_OperadoresSupervisadosConSupervisor_lst, "OperadoresSupervisadosConSupervisor_lst", { idOperador: window.parent.idOp() });
    _Personas_Tocadas_lst({ d: window.parent.PersonasTocadas });
}
function _OperadoresSupervisadosConSupervisor_lst(msg)
{
    LlenarCombo({ Combo: "cboOperadorAviso", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    $("#cboOperadorAviso").val(window.parent.idOp());
}
function _Personas_Tocadas_lst(msg)
{
    PersonasTocadas = msg.d;
    LlenarCombo({ Combo: "cboaPersona", Resultado: msg.d, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
}
function Crear()
{
    var Aviso = {};
    Aviso.idOperador = $("#cboOperadorAviso").val();
    Aviso.idOperadorCrea = window.parent.idOpL();
    Aviso.idPersona = $("#cboaPersona").val();
    if (Aviso.idPersona == "") { Aviso.idPersona = undefined }
    if ($("#dtpFechaAviso").val() == "")
    {
        Mensaje({ mensaje: "Debe seleccionar una fecha válida para este aviso" });
        return;
    }
    if ($("#txtNuevoAviso").val() == "")
    {
        Mensaje({ mensaje: "Debe seleccionar un Texto para este aviso" });
        return;
    }
    Aviso.FechaAviso = Convert.ToDateTime($("#dtpFechaAviso").val() + " " + $("#cboHoraAviso").val() + ":" + $("#cboMinutoAviso").val()).ToString("JSON");
    Aviso.Aviso = $("#txtNuevoAviso").val();
    LlamarServicio(_Avisos_ins, "Avisos_ins", { Aviso: Aviso });
}
function FiltrarPersona()
{
    var filtro = $("#txtFiltroPersona").val().toUpperCase();
    if (filtro == "")
    {
        LlenarCombo({ Combo: "cboaPersona", Resultado: PersonasTocadas, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
    } else
    {
        LlenarCombo({ Combo: "cboaPersona", Resultado: PersonasTocadas.where(function (a) { return a.Nombre.toUpperCase().indexOf(filtro) != -1; }), CampoId: "idPersona", CampoValor: "Nombre" });
    }
}
function _Avisos_ins(msg)
{
    if (msg.d === true)
    {
        Mensaje({ mensaje: "Tu aviso ha sido creado exitosamente" });
        $("#txtNuevoAviso").val("");
        window.parent.CerrarEmergente();
    } else
    {
        Mensaje({ mensaje: "El Aviso no ha sido creado" });
    }
}
