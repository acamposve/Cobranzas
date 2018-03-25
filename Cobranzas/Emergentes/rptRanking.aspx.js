/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js" />
/// <reference path="/Scripts/ordenacion.js" />
/// <reference path="/Scripts/validaciones.js" />
$(document).ready(Inicializar);
function Inicializar() {
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    Argumentos = searchToJSON();
    $("#idOperador").val(Argumentos.idOperador);
    $("input.fecha").val(new Date().ToString(Regional.FormatoFecha));
    LlamarServicio(_Metas_lst, "Metas_lst", { idOperador: Argumentos.idOperador });
}
function _Metas_lst(msg) {
    Tabla({
        Resultado: msg.d,
        Contenedor: "lstMetas",
        idSeleccion: "idMeta",
        Campos: [{ Titulo: "Goal", Campo: "Nombre", Clase: "grdTexto" }]
    });
    //LlenarCombo({ Combo: 'lstMetas', Resultado: msg.d, CampoId: 'idMeta', CampoValor: 'Nombre' });
    //$("#lstMetas").click(function () { this.selected = !this.selected; });
}
function Preparar() {
    /*var options = $("#lstMetas")[0].getElementsByTagName("OPTION");
    for (var i = 0; i < options.length; i++) {
        var option = options[i];
        if (option.selected) {
            seleccion += "," + option.value;
        }
    }
    if (seleccion.length > 0) seleccion = seleccion.substring(1);*/
    var seleccion = "";
    var OP = ObtenerSeleccionados("lstMetas").select("idMeta");
    for (var i = 0; i < OP.length; i++) {
        seleccion += "," + OP[i];
    }
    if (seleccion.length > 0) seleccion = seleccion.substring(1);
    $("#selMetas").val(seleccion);
}