﻿/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar() {

}
function AbrirPersona() {
    var idPersona = $("#idPersona").val();
    if (idPersona == "") return;
    window.parent.Persona_Mostrar(idPersona, true);
}