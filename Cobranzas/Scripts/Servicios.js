/// <reference path="jquery-1.10.2-vsdoc.js"/>
/// <reference path="General.js"/>
/// <reference path="InterfazGrafica.js"/>
/// <reference path="ordenacion.js">
//---------------------------------------------------------------General
//#region General
//Parámetros Generales para las llamadas a ajax.
Serv = "/Servicios/wsCobranzas.svc/";
llamadas = 0;
$(document).bind("ajaxSend", function ()
{
    llamadas++;
    $("#Numero").text(llamadas);
}).bind("ajaxComplete", function (event, xhr, settings)
{
    /*llamadas--;
    $("#Numero").text(llamadas);
    if (llamadas == 0)
    {
        $("#Cargando").hide();
    }*/
}).bind("ajaxSuccess", function (event, xhr, settings)
{
    llamadas--;
    $("#Numero").text(llamadas);
}).bind("ajaxError", function (event, jqXHR, ajaxSettings, thrownError)
{
    llamadas--;
    $("#Numero").text(llamadas);
    try
    {
        var Error = JSON.parse(jqXHR.responseText);
        Mensaje({ mensaje: "Error en WebService: <br/>" + Error.Message });
    } catch (e)
    {
        //Mensaje({ mensaje: "Error en WebService" });
        if (jqXHR.responseText != "")
        {
            Mensaje({ mensaje: jqXHR.responseText });
        }
    }
}).bind("ajaxStart", function (event, xhr, settings)
{
    $("#Cargando").show();
}).bind("ajaxStop", function (event, xhr, settings)
{
    llamadas=0;
    $("#Cargando").hide();
    $("#Numero").text(llamadas);
});
$.ajaxSetup({ cache: false, type: "PUT", contentType: "application/json; charset=utf-8"/*, error: ErrorWS*/ });
//function ErrorWS(jqXHR, textStatus, errorThrown)
//{
//    try
//    {
//        var Error = JSON.parse(jqXHR.responseText);
//        Mensaje({ mensaje: "Error en WebService: <br/>" + Error.Message });
//    } catch (e)
//    {
//        //Mensaje({ mensaje: "Error en WebService" });
//        if (jqXHR.responseText != "")
//        {
//            Mensaje({ mensaje: jqXHR.responseText });
//        }
//    }
//}
////#endregion
function Salir(idOperador)
{
    $.ajax({ url: Serv + "Salir", type: "POST", data: JSON.stringify({ idOperador: idOperador }), async: false });
}
function LlamarServicio(Respuesta, Servicio, Argumentos, Asincronico)
{
    if (Asincronico == undefined) Asincronico = true;
    $.ajax({ url: Serv + Servicio, type: "POST", data: JSON.stringify(Argumentos), success: Respuesta, async: Asincronico });


}


