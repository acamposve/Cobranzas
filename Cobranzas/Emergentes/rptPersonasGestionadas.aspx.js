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
    $("#idPais").val(window.parent.Paises.join(","));
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    Argumentos = searchToJSON();
    $("#idOperador").val(Argumentos.idOperador);
    $("input.fecha").val(new Date().ToString(Regional.FormatoFecha));
    //Clientes_lst(_Clientes_lst);RA
    LlamarServicio(_Clientes_lst, "Clientes_lst");
    //OperadoresSupervisados_lst(_OperadoresSupervisados_lst, Argumentos.idOperador);RA
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Argumentos.idOperador });
    LlenarCombo({ Combo: "cboPais", Resultado: window.parent.Paises, DivResultado: "lstPais" });
}
function _OperadoresSupervisados_lst(msg)
{
    LlenarCombo({ Combo: "cboOperador", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre"});
}
function _Clientes_lst(msg)
{
    Clientes = msg.d;
    LlenarCombo({ Combo: "cboCliente", Resultado: Clientes, CampoId: "id", CampoValor: "Nombre", TextoNull: "Todos", ValorNull: "0" });
}
function Ejecutar()
{
    var Paises = ObtenerLista("lstPais").join(",");
    if (Paises == "") {
        var Paises = window.parent.Paises.join(",");
    }
    var idCliente = Convert.ToInt32($("#cboCliente").val());
    if (idCliente == 0) idCliente = null;
    var TipoRep= $("#cboTipoReporte").val();
    if (TipoRep=="null") TipoRep=null;
    LlamarServicio(_PersonasGestionadas_rpt, "PersonasGestionadas_rpt",
    {
        FechaDesde: Convert.ToDateTime($("#dtpFechaDesde").val()).ToString("JSON"),
        FechaHasta: Convert.ToDateTime($("#dtpFechaHasta").val()).ToString("JSON"),
        FechaFin: Convert.ToDateTime($("#dtpFechaFin").val()).ToString("JSON"),
        idOperador: $("#cboOperador").val(),
        idCliente: idCliente,
        Supervisados: Argumentos.Supervisados == 1 && Convert.ToInt32($("#cboOperador").val()) == Argumentos.idOperador,
        Gestionados: TipoRep,
        Paises: Paises
    });
}
function _PersonasGestionadas_rpt(msg)
{
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Country", Campo: "Pais", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "AssignmentDate", Campo: "FechaAsignacion", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "0-30", Campo: "M0a30", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "30-45", Campo: "M30a45", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "45-60", Campo: "M45a60", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "60-90", Campo: "M60a90", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "90-120", Campo: "M90a120", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "120-180", Campo: "M120a180", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: ">180", Campo: "M180ainf", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "LastManagement", Campo: "UltimaGestion", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "LastManagement", Campo: "StatusUltimaGestion", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });
}

function SeleccionarPais() {
    var paises = ObtenerLista("lstPais");

    var CF = Clientes.where(function (a) { return paises.contains(a.idPais); });
    LlenarCombo({ Combo: "cboCliente", Resultado: CF, CampoId: "id", CampoValor: "Nombre", TextoNull: "«Todos»" });
}