/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar()
{
    Param = searchToJSON();
    idReclamo = Param.idReclamo;
    $("#pnlPeCuentasReclamo").html("Cargando...");
    //Cuentas_Reclamos_sel(_Cuentas_Reclamos_sel, idReclamo);RA
    LlamarServicio(_Cuentas_Reclamos_sel, "Cuentas_Reclamos_sel", { idReclamo : idReclamo });
}
function MostrarCuentas(Cuentas)
{
    Tabla({
        Contenedor: "pnlPeCuentasReclamo",
        Resultado: Cuentas,
        LimitarAltura: 200,
        Campos: [
            { Titulo: "Document", Campo: "Documento", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Date", Campo: "Fecha", Clase: "grdFecha", Ordenacion: true, Filtro: true },
            { Titulo: "Overdue", Campo: "Antiguedad", Clase: "grdEntero", Ordenacion: true, Filtro: true },
            { Titulo: "Client", Campo: "Cliente", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Product", Campo: "Producto", Clase: "grdTexto", Ordenacion: true, Filtro: true },
            { Titulo: "Total", Campo: "Total", Campo2: "Moneda", Clase: "grdTexto", Ordenacion: false, Filtro: false },
            { Titulo: "Remaining", Campo: "Deuda", Campo2: "Moneda", Clase: "grdDecimal", Ordenacion: false, Filtro: false },
            { Titulo: "TotalUSD", Campo: "TotalDolar", ToolTip: "CambioDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingUSD", Campo: "DeudaDolar", ToolTip: "CambioDolar", Clase: "grdDecimal", Post: "USD", Ordenacion: true, Filtro: true },
            { Titulo: "TotalLocal", Campo: "TotalLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "RemainingLocal", Campo: "DeudaLocal", Campo2: "MonedaLocal", ToolTip: "CambioLocal", Clase: "grdDecimal", Ordenacion: true, Filtro: true },
            { Titulo: "Status", Campo: "Status", Clase: "grdTexto", Ordenacion: true, Filtro: true }
        ]
    });
}
function MostrarSoportes(Soportes)
{
    var pnlSop = $("#pnlSoportes");
    for (var i = 0; i < Soportes.length; i++)
    {
        var a = document.createElement("a");

        $(a).addClass("Soporte");
        $(a).text(Persona.Soportes[i].Nombre);
        a.onclick = new Function('Emergente({url:"/Emergentes/ctrlSoportes.aspx?idSoporte=' + Persona.Soportes[i].idSoporte + '"});'); //Persona.Soportes[i].Ubicacion;
        a.target = "_blank";
        pnlSop[0].appendChild(a);
    }
}
function _Cuentas_Reclamos_sel(msg)
{
    var Resp = msg.d;

    $("#lblMotivo").text(Resp.Motivo);
    $("#lblDescripcion").text(Resp.Descripcion);
    $("#lblStatus").text(Resp.Status);
    $("#lblDepartamento").text(Resp.Departamento);
    $('#lblFecha').text(AFechaMuyCorta(Resp.Fecha));
    $("#lblCodigo").text(Resp.Codigo);
    MostrarCuentas(Resp.CuentasReclamo);
    MostrarSoportes(Resp.Soportes);
}
function MostrarHistorial()
{
    Emergente({ url: "/Emergentes/ctrlReporte.aspx?idTipoReporte=12&CodigoReclamo=" + $("#lblCodigo").text(), width: 400, height: 300 });
}