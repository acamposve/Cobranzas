/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    LlamarServicio(_Pasos_lst, "Pasos_lst");
    LlamarServicio(_Flujos_lst, "Flujos_lst");
    LlamarServicio(_Campanas_lst, "Campanas_lst");
    LlamarServicio(_OperadoresSupervisados_lst, "OperadoresSupervisados_lst", { idOperador : $("#idOperador").val()});
    LlamarServicio(_CuentasSinAsignar_lst, "CuentasSinAsignar_lst", {idOperador:$("#idOperador").val()});
}
function _Campanas_lst(msg)
{
    LlenarCombo({ Combo: "cboCampana", Resultado: msg.d, CampoId: "idCampana", CampoValor: "Nombre" })
}
function _OperadoresSupervisados_lst(msg)
{
    LlenarCombo({ Combo: "cboOperador", Resultado: msg.d, CampoId: "idOperador", CampoValor: "Nombre" })
}
function _Flujos_lst(msg)
{
    LlenarCombo({ Combo: "cboCFlujo", Resultado: msg.d, CampoId: "idFlujo", CampoValor: "Nombre" })
    LlenarCombo({ Combo: "cboOFlujo", Resultado: msg.d, CampoId: "idFlujo", CampoValor: "Nombre" })
}
function _Pasos_lst(msg)
{
    LlenarCombo({ Combo: "cboCPaso", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" })
    LlenarCombo({ Combo: "cboOPaso", Resultado: msg.d, CampoId: "idPaso", CampoValor: "Nombre" })
}

function _CuentasSinAsignar_lst(msg)
{

    Tabla({
        Contenedor: "pnlCuentas",
        Resultado: msg.d,
        TodosSeleccionados: false,
        idSeleccion: "idCuenta",
        Paginacion:500,
        Campos: [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Pre: "", Post: "", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Pre: "", Post: "", Ordenacion: true, Filtro: true },
            { Titulo: "Person", Campo: "Persona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "CodigoPersona", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Code", Campo: "CodigoCliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            //{ Titulo: "TotalUSD", Campo: "TotalDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            //{ Titulo: "DeudaUSD", Campo: "DeudaDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            //{ Titulo: "TasaUSD", Campo: "CambioDolar", Clase: "grdDecimal", Pre: "", Post: "", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Remaining", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            //{ Titulo: "TasaLocal", Campo: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            //{ Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            //{ Titulo: "DeudaLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
        ]
        //FuncSeleccionar: "_FuncSeleccionar_",
        //FuncEliminar: "_FuncEliminar_"
    });
}
function AsignarOperador()
{
    //Operadores_Cuentas_ins(_Operadores_Cuentas_ins, $("#cboOperador").val(), ObtenerSeleccionados("pnlCuentas").select("idCuenta"));RA
    LlamarServicio(_Operadores_Cuentas_ins, "Operadores_Cuentas_ins", { idOperador: $("#cboOperador").val(), Cuentas: ObtenerSeleccionados("pnlCuentas").select("idCuenta") });
}
function AsignarCampana()
{
    //Campanas_Cuentas_ins(_Campanas_Cuentas_ins, $("#cboCampana").val(), ObtenerSeleccionados("pnlCuentas").select("idCuenta"));RA
    LlamarServicio(_Campanas_Cuentas_ins, "Campanas_Cuentas_ins", { idCampana: $("#cboCampana").val(), Cuentas: ObtenerSeleccionados("pnlCuentas").select("idCuenta") });
}
function _Operadores_Cuentas_ins()
{
    //CuentasSinAsignar_lst(_CuentasSinAsignar_lst);RA
    LlamarServicio(_CuentasSinAsignar_lst, "CuentasSinAsignar_lst");
}
function _Campanas_Cuentas_ins()
{
    //CuentasSinAsignar_lst(_CuentasSinAsignar_lst);RA
    LlamarServicio(_CuentasSinAsignar_lst, "CuentasSinAsignar_lst");
}
