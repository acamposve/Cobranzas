/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
function Inicializar() {   
    //Motivos_Reclamo_lst(_Motivos_Reclamo_lst);RA
    if (window.parent.idOpA == undefined) return;
    LlamarServicio(_Motivos_Reclamo_lst, "Motivos_Reclamo_lst", { idPais: window.parent.Pais() });
    //LlamarServicio(_ReclamosDepartamentos_lst,"ReclamosDepartamentos_lst",{idOrigen:1,idPais:"VEN"});
    var Param = searchToJSON();
    //    var Cuentas = window.parent.Cuentas;
    var Oper = Param.oper;   
    var idReclamo = Param.idReclamo;
    idPersona = window.parent.idPersona;
    $("#pnlPeCuentasReclamo").html("Cargando...");
    if (Oper == 'Ins') {
        $('#lblFechaReclamo').text(AFechaMuyCorta(new Date()));
        if (window.parent.Cuentas != undefined) {
            LlamarServicio(_Cuentas_lst, "Cuentas_lst", { lstCuentas: window.parent.Cuentas });
        }
        //      LlamarServicio(_Cuentas_Reclamos_lst, "Cuentas_Reclamos_lst", { Cuentas: Cuentas });
        //Cuentas_Reclamos_lst(_Cuentas_Reclamos_lst, Cuentas);
    } else if (Oper == 'Ver') {
        //Cuentas_Reclamos_sel(_Cuentas_Reclamos_sel, idReclamo);
        LlamarServicio(_Cuentas_Reclamos_sel, "Cuentas_Reclamos_sel", { idReclamo: idReclamo });
    }
    lstAdjuntos = [];
}
function _ReclamosDepartamentos_lst(msg) {
    LlenarCombo({ Combo: "cboDepartamento", Resultado: msg.d, CampoId: "idDepartamento", CampoValor: "Departamento" });
}
//function _Cuentas_Reclamos_lst(msg)
function _Cuentas_lst(msg) {
    MostrarCuentas(msg.d);
}
function MostrarCuentas(Cuentas) {
    Tabla({
        Contenedor: "pnlPeCuentasReclamo",
        Resultado: Cuentas,
        idSeleccion: "idCuenta",
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
function _Cuentas_Reclamos_sel(msg) {
    var Resp = msg.d;

    $("#cboMotivosReclamo").val(Resp.DescripcionMotivo);
    $("#txtDescripcionReclamo").val(Resp.Descripcion);
    $("#cboMotivosReclamo").val(Resp.DescripcionMotivo);
    $('#lblFechaReclamo').text(AFechaMuyCorta(Resp.Fecha));

    MostrarCuentas(Resp.CuentasReclamo);

}
function _Motivos_Reclamo_lst(msg) {
    LlenarCombo({ Combo: "cboMotivosReclamo", Resultado: msg.d, CampoId: "idReclamoMotivo", CampoValor: "Descripcion" });
}
function _Reclamos_ins(msg) {
    var resp = msg.d;
    if (resp == false) {
        Mensaje({ mensaje: "No se pudo guardar el reclamo." });
    }
    else {
        window.parent.RefrescarPersona();
        window.parent.CerrarEmergente();
        window.parent.Mensaje({ mensaje: "El reclamo se guardó correctamente, favor esperar un máximo de 20 minutos para que este sea replicado al sistema operativo" });
    }
}
function Insertar_Reclamo() {
    if ($("#txtDescripcionReclamo").val() == "") {
        Mensaje({ mensaje: Recursos["msgMustEnterDescription"] });
        return;
    }
    if (lstAdjuntos.length == 0) {
        Mensaje({ mensaje: "Debe adjuntar soportes para este Reclamo" });
        return;
    }

    var Reclamo = {};
    Reclamo.Descripcion = $("#txtDescripcionReclamo").val();
    Reclamo.idReclamoMotivo = $("#cboMotivosReclamo").val();
    Reclamo.idDepartamento = 1; // $("#cboDepartamento").val();
    Reclamo.idPersona = window.parent.idPersona;
    Reclamo.idOperador = window.parent.idOperadorLog;
    Reclamo.Cuentas = ObtenerTodos("pnlPeCuentasReclamo").select("idCuenta"); //Cuentas;
    LlamarServicio(_Reclamos_ins, "Reclamos_ins", { Reclamo: Reclamo, Adjuntos: lstAdjuntos });
}
function Adjuntar() {
    Emergente({ url: "ctrlSubirSoportes.aspx", width: 300, height: 200 });
}
function AgregarAdjunto(Archivo) {
    var div = document.createElement("DIV");
    div.onclick = function () {
        var Archivo = $(this).text();
        this.parentNode.removeChild(this);
        for (var i = 0; i < lstAdjuntos.length; i++) {
            var Parte = lstAdjuntos[i].substr(pos + 1);
            if (Parte == Archivo) {
                a.splice(i); break;
            }
        }
    };
    var pos = Archivo.indexOf("_");
    $(div).text(Archivo.substr(pos + 1));
    div.className = "Telefono";
    lstAdjuntos.push(Archivo);
    $("#Adjuntos")[0].appendChild(div);
}
