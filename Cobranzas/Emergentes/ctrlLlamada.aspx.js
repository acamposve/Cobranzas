/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    if (window.parent.idP == undefined) { return; }
    Args = searchToJSON();
    Estado2();
    $("#Llamada").text(Args.Numero);
}
function Llamar(Numero, idTelefono, idPersona) {
    $("#CanalPropio").val("");
    $("#CanalCliente").val("");
    $("#btnColgar")[0].disabled = true;
    $("#btnTransferir")[0].disabled = true;
    LlamarServicio(_Llamar, "Llamar", { Telefono: Numero, idTelefono: idTelefono, idPersona: idPersona });
}
function _Llamar(msg) {
    if (msg.d.Error!="") {
        if (msg.d.Error.indexOf("Originate") != -1) {

        } else {
            _Estado({ d: 0 });
            return;
        }
    }
    if (msg.d.Error != "") {
        $("#CanalPropio").val(msg.d.Canal);
        $("#CanalCliente").val(msg.d.DestChannel);

        $("#btnColgar")[0].disabled = false;
        $("#btnTransferir")[0].disabled = false;
    }
    /*if (msg.d.Canal == "") {
        _Estado({ d: 0 });
    }*/
    setTimeout("Estado()", 5000);
    Temp = setInterval("Timer()", 1000);
    Tiempo = 0;
    Estado();
}
function Colgar() {
    var Canal = $("#CanalPropio").val();
    LlamarServicio(_Colgar, "Colgar", { Canal: Canal });
}
function _Colgar(msg) {
    window.parent.FinalizarLlamada();
}
function Transferir() {
    var Canal = $("#CanalCliente").val();
    var Extension = $("#txtExtension").val();
    LlamarServicio(_Transferir, "Transferir", { Canal: Canal, Extension: Extension });
}
function _Transferir(msg) {
    window.parent.FinalizarLlamada();
}
function Estado() {
    LlamarServicio(_Estado, "Estado", {});
}
function Estado2() {
    LlamarServicio(_Estado2, "Estado", {});
}
function _Estado2(msg) {
    if (msg.d == 0) {
        Llamar(Args.Numero, Args.idTelefono, Args.idPersona);
    }
    else {
        $("#Status").text("Esperando a que la línea se desocupe...");
        setTimeout("Estado2()", 3000);
    }
}
function _Estado(msg) {
    if (msg.d == 0) {
        setTimeout("_Colgar(null);", 2000);
        $("#Status").text("Llamada finalizada");
        clearInterval(Temp);
    }
    else {
        $("#Status").text(ConvertirStatus(msg.d));
        setTimeout("Estado()", 5000);
    }
}
function ConvertirStatus(Status) {
    if (Status == 0) return "Desocupado";
    Result = "";
    if (Status >= 16) {
        Result += "En espera;"
        Status -= 16;
    }
    if (Status >= 8) {
        Result += "Repicando;"
        Status -= 8;
    }
    if (Status >= 4) {
        Result += "No disponible;"
        Status -= 4;
    }
    if (Status >= 2) {
        Result += "Ocupado;"
        Status -= 2;
    }
    if (Status >= 1) {
        Result += "En uso;"
        Status -= 1;
    }
    return Result.substr(0, Result.length - 1);
}
function Timer() {
    Tiempo = Tiempo + 1;
    $("#Temporizador").text(Convert.ToTimeStamp(Tiempo));
}