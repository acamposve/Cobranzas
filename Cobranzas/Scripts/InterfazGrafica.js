/// <reference path="jquery-1.10.2-vsdoc.js"/>
/// <reference path="General.js"/>

function PonerHora()
{
    var fecha = new Date();
    var FechaT = Dos(fecha.getDate()) + "/" + Dos(fecha.getMonth() + 1) + "/" + fecha.getFullYear().toString();
    var HoraT = Dos(fecha.getHours()) + ":" + Dos(fecha.getMinutes());
    $('#lblDate').text(FechaT);
    $('#lblTime').text(HoraT);
}
function Llamada(argumentos)
{
    var iframe = $("#llamada iframe");
    iframe[0].src = argumentos.url;
    argumentos.modal = false;
    argumentos.width = 400;
    argumentos.minWidth = 400;
    argumentos.height = 300;
    argumentos.minHeight = 300;
    $("#llamada")[0].title = Recursos["titSystem"];
    var temp = argumentos.close;
    argumentos.close = function (event, ui) { if (temp != undefined) { temp(); } $("#llamada iframe")[0].src = "about:blank"; };
    //argumentos.hide = { effect: "fade", duration: 500 };
    var d = $("#llamada").dialog(argumentos);
    d.dialogExtend({
        /*        "minimizable": true,
        "dblclick": "minimize",
        "icons": { "minimize": "ui-icon-circle-minus" },*/
        "closable": false
    });
}
function Emergente(argumentos)
{
    /// <summary>
    ///     Llama una ventana emergente
    /// </summary>
    /// <param name="Parametros" type="Object">
    ///     Un objeto con las propiedades para el levantamiento de la ventana emergente
    ///     &#10;    * url: Dirección a mostrar (string)
    /// </param>
    /// <returns type="void" />
    var iframe = $("#emergente iframe");
    //iframe[0].src = 'about:blank';

    //    iframe[0].src = '';
    //    iframe2 = iframe[0].cloneNode(true);
    //    iframe[0].parentNode.appendChild(iframe2);
    //    iframe2.parentNode.removeChild(iframe[0]);
    //iframe2.src = argumentos.url;
    //    setTimeout('$("#emergente iframe")[0]="' + argumentos.url + '"', 0);
    iframe[0].src = argumentos.url;
    if (argumentos.modal == undefined) { argumentos.modal = true; }
    if (argumentos.width == undefined) { argumentos.width = 800; }
    if (argumentos.minWidth == undefined) { argumentos.minWidth = 600; }
    if (argumentos.height == undefined) { argumentos.height = 600; }
    if (argumentos.minHeight == undefined) { argumentos.minHeight = 400; }
    if (argumentos.titulo != undefined) $("#emergente")[0].title = argumentos.titulo;
    if (argumentos.titulo == undefined) $("#emergente")[0].title = Recursos["titSystem"];
    var temp = argumentos.close;
    argumentos.close = function (event, ui) { if (temp != undefined) { temp(); } $("#emergente iframe")[0].src = "about:blank"; };
    //argumentos.hide = { effect: "fade", duration: 500 };
    //argumentos.show = { effect: "fade", duration: 1000 };
    var d = $("#emergente").dialog(argumentos);
    if (argumentos.minimizable != undefined)
    {
        d.dialogExtend({
            "minimizable": true,
            "dblclick": "minimize",
            "icons": { "minimize": "ui-icon-circle-minus" }
        });
    }
}
function CerrarEmergente()
{
    $("#emergente").dialog("destroy");
}
function CerrarLlamada()
{
    $("#llamada iframe")[0].src = 'about:blank';
    $("#llamada").dialog("destroy");
}

function Mensaje(argumentos)
{
    $("#msgbox").html(argumentos.mensaje);
    if (argumentos.titulo != undefined) $("#msgbox")[0].title = argumentos.titulo;
    if (argumentos.titulo == undefined) $("#msgbox")[0].title = Recursos["titSystem"];
    argumentos.modal = true;
    if (argumentos.buttons == undefined)
    {
        argumentos.buttons = { Aceptar: function () { $(this).dialog("close"); } };
    }
    $("#msgbox").dialog(argumentos);
}
function Preguntar(argumentos)
{
    $("#msgbox").html(argumentos.mensaje);
    if (argumentos.titulo != undefined) $("#msgbox")[0].title = argumentos.titulo;
    if (argumentos.titulo == undefined) $("#msgbox")[0].title = Recursos["titSystem"];
    argumentos.modal = true;
    if (argumentos.buttons == undefined)
    {
        //        argumentos.buttons = { Cancelar: function () { $(this).dialog("close"); } };
        //        argumentos.buttons = { Aceptar: argumentos.funcion };
        argumentos.buttons = [{ text: "Aceptar", click: function () { $(this).dialog("close"); argumentos.funcion(); } }, { text: "Cancelar", click: function () { $(this).dialog("close"); } }];
    }
    $("#msgbox").dialog(argumentos);
}