/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar() {
    if (window.parent.idOp == undefined) return;
    $("#tabs").tabs({
        show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui) {
        }
    });
    var Args = searchToJSON();
    LlamarServicio(_Metas_Detalle_sel, "Metas_Detalle_sel", { idMeta: Args.idMeta, Fecha: Convert.ToDateTime(Args.Fecha).ToString("JSON"), idOperador: Args.idOperador });
    $("#idMeta").val(Args.idMeta);
    $("#idOperador").val(Args.idOperador);
    $("#Fecha").val(Convert.ToDateTime(Args.Fecha).ToString());
    LlamarServicio(_Pagos_Operador_Meta_lst, "Pagos_Operador_Meta_lst", { idOperador: Args.idOperador, idMeta: Args.idMeta, FechaIni: Convert.ToDateTime(Args.Fecha).ToString("JSON") });
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
            { Titulo: "Goal", Campo: "Meta", Post:"USD", Clase: "grdDecimal", Ordenacion: false, Filtro: false, Resumen: "TOTAL" }
    ];
    //Cuentas
    Tabla({
        //        idSeleccion: "idCuenta",
        LimitarAltura: 300,
        Contenedor: "pnlCuentas",
        Resultado: Detalle.Cuentas,
        FuncSeleccionar: "Seleccionar(«idPersona»);",
        Campos: Campos
    });
    //Exclusiones
    Tabla({
        //        idSeleccion: "idCuenta",
        LimitarAltura: 300,
        Contenedor: "pnlExclusiones",
        Resultado: Detalle.Exclusiones,
        FuncEliminar: "Seleccionar(«idPersona»);",
        Campos: Campos
    });
    //Inclusiones
    Tabla({
        //        idSeleccion: "idCuenta",
        LimitarAltura: 300,
        Contenedor: "pnlInclusiones",
        Resultado: Detalle.Inclusiones,
        FuncEliminar: "Seleccionar(«idPersona»);",
        Campos: Campos
    });
    $("#txtAjusteOperadorDet").val(Detalle.Ajuste);
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
            { Titulo: "$", Campo: "MontoDolar", Post: "USD", Clase: "grdDecimal", Ordenacion: true, Filtro: true, Resumen: "TOTAL" },
            { Titulo: "Esp", Campo: "Esp", Clase: "grdBooleano", Ordenacion: true, Filtro: true }
        ]
    });
}
