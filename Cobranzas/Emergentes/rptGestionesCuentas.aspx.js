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
    LlenarCombo({ Combo: "cboPais", Resultado: window.parent.Paises, TextoNull: "Todos", ValorNull: "%%%", DivResultado: "lstPais" });
    LlamarServicio(_Clientes_lst, "Clientes_lst");

}

function FiltrarPorPais() {
    var Pais = $("#cboPais").val();
    var ListaClientes = Pais == "%%%" ? Clientes : Clientes.where(function (a) { return a.Nombre.indexOf("(" + Pais + ")") != -1 });
    LlenarCombo({ Combo: "cboCliente", Resultado: ListaClientes, CampoId: "id", CampoValor: "Nombre"});
}

function SeleccionarPais() {
    var paises = ObtenerLista("lstPais");
    var CF = Clientes.where(function (a) { return paises.contains(a.idPais); });
    LlenarCombo({ Combo: "cboCliente", Resultado: CF, CampoId: "id", CampoValor: "Nombre"});
}

function _Clientes_lst(msg) {
    Clientes = msg.d;
    LlenarCombo({ Combo: "cboCliente", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre" });
}
function Ejecutar() {
    var idCliente = Convert.ToInt32($("#cboCliente").val());
    if (idCliente == 0) idCliente = null;
    LlamarServicio(_GestionesCuentas_rpt, "GestionesCuentas_rpt", { idCliente: idCliente });
}
function _GestionesCuentas_rpt(msg) {
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Country", Campo: "Pais", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Account", Campo: "Factura", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Type", Campo: "TipoFactura", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "FechaCreacion", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "ExchangeRate", Campo: "Cambio", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Category", Campo: "Categoria", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Current", Campo: "Corriente", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "M30", Campo: "M30", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "M45", Campo: "M45", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "M60", Campo: "M60", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "M90", Campo: "M90", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "M120", Campo: "M120", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "M180", Campo: "M180", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "LastManagement", Campo: "UltimaGestion", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "FechaUltimaGestion", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Operator", Campo: "UltimoOperador", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });
}
function Preparar() {
    $("#idCliente").val($("#cboCliente").val());
}