/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js" />
/// <reference path="/Scripts/ordenacion.js" />
/// <reference path="/Scripts/validaciones.js" />
$(document).ready(Inicializar);
function Inicializar()
{
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    Argumentos = searchToJSON();
    $("#idOperador").val(Argumentos.idOperador);
    $("input.fecha").val(new Date().ToString(Regional.FormatoFecha));
}

function ConsultarLogins()
{
    LlamarServicio(_Logins_rpt, "Logins_rpt", {
        FechaDesde: Convert.ToDateTime($("#dtpFechaDesde").val()).ToString("JSON"),
        FechaHasta: Convert.ToDateTime($("#dtpFechaHasta").val()).ToString("JSON"),
        idOperador: Argumentos.idOperador,
        Supervisados: Argumentos.Supervisados
    });
}
function _Logins_rpt(msg)
{
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: false, Filtro: false },
            { Titulo: "Start", Campo: "Inicio", Clase: "grdHora", Ordenacion: true, Filtro: true },
            { Titulo: "End", Campo: "Fin", Clase: "grdHora", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });
}
