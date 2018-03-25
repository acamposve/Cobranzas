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
    LlenarCombo({ Combo: "cboPais", Resultado: window.parent.Paises, TextoNull: "Todos", ValorNull: "%%%", DivResultado: "lstPais" });
    LlamarServicio(_Clientes_lst, "Clientes_lst");
}
function _Clientes_lst(msg)
{
    Clientes = msg.d;
    LlenarCombo({ Combo: "cboCliente", Resultado: Clientes, CampoId: "id", CampoValor: "Nombre" });
}
function Ejecutar()
{
    var idCliente = Convert.ToInt32($("#cboCliente").val());
    if (idCliente == 0) idCliente = null;
    LlamarServicio(_ClienteCuentas_rpt, "ClienteCuentas_rpt", { idCliente: idCliente });
}
function _ClienteCuentas_rpt(msg)
{
    Tabla({
        Contenedor: "pnlResultados",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Country", Campo: "Pais", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Account", Campo: "Factura", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Type", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "BL", Campo: "BL", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Buque", Campo: "Buque", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "FechaDocumento", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Currency", Campo: "Moneda", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Current", Campo: "MontoRestante", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Moneda(Local)", Campo: "MonedaLocal", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Restente(Local)", Campo: "MontoLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Exchange", Campo: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Viaje", Campo: "Viaje", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "PuertoCarga", Campo: "PuertoCarga", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "PuertoDescarga", Campo: "PuertoDescarga", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "TipoPersona", Campo: "TipoPersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Category", Campo: "Categoria", Clase: "grdTexto", Ordenacion: true, Filtro: true },
        ]
    });
}
function Preparar()
{
    $("#idCliente").val($("#cboCliente").val());
}
function FiltrarPorPais() {
    var Pais = $("#cboPais").val();

    var ListaClientes = Pais == "%%%" ? Clientes : Clientes.where(function (a) { return a.Nombre.indexOf("(" + Pais + ")") != -1 });
    LlenarCombo({ Combo: "cboCliente", Resultado: ListaClientes, CampoId: "id", CampoValor: "Nombre"});
}

function SeleccionarPais() {
    var paises = ObtenerLista("lstPais");
    var CF = Clientes.where(function (a) { return paises.contains(a.idPais); });
    LlenarCombo({ Combo: "cboCliente", Resultado: CF, CampoId: "id", CampoValor: "Nombre" });
}