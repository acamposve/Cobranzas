/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js" />
/// <reference path="/Scripts/ordenacion.js" />
/// <reference path="/Scripts/validaciones.js" />

function Inicializar()
{
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    Argumentos = searchToJSON();
    $("#idOperador").val(Argumentos.idOperador);
    $("input.fecha").val(AFechaMuyCorta(new Date()));

}
function ConsultarAvisos()
{
    //var FechaDesde = FechaJaponesa($("#dtpFechaDesde").val());
    //var FechaHasta = FechaJaponesa($("#dtpFechaHasta").val());
    var FechaDesde = Convert.ToDate($("#dtpFechaDesde").val()).ToString("JSON");
    var FechaHasta = Convert.ToDate($("#dtpFechaHasta").val()).ToString("JSON");
    //Avisos_Creados_lst(_Avisos_Creados_lst, Argumentos.idOperador, FechaDesde, FechaHasta);RA
    LlamarServicio(_Avisos_Creados_lst, "Avisos_Creados_lst", { idOperador : Argumentos.idOperador, FechaDesde : FechaDesde, FechaHasta : FechaHasta });
}
function _Avisos_Creados_lst(msg)
{
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Reminder", Campo: "Aviso", Clase: "grdTexto", Ordenacion: false, Filtro: false},
            { Titulo: "Creator", Campo: "OperadorCrea", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "CreationDate", Campo: "FechaCrea", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
            { Titulo: "OriginalDate", Campo: "FechaOriginal", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "FechaAviso", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
            { Titulo: "CancelationDate", Campo: "FechaCancelado", Clase: "grdFechaHora", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "NombrePersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "CodigoPersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Comment", Campo: "Comentario", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });

}
