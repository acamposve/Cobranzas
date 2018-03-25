/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    Args = searchToJSON();
    Modificado = false;
    $("#tabs").tabs({ show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui)
        {
            //antes de activar
        }
    });
    LlamarServicio(Pasos_Respuesta, "Pasos_lst");
    LlamarServicio(Flujos_Respuesta, "Flujos_lst");
    LlamarServicio(Reglas_Respuesta, "Reglas_lst");
    LlamarServicio(TiposCliente_Respuesta, "TiposCliente_lst");
}
function Flujos_Respuesta(msg)
{
    var Deudas = msg.d;
    $("#pnlDistFlujos").html("");
    var pnl = $("#pnlDistFlujos")[0];
    var tabla = CrearTabla("Tabla");
    pnl.appendChild(tabla);
    var fila = AgregarFila(tabla);
    AgregarHeader(fila, "Nombre", "grdTexto", true, true);
    AgregarHeader(fila, "X", "grdTexto", false, false); //.style.width = "16px";

    for (i = 0; i < Deudas.length; i++)
    {
        fila = AgregarFila(tabla, "itmSeleccionable");
        //fila.onclick = Function("Flujos_sel(Flujos_RespuestaDet," + Deudas[i].idFlujo + ");$('#pstDetalles').click();");RA
        fila.onclick = Function("LlamarServicio(Flujos_RespuestaDet, 'Flujos_sel', { idFlujo : " + Deudas[i].idFlujo + "});$('#pstDetalles').click();"); 
        AgregarCelda(fila, Deudas[i].Nombre, "grdTexto");
        var celda = AgregarCelda(fila, "", "grdTexto");
        celda.className = "btnEliminar16";
        celda.onclick = new Function("Flujo_Eliminar(" + Deudas[i].idFlujo + ");try{event.cancelBubble=true;event.stopPropagation();}catch(e){};return false;");
    }
    RedefinirAlternativo(tabla);
}
function Flujo_Eliminar(idFlujo)
{
    Mensaje({ mensaje: "¿Está seguro de que desea Eliminar este flujo?", buttons: { "Sí": new Function("Eliminar_Flujo(" + idFlujo + ");$(this).dialog('close');"), "No": function () { $(this).dialog("close"); } } })
}
function Eliminar_Flujo(idFlujo)
{
    //Flujos_del(Flujos_del_Respuesta, idFlujo);RA
    LlamarServicio(Flujos_del_Respuesta, "Flujos_del", { idFlujo: idFlujo });
}
function Flujos_del_Respuesta(msg){
    if (msg.d)
    {
        //Flujos_lst(Flujos_Respuesta);RA
        LlamarServicio(Flujos_Respuesta, "Flujos_lst");
    }
}
function TiposCliente_Respuesta(msg)
{
    LlenarCombo({Combo:"cboTipoCliente", Resultado:msg.d, CampoId:"idTipoCliente", CampoValor:"Nombre"});
}
function Reglas_Respuesta(msg)
{
    LlenarCombo({Combo:"cboReglaSalida", Resultado:msg.d, CampoId:"idRegla", CampoValor:"Nombre"});
}
function Pasos_Respuesta(msg)
{
    var i;
    var respuesta = msg.d;
    for (i = 0; i < respuesta.length; i++)
    {
        DevolverPaso(respuesta[i].idPaso, respuesta[i].Nombre, "0px,0px");
    }
    $(".cajas").hide();
    LlenarCombo({ Combo: "cboPasos", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" });
}
function Restablecer()
{
    try { $(".cajas").draggable("destroy"); } catch (e) { }
    try { $(".cajas").droppable("destroy"); } catch (e) { }
    $(".cajas").css("cursor", "default");
}
function AgregarPaso()
{
    var idPaso = $("#cboPasos").val();
    var Paso = DevolverPaso(idPaso, "--", "0px,0px");
}

function Flujos_RespuestaDet(msg)
{
    $(".cajas").hide();
    Flujo = msg.d;
    $("#idFlujo").val(Flujo.idFlujo);
    $("#txtNombreFlujo").val(Flujo.Nombre);
    $("#cboTipoCliente").val(Flujo.idTipoCliente);
    $("#cboReglaSalida").val(Flujo.idReglaSalida);
    var i;
    for (i = 0; i < Flujo.Pasos.length; i++)
    {
        var Paso = Flujo.Pasos[i];
        DevolverPaso(Paso.idPaso, Paso.NombrePaso, Paso.Posicion);
    }
    Dibujar();
}
function Dibujar()
{
    $(".reglas").remove();
    var contenedor = $("#ContenedorPasos")[0];
    try
    {
        var lienzo = document.getElementById("lienzo");
        var contexto = lienzo.getContext('2d');
        contexto.clearRect(0, 0, contexto.canvas.width, contexto.canvas.height);
        var i;
        for (i = 0; i < Flujo.FlujoAvance.length; i++)
        {
            var color = /*SeleccionarColor(i,*/"#000040"/*)*/;
            contexto.FlechaFlujoAvance(Flujo.FlujoAvance[i], color);
            var el = document.createElement("div");
            el.className = "reglas";
            el.id = "regla" + i;
            $(el).text(Flujo.FlujoAvance[i].Regla);
            el.title = Flujo.FlujoAvance[i].Regla;
            el.onclick = new Function("CrearFlujoAvance(0,0," + i + ");");
            var p1 = $(DevolverPaso(Flujo.FlujoAvance[i].idPasoInicio));
            var p2 = $(DevolverPaso(Flujo.FlujoAvance[i].idPasoFinal));
            contenedor.appendChild(el);
            var porc = 0.6;
            var x = p1.position().left * (1 - porc) + p2.position().left * porc + (p1.width() - $(el).width()) / 2;
            var y = p1.position().top * (1 - porc) + p2.position().top * porc + (p1.height() - $(el).height()) / 2;
            var angulo = Math.atan2(p2.position().top - p1.position().top, p2.position().left - p1.position().left);
            $(el).css("left", x); $(el).css("top", y);
            if (angulo > Math.PI / 2) angulo = angulo - Math.PI;
            if (angulo < -Math.PI / 2) angulo = angulo + Math.PI;
            $(el).rotar(angulo, "rad");
        }
    }
    catch (e)
    {
        //prueba
    }
}
function ModificarDiagrama()
{
    Restablecer();
    $(".cajas").draggable({
        containment: "parent",
        stack: ".cajas",
        //helper:"clone",
        drag: function ()
        {
            Dibujar();
        },
        stop: function ()
        {
            Dibujar();
        }
    });
    $(".cajas").css("cursor", "move");
}
function EsValido()
{
    var Result = true;
    return Result;
}
function Guardar()
{
    if (!EsValido()) return;
    Restablecer();

    Flujo.Nombre = $("#txtNombreFlujo").val();
    Flujo.idTipoCliente = $("#cboTipoCliente").val();
    Flujo.idReglaSalida = $("#cboReglaSalida").val();
    var i = 0;
    $(".cajas").each(function ()
    {
        if ($(this).css("display") != "none")
        {
            if (Flujo.Pasos[i] == undefined) Flujo.Pasos[i] = {};
            Flujo.Pasos[i].Posicion = $(this).css("left") + "," + $(this).css("top");
            Flujo.Pasos[i].idPaso = this.id.substr(4);
            i++;
        }
    })
    //Flujos_sav(Guardado, Flujo);RA
    LlamarServicio(Guardado, "Flujos_sav", { Flujoins: Flujo });
}
function Guardado(msg)
{
    Mensaje({ mensaje: "Flujo Guardado Satisfactoriamente" });
    $("#idFlujo").val(msg.d);
    //Flujos_lst(Flujos_Respuesta);RA
    LlamarServicio(Flujos_Respuesta, "Flujos_lst");
}
function CrearReglas()
{
    Restablecer();
    $(".cajas").draggable({
        containment: "parent",
        helper: function (event) { return $("<span style='border:1px solid black;cursor:pointer;'>" + $(this).text() + "</span>")[0]; },
        cursorAt: { top: -1, left: -1 },
        drag: function ()
        {
            //Dibujar();
        },
        stop: function ()
        {
            //Dibujar();
        }
    });
    $(".cajas").droppable({
        drop: function (event, ui)
        {
            CrearFlujoAvance(ui.draggable[0].id.substr(4), this.id.substr(4));
        }
    });
    $(".cajas").css("cursor", "pointer");
}
function CrearFlujoAvance(idInicio, idFin, NroFlujoAvance)
{
    var idregla;
    if (NroFlujoAvance == undefined)
    {
        TempInicio = idInicio;
        TempFin = idFin;
        FlujoAvanceActual = Flujo.FlujoAvance.length;
        idregla = "";
    } else
    {
        TempInicio = Flujo.FlujoAvance[NroFlujoAvance].idPasoInicio;
        TempFin = Flujo.FlujoAvance[NroFlujoAvance].idPasoFinal;
        FlujoAvanceActual = NroFlujoAvance;
        idregla = "&idRegla=" + Flujo.FlujoAvance[NroFlujoAvance].idRegla;
    }
    Emergente({ url: '/Emergentes/ctrlReglas.aspx?sel=1' + idregla+"&idOperador="+Arg.idOperador, width: 600, height: 500 });
}
function CrearRegla(idRegla, NombreRegla)
{
    if (idRegla == 0)
    {
        Flujo.FlujoAvance.splice(FlujoAvanceActual, 1);
    }
    else
    {
        NuevaRegla = {
            idPasoInicio: TempInicio,
            idPasoFinal: TempFin,
            idRegla: idRegla,
            Regla: NombreRegla
        }
        Flujo.FlujoAvance[FlujoAvanceActual] = NuevaRegla;
    }
    Dibujar();
}
function Nuevo()
{
    $("#idFlujo").val(0);
    $("#txtNombreFlujo").val("Nuevo Flujo");
    $("#cboTipoCliente").val(1); //warning: Cable;
    $("#cboReglaSalida").val(1); //warning:Cable;
    Flujo = { idFlujo: 0, Nombre: "", idTipoCliente: 1, idReglaSalida: 1, Pasos: Array(), FlujoAvance: Array() }; //warning:Cable
    $(".cajas").hide();
    Dibujar();
    $('#pstDetalles').click();
}
function DevolverPaso(id, Nombre, Posicion)
{
    var result = $("#Paso" + id);
    if (result.length > 0)
    {
        result = result[0];
    }
    else
    {
        result = document.createElement("div");
        result.className = "cajas";
        result.id = "Paso" + id;
        $(result).text(Nombre);
        $("#ContenedorPasos")[0].appendChild(result);
    }

    if (Posicion != undefined)
    {
        var pos = Posicion.split(",");
        $(result).css({ left: pos[0], top: pos[1] });
        $(result).show();
    }
    return result;
}
function CanvasClick()
{
    var lienzo = document.getElementById("lienzo");
    var contexto = lienzo.getContext('2d');
    var color = ColorPixel(contexto, event.offsetX, event.offsetY);
    if (color != [0, 0, 0])
    {
        var i;
        for (i = 0; i < Flujo.FlujoAvance.length; i++)
        {
            if (SeleccionarColor(i, "#000040") == color)
            {
                alert(Flujo.FlujoAvance[i].PasoInicio + "," + Flujo.FlujoAvance[i].PasoFin + "," + Flujo.FlujoAvance[i].Regla);
            }
        }
    }
}
function ColorPixel(contexto, x, y)
{
    var result = Array();
    var pixel = contexto.getImageData(x, y, 1, 1);
    var i;
    for (i = 0; i < 3; i++)
    {
        result[i] = pixel.data[i];
    }
    //contexto.beginPath();
    //contexto.arc(x, y, 5, 0, Math.PI * 2);
    //contexto.fill();
    return result;
}
