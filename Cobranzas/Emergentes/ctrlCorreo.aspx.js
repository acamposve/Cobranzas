/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar()
{
    //Personas_Tocadas_lst(_Personas_Tocadas_lst, $("#idOperador").val());RA
    //LlamarServicio(_Personas_Tocadas_lst, "Personas_Tocadas_lst", { idOperador: $("#idOperador").val() });
    if (window.parent.PersonasTocadas == undefined) return;
    _Personas_Tocadas_lst({ d: window.parent.PersonasTocadas });
    //Correos_Personas_lst(_Correos_Personas_lst, $("#idCorreo").val());RA
    LlamarServicio(_Correos_Personas_lst, "Correos_Personas_lst", { idCorreo: $("#idCorreo").val() });
}
function _Correos_Personas_lst(msg)
{
    $("#pnlPersonas").empty();
    for (var i = 0; i < msg.d.length; i++)
    {
        crearPersona(msg.d[i].idPersona, msg.d[i].Nombre);
    }
}
function crearPersona(id, Nombre)
{
    var a = document.createElement("div");
    a.className = "Adjunto";
    $(a).text(Nombre);
    //a.onclick = new Function('EliminarPersona(' + id + ');');
    a.onclick = new Function("Mensaje({mensaje:'¿Está seguro de querer eliminar la asociación de este correo a " + Nombre + "?' ,buttons: { 'Sí': new Function('EliminarPersona(" + id + "); $(this).dialog(\"close\");') ,'No': function () { $(this).dialog('close'); }}}) ;");
    $("#pnlPersonas")[0].appendChild(a);
}
function _Personas_Tocadas_lst(msg)
{
    PersonasTocadas = msg.d;
    LlenarCombo({ Combo: "cboaPersona", Resultado: msg.d, CampoId: "idPersona", CampoValor: "Nombre", TextoNull: "<<Niguno>>" });
}
function AsignarCorreosAPersona(CrearRegla)
{
    var idPersona = $("#cboaPersona").val();
    if (idPersona == "") { Mensaje({ mensaje: Recursos["MustSelectAPerson"] }); return; }
    var CorreosSel = [$("#idCorreo").val()];
    //Correos_Persona_ins(_Correos_Persona_ins, CorreosSel, idPersona, CrearRegla);RA
    LlamarServicio(_Correos_Persona_ins, "Correos_Persona_ins", { Correos: CorreosSel, idPersona: idPersona, CrearRegla: CrearRegla });
}
function _Correos_Persona_ins(msg)
{
    Mensaje({ mensaje: "El correo ha sido asignado a la persona satisfactoriamente" });
    //Correos_Personas_lst(_Correos_Personas_lst, $("#idCorreo").val());RA
    LlamarServicio(_Correos_Personas_lst, "Correos_Personas_lst", { idCorreo: $("#idCorreo").val() });
}
function EliminarPersona(idPersona)
{
    //Correos_Persona_del(_Correos_Persona_del, $("#idCorreo").val(), idPersona);RA
    LlamarServicio(_Correos_Persona_del, "Correos_Persona_del", { idCorreo: $("#idCorreo").val(), idPersona: idPersona });
}
function _Correos_Persona_del(msg)
{
    //Correos_Personas_lst(_Correos_Personas_lst, $("#idCorreo").val());RA
    LlamarServicio(_Correos_Personas_lst, "Correos_Personas_lst", { idCorreo: $("#idCorreo").val() });
}
function Respuesta(Tipo)
{
    location.href = "ctrlEnvioCorreo.aspx?idCorreo=" + $("#idCorreo").val() + "&Accion=" + Tipo + "&idOperador=" + $("#idOperador").val();
}
function MarcarComoPersonal()
{
    LlamarServicio(_Correos_MarcarPersonal_upd, "Correos_MarcarPersonal_upd", { Correos: [$("#idCorreo").val()], TipoEspecial: 2 });
}
function _Correos_MarcarPersonal_upd(msg)
{
    Mensaje({ mensaje: "Correo marcado como personal." });
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
function AsignarCorreosGrupodeEmpresa(CrearRegla) {


    


    var idPersona = $("#cboaPersona").val();

    LlamarServicio(_CorreoGrupoEmpresa_lst, "CorreoGrupoEmpresa_lst", { idPersona: idPersona });

    



    function _CorreoGrupoEmpresa_lst(msg) {


        for (var i=0; i<msg.d.length; i++){
        
    var persona = msg.d[i].idPersonaContacto
    var CorreosSel = [$("#idCorreo").val()];
    //Correos_Persona_ins(_Correos_Persona_ins, CorreosSel, idPersona, CrearRegla);RA
    LlamarServicio(_Correos_Persona_ins, "Correos_Persona_ins", { Correos: CorreosSel, idPersona: persona, CrearRegla: CrearRegla });


        
        }

    }



}

