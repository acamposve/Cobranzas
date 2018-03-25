/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Servicios.js"/>
function Inicializar()
{
    $("#idOperador").val(searchToJSON().idOperador);
    //Personas_Tocadas_lst(_Personas_Tocadas_lst, idOp());RA
//    LlamarServicio(_Personas_Tocadas_lst, "Personas_Tocadas_lst", { idOperador:idOp() });
    _Personas_Tocadas_lst({ d: window.parent.PersonasTocadas });
    ActualizarListado();
}
function ActualizarListado()
{
    //CorreosFiltros_lst(_CorreosFiltros_lst, idOp());RA
    LlamarServicio(_CorreosFiltros_lst, "CorreosFiltros_lst", { idOperador: idOp() });
}
function _CorreosFiltros_lst(msg)
{
    Tabla({
        Contenedor: "pnlFiltros",
        Resultado: msg.d,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "MailFrom", Campo: "De", Clase: "grdTexto", Ordenacion: true, Filtro: true},
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true},
        ],
        FuncSeleccionar: "Seleccionar(«idCorreoFiltro»,«idPersona»,'«De»')",
        FuncEliminar: "Eliminar(«idCorreoFiltro»)"
    });
}
function Seleccionar(idFiltro, idPersona, De)
{
    $("#idFiltro").val(idFiltro);
    $("#cboPersona").val(idPersona);
    $("#txtDe").val(De);
}
function _Personas_Tocadas_lst(msg)
{
    PersonasTocadas = msg.d;
    LlenarCombo({ Combo: "cboPersona", Resultado: msg.d, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
    //LlenarCombo({ Combo: "cboPersona", Resultado: msg.d, CampoId: "idPersona", CampoValor: "Nombre" });
}
function idOp()
{
    return $("#idOperador").val();
}
function Agregar()
{
    var CorreoFiltro = {
        idCorreoFiltro:0,
        idOperador:idOp(),
        De:$("#txtDe").val(),
        idPersona:$("#cboPersona").val(),
        Persona:""
    }
    //CorreosFiltros_sav(_CorreosFiltros_sav, CorreoFiltro);RA
    LlamarServicio(_CorreosFiltros_sav, "CorreosFiltros_sav", { CorreoFiltro: CorreoFiltro });
}
function _CorreosFiltros_sav(msg)
{
    ActualizarListado();
}
function Eliminar(idCorreoFiltro)
{
    //CorreosFiltros_del(_CorreosFiltros_del, idCorreoFiltro);RA
    LlamarServicio(_CorreosFiltros_del, "CorreosFiltros_del", { idCorreoFiltro: idCorreoFiltro });
}
function _CorreosFiltros_del(msg)
{
    ActualizarListado();
}
function FiltrarPersona()
{
    var filtro = $("#txtFiltroPersona").val().toUpperCase();
    if (filtro == "")
    {
        LlenarCombo({ Combo: "cboPersona", Resultado: PersonasTocadas, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
    } else
    {
        LlenarCombo({ Combo: "cboPersona", Resultado: PersonasTocadas.where(function (a) { return a.Nombre.toUpperCase().indexOf(filtro) != -1; }), CampoId: "idPersona", CampoValor: "Nombre" });
    }
}