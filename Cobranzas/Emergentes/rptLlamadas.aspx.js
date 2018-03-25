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

function Consultar()
{
    LlamarServicio(_Llamadas_rpt, "Llamadas_rpt", {
        FechaDesde: Convert.ToDateTime($("#dtpFechaDesde").val()).ToString("JSON"),
        FechaHasta: Convert.ToDateTime($("#dtpFechaHasta").val()).ToString("JSON"),
        idOperador: Argumentos.idOperador,
        Supervisados: Argumentos.Supervisados
    });
}
function _Llamadas_rpt(msg)
{
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Status", Campo: function (a) { return "/Img/" + a.Tipo + "24.png"; }, Clase: "grdTexto", Ordenacion: "TipoStatus", Tooltip: "Descripcion", Filtro: false, Imagen: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFechaHora", Ordenacion: false, Filtro: false },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Phone", Campo: "Telefono", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Duration", Campo: "Duracion", Clase: "grdTimeStamp", Ordenacion: true, Filtro: true },
            { Titulo: "EffectiveDuration", Campo: "DuracionEfectiva", Clase: "grdTimeStamp", Ordenacion: true, Filtro: true },
            { Titulo: "Recording", Campo: "Grabacion", Clase: "grdTexto", Ordenacion: true, Filtro: true, OnClick: "Abrir('«idLlamada»');" }
        ]

    });
}
function Abrir(idLlamada)
{
    location.href = "/Emergentes/Grabaciones.aspx?idLlamada=" + idLlamada;
}