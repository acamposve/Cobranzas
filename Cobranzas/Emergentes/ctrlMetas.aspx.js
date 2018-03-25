/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {
    Args = searchToJSON();
    LlamarServicio(_Metas_lst, "Metas_lst", { idOperador: Args.idOperador });
    LlamarServicio(_Reglas_lst, "Reglas_lst", { idOperador: Args.idOperador });
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Args.idOperador });
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
            if (ui.newPanel[0].id == "tabDetalles" && $('#idMeta').val() == "") {
                Mensaje({ mensaje: "No ha seleccionado ningún item para ver" });
                return false;
            }
            if ((ui.newPanel[0].id == "tabConsultar" || ui.newPanel[0].id == "tabRanking") && ($('#idMeta').val() == "" || $('#idMeta').val() == "0")) {
                Mensaje({ mensaje: "No ha seleccionado ningún item o no ha Guardado" });
                return false;
            }
            if (ui.newPanel[0].id == "tabDetalles") {
                if ($("#idMeta").val() == "0") {
                    $("#pnlOperadores").hide();
                }
                else {
                    $("#pnlOperadores").show();
                }
            }
        }
    });
    $("#tabs2").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
        }
    });
}

function _Metas_lst(msg) {
    Tabla({
        Contenedor: "pnlListado",
        Resultado: msg.d,
        FuncSeleccionar: 'Seleccionar(«idMeta»);',
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Rule", Campo: "Regla", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha", Ordenacion: true, Filtro: true }
        ]
    })
}

function _OperadoresSupervisados_lst(msg) {
    LlenarCombo({ Combo: "cboOperadores", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    //LlenarCombo({ Combo: "cbomDueno", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
}

function _Reglas_lst(msg) {
    LlenarCombo({ Combo: "cboRegla", Resultado: msg.d, CampoId: "idRegla", CampoValor: "Nombre", TextoNull: "«Meta Manual»", ValorNull: "null" });
}

function Nuevo() {
    $("#idMeta").val(0);
    $("#txtNombre").val("");
    $("#txtFechaInicio").val("");
    $("#txtFechaFin").val("");
    $("#cboRegla").val("0");
    $("#cboFrecuencia").val("0");
    $("#pstDetalles").click();
    $("#chkAplicaExclusiones")[0].checked = true;
}
function Guardar() {
    var Meta = {};
    Meta.idMeta = $("#idMeta").val();
    Meta.Nombre = $("#txtNombre").val();
    Meta.FechaInicio = Convert.ToDateTime($("#txtFechaInicio").val()).ToString("JSON");
    Meta.idRegla = $("#cboRegla").val();
    Meta.Frecuencia = $("#cboFrecuencia").val();
    Meta.AplicaExclusiones = $("#chkAplicaExclusiones")[0].checked;
    LlamarServicio(_Metas_sav, "Metas_sav", { Meta: Meta });
}
function _Metas_sav(msg) {
    $("#idMeta").val(msg.d);
    LlamarServicio(_Metas_lst, "Metas_lst", { idOperador: Args.idOperador });
    $("#pnlOperadores").show();
    Seleccionar(msg.d);
    Mensaje({ mensaje: "La meta fue Guardada satisfactoriamente" });
}
function Seleccionar(idMeta) {
    LlamarServicio(_Metas_sel, "Metas_sel", { idMeta: idMeta });
}
function _Metas_sel(msg) {
    Meta = msg.d;
    $("#idMeta").val(Meta.idMeta);
    $("#txtNombre").val(Meta.Nombre);
    $("#txtFechaInicio").val(AFechaMuyCorta(Meta.FechaInicio));
    $("#txtFechaFin").val(AFechaMuyCorta(Meta.FechaFin));
    $("#cboRegla").val(Meta.idRegla);
    $("#cboFrecuencia").val(Meta.Frecuencia);
    $("#chkAplicaExclusiones")[0].checked = Meta.AplicaExclusiones;
    LlenarCombo({ Combo: "cboMetas", Resultado: Meta.Metas, CampoId: "Fecha", CampoValor: "Fecha" });
    LlenarCombo({ Combo: "cboMetaRanking", Resultado: Meta.Metas, CampoId: "Fecha", CampoValor: "Fecha" });
    try {
        ConsultarOperadores();
    } catch (e) {
       
    } 
    Tabla({
        Contenedor: "pnlOperadoresMetas",
        Resultado: Meta.Operadores,
        FuncEliminar: 'EliminarOperador(«id»);',
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });
    $("#pstDetalles").click();
    $("#tabs2").hide();

}
function EliminarOperador(idOperador) {
    LlamarServicio(_Metas_Operadores_del, "Metas_Operadores_del", { idMeta: $("#idMeta").val(), idOperador: idOperador });
}
function InsertarOperador() {
    var idOperador = $("#cboOperadores").val();
    //el _del es intencional, porque igual hay que refrescar el listado.
    LlamarServicio(_Metas_Operadores_del, "Metas_Operadores_ins", { idMeta: $("#idMeta").val(), idOperador: idOperador });
}
function _Metas_Operadores_del(msg) {
    Seleccionar($("#idMeta").val());
}
function Consultar() {
    LlamarServicio(_Metas_Detalle_sel, "Metas_Detalle_sel", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val() });
    LlamarServicio(_Pagos_Operador_Meta_lst, "Pagos_Operador_Meta_lst", { idOperador: $("#cboOperadorMeta").val(), idMeta: $("#idMeta").val(), FechaIni: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON") });
}
function CambiarAjuste() {
    LlamarServicio(_Metas_Ajuste_upd, "Metas_Ajuste_upd", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), Monto:$("#txtAjusteOperadorDet").val() });
}
function _Metas_Ajuste_upd() {
    Mensaje({mensaje:"El ajuste fue realizado correctamente"});
}
function ConsultarRanking() {
    LlamarServicio(_Ranking_lst, "Ranking_lst", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetaRanking").val()).ToString("JSON") });
}
function _Ranking_lst(msg) {
    Tabla({
        Contenedor: "pnlRanking",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Goal", Campo: "Meta", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Goalsset", Campo: "MetaFija", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Real", Campo: "Real", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Perc", Campo: "Porc", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "PercGoalSet", Campo: "PorcMetaFija", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
        ]
    })
}
function _Metas_Detalle_sel(msg) {
    Detalle = msg.d;
    Campos = [
            //{ Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true, Resumen: "CUENTA" },
            //{ Titulo: "Code", Campo: "CodigoPersona", Clase: "grdTexto", Ordenacion: true, Filtro: true, Resumen: "CUENTA" },
            //{ Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true, Resumen: "CUENTA" },
            //{ Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            //{ Titulo: "DeliveryDate", Campo: "FechaEntrega", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            //{ Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true, Resumen: "NINGUNO" },
            //{ Titulo: "Client", Campo: "CodigoCliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            //{ Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            //{ Titulo: "Total", Campo: "Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            //{ Titulo: "Remaining", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            //{ Titulo: "TotalUSD", Campo: "TotalDolar", ToolTip: "CambioDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            //{ Titulo: "RemainingUSD", Campo: "DeudaDolar", ToolTip: "CambioDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            //{ Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            //{ Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            //{ Titulo: "Claim", Campo: "EnReclamo", Clase: "grdBooleano" },
            //{ Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true }
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true, Resumen: "CUENTA" },
            { Titulo: "Code", Campo: "CodigoPersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true, Resumen: "NINGUNO" },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Goal", Campo: "Meta", Post: "USD", Clase: "grdDecimal", Ordenacion: false, Filtro: false, Resumen: "TOTAL" }
    ];
    //Cuentas
    Tabla({
        idSeleccion: "idCuenta",
        LimitarAltura: 300,
        Contenedor: "pnlCuentas",
        Resultado: Detalle.Cuentas,
        FuncEliminar: "ExcluirCuenta(«idCuenta»);",
        Campos: Campos
    });
    //Exclusiones
    Tabla({
        idSeleccion: "idCuenta",
        LimitarAltura: 300,
        Contenedor: "pnlExclusiones",
        Resultado: Detalle.Exclusiones,
        FuncEliminar: "DesExcluirCuenta(«idCuenta»);",
        Campos: Campos
    });
    //Inclusiones
    Tabla({
        idSeleccion: "idCuenta",
        LimitarAltura: 300,
        Contenedor: "pnlInclusiones",
        Resultado: Detalle.Inclusiones,
        FuncEliminar: "ExcluirCuenta(«idCuenta»);",
        Campos: Campos
    });
    $("#txtAjusteOperadorDet").val(Detalle.Ajuste);
    $("#tabs2").show();
}
function InlcuirCuenta(Codigo, Cuenta) //Cuenta indica si es una cuenta o es todas las cuentas de una persona
{
    LlamarServicio(Consultar(), "Metas_Cuentas_ins", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), Codigo: Codigo, Cuenta: Cuenta });
}
function ExcluirCuenta(idCuenta) {
    LlamarServicio(Consultar, "Metas_Cuentas_del", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), idCuenta: idCuenta });
}
function DesExcluirCuenta(idCuenta) {
    LlamarServicio(Consultar, "Metas_Cuentas_del_undo", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), idCuenta: idCuenta });
}
function EliminarCuentas() {
    var Cuentas = ObtenerSeleccionados("pnlCuentas").select("idCuenta");
    LlamarServicio(Consultar, "Metas_Cuentas_del2", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), idCuentas: Cuentas });
}
function EliminarExclusiones() {
    var Cuentas = ObtenerSeleccionados("pnlExclusiones").select("idCuenta");
    LlamarServicio(Consultar, "Metas_Cuentas_del_undo2", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), idCuentas: Cuentas });
}
function EliminarInclusiones() {
    var Cuentas = ObtenerSeleccionados("pnlInclusiones").select("idCuenta");
    LlamarServicio(Consultar, "Metas_Cuentas_del2", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON"), idOperador: $("#cboOperadorMeta").val(), idCuentas: Cuentas });
}
function ConsultarOperadores() {
    LlamarServicio(_MetasDet_Operadores_lst, "MetasDet_Operadores_lst", { idMeta: $("#idMeta").val(), Fecha: Convert.ToDateTime($("#cboMetas").val()).ToString("JSON") });
}
function _MetasDet_Operadores_lst(msg) {
    LlenarCombo({ Combo: "cboOperadorMeta", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre" });
}
function EjecutarMetas() {
    LlamarServicio(Consultar, "EjecutarMetas", {});
}
function Preparar() {
    $("#Fecha").val($("#cboMetas").val());
    $("#FechaRanking").val($("#cboMetaRanking").val());
    $("#idOperador").val($("#cboOperadorMeta").val());
}

function _Pagos_Operador_Meta_lst(msg) {
    Tabla({
        LimitarAltura: 300,
        Contenedor: "pnlPagosMeta",
        Resultado: msg.d,
        Campos: [
            { Titulo: "Pago", Campo: "Pago", Clase: "grdTexto", Ordenacion: true, Filtro: true, Resumen: "CUENTA" },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "Codigo", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Document", Campo: "Cuenta", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Amount", Campo: "Monto", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "$", Campo: "MontoDolar", Post:"USD", Clase: "grdDecimal", Ordenacion: true, Filtro: true, Resumen: "TOTAL" },

        ]
    });
}
