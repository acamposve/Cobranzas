/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    Args = searchToJSON();
    LlamarServicio(_Campanas_lst, "Campanas_lst", {idOperador:Args.idOperador});
    LlamarServicio(_Reglas_lst, "Reglas_lst", {idOperador:Args.idOperador});
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador: Args.idOperador});
    LlamarServicio(_ModoDistOperadores_lst, "ModoDistOperadores_lst");
    LlamarServicio(_Flujos_lst, "flujos_lst");
    LlamarServicio(_Pasos_lst, "Pasos_lst");
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("#tabs").tabs({ show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui)
        {
            if (ui.newPanel[0].id == "tabDetalles" && $('#idCampana').val() == "")
            {
                Mensaje({ mensaje: "No ha seleccionado ninguna campana para ver" });
                return false;
            }
        }
    });
    $("#tabs2").tabs({ show: { effect: "fade", duration: 100 }, hide: { effect: "fade", duration: 100 },
        beforeActivate: function (event, ui)
        {
            switch (ui.newPanel[0].id)
            {
                case "Datos":
                    break;
                case "tabCuentas":
                    break;
                case "tabCuentasInactivas":
                    break;
                case "tabCuentasSinAsignar":
                    //CuentasSinAsignar_lst(_CuentasSinAsignar_lst);RA
                    LlamarServicio(_CuentasSinAsignar_lst, "CuentasSinAsignar_lst", { idOperador: Args.idOperador });
                    break;
                case "Estadísticas":
                    break;
                case "Operaciones":
                    break;

            }
            /*if (ui.newPanel[0].id == "tabDetalles" && $('#idCampana').val() == "")
            {
            Mensaje({ mensaje: "No ha seleccionado ninguna campana para ver" });
            return false;
            }*/
        }
    });

}
function _CuentasSinAsignar_lst(msg)
{
    Tabla({
        Contenedor: "pnlCuentasSinAsignar",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idCuenta",
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "TotalUSD", Campo: "TotalDolar", Clase: "grdDecimal", ToolTip: "CambioDolar", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "DeudaDolar", Clase: "grdDecimal", ToolTip: "CambioDolar", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "Remaining", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true }
        ]
    });
}

function _Campanas_lst(msg)
{
    Tabla({
        Contenedor: "pnlDistCampanas",
        Resultado: msg.d,
        FuncSeleccionar: 'SelCampana(«idCampana»);',
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Name", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "EstimatedEndDate", Campo: "FechaEstimadaFin", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Weight", Campo: "Peso", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Active", Campo: "Activa", Clase: "grdBooleano", Ordenacion: true, Filtro: true }
        ]
    })
}

function SelCampana(idCampana)
{
    LlamarServicio(_Campanas_sel, "Campanas_sel", { idCampana: idCampana });
}
function _Campanas_sel(msg)
{
    Campana = msg.d;

    $("#idCampana").val(Campana.idCampana);
    $("#txtmNombreCampana").val(Campana.Nombre);
    $("#txtmFechaInicio").val(AFechaMuyCorta(Campana.FechaInicio));
    $("#txtmFechaEstimadaFin").val(AFechaMuyCorta(Campana.FechaEstimadaFin));
    $("#txtmFechaFin").val(AFechaMuyCorta(Campana.FechaFin));
    $("#txtmPeso").val(Campana.Peso);
    $("#txtmMontoMeta").val(Campana.MontoMeta.ToString());
    $("#cbomFlujo").val(Campana.idFlujo);
    $("#cbomPaso").val(Campana.idPaso);
    $("#cbomTipoAsignacion").val(Campana.TipoCampana);
    $("#cbomDueno").val(Campana.idOperadorDueno);
    $("#chkmActiva")[0].checked = Campana.Activa;
    $("#cbomReglaSalida").val(Campana.idReglaSalida);
    var combo = document.createElement("select");
    var Operadores = Campana.Campanas_Operadores.where(function (x) { return x.FechaFin == null; }).select({ idOperador: "idOperador", Operador: "Operador" });
    LlenarCombo({ Combo: combo, Resultado: Operadores, CampoId: "idOperador", CampoValor: "Operador", TextoNull: "<Ninguno>" });
    LlenarCombo({ Combo: "cboOperadorAsignar", Resultado: Operadores, CampoId: "idOperador", CampoValor: "Operador", TextoNull: "<Ninguno>" });
    LlenarCombo({ Combo: "cboOperador", Resultado: Operadores, CampoId: "idOperador", CampoValor: "Operador", TextoNull: "<Ninguno>", ValorNull: "0" });

    Tabla({
        Contenedor: "pnlOperadoresCampana",
        Resultado: Campana.Campanas_Operadores,
        //idSeleccion: "idOperador",
        FuncEliminar: "EliminarOperador(«idOperador»)",
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Operator", Campo: "Operador", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "AccountsInCampaign", Campo: "CuentasCampana", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "TotalAccounts", Campo: "CuentasTotales", Clase: "grdEntero", Ordenacion: true, Filtro: true },
        ]
    })
    $('#pstDetalles').click();
}
function ConsultarActivas()
{
    LlamarServicio(_Campanas_Cuentas_Activas_lst, "Campanas_Cuentas_Activas_lst", { idCampana: Campana.idCampana, idOperador: $("#cboOperador").val() });
}
function _Campanas_Cuentas_Activas_lst(msg)
{
    var combo = $("#cboOperadorAsignar")[0];
    Tabla({
        Contenedor: "pnlCuentas",
        Resultado: msg.d,
        idSeleccion: "idCuenta",
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Operator", Campo: "idOperador", Clase: "grdTexto", Combo: combo, Ordenacion: true, Filtro: true },
            { Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Document", Campo: "Cuenta.Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Cuenta.Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Cuenta.Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Cuenta.Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cuenta.Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Cuenta.Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "TotalUSD", Campo: "Cuenta.TotalDolar", Clase: "grdDecimal", ToolTip: "Cuenta.CambioDolar", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "Cuenta.DeudaDolar", Clase: "grdDecimal", ToolTip: "Cuenta.CambioDolar", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Cuenta.Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "Remaining", Campo: "Cuenta.Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalLocal", Campo: "Cuenta.TotalLocal", Campo2: "MonedaLocal", ToolTip: "Cuenta.CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "Cuenta.DeudaLocal", Campo2: "MonedaLocal", ToolTip: "Cuenta.CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true }
        ]
    })
}
function ConsultarInactivas()
{
    LlamarServicio(_Campanas_Cuentas_Inactivas_lst, "Campanas_Cuentas_Inactivas_lst", { idCampana: Campana.idCampana, FechaIni: Convert.ToDateTime($("#dtpFechaIni").val()).ToString("JSON"), FechaFin: Convert.ToDateTime($("#dtpFechaIni").val()).ToString("JSON") });
}
function _Campanas_Cuentas_Inactivas_lst(msg)
{
    var combo = $("#cboOperadorAsignar")[0];
    Tabla({
        Contenedor: "pnlCuentasInactivas",
        Resultado: msg.d,
        //idSeleccion: "idCuenta",
        LimitarAltura: 200,
        Campos: [
        //{ Titulo: "Operator", Campo: "idOperador", Clase: "grdTexto", Combo: combo, Ordenacion: true, Filtro: true },
            {Titulo: "StartDate", Campo: "FechaInicio", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "EndDate", Campo: "FechaFin", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Document", Campo: "Cuenta.Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Cuenta.Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Cuenta.Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Cuenta.Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cuenta.Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Cuenta.Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "TotalUSD", Campo: "Cuenta.TotalDolar", Clase: "grdDecimal", ToolTip: "Cuenta.CambioDolar", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "Cuenta.DeudaDolar", Clase: "grdDecimal", ToolTip: "Cuenta.CambioDolar", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Cuenta.Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "Remaining", Campo: "Cuenta.Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalLocal", Campo: "Cuenta.TotalLocal", Campo2: "MonedaLocal", ToolTip: "Cuenta.CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "Cuenta.DeudaLocal", Campo2: "MonedaLocal", ToolTip: "Cuenta.CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true }
        ]
    })
}
function Nuevo()
{
    $('#Manejo').hide();
    $('.Paso').hide();
    $('#Paso1').show();
    $('#Wizard').fadeIn();
}
function AsignarOperador()
{
    //Operadores_Cuentas_ins(_Operadores_Cuentas_ins, $("#cboOperadorAsignar").val(), ObtenerSeleccionados("pnlCuentas").select("idCuenta"));RA
    LlamarServicio(_Operadores_Cuentas_ins, "Operadores_Cuentas_ins", { idOperador: $("#cboOperadorAsignar").val(), Cuentas: ObtenerSeleccionados("pnlCuentas").select("idCuenta") });
}
function _Operadores_Cuentas_ins()
{
    //Campanas_sel(_Campanas_sel, Campana.idCampana);RA
    LlamarServicio(_Campanas_sel, "Campanas_sel", { idCampana: Campana.idCampana });
}

function _OperadoresSupervisados_lst(msg)
{
    LlenarCombo({ Combo: "cbomOperadores", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    LlenarCombo({ Combo: "cboOperadores", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    LlenarCombo({ Combo: "cbomDueno", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" });
    Tabla({
        Contenedor: "pnlOperadores",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idOperador",
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Operator", Campo: "Nombre", Clase: "grdTexto", Ordenacion: true, Filtro: true },
        ]
    });
}
function _Reglas_lst(msg)
{
    LlenarCombo({ Combo: "cboReglas", Resultado: msg.d, CampoId: "idRegla", CampoValor: "Nombre", TextoNull: "«Campaña Vacía»", ValorNull: 0 });
    LlenarCombo({ Combo: "cbomReglaSalida", Resultado: msg.d, CampoId: "idRegla", CampoValor: "Nombre", TextoNull: "«Ninguna»", ValorNull: 0 });
    LlenarCombo({ Combo: "cboReglaSalida", Resultado: msg.d, CampoId: "idRegla", CampoValor: "Nombre", TextoNull: "«Ninguna»", ValorNull: 0 });
}
function _Flujos_lst(msg)
{
    LlenarCombo({ Combo: "cboFlujo", Resultado: msg.d, CampoId: "idFlujo", CampoValor: "Nombre" });
    LlenarCombo({ Combo: "cbomFlujo", Resultado: msg.d, CampoId: "idFlujo", CampoValor: "Nombre" });
}
function _Pasos_lst(msg)
{
    LlenarCombo({ Combo: "cboPaso", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" });
    LlenarCombo({ Combo: "cbomPaso", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" });
}
function _ModoDistOperadores_lst(msg)
{
    LlenarCombo({ Combo: "cboModosDistribucion", Resultado: msg.d, CampoId: "id", CampoValor: "Nombre" });
}
function Campana_Config()
{
    Campana = {};
    Campana.Nombre = $("#txtNombreCampana").val();
    Campana.idReglaCreacion = $("#cboReglas").val();
    Campana.idCampana = 0;
    Campana.idModoDistOperadores = $("#cboModosDistribucion").val();
    Campana.idFlujo = $("#cboFlujo").val();
    Campana.idPaso = $("#cboPaso").val();
    Campana.FechaInicio = aJSON($("#dtpFechaInicio").val());
    Campana.FechaEstimadaFin = aJSON($("#dtpFechaEstimadaFin").val());
    Campana.Campanas_Cuentas = Array();
    Campana.Campanas_Operadores = Array();
    Campana.Campanas_Operadores = ObtenerSeleccionados("pnlOperadores");
    Campana.Campanas_Cuentas = ObtenerSeleccionados("pnlDistCuentasReglas", "idOperador");
    Campana.Peso = -1;
    Campana.TipoCampana = $("#cboTipoAsignacion").val();
    Campana.Activa = 1;
    Campana.idOperadorDueno = 0;
    Campana.idReglaSalida = $("#cboReglaSalida").val();
    /*if (Campana.Campanas_Cuentas.length == 0)
    {
    Mensaje({ mensaje: "No ha seleccionado ninguna cuenta" });
    return false;
    }*/
    //Campanas_ins(Resultado, Campana);RA
    LlamarServicio(Resultado, "Campanas_ins", { Campanains: Campana });
    return true;
}
function GuardarCampana()
{
    Campana.Nombre = $("#txtmNombreCampana").val();
    Campana.FechaInicio = aJSON($("#txtmFechaInicio").val());
    Campana.FechaEstimadaFin = aJSON($("#txtmFechaEstimadaFin").val());
    //    if (Campana.FechaInicio == "") Campana.FechaInicio == undefined;
    //    if (Campana.FechaEstimadaFin == "") Campana.FechaEstimadaFin == undefined;
    //    if (Campana.FechaFin == "") Campana.FechaFin == undefined;
    try
    {
        var sel = ObtenerTodos("pnlCuentas", "idOperador").select({ idCuenta: "idCuenta", idOperador: "idOperador==''?null:idOperador", FechaInicio: '"' + aJSON("01/01/1970") + '"' });
        Campana.Campanas_Cuentas = sel;
    } catch (e) { }
    Campana.idFlujo = $("#cbomFlujo").val();
    Campana.idPaso = $("#cbomPaso").val();
    Campana.Campanas_Operadores = Array();
    Campana.Peso = Convert.ToInt32($("#txtmPeso").val());
    Campana.TipoCampana = $("#cbomTipoAsignacion").val();
    Campana.idReglaSalida = $("#cbomReglaSalida").val();
    Campana.idOperadorDueno = Convert.ToInt32($("#cbomDueno").val());
    Campana.Activa = $("#chkmActiva")[0].checked;
    //Campanas_sav(Resultado2, Campana);RA
    LlamarServicio(Resultado2, "Campanas_sav", { Campanains: Campana });
    return true;
}
function Resultado(msg)
{
    //Campanas_lst(_Campanas_lst);RA
    LlamarServicio(_Campanas_lst, "Campanas_lst");
    $('#Wizard').hide(); $('#Manejo').fadeIn();
    Mensaje({ mensaje: "Campaña Creada con éxito" });
}
function Resultado2(msg)
{
    Mensaje({ mensaje: "Campaña Guardada con éxito" });
    //Campanas_lst(_Campanas_lst);RA
    LlamarServicio(_Campanas_lst, "Campanas_lst");
}
function _Campanas_SeleccionCuentas_lst(msg)
{
    var combo = $("#cboOperadores2")[0];
    Tabla({
        Contenedor: "pnlDistCuentasReglas",
        Resultado: msg.d,
        idSeleccion: "idCuenta",
        TodosSeleccionados: true,
        LimitarAltura: 200,
        Paginacion: 10000,
        Campos: [
            { Titulo: "Operator", Campo: "idOperador", Clase: "grdEntero", Combo: combo, Ordenacion: false, Filtro: false },
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "TotalUSD", Campo: "TotalDolar", Post: "USD", ToolTip: "CambioDolar", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "DeudaDolar", Post: "USD", ToolTip: "CambioDolar", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Deuda", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
        ]
    })
}
function SeleccionarOperadores()
{
    lstOperadores = ObtenerSeleccionados("pnlOperadores", "Nombre");
    LlenarCombo({ Combo: $('#cboOperadores2')[0], Resultado: lstOperadores, CampoId: "idOperador", CampoValor: "Nombre", TextoNull: "«Ninguno»", ValorNull: "0" });
    lstOperadores = lstOperadores.select("idOperador");
}
function CambiarPaso(anterior, siguiente)
{

    if (anterior == 1 && siguiente == 2)
    {
        if (!EsValido()) return false;
        SeleccionarOperadores();
        /*Campanas_SeleccionCuentas_lst(
        _Campanas_SeleccionCuentas_lst,
        $('#cboReglas').val(),
        $('#cboModosDistribucion').val(),
        lstOperadores

        );RA*/
        LlamarServicio(_Campanas_SeleccionCuentas_lst, "Campanas_SeleccionCuentas_lst", { idRegla: $('#cboReglas').val(), idModoDistOperadores: $('#cboModosDistribucion').val(), Operadores: lstOperadores });
    }
    $('#Paso' + anterior).hide(); $('#Paso' + siguiente).fadeIn();
}
function EsValido()
{
    if ($("#txtNombreCampana").val() == "")
    {
        Mensaje({ mensaje: "No ha seleccionado un nombre para la campaña" });
        return false;
    }
    if ($("#dtpFechaInicio").val() == "")
    {
        Mensaje({ mensaje: "No ha seleccionado una fecha de inicio para la campaña" });
        return false;
    }
    SeleccionarOperadores();
    return true;
}
function Finalizar()
{
    Campana_Config();
}
function AgregarOperador()
{
    //Campanas_Operadores_ins(_Campanas_Operadores_ins, Campana.idCampana, $("#cbomOperadores").val());RA
    LlamarServicio(_Campanas_Operadores_ins, "Campanas_Operadores_ins", { idCampana: Campana.idCampana, idOperador: $("#cbomOperadores").val() });
}
function _Campanas_Operadores_ins()
{
    //Campanas_sel(_Campanas_sel, Campana.idCampana);RA
    LlamarServicio(_Campanas_sel, "Campanas_sel", { idCampana: Campana.idCampana });
}
function EliminarOperador(idOperador)
{
    //Campanas_Operadores_del(_Campanas_Operadores_del, Campana.idCampana, idOperador);RA
    LlamarServicio(_Campanas_Operadores_del, "Campanas_Operadores_del", { idCampana: Campana.idCampana, idOperador: idOperador });
}
function _Campanas_Operadores_del()
{
    //Campanas_sel(_Campanas_sel, Campana.idCampana);RA
    LlamarServicio(_Campanas_sel, "Campanas_sel", { idCampana: Campana.idCampana });
}
function AsignarCuentas()
{
    var Cuentas = ObtenerSeleccionados("pnlCuentasSinAsignar").select("idCuenta");
    //Campanas_Cuentas_ins(_Campanas_Cuentas_ins, Campana.idCampana, Cuentas);RA
    LlamarServicio(_Campanas_Cuentas_ins, "Campanas_Cuentas_ins", { idCampana: Campana.idCampana, Cuentas: Cuentas });
}
function _Campanas_Cuentas_ins()
{
    //CuentasSinAsignar_lst(_CuentasSinAsignar_lst);RA
    LlamarServicio(_CuentasSinAsignar_lst, "CuentasSinAsignar_lst");
    //Campanas_sel(_Campanas_sel, Campana.idCampana);RA
    LlamarServicio(_Campanas_sel, "Campanas_sel", { idCampana: Campana.idCampana });
}
function EliminarCuentas()
{
    var Cuentas = ObtenerSeleccionados("pnlCuentas").select("idCuenta");
    //Campanas_Cuentas_del(_Campanas_Cuentas_del, Campana.idCampana, Cuentas);RA
    LlamarServicio(_Campanas_Cuentas_del, "Campanas_Cuentas_del", { idCampana: Campana.idCampana, Cuentas: Cuentas });
}
function _Campanas_Cuentas_del()
{
    //CuentasSinAsignar_lst(_CuentasSinAsignar_lst);RA
    LlamarServicio(_CuentasSinAsignar_lst, "CuentasSinAsignar_lst");
    //Campanas_sel(_Campanas_sel, Campana.idCampana);RA
    LlamarServicio(_Campanas_sel, "Campanas_sel", { idCampana: Campana.idCampana });
}