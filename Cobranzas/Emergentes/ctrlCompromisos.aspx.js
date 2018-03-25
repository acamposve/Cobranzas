/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Conversion.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    if (window.parent.Cuentas != undefined)
    {
        LlamarServicio(_Cuentas_lst, "Cuentas_lst", { lstCuentas: window.parent.Cuentas });
    }
    $("input.fecha").datepicker($.datepicker.regional["es"]);
    $("#dtpFechaInicial").val((new Date()).ToString(Regional.FormatoFecha));
}
function _Cuentas_lst(msg)
{
    Cuentas = msg.d;
    CuentasOrig = JSON.parse(JSON.stringify(Cuentas));
    $("#txtMontoTotal").val(Cuentas.select("Deuda").sum().ToString());
    Crear();
}
function Valor()
{
    return $("#txtCantidad").val();
}
function Guardar()
{
    var Resultados = ObtenerTodos("pnlPrincipal");
    var Compromiso = {};
    Compromiso.Descripcion = $("#txtDescripcion").val();
    Compromiso.idOperador = window.parent.idOperadorLog;
    Compromiso.Compromisos_Cuentas = [];
    for (var i = 0; i < Resultados.length; i++)
    {
        var j = 0;
        while (true)
        {
            if (Resultados[i]["Fecha" + j] == undefined) break;
            var CC = {};
            CC.Fecha = Convert.ToDateTime(Resultados[i]["Fecha" + j]).ToString("JSON");
            CC.idCuenta = Resultados[i]["idCuenta"];
            CC.Monto = Convert.ToDecimal(Resultados[i]["Monto" + j]);
            if (CC.Monto > 0)
            {
                Compromiso.Compromisos_Cuentas.push(CC);
            }
            j++;
        }
    }
    LlamarServicio(_Compromisos_ins, "Compromisos_ins", { Compromiso: Compromiso });
}
function _Compromisos_ins(msg)
{
    if (msg.d == true)
    {
        window.parent.Mensaje({ mensaje: "Su compromiso fue guardado correctamente" });
        window.parent.CerrarEmergente();
        window.parent.RefrescarCompromisos();
    }
}
function Crear()
{
    var Campos = [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "TotalUSD", Campo: "Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: true, Filtro: false },
            { Titulo: "RemainingUSD", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: false },
            { Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: false, Filtro: false }
        ];
    var l = Valor();
    for (var i = 0; i < l; i++)
    {
        Campos.push({ Titulo: "Fecha", Campo: "Fecha" + i, Clase: "grdFecha", Ordenacion: false, Filtro: false, Input: true })
        Campos.push({ Titulo: "TotalUSD", Campo: "Monto" + i, Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false, Input: true })
    }
    var Total = Cuentas.select("Deuda").sum();
    var MontoCuotas = Convert.ToDecimal($("#txtMontoTotal").val());
    var MontoCuota = MontoCuotas / l;
    $("#txtMontoCuota").val(MontoCuota.ToString())
    var f = Convert.ToDateTime($("#dtpFechaInicial").val());
    for (var j = 0; j < Cuentas.length; j++)
    {
        for (var i = 0; i < l; i++)
        {
            var f2 = new Date(f.getTime());
            var Fecha = (new Date(f2.setMonth(f2.getMonth() + i))); //.ToString(Regional.FormatoFecha);
            Cuentas[j]["Fecha" + i] = Fecha;
            Cuentas[j]["Monto" + i] = Cuentas[j]["Deuda"] / l * Total / MontoCuotas;
        }
    }

    Tabla({
        Contenedor: "pnlPrincipal",
        Resultado: Cuentas,
        TodosSeleccionados: true,
        idSeleccion: "idCuenta",
        Campos: Campos
    });
}
function Crear2()
{
    Cuentas = JSON.parse(JSON.stringify(CuentasOrig));
    var Campos = [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "TotalUSD", Campo: "Total", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "RemainingUSD", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", Clase: "grdDecimal", Ordenacion: false, Filtro: false }
        ];
    var f = Convert.ToDateTime($("#dtpFechaInicial").val());
    var Monto = Convert.ToDecimal($("#txtMontoTotal").val());
    var CantidadCuotas = Valor();
    var MontoCuota = Monto / CantidadCuotas; //Monto de la Cuota;
    $("#txtMontoCuota").val(MontoCuota.ToString());
    var Restante = MontoCuota;
    var NroCuota = 1;
    var FechaCuota = f;
    var w = 0;
    for (var j = 0; j < Cuentas.length; j++)
    {
        var Cuenta = Cuentas[j];
        var RestanteCuenta = Cuenta.Deuda;
        var n = 0;
        while (RestanteCuenta > 0)
        {
            if (NroCuota > CantidadCuotas) break;
            if (RestanteCuenta > Restante) //Se aplica un pago parcial y se adelanta la otra cuota
            {
                Cuentas[j]["Fecha" + n] = FechaCuota;
                Cuentas[j]["Monto" + n] = Restante;
                RestanteCuenta -= Restante;
                n++;
                NroCuota++;
                var f2 = new Date(f.getTime());
                FechaCuota = (new Date(f2.setMonth(f2.getMonth() + NroCuota - 1)));
                Restante = MontoCuota;
            }
            else
            {
                Cuentas[j]["Fecha" + n] = FechaCuota;
                Cuentas[j]["Monto" + n] = RestanteCuenta;
                Restante -= RestanteCuenta;
                RestanteCuenta = 0;
            }
        }
        w = (n > w) ? n : w;
    }
    w++;
    for (var i = 0; i < w; i++)
    {
        Campos.push({ Titulo: "Date", Campo: "Fecha" + i, Clase: "grdFecha", Ordenacion: false, Filtro: false, Input: true })
        Campos.push({ Titulo: "TotalUSD", Campo: "Monto" + i, Clase: "grdDecimal", Ordenacion: false, Filtro: false, Input: true })
    }

    Tabla({
        Contenedor: "pnlPrincipal",
        Resultado: Cuentas,
        TodosSeleccionados: true,
        idSeleccion: "idCuenta",
        Campos: Campos
    });
}