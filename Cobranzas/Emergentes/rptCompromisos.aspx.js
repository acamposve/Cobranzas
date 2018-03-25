/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js" />
/// <reference path="/Scripts/ordenacion.js" />
/// <reference path="/Scripts/validaciones.js" />

function Inicializar() {
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    Argumentos = searchToJSON();
    $("#idOperador").val(Argumentos.idOperador);
    $("input.fecha").val(AFechaMuyCorta(new Date()));
    if (Argumentos.Supervisados == 0) {
        $("#filaOperador").hide();
        $("#Supervisado").val(0);
    } else {
        //OperadoresSupervisados_lst(_OperadoresSupervisados_lst, Argumentos.idOperador);RA
        LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Argumentos.idOperador });
        $("#Supervisado").val(1);
    }

}
function _OperadoresSupervisados_lst(msg) {
    LlenarCombo({ Combo: "cboOperador", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre", TextoNull: "Todos", ValorNull: Argumentos.idOperador })
    $("#cboOperador").val(Argumentos.idOperador);
}
function ConsultarCompromisos() {
    //    var FechaDesde = FechaJaponesa($("#dtpFechaDesde").val());RA
    //    var FechaHasta = FechaJaponesa($("#dtpFechaHasta").val());RA
    var FechaDesde = Convert.ToDate($("#dtpFechaDesde").val()).ToString("JSON");
    var FechaHasta = Convert.ToDate($("#dtpFechaHasta").val()).ToString("JSON");
    if (Argumentos.Supervisados == 0) {
        var idOperador = $("#idOperador").val();
    } else {
        var idOperador = $("#cboOperador").val();
    }

    var Supervisado = $("#Supervisado").val();
    LlamarServicio(_Compromisos_rpt, "Compromisos_rpt", { idOperador: idOperador, FechaDesde: FechaDesde, FechaHasta: FechaHasta, Supervisado: Supervisado });
}
function _Compromisos_rpt(msg) {
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "TotalLocal", Campo: "MontoLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "TotalUSD", Campo: "MontoDolar", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "RestanteLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "RestanteDolar", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ],
        FuncSeleccionar: "AbrirPersona(«idPersona»);"
    });

}
function AbrirPersona(idPersona) {
    window.parent.Persona_Mostrar(idPersona);
}